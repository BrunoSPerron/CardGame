using Godot;
using System;
using System.Collections.Generic;

public class CharacterWrapper : BaseCardWrapper
{
    private CharacterModel model;
    public CharacterModel Model
    {
        set
        {
            model = value;

            Card.SetLabel(value.Name);
            Card.Front.GetNode<IconCounter>("ActionCounter").SetMax(value.ActionPoint);
            Card.Front.GetNode<IconCounter>("LifeCounter").SetMax(value.HitPoint);
            Card.Front.GetNode<IconCounter>("PowerCounter").SetMax(value.Power);

            combatDeckManager = new CombatDeckManager(value.CombatDeck);
            fieldDeckManager = new FieldDeckManager(value.FieldDeck);
        }
    }

    private CombatDeckManager combatDeckManager;
    public CombatDeckManager CombatDeckManager
    {
        get
        {
            if (combatDeckManager == null)
            {
                combatDeckManager = DeckFactory.CreateCombatDeckManagerFromModel(
                    model.CombatDeck);
            }
            return combatDeckManager;
        }
        set => combatDeckManager = value;
    }

    private FieldDeckManager fieldDeckManager;
    public FieldDeckManager FieldDeckManager
    {
        get
        {
            if (fieldDeckManager == null)
            {
                fieldDeckManager = DeckFactory.CreateFieldDeckManagerFromModel(
                    model.FieldDeck);
            }
            return fieldDeckManager;
        }
        set => fieldDeckManager = value;
    }

    // ===== Model related attributes =====

    public int CurrentActionPoint
    {
        get => model.CurrentActionPoint;
        set
        {
            if (model.CurrentActionPoint != value)
            {
                Card.Front.GetNode<IconCounter>("ActionCounter").SetCurrent(value);
                model.CurrentActionPoint = value;
            }
        }
    }

    public List<ItemModel> Items
    {
        get => model.Items;
        set
        {
            model.Items = value;
            foreach (ItemModel item in model.Items)
            {
                fieldDeckManager.AddBonusCardsFromItem(item);
                combatDeckManager.AddBonusCardsFromItem(item);
            }
        }
    }

    public int MaxActionPoint
    {
        get => model.ActionPoint;
        set
        {
            if (model.ActionPoint != value)
            {
                Card.Front.GetNode<IconCounter>("ActionCounter").SetMax(value);
                model.ActionPoint = value;
            }
        }
    }

    public int MaxHitPoint
    {
        get => model.HitPoint;
        private set
        {
            if (model.HitPoint != value)
            {
                Card.Front.GetNode<IconCounter>("LifeCounter").SetMax(value);
                model.HitPoint = value;
            }
        }
    }

    public int Power
    {
        get => model.Power;
        private set
        {
            if (model.Power != value)
            {
                Card.Front.GetNode<IconCounter>("PowerCounter").SetMax(value);
                model.Power = value;
            }
        }
    }

    public Vector2Int WorldPosition
    {
        get => model.WorldPosition;
        set => model.WorldPosition = value;
    }

    // ===== Methods unique to this class =====

    public CharacterWrapper(Card card, CharacterModel character)
    {
        Card = card;
        Model = character;
    }

    public void AddItem(ItemModel item)
    {
        model.Items.Add(item);
        UpdateBonusDeckCards();
    }

    private void UpdateBonusDeckCards()
    {
        fieldDeckManager.UpdateBonusCards(Items);
        combatDeckManager.UpdateBonusCards(Items);
    }
}
