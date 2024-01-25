using Godot;
using System;
using System.Collections.Generic;

public class Card : Area2D
{
    [Signal]
    public delegate void OnDragEnd(Card OriginCard, Card TargetCard = null);
    [Signal]
    public delegate void OnDragStart(Card card);
    [Signal]
    public delegate void OnCombatDeckClick(Card card);
    [Signal]
    public delegate void OnFieldDeckClick(Card card);
    [Signal]
    public delegate void OnInventoryButtonClick(Card Card);

    [Export]
    public bool faceDownOnEnterTree = false;
    [Export]
    public bool IsDraggable = true;

    public bool IsBeingDragged { get; internal set; } = false;
    public bool IsStackTarget = false;
    public bool DownStateOnHover = false;
    public float LerpDeltaMultiplier = 6;

    [Export]
    public Vector2 Target { get; private set; }

    ///<summary>Direct access to the parent Z-Index. Bypass the call to CardZIndexManager when setting the value.</summary>
    internal int BaseZIndex { get => base.ZIndex; set => base.ZIndex = value; }

    public new int ZIndex
    {
        get => base.ZIndex;
        set
        {
            if (value != base.ZIndex)
            {
                base.ZIndex = value;
                CardManager.OnCardZIndexModified(this);
            }
        }
    }

    private Sprite background;
    public Sprite Background
    {
        get
        {
            if (background == null)
                background = Front.GetNode<Sprite>("Background");
            return background;
        }
        set => background = value;
    }


    private Node2D back;
    public Node2D Back
    {
        get
        {
            if (back == null)
                back = GetNode<Node2D>("Back");
            return back;
        }
    }

    private Node2D front;
    public Node2D Front
    {
        get
        {
            if (front == null)
                front = GetNode<Node2D>("Front");
            return front;
        }
    }

    public IconCounter CostCounter => Front.GetNode<IconCounter>("ActionCostCounter");

    private readonly List<CardAnimationBase> animations = new List<CardAnimationBase>();

    public bool IsFaceDown { get; private set; } = true;
    public bool IsMoving { get; private set; } = true;

    // ===== GD Methods Override =====

    public override void _EnterTree()
    {
        CardManager.AddCard(this);
        IsFaceDown = faceDownOnEnterTree;
        if (faceDownOnEnterTree)
        {
            Front.Visible = false;
            Back.Visible = true;
        }
        else
        {
            Front.Visible = true;
            Back.Visible = false;
        }

        // TODO - Fix the problem with FlipToTarget cancel and remove this DuctTape
        Front.Scale = Vector2.One;
        Back.Scale = Vector2.One;
    }

    public override void _ExitTree()
    {
        CardManager.RemoveCard(this);
        for (int i = animations.Count - 1; i > 0; i--)
            animations[i].ForceEnd();
        animations.Clear();
        foreach (Node child in GetChildren())
            if (child is AudioStreamPlayer)
                child.QueueFree();
    }

    public override void _PhysicsProcess(float delta)
    {
        ProcessMovement(delta);
        ProcessAnimation(delta);
    }

    public string Serialize()
    {
        return "Serialized Card name: "
            + Front.GetNode<PixelText>("CardTitle").Value
            + System.Environment.NewLine
            + " - Draggable: " + IsDraggable + System.Environment.NewLine
            + " - Stack Target: " + IsStackTarget + System.Environment.NewLine
            + " - Face Down: " + IsFaceDown + System.Environment.NewLine;
    }

    // ===== Methods unique to this class =====

    public void ClearAnimations()
    {
        for (int i = animations.Count - 1; i > 0; i--)
            animations[i].ForceEnd();
        animations.Clear();
    }

    public void Flip(float animationTimeInSec = 0.4f)
    {
        AudioStreamMP3 sound = ResourceLoader.Load<AudioStreamMP3>(
            "res://Audio/Fx/cardFlip.mp3");
        AudioHelper.PlaySoundOnCard(this, sound, .7f);
        IsFaceDown = !IsFaceDown;
        bool isFlippingAlready = false;
        foreach (CardAnimationBase animation in animations)
        {
            if (animation is CardFlip cardFlip)
            {
                isFlippingAlready = true;
                cardFlip.Reverse();
            }
        }
        if (!isFlippingAlready)
        {
            animations.Add(new CardFlip(this, animationTimeInSec));
        }
    }

    public void FlipToPosition(Vector2 newTarget)
    {
        MoveToPosition(newTarget);
        bool alreadyFlipping = false;
        for (int i = animations.Count - 1; i >= 0; i--)
        {
            if (animations[i] is CardFlipToTarget)
            {
                alreadyFlipping = true;
                animations.RemoveAt(i);
            }
        }

        if (alreadyFlipping || newTarget.DistanceSquaredTo(GlobalPosition) < 144)
        {
            Flip();
        }
        else
        {
            AudioStreamMP3 sound = ResourceLoader.Load<AudioStreamMP3>(
                "res://Audio/Fx/cardFlip.mp3");
            AudioHelper.PlaySoundOnCard(this, sound, .7f);
            IsFaceDown = !IsFaceDown;
            animations.Add(new CardFlipToTarget(this));
        }
    }

    public string GetLabel()
    {
        return Front.GetNode<PixelText>("CardTitle").Value;
    }

    public Vector2 GetSize()
    {
        return Background.RegionRect.Size * Scale;
    }

    public void MoveToPosition(Vector2 globalPosition)
    {
        Target = globalPosition;
        IsMoving = true;
    }

    private void ProcessAnimation(float delta)
    {
        List<CardAnimationBase> toRemove = new List<CardAnimationBase>();

        foreach (CardAnimationBase animation in animations)
            if (animation.Process(delta))
                toRemove.Add(animation);

        foreach (CardAnimationBase animation in toRemove)
            animations.Remove(animation);
    }

    private void ProcessMovement(float delta)
    {
        if (IsMoving)
        {
            Vector2 globalPos = GlobalPosition;

            float lerpDelta = delta * LerpDeltaMultiplier;
            Vector2 newPos = new Vector2(
                Mathf.Lerp(globalPos.x, Target.x, lerpDelta),
                Mathf.Lerp(globalPos.y, Target.y, lerpDelta));
            if (newPos.DistanceSquaredTo(Target) < 0.01f)
                IsMoving = false;
            GlobalPosition = newPos;
        }
    }

    public void SetAlpha(float value)
    {
        Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, value);
    }

    public void SetLabel(string name)
    {
        Front.GetNode<PixelText>("CardTitle").Value = name;
    }

    public void SetBackgroundState(PressableState state)
    {
        Vector2 oldSize = Background.RegionRect.Size;
        float xOffset = DownStateOnHover & state == PressableState.OVER
          ? (int)PressableState.DOWN * oldSize.x
          : (int)state * oldSize.x;
        Background.RegionRect = new Rect2(xOffset, 0, oldSize);
    }

    // ===== Signals =====

    public void OnCardOverlap(Area2D target)
    {
        if (IsBeingDragged)
            CardHandlingControl.OnDraggedCardOverlap(target as Card);
    }

    public void OnCombatDeckClicked()
    {
        EmitSignal("OnCombatDeckClick", this);
    }

    public void OnFieldDeckClicked()
    {
        EmitSignal("OnFieldDeckClick", this);
    }

    public void OnInventoryClicked()
    {
        EmitSignal("OnInventoryButtonClick", this);
    }

    public void OnOverlapEnd(Area2D target)
    {
        if (IsBeingDragged)
            CardHandlingControl.OnDraggedCardOverlapEnd(target as Card);
    }
}
