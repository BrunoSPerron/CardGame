using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DeckFactory
{
    public static CombatDeckManager CreateBadCombatDeckManager()
    {
        CombatDeckModel deckInfo = new CombatDeckModel
        {
            BaseDeck = CreateBadCombatDeck()
        };
        return new CombatDeckManager(deckInfo);
    }

    public static List<CombatCardModel> CreateBadCombatDeck(int deckSize = 20)
    {
        List<CombatCardModel> baseDeck = new List<CombatCardModel>();
        for (int i = 0; i < deckSize; i++)
            baseDeck[i] = CreateDefaultCombatCard();
        return baseDeck;
    }

    public static FieldDeckManager CreateBadFieldDeckManager()
    {
        FieldDeckModel deckInfo = new FieldDeckModel
        {
            BaseDeck = CreateBadFieldDeck()
        };
        return new FieldDeckManager(deckInfo);
    }

    public static List<FieldCardModel> CreateBadFieldDeck(int deckSize = 20)
    {
        FieldCardModel[] fieldDeck = new FieldCardModel[deckSize];
        for (int i = 0; i < deckSize; i++)
            fieldDeck[i] = CreateDefaultFieldCard();
        return fieldDeck.ToList();
    }

    private static CombatCardModel CreateDefaultCombatCard()
    {
        return new CombatCardModel();
    }

    private static FieldCardModel CreateDefaultFieldCard()
    {
        return new FieldCardModel();
    }

    public static FieldDeckManager CreateFieldDeckManagerFromModel(FieldDeckModel fieldDeck)
    {
        if (fieldDeck.BaseDeck == null || fieldDeck.BaseDeck.Count < 5)
        {
            GD.PrintErr(
                "Deck Factory error: Field deck missing, a generic one will be used. Mod: '"
                + fieldDeck.Mod + "' - At : " + fieldDeck.JsonFilePath);
            return CreateBadFieldDeckManager();
        }

        return new FieldDeckManager(fieldDeck);
    }

    public static CombatDeckManager CreateCombatDeckManagerFromModel(CombatDeckModel combatDeck)
    {
        if (combatDeck.BaseDeck == null || combatDeck.BaseDeck.Count < 5)
        {
            GD.PrintErr(
                "Deck Factory error: Combat deck missing, a generic one will be used. Mod: '"
                + combatDeck.Mod + "' - At : " + combatDeck.JsonFilePath);
            return CreateBadCombatDeckManager();
        }

        return new CombatDeckManager(combatDeck);
    }
}
