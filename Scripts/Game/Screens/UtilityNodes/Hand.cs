using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hand: Node2D, IEnumerable<BaseCardWrapper>
{
    // TODO add -> public Vector2Int Pivot = new Vector2Int(0,0);

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

    public void AddCards(BaseCardWrapper[] cardWrappers)
    {
        foreach (BaseCardWrapper cardWrapper in cardWrappers)
        {
            Cards.Add(cardWrapper);
            AddChild(cardWrapper.Card);
        }
        UpdateCardsPositions();
    }

    public List<BaseCardWrapper> DiscardHand()
    {
        List<BaseCardWrapper> cardsRemoved = Cards;
        foreach (BaseCardWrapper wrapper in Cards)
            RemoveChild(wrapper.Card);
        Cards.Clear();
        return cardsRemoved;
    }

    public void RemoveCard(FieldCardWrapper wrapper)
    {
        Cards.Remove(wrapper);
        RemoveChild(wrapper.Card);
        UpdateCardsPositions();
    }

    public void UpdateCardsPositions()
    {
        float offsetBetweenCards = targetXOffset;
        int handSize = Cards.Count;
        float originX = CONSTS.SCREEN_CENTER.x - ((handSize - 1) * targetXOffset) / 2f;
        if (originX > maxWidth / 2)
        {
            originX = - maxWidth / 2;
            offsetBetweenCards = (maxWidth - cardWidth) / Cards.Count;
        }

        for (int i = 0; i < handSize; i++)
            Cards[i].Card.MoveToPosition(new Vector2(
                originX + i * offsetBetweenCards, Position.y));
    }

    public void Destroy()
    {
        foreach (BaseCardWrapper wrapper in this)
        {
            RemoveChild(wrapper.Card);
            Game.CleanCard(wrapper.Card);
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