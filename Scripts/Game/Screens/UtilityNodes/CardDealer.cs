using Godot;
using System;
using System.Collections.Generic;

public class CardDealer : Position2D
{
    public Node2D cardsParent;
    public float SecondsBetweenDraws = 0.15f;
    public float timer = 0f;
    private readonly List<CardAndTarget> cards = new List<CardAndTarget>();

    public override void _Ready()
    {
        foreach (CardAndTarget card in cards)
            ReadyNewChild(card);

        if (cards.Count == 0 || cardsParent == null)
        {
            GD.PrintErr("Card dealer error: No cards on creation");
            QueueFree();
        }
    }

    public override void _Process(float delta)
    {
        timer += delta;
        if (timer > SecondsBetweenDraws)
        {
            timer -= SecondsBetweenDraws;
            CardAndTarget cardAndTarget = cards[0];
            cards.RemoveAt(0);
            cardAndTarget.Card.FlipToPosition(cardAndTarget.Target);
            if (cards.Count == 0)
                QueueFree();
        }
    }

    public void AddCard(Card card, bool parentIsInTree = true)
    {
        CardAndTarget cardAndTarget = new CardAndTarget
        {
            Card = card,
            Target = card.Target
        };
        cards.Add(cardAndTarget);
        if (parentIsInTree)
            ReadyNewChild(cardAndTarget);
    }

    public void AddPriorityCard(Card card, bool parentIsInTree = true)
    {
        CardAndTarget cardAndTarget = new CardAndTarget {
            Card = card, Target = card.Target
        };
        cards.Insert(0, cardAndTarget);
        if (parentIsInTree)
            ReadyNewChild(cardAndTarget);
    }

    private void ReadyNewChild(CardAndTarget cardAndTarget)
    {
        Card card = cardAndTarget.Card;
        bool defaultFaceDownOnEnter = card.faceDownOnEnterTree;
        card.faceDownOnEnterTree = true;
        cardsParent.AddChild(card);
        card.faceDownOnEnterTree = defaultFaceDownOnEnter;
        card.Position = Position;
        card.MoveToPosition(Position);
    }

    // === Private struct ===

    private struct CardAndTarget
    {
        public Card Card;
        public Vector2 Target;
    }
}
