using Godot;
using System;
using System.Drawing;


internal class CardFlipToTarget : CardAnimationBase
{
    float halfFlipDistance;
    bool halfFlipUpdated = false;

    internal CardFlipToTarget(Card card) : base(card)
    {
        //wasDraggable = card.IsDraggable;
        //card.IsDraggable = false;
        Vector2 globalPosition = card.GlobalPosition;
        halfFlipDistance = (globalPosition - card.Target).Length() / 2f;
    }

    internal override bool Process(float delta)
    {
        if (Card.IsBeingDragged)
        {
            ScaleToOne();
            return true;
        }

        //TODO implement reversed
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

            if (deltaDistance < Mathf.Epsilon)
            {
                ScaleToOne();
                return true;
            }
        }
        return false;
    }
}
