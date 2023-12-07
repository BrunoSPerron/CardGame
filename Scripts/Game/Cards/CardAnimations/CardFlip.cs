using Godot;
using System;
using System.Drawing;


internal class CardFlip : CardAnimationBase
{

    private float timer = 0;
    private float flipTime;
    private float halfFlipTime = -1;
    private bool halfFlipUpdated = false;

    internal CardFlip(Card card, float timeInSeconds = 0.4f) : base(card)
    {
        flipTime = timeInSeconds;
        halfFlipTime = timeInSeconds / 2;
    }

    internal override bool Process(float delta)
    {
        if (reversed)
            return ReverseProcess(delta);
        else
            return RegularProcess(delta);
    }

    private bool RegularProcess(float delta)
    {
        if (timer > halfFlipTime)
        {
            float currentTime = timer - halfFlipTime;
            if (!halfFlipUpdated)
            {
                Card.Front.Visible = !Card.IsFaceDown;
                Card.Back.Visible = Card.IsFaceDown;
                halfFlipUpdated = true;
            }
            if (Card.IsFaceDown)
                Card.Back.Scale = new Vector2(currentTime / halfFlipTime, 1);
            else
                Card.Front.Scale = new Vector2(currentTime / halfFlipTime, 1);
        }
        else
        {
            if (halfFlipUpdated)
            {
                Card.Front.Visible = Card.IsFaceDown;
                Card.Back.Visible = !Card.IsFaceDown;
                halfFlipUpdated = false;
            }
            if (Card.IsFaceDown)
                Card.Front.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
            else
                Card.Back.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
        }

        timer += delta;
        if (flipTime < timer)
        {
            ScaleToOne();
            return true;
        }
        return false;
    }

    private bool ReverseProcess(float delta)
    {
        if (timer > halfFlipTime)
        {
            float currentTime = timer - halfFlipTime;
            if (halfFlipUpdated)
            {
                Card.Front.Visible = Card.IsFaceDown;
                Card.Back.Visible = !Card.IsFaceDown;
                halfFlipUpdated = false;
            }
            if (Card.IsFaceDown)
                Card.Front.Scale = new Vector2(currentTime / halfFlipTime, 1);
            else
                Card.Back.Scale = new Vector2(currentTime / halfFlipTime, 1);
        }
        else
        {
            if (!halfFlipUpdated)
            {
                Card.Front.Visible = !Card.IsFaceDown;
                Card.Back.Visible = Card.IsFaceDown;
                halfFlipUpdated = true;
            }
            if (Card.IsFaceDown)
                Card.Back.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
            else
                Card.Front.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
        }

        timer -= delta;
        if (timer < 0)
        {
            ScaleToOne();
            return true;
        }

        return false;
    }
}
