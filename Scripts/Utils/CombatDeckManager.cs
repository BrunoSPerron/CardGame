using Godot;
using System;
using System.Collections.Generic;

public class CombatDeckManager : BaseDeckManager
{
    public override BaseDeckModel BaseModel => Model;

    public CombatDeckModel Model { get; private set; }
    public CombatDeckManager(CombatDeckModel deckInfo)
    {
        Model = deckInfo;
    }

    public void AddBonusCardsFromItem(ItemModel item)
    {
        for (int i = 0; i < item.CombatCards.Length; i++)
        {
            string cardName = item.CombatCards[i];
            CombatCardModel cardModel = JsonLoader.GetCombatCardModel(item.Mod, cardName);
            Model.BonusCards.Add(new BonusCombatCardModel()
            {
                Card = cardModel,
                ItemSourceId = item.ID,
                OverrideCardIndex = 0,
                Priority = i
            });
        }
    }

    public override List<BaseCardWrapper> GetSortedBaseDeck()
    {
        List<BaseCardWrapper> wrappers = new List<BaseCardWrapper>();

        foreach (CombatCardModel model in Model.BaseDeck)
            wrappers.Add(CardFactory.CreateCardFromCombatCardModel(model));

        return wrappers;
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
