using Godot;
using System;

public static class DeckFactory
{
    public static DeckWrapper CreateNewCombatDeckWrapper()
    {
        DeckModel deckInfo = new DeckModel
        {
            BaseDeck = CreateNewCombatDeck()
        };
        return new DeckWrapper(deckInfo);
    }

    public static DeckCardModel[] CreateNewCombatDeck(int deckSize = 20)
    {
        DeckCardModel[] baseDeck = new DeckCardModel[deckSize];
        for (int i = 0; i < deckSize; i++)
            baseDeck[i] = CreateDefaultCombatCard();
        return baseDeck;
    }

    public static DeckWrapper CreateNewFieldDeckWrapper()
    {
        DeckModel deckInfo = new DeckModel
        {
            BaseDeck = CreateNewFieldDeck()
        };
        return new DeckWrapper(deckInfo);
    }

    public static DeckCardModel[] CreateNewFieldDeck(int deckSize = 20)
    {
        DeckCardModel[] baseDeck = new DeckCardModel[deckSize];
        for (int i = 0; i < deckSize; i++)
            baseDeck[i] = CreateDefaultFieldCard();
        return baseDeck;
    }

    private static CombatCardModel CreateDefaultCombatCard()
    {
        return new CombatCardModel();
    }

    private static FieldCardModel CreateDefaultFieldCard()
    {
        return new FieldCardModel();
    }
}

