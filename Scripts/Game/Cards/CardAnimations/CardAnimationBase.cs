using Godot;
using System;

public abstract class CardAnimationBase : Node
{
    protected Card Card { get; private set; }
    protected bool reversed = false;
    public CardAnimationBase(Card card)
    {
        Card = card;
    }

    public abstract void Destroy();

    public void Reverse()
    {
        reversed = !reversed;
    }

    protected void ScaleToOne()
    {
        Card.Front.Scale = Vector2.One;
        Card.Back.Scale = Vector2.One;
    }
}
