﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CombatDeckManager : BaseDeckManager
{
    public override BaseDeckModel BaseModel => Model;

    public CombatDeckModel Model { get; private set; }

    private List<CombatCardWrapper> deck = new List<CombatCardWrapper>();
    private readonly List<CombatCardWrapper> hand = new List<CombatCardWrapper>();
    private readonly List<CombatCardWrapper> discardPile = new List<CombatCardWrapper>();

    public CombatDeckManager(CharacterWrapper character, CombatDeckModel deckInfo)
        : base(character)
    {
        Model = deckInfo;
    }

    public void AddBonusCardsFromItem(ItemModel item)
    {
        for (int i = 0; i < item.CombatCards.Length; i++)
        {
            int placementIndex = Model.CombatDeck.Count - 1;
            int overridePriority = 0;
            bool overridePositionFound = false;
            if (placementIndex == -1)
            {
                placementIndex = 0;
            }
            else
            {
                while (!overridePositionFound)
                {
                    placementIndex--;
                    if (placementIndex < 0)
                    {
                        placementIndex = Model.CombatDeck.Count - 1;
                        overridePriority++;
                    }
                    if (Model.BonusCards
                        .Where((c) => { return c.OverrideCardIndex == placementIndex; })
                        .Count() < overridePriority)
                    {
                        overridePositionFound = true;
                    }
                }
            }

            string cardName = item.CombatCards[i];
            FileToLoad combatCardToLoad = PathHelper.GetFileToLoadInfo(cardName, item);
            CombatCardModel cardModel = JsonLoader.GetCombatCardModel(combatCardToLoad);
            Model.BonusCards.Add(new BonusCombatCardModel()
            {
                CombatCard = cardModel,
                ItemSourceId = item.ID,
                OverrideCardIndex = placementIndex,
                Priority = overridePriority
            });
        }
    }

    public void Clear()
    {
        foreach (CombatCardWrapper wrapper in deck)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();
        foreach (CombatCardWrapper wrapper in hand)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();
        foreach (CombatCardWrapper wrapper in discardPile)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();

        deck.Clear();
        hand.Clear();
        discardPile.Clear();
    }

    public CombatCardWrapper Draw()
    {
        if (deck == null)
        {
            GD.PrintErr("Deck manager error: Tried to draw from a non-existing deck");
            return CardFactory.CreateFrom(new CombatCardModel());
        }

        if (deck.Count == 0)
        {
            if (discardPile.Count == 0)
                return null;
            Shuffle();
        }

        hand.Add(deck[0]);
        CombatCardWrapper wrapper = deck[0];
        deck.RemoveAt(0);
        return wrapper;
    }

    public List<CombatCardWrapper> DrawMultiple(int amount)
    {
        List<CombatCardWrapper> wrappers = new List<CombatCardWrapper>();
        for (int i = 0; i < amount; i++)
            wrappers.Add(Draw());
        return wrappers;
    }

    public override List<BaseBonusCardWrapper> GetBonusCards()
    {
        List<BaseBonusCardWrapper> wrappers = new List<BaseBonusCardWrapper>();
        foreach (BonusCombatCardModel cardModel in Model.BonusCards)
        {
            BaseBonusCardWrapper wrapper = CardFactory.CreateFrom(cardModel);
            wrappers.Add(wrapper);
        }
        return wrappers;
    }
    public CombatCardWrapper[] GetHand()
    {
        return hand.ToArray();
    }

    public override List<BaseCardWrapper> GetSortedBaseDeck()
    {
        List<BaseCardWrapper> wrappers = new List<BaseCardWrapper>();
        foreach (CombatCardModel model in Model.CombatDeck)
            wrappers.Add(CardFactory.CreateFrom(model));
        return wrappers;
    }

    public void Reset()
    {
        Clear();

        List<CombatCardWrapper> wrappedDeck = new List<CombatCardWrapper>();

        if (Model.CombatDeck == null || Model.CombatDeck.Count == 0)
        {
            List<CombatCardModel> combatDeck = DeckFactory.CreateBadCombatDeck();
            Model.CombatDeck = new List<CombatCardModel>();
            foreach (CombatCardModel combatCard in combatDeck)
                Model.BaseDeck.Add(combatCard);
            GD.PrintErr("Deck manager error: No combat deck. Using bad deck");
        }

        BonusCombatCardModel[] bonusModels = Model.BonusCards.ToArray();

        List<BonusCombatCardModel>[] bonusCardsAtindex
            = new List<BonusCombatCardModel>[Model.CombatDeck.Count];

        for (int i = 0; i < bonusCardsAtindex.Length; i++)
            bonusCardsAtindex[i] = new List<BonusCombatCardModel>();

        for (int i = 0; i < bonusModels.Length; i++)
        {
            BonusCombatCardModel bonusCardModel = bonusModels[i];
            int overrideIndex = bonusCardModel.OverrideCardIndex;
            bonusCardsAtindex[overrideIndex].Add(bonusCardModel);
        }

        for (int i = 0; i < Model.CombatDeck.Count; i++)
        {
            List<BonusCombatCardModel> currenBonusCards = bonusCardsAtindex[i];
            if (currenBonusCards.Count != 0)
            {
                BonusCombatCardModel cardToUse = currenBonusCards[0];
                for (int j = 1; j < currenBonusCards.Count; j++)
                {
                    if (currenBonusCards[j].Priority > cardToUse.Priority)
                        cardToUse = currenBonusCards[j];
                }
                wrappedDeck.Add(CardFactory.CreateFrom(cardToUse.CombatCard));
            }
            else
            {
                wrappedDeck.Add(CardFactory.CreateFrom(Model.CombatDeck[i]));
            }
        }

        deck = wrappedDeck;
        Shuffle();
    }

    public void Shuffle()
    {
        deck.AddRange(discardPile);
        discardPile.Clear();
        deck.Shuffle();
    }

    public void UpdateBonusCards(List<ItemModel> items)
    {
        BonusCombatCardModel[] combatCardModels = Model.BonusCards.ToArray();

        foreach (BonusCombatCardModel cardModel in combatCardModels)
            if (!items.Exists((item) => item.ID == cardModel.ItemSourceId))
                Model.BonusCards.Remove(cardModel);

        foreach (ItemModel item in items)
            if (!Model.BonusCards.Exists((bonus) => bonus.ItemSourceId == item.ID))
                AddBonusCardsFromItem(item);
    }
}
