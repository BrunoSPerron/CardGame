using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DeckFactory
{
    public static CombatDeckManager CreateBadCombatDeckManager(CharacterWrapper parent)
    {
        CombatDeckModel deckInfo = new CombatDeckModel
        {
            CombatDeck = CreateBadCombatDeck()
        };
        return new CombatDeckManager(parent, deckInfo);
    }

    public static List<CombatCardModel> CreateBadCombatDeck(int deckSize = 20)
    {
        List<CombatCardModel> baseDeck = new List<CombatCardModel>();
        for (int i = 0; i < deckSize; i++)
            baseDeck[i] = CreateDefaultCombatCard();
        return baseDeck;
    }

    public static FieldDeckManager CreateBadFieldDeckManager(CharacterWrapper parent)
    {
        FieldDeckModel deckInfo = new FieldDeckModel
        {
            FieldDeck = CreateBadFieldDeck()
        };
        return new FieldDeckManager(parent, deckInfo);
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

    public static FieldDeckManager CreateFieldDeckManagerFromModel(
        CharacterWrapper parent, FieldDeckModel fieldDeck)
    {
        if (fieldDeck.FieldDeck == null || fieldDeck.FieldDeck.Count == 0)
        {
            GD.PrintErr(
                "Deck Factory error: Field deck missing, a generic one will be used. Mod: '"
                + fieldDeck.Mod + "' - At : " + fieldDeck.JsonFilePath);
            return CreateBadFieldDeckManager(parent);
        }

        return new FieldDeckManager(parent, fieldDeck);
    }

    public static CombatDeckManager CreateCombatDeckManagerFromModel(
        CharacterWrapper parent, CombatDeckModel combatDeck)
    {
        if (combatDeck.CombatDeck == null || combatDeck.CombatDeck.Count == 0)
        {
            GD.PrintErr(
                "Deck Factory error: Combat deck missing, a generic one will be used. Mod: '"
                + combatDeck.Mod + "' - At : " + combatDeck.JsonFilePath);
            return CreateBadCombatDeckManager(parent);
        }

        return new CombatDeckManager(parent, combatDeck);
    }
}
