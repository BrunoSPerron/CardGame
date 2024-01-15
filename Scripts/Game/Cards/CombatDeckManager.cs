using Godot;
using System;

public class CombatDeckManager : BaseDeckManager
{
    public new CombatDeckModel Model { get; private set; }

    public CombatDeckManager(CombatDeckModel deckInfo) : base(deckInfo)
    {
        Model = deckInfo;
    }

    public CombatCardWrapper[] GetCombatDeck()
    {
        if (Model.BaseDeck == null || Model.BaseDeck.Length < 1)
        {
            Model.BaseCombatDeck = DeckFactory.CreateNewCombatDeck();
            GD.PrintErr("Combat deck wrapper error: No field deck. Using bad field deck");
        }

        CombatCardWrapper[] wrappedDeck = new CombatCardWrapper[Model.BaseDeck.Length];
        for (int i = 0; i < Model.BaseDeck.Length; i++)
        {
            CombatCardModel cardInfo = Model.BaseCombatDeck[i];
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
