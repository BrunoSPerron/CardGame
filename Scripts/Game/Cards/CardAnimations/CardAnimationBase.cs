using Godot;
using System;

public abstract class CardAnimationBase
{
    protected Card Card { get; private set; }
    protected bool reversed = false;
    public CardAnimationBase(Card card)
    {
        Card = card;
    }

    /// <returns>Whether the animation is over.</returns>
    internal abstract bool Process(float delta);

    public abstract void ForceEnd();

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
