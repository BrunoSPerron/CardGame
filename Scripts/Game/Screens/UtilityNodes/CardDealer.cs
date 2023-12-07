using Godot;
using System;
using System.Collections.Generic;

public class CardDealer : Position2D
{
    public Node2D cardsParent;
    public float SecondsBetweenDraws = 0.15f;
    public float timer = 0f;
    public readonly List<Card> cards = new List<Card>();

    public override void _Ready()
    {
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
            Card card = cards[0];
            cards.RemoveAt(0);
            card.Position = Position;
            card.faceDownOnReady = !card.faceDownOnReady;
            cardsParent.AddChild(card);
            card.FlipToPosition(card.Target);
            if (cards.Count == 0)
                QueueFree();
        }
    }
}
