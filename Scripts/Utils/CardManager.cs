using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * IMPORTANT REMINDER:
 *   Use `Card.BaseZIndex = value` to change a card's ZIndex from here.
 *   Using the usual ZIndex attribute calls `OnCardZIndexModified`. This will most
 *     likely result in an infinite loop or unexpected behavior.
 */

public static class CardManager
{
    public static int NumberOfCards { get; private set; } = 0;
    private static readonly List<Card> cards = new List<Card>();
    private static Card focusCard = null;

    internal static void AddCard(Card card)
    {
        cards.Add(card);
        NumberOfCards++;
        OnCardZIndexModified(card);
    }

    public static List<Card> GetCardsInCharacterWrappers(List<CharacterWrapper> wrappers)
    {
        List<Card> cards = new List<Card>();
        foreach (BaseCardWrapper wrapper in wrappers)
            cards.Add(wrapper.Card);
        return cards;
    }

    internal static Card GetTopCardInIntersectPointResults(
        Godot.Collections.Array cardEnumerable)
    {
        Card topCard = null;
        foreach (Godot.Collections.Dictionary item in cardEnumerable)
        {
            Card card = item["collider"] as Card;
            if ((topCard?.ZIndex ?? -4097) < card.ZIndex)
                topCard = card;

        }
        return topCard;
    }

    internal static void OnCardZIndexModified(Card card)
    {
        int oldIndex = cards.FindIndex(x => x == card);
        cards.RemoveAt(oldIndex);
        if (card.BaseZIndex >= NumberOfCards)
            card.BaseZIndex = NumberOfCards - 1;
        int newIndex = card.BaseZIndex;
        cards.Insert(card.BaseZIndex, card);
        int min = Math.Min(newIndex, oldIndex);
        int max = Math.Max(newIndex, oldIndex);
        UpdateZIndexes(min, max);
    }

    internal static void RemoveCard(Card card)
    {
        int index = cards.FindIndex(x => x == card);
        cards.RemoveAt(index);
        NumberOfCards--;
        UpdateZIndexes(index);
    }

    public static void SetFocus(Card card)
    {
        if (focusCard != null)
            RemoveFocus();
        focusCard = card;
    }

    public static void StackCards(List<Card> cards, Vector2 position)
    {
        int amount = cards.Count;
        for (int i = 0; i < amount; i++)
        {
            Card card = cards[i];
            card.ZIndex = 4096;
            card.MoveToPosition(
                position + new Vector2(
                    amount * 4 / 2 - 4 * i,
                    5 + 16 * (i + 1)
                )
            );
        }
    }

    public static Card RemoveFocus()
    {
        Card card = focusCard;
        focusCard.ZIndex = NumberOfCards - 1;
        if (focusCard != null)
            focusCard = null;
        return card;
    }

    public static void UpdateZIndexes(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1)
            endIndex = NumberOfCards - 1;
        for (int i = startIndex; i < endIndex; i++)
            cards[i].BaseZIndex = i;

        if (focusCard != null)
            focusCard.BaseZIndex = NumberOfCards;
    }
}
