using Godot;
using System;

public class CombatDeckWrapper : BaseDeckWrapper
{
    public new CombatDeckModel Model { get; private set; }

    public CombatDeckWrapper(CombatDeckModel deckInfo) : base(deckInfo)
    {
        Model = deckInfo;
    }

    public CombatCardWrapper[] GetCombatDeck()
    {
        if (Model.BaseDeck == null || Model.BaseDeck.Length < 1)
        {
            Model.CombatDeck = DeckFactory.CreateNewCombatDeck();
            GD.Print("Combat deck wrapper error: No field deck. Using bad field deck");
        }

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

    public override BaseCardWrapper[] GetBaseDeck()
    {
        CombatCardWrapper[] combatDeck = GetCombatDeck();
        BaseCardWrapper[] baseDeck = new BaseCardWrapper[combatDeck.Length];
        for (int i = 0; i < baseDeck.Length; i++)
            baseDeck[i] = combatDeck[i];
        return baseDeck;
    }
}
