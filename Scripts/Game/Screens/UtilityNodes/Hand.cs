using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hand: Node2D, IEnumerable<BaseCardWrapper>
{
    public Vector2 Pivot = new Vector2(0, 800);

    public int maxWidth = 800;
    public int targetXOffset = 50;
    public int cardWidth = 87;

    public readonly List<BaseCardWrapper> Cards = new List<BaseCardWrapper>();
    public int Size => Cards.Count;

    public Game Game;

    public void AddCard(BaseCardWrapper cardWrapper)
    {
        Cards.Add(cardWrapper);
        AddChild(cardWrapper.Card);
        UpdateCardsPositions();
    }

    public void AddCards(List<BaseCardWrapper> cardWrappers)
    {
        AddCards(cardWrappers.ToArray());
    }

    public void AddCards(BaseCardWrapper[] cardWrappers)
    {
        foreach (BaseCardWrapper cardWrapper in cardWrappers)
        {
            Cards.Add(cardWrapper);
            AddChild(cardWrapper.Card);
        }
        UpdateCardsPositions();
    }

    public void Clear()
    {
        foreach (BaseCardWrapper wrapper in this)
        {
            RemoveChild(wrapper.Card);
            Game.CleanCard(wrapper.Card);
        }
    }

    public void Destroy()
    {
        Clear();
        QueueFree();
    }

    public List<BaseCardWrapper> DiscardHand()
    {
        List<BaseCardWrapper> cardsRemoved = Cards;
        foreach (BaseCardWrapper wrapper in Cards)
            RemoveChild(wrapper.Card);
        Cards.Clear();
        return cardsRemoved;
    }

    public void RemoveCard(BaseCardWrapper wrapper)
    {
        wrapper.Card.Rotation = 0;
        Cards.Remove(wrapper);
        RemoveChild(wrapper.Card);
        UpdateCardsPositions();
    }

    public void UpdateCardsPositions()
    {
        float offsetBetweenCards = targetXOffset;
        int handSize = Cards.Count;
        float originX = CONSTS.SCREEN_CENTER.x - (handSize - 1) * targetXOffset / 2f;
        if (originX > maxWidth / 2)
        {
            originX = - maxWidth / 2;
            offsetBetweenCards = (maxWidth - cardWidth) / Cards.Count;
        }

        Vector2 globalPivot = Pivot + Position;
        float pivotMagnitude = Mathf.Sqrt(Pivot.x * Pivot.x + Pivot.y * Pivot.y);
        for (int i = 0; i < handSize; i++)
        {
            Card card = Cards[i].Card;
            var target = new Vector2(originX + i * offsetBetweenCards, Position.y);

            float xDiff = target.x - globalPivot.x;
            float yDiff = target.y - globalPivot.y;
            double angle = Math.Atan2(yDiff, xDiff);
            card.Rotation = (float)(angle + Math.PI / 2);

            float x = globalPivot.x + pivotMagnitude * (float)Math.Cos(angle);
            float y = globalPivot.y + pivotMagnitude * (float)Math.Sin(angle);
            card.MoveToPosition(new Vector2(x, y));
        }
    }


    public IEnumerator<BaseCardWrapper> GetEnumerator()
    {
        foreach (BaseCardWrapper wrapper in Cards)
            yield return wrapper;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new HandEnumerator(Cards.ToArray());
    }
}


public class HandEnumerator : IEnumerator
{
    public BaseCardWrapper[] Wrappers;

    int position = -1;

    public HandEnumerator(BaseCardWrapper[] list)
    {
        Wrappers = list;
    }

    public bool MoveNext()
    {
        position++;
        return (position < Wrappers.Length);
    }

    public void Reset()
    {
        position = -1;
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public BaseCardWrapper Current
    {
        get
        {
            try
            {
                return Wrappers[position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}