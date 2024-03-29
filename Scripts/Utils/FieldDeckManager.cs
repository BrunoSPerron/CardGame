﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class FieldDeckManager : BaseDeckManager
{
    public override BaseDeckModel BaseModel => Model;

    public FieldDeckModel Model { get; private set; }

    private List<FieldCardWrapper> deck = new List<FieldCardWrapper>();
    private readonly List<FieldCardWrapper> hand = new List<FieldCardWrapper>();
    private readonly List<FieldCardWrapper> discardPile = new List<FieldCardWrapper>();

    public FieldDeckManager(CharacterWrapper character, FieldDeckModel deckInfo)
        : base(character)
    {
        Model = deckInfo;
    }

    public void AddBonusCardsFromItem(ItemModel item)
    {
        for (int i = 0; i < item.FieldCards.Length; i++)
        {
            int placementIndex = Model.FieldDeck.Count - 1;
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
                        placementIndex = Model.FieldDeck.Count - 1;
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

            string cardName = item.FieldCards[i];
            FileToLoad fileToLoad = PathHelper.GetFileToLoadInfo(cardName, item);
            FieldCardModel cardModel = JsonLoader.GetFieldCardModel(fileToLoad);
            Model.BonusCards.Add(new BonusFieldCardModel()
            {
                FieldCard = cardModel,
                ItemSourceId = item.ID,
                OverrideCardIndex = placementIndex,
                Priority = overridePriority
            });
        }
    }

    public void Clear()
    {
        foreach (FieldCardWrapper wrapper in deck)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();
        foreach (FieldCardWrapper wrapper in hand)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();
        foreach (FieldCardWrapper wrapper in discardPile)
            if (!wrapper.Card.IsInsideTree())
                wrapper.Card.QueueFree();

        deck.Clear();
        hand.Clear();
        discardPile.Clear();
    }

    public FieldCardWrapper Draw()
    {
        if (deck == null)
        {
            GD.PrintErr("Deck manager error: Tried to draw from a non-existing deck");
            return CardFactory.CreateFrom(new FieldCardModel());
        }

        if (deck.Count == 0)
        {
            if (discardPile.Count == 0)
                return null;
            Shuffle();
        }

        hand.Add(deck[0]);
        FieldCardWrapper wrapper = deck[0];
        deck.RemoveAt(0);
        return wrapper;
    }

    public List<FieldCardWrapper> DrawMultiple(int amount)
    {
        List<FieldCardWrapper> wrappers = new List<FieldCardWrapper>();
        for (int i = 0; i < amount; i++)
            wrappers.Add(Draw());
        return wrappers;
    }

    public override List<BaseBonusCardWrapper> GetBonusCards()
    {
        List<BaseBonusCardWrapper> wrappers = new List<BaseBonusCardWrapper>();
        foreach (BonusFieldCardModel cardModel in Model.BonusCards)
        {
            BaseBonusCardWrapper wrapper = CardFactory.CreateFrom(cardModel);
            wrappers.Add(wrapper);
        }
        return wrappers;
    }

    public FieldCardWrapper[] GetHand()
    {
        return hand.ToArray();
    }

    public override List<BaseCardWrapper> GetSortedBaseDeck()
    {
        List<BaseCardWrapper> wrappers = new List<BaseCardWrapper>();
        foreach (FieldCardModel model in Model.FieldDeck)
            wrappers.Add(CardFactory.CreateFrom(model));
        return wrappers;
    }

    public void Reset()
    {
        Clear();

        List<FieldCardWrapper> wrappedDeck = new List<FieldCardWrapper>();

        if (Model.FieldDeck == null || Model.FieldDeck.Count == 0)
        {
            List<FieldCardModel> fieldDeck = DeckFactory.CreateBadFieldDeck();
            Model.FieldDeck = new List<FieldCardModel>();
            foreach (FieldCardModel fieldCard in fieldDeck)
                Model.BaseDeck.Add(fieldCard);
            GD.PrintErr("Deck manager error: No field deck. Using bad deck");
        }

        BonusFieldCardModel[] bonusModels = Model.BonusCards.ToArray();

        List<BonusFieldCardModel>[] bonusCardsAtindex
            = new List<BonusFieldCardModel>[Model.FieldDeck.Count];

        for (int i = 0; i < bonusCardsAtindex.Length; i++)
            bonusCardsAtindex[i] = new List<BonusFieldCardModel>();

        for (int i = 0; i < bonusModels.Length; i++)
        {
            BonusFieldCardModel bonusCardModel = bonusModels[i];
            int overrideIndex = bonusCardModel.OverrideCardIndex;
            bonusCardsAtindex[overrideIndex].Add(bonusCardModel);
        }

        for (int i = 0; i < Model.FieldDeck.Count; i++)
        {
            List<BonusFieldCardModel> currenBonusCards = bonusCardsAtindex[i];
            if (currenBonusCards.Count != 0)
            {
                BonusFieldCardModel cardToUse = currenBonusCards[0];
                for (int j = 1; j < currenBonusCards.Count; j++)
                {
                    if (currenBonusCards[j].Priority > cardToUse.Priority)
                        cardToUse = currenBonusCards[j];
                }
                wrappedDeck.Add(CardFactory.CreateFrom(cardToUse.FieldCard));
            }
            else
            {
                wrappedDeck.Add(CardFactory.CreateFrom(Model.FieldDeck[i]));
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
        BonusFieldCardModel[] fieldCardModels = Model.BonusCards.ToArray();

        foreach (BonusFieldCardModel cardModel in fieldCardModels)
            if (!items.Exists((item) => item.ID == cardModel.ItemSourceId))
                Model.BonusCards.Remove(cardModel);

        foreach (ItemModel item in items)
            if (!Model.BonusCards.Exists((bonus) => bonus.ItemSourceId == item.ID))
                AddBonusCardsFromItem(item);
    }
}
