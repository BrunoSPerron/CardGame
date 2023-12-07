using Godot;
using System;
using System.Collections.Generic;

public class DeckWrapper
{
    public DeckModel Info { get; private set; }

    public DeckWrapper(DeckModel deckInfo) 
    {
        Info = deckInfo;
    }

    public BaseCardWrapper[] GenerateBaseDeck()
    {
        if (Info.BaseDeck == null)
            Info.BaseDeck = DeckFactory.CreateNewCombatDeck();

        BaseCardWrapper[] wrappedDeck = new BaseCardWrapper[Info.BaseDeck.Length];
        for (int i = 0; i < Info.BaseDeck.Length; i++)
        {
            DeckCardModel cardInfo = Info.BaseDeck[i];
            BaseCardWrapper cardInfoWrapper = CardFactory.CreateCardFromDeckCardModel(cardInfo);
            wrappedDeck[i] = cardInfoWrapper;
        }

        return wrappedDeck;
    }
}
