using Godot;
using System;

public class CombatDeckWrapper
{
    public CombatDeckModel Model { get; private set; }

    public CombatDeckWrapper(CombatDeckModel deckInfo)
    {
        Model = deckInfo;
    }

    public CombatCardWrapper[] GenerateBaseDeck()
    {
        if (Model.BaseDeck == null)
            Model.CombatDeck = DeckFactory.CreateNewCombatDeck();

        CombatCardWrapper[] wrappedDeck = new CombatCardWrapper[Model.BaseDeck.Length];
        for (int i = 0; i < Model.BaseDeck.Length; i++)
        {
            CombatCardModel cardInfo = Model.CombatDeck[i];
            CombatCardWrapper cardInfoWrapper = CardFactory.CreateCardFromCombatCardModel(
                cardInfo);
            wrappedDeck[i] = cardInfoWrapper;
        }

        return wrappedDeck;
    }
}
