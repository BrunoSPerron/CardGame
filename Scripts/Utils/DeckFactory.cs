using Godot;
using System;

public static class DeckFactory
{
    public static CombatDeckWrapper CreateNewCombatDeckWrapper()
    {
        CombatDeckModel deckInfo = new CombatDeckModel
        {
            CombatDeck = CreateNewCombatDeck()
        };
        return new CombatDeckWrapper(deckInfo);
    }

    public static CombatCardModel[] CreateNewCombatDeck(int deckSize = 20)
    {
        CombatCardModel[] baseDeck = new CombatCardModel[deckSize];
        for (int i = 0; i < deckSize; i++)
            baseDeck[i] = CreateDefaultCombatCard();
        return baseDeck;
    }

    public static FieldDeckWrapper CreateNewFieldDeckWrapper()
    {
        FieldDeckModel deckInfo = new FieldDeckModel
        {
            FieldDeck = CreateNewFieldDeck()
        };
        return new FieldDeckWrapper(deckInfo);
    }

    public static FieldCardModel[] CreateNewFieldDeck(int deckSize = 20)
    {
        FieldCardModel[] fieldDeck = new FieldCardModel[deckSize];
        for (int i = 0; i < deckSize; i++)
            fieldDeck[i] = CreateDefaultFieldCard();
        return fieldDeck;
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

