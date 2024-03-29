﻿using Godot;
using System;


internal class CardFlipToTarget : CardAnimationBase
{
    private readonly float halfFlipDistance;
    private bool halfFlipUpdated = false;

    public CardFlipToTarget(Card card) : base(card)
    {
        Vector2 globalPosition = card.GlobalPosition;
        halfFlipDistance = (globalPosition - card.Target).Length() / 2f;
    }

    public override void _Process(float delta)
    {
        if (Card.IsBeingDragged)
        {
            Destroy();
            return;
        }

        Vector2 globalPos = Card.GlobalPosition;
        float distance = (Card.Target - globalPos).Length();
        if (distance > halfFlipDistance)
        {
            if (halfFlipUpdated)
            {
                Card.Front.Visible = Card.IsFaceDown;
                Card.Back.Visible = !Card.IsFaceDown;
                halfFlipUpdated = false;
            }
            float deltaDistance = (halfFlipDistance - distance) / halfFlipDistance;
            if (deltaDistance > 1)
            {
                ScaleToOne();
            }
            else
            {
                if (Card.IsFaceDown)
                    Card.Front.Scale = new Vector2(deltaDistance, 1);
                else
                    Card.Back.Scale = new Vector2(deltaDistance, 1);
            }
        }
        else
        {
            if (!halfFlipUpdated)
            {
                Card.Front.Visible = !Card.IsFaceDown;
                Card.Back.Visible = Card.IsFaceDown;
                halfFlipUpdated = true;
            }

            float deltaDistance = distance / halfFlipDistance;
            if (Card.IsFaceDown)
                Card.Back.Scale = new Vector2(1 - deltaDistance, 1);
            else
                Card.Front.Scale = new Vector2(1 - deltaDistance, 1);

            if (deltaDistance < 0.001f)
            {
                Destroy();
            }
        }
    }

    public override void _Ready()
    {
        Card.IsFaceDown = !Card.IsFaceDown;

        AudioStreamMP3 sound = ResourceLoader.Load<AudioStreamMP3>(
            "res://Audio/Fx/cardFlip.mp3");

        AudioStreamPlayer player = new AudioStreamPlayer
        {
            Stream = sound,
            PitchScale = .7f
        };
        AddChild(player);
        player.Play();
    }

    public override void Destroy()
    {
        if (Card.IsFaceDown)
        {
            Card.Front.Visible = false;
            Card.Back.Visible = true;
        }
        else
        {
            Card.Front.Visible = true;
            Card.Back.Visible = false;
        }
        ScaleToOne();
        QueueFree();
    }
}
