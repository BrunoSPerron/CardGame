using Godot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
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
            string cardName = item.FieldCards[i];
            FieldCardModel cardModel = JsonLoader.GetFieldCardModel(item.Mod, cardName);
            Model.BonusCards.Add(new BonusFieldCardModel()
            {
                Card = cardModel,
                ItemSourceId = item.ID,
                OverrideCardIndex = 0,
                Priority = i
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
            return CardFactory.CreateCardFromFieldCardModel(new FieldCardModel());
        }

        if (deck.Count == 0)
        {
            if (discardPile.Count == 0)
                return null;
            Shuffle();
        }

        hand.Add(deck[0]);
        deck.RemoveAt(0);
        FieldCardWrapper wrapper = deck[0];
        return wrapper;
    }

    public List<FieldCardWrapper> DrawMultiple(int amount)
    {
        List<FieldCardWrapper> wrappers = new List<FieldCardWrapper>();
        int i = 0;
        while (i < amount)
        {
            wrappers.Add(Draw());
            i++;
        }

        return wrappers;
    }

    public override List<BaseBonusCardWrapper> GetBonusCards()
    {
        List<BaseBonusCardWrapper> wrappers = new List<BaseBonusCardWrapper>();
        foreach (BonusFieldCardModel cardModel in Model.BonusCards)
        {
            BaseBonusCardWrapper wrapper = CardFactory.CreateCardFromBonusFieldCard(cardModel);
            wrappers.Add(wrapper);
        }
        return wrappers;
    }

    public override List<BaseCardWrapper> GetSortedBaseDeck()
    {
        List<BaseCardWrapper> wrappers = new List<BaseCardWrapper>();

        foreach (FieldCardModel model in Model.FieldDeck)
            wrappers.Add(CardFactory.CreateCardFromFieldCardModel(model));

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
                wrappedDeck.Add(CardFactory.CreateCardFromFieldCardModel(cardToUse.Card));
            }
            else
            {
                wrappedDeck.Add(CardFactory.CreateCardFromFieldCardModel(Model.FieldDeck[i]));
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
