using Godot;
using System;
using System.Collections.Generic;

public class CardHandlingControl : Node
{
    private static Card cardBeingDragged;
    private static Vector2 cardBeingDraggedOrigin;
    private static Card currentStackTarget = null;
    private static Node2D currentMaskTarget = null;
    private static bool isDragging = false;
    private static Node2D rootNode;
    private static Vector2 offset;
    private static readonly List<Card> stackTargets = new List<Card>();

    public override void _Ready()
    {
        rootNode = GetParent<Node2D>();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (isDragging)
        {
            Vector2 mousePosition = rootNode.GetViewport().GetMousePosition();
            cardBeingDragged.MoveToPosition(mousePosition - offset);
            if (stackTargets.Count > 1)
                UpdateStackTargets();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.Pressed)
                OnLeftMouseDown(eventMouseButton.Position);
            else
                OnLeftMouseUp(eventMouseButton.Position);
        }
        else if (@event is InputEventMouseMotion mouseMotion)
        {
            OnMouseMotion(mouseMotion);
        }
    }

    internal static void ClearStackTargets()
    {
        foreach (Card card in stackTargets)
            card.SetBackgroundState(PressableState.STAND_BY);
        stackTargets.Clear();
        currentStackTarget = null;
    }

    private static Tuple<Node2D, int> GetTopMaskObjectAtPosition(Vector2 position)
    {
        Physics2DDirectSpaceState spaceState = rootNode.GetWorld2d().DirectSpaceState;
        Godot.Collections.Array maskResults = spaceState.IntersectPoint(
            position, 32, new Godot.Collections.Array(), 8, false, true);

        Node2D topMaskObject = null;
        int topMaskZIndex = -4097;
        foreach (Godot.Collections.Dictionary item in maskResults)
        {
            Node2D maskObject = item["collider"] as Node2D;
            int maskZIndex;
            if (maskObject.ZAsRelative)
            {
                var parent = maskObject.GetParent<Node2D>();
                var grandParent = parent.GetParent<Node2D>();
                maskZIndex = grandParent.ZIndex + parent.ZIndex + maskObject.ZIndex;
            }
            else
            {
                maskZIndex = maskObject.ZIndex;
            }

            if (topMaskZIndex < maskZIndex)
            {
                topMaskObject = maskObject;
                topMaskZIndex = maskZIndex;
            }

        }
        return new Tuple<Node2D, int>(topMaskObject, topMaskZIndex);
    }

    private static void StartDragging(Card newCardBeingDragged)
    {
        cardBeingDragged = newCardBeingDragged;
        cardBeingDraggedOrigin = cardBeingDragged.Target;
        cardBeingDragged.LerpDeltaMultiplier = 8.0f;
        cardBeingDragged.ZIndex = 4096;
        cardBeingDragged.IsBeingDragged = true;
        isDragging = true;
        offset = cardBeingDragged.GetLocalMousePosition() * cardBeingDragged.Scale;
        Godot.Collections.Array overlaps = cardBeingDragged.GetOverlappingAreas();
        foreach (Card card in overlaps)
            if (card.IsStackTarget)
                stackTargets.Add(card);
        UpdateStackTargets();
        CardManager.SetFocus(cardBeingDragged);
        cardBeingDragged.EmitSignal("OnDragStart", cardBeingDragged);
    }

    private static void StopDragging()
    {
        cardBeingDragged.MoveToPosition(cardBeingDraggedOrigin);
        cardBeingDragged.LerpDeltaMultiplier = 3.5f;
        cardBeingDragged.IsBeingDragged = false;
        cardBeingDragged.EmitSignal("OnDragEnd", cardBeingDragged, currentStackTarget);
        cardBeingDragged = null;
        CardManager.RemoveFocus();
        isDragging = false;
        ClearStackTargets();
    }

    internal static void OnDraggedCardOverlap(Card target)
    {
        if (target.IsStackTarget)
        {
            stackTargets.Add(target);
            UpdateStackTargets();
        }
    }

    internal static void OnDraggedCardOverlapEnd(Card target)
    {
        int index = stackTargets.FindIndex(x => x == target);
        if (index != -1)
        {
            stackTargets[index].SetBackgroundState(PressableState.STAND_BY);
            stackTargets.RemoveAt(index);
            UpdateStackTargets();
        }
    }

    private void OnLeftMouseDown(Vector2 mousePosition)
    {
        World2D world = rootNode.GetWorld2d();
        Physics2DDirectSpaceState spaceState = world.DirectSpaceState;
        Godot.Collections.Array results = spaceState.IntersectPoint(
            mousePosition, 64, new Godot.Collections.Array(), 4, false, true);
        Card topCard = CardManager.GetTopCardInIntersectPointResults(results);

        (Node2D topMaskObject, int topMaskZIndex) = GetTopMaskObjectAtPosition(mousePosition);

        bool dragNewCard = true;
        if (topMaskObject != null)
        {
            if (topCard != null && topMaskZIndex >= topCard.ZIndex)
            {
                dragNewCard = false;
                if (topMaskObject is CardButtonBase button)
                    button.OnMouseDown();
            }
        }

        if (dragNewCard && (topCard?.IsDraggable ?? false))
            StartDragging(topCard);
    }

    private void OnLeftMouseUp(Vector2 mousePosition)
    {
        if (isDragging)
            StopDragging();
        else
        {
            (Node2D topMaskObject, _) = GetTopMaskObjectAtPosition(mousePosition);
            if (topMaskObject is CardButtonBase button)
                button.OnMouseUp();
        }
    }

    private void OnMouseMotion(InputEventMouseMotion mouseMotion)
    {
        if (!isDragging)
        {
            (Node2D topMaskObject, _) = GetTopMaskObjectAtPosition(mouseMotion.Position);
            if (topMaskObject != currentMaskTarget)
            {
                if (currentMaskTarget is CardButtonBase oldButton)
                    oldButton.OnMouseExit();
                currentMaskTarget = topMaskObject;
                if (currentMaskTarget is CardButtonBase newButton)
                    newButton.OnMouseEnter(mouseMotion);
            }
        }
    }

    private static void UpdateStackTargets()
    {
        if (isDragging)
        {
            Vector2 mousePosition = rootNode.GetViewport().GetMousePosition();
            Card nearestCard = null;
            float nearestCardSquareDistance = float.MaxValue;
            foreach (Card card in stackTargets)
            {
                float squaredDistance = card.Position.DistanceSquaredTo(
                    mousePosition);
                if (squaredDistance < nearestCardSquareDistance)
                {
                    nearestCard = card;
                    nearestCardSquareDistance = squaredDistance;
                }
            }
            foreach (Card card in stackTargets)
            {
                card.SetBackgroundState(card == nearestCard
                    ? PressableState.OVER
                    : PressableState.STAND_BY);
            }
            currentStackTarget = nearestCard;
        }
        else
        {
            ClearStackTargets();
        }
    }
}
