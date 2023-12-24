using Godot;
using System;
using System.Collections.Generic;

public class CardHand: Node2D
{
    // TODO add -> public Vector2Int Pivot = new Vector2Int(0,0);

    public int maxWidth = 800;
    public int targetXOffset = 50;
    public int cardWidth = 87;

    private readonly List<BaseCardWrapper> Cards = new List<BaseCardWrapper>();

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
            GD.Print(cardWrapper);
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
}
