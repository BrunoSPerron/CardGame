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

            combatDeckManager = new CombatDeckManager(this, value.CombatDeck);
            fieldDeckManager = new FieldDeckManager(this, value.FieldDeck);
            UpdateBonusDeckCards();
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
                    this, model.CombatDeck);
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
                    this, model.FieldDeck);
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

    public int CurrentHitPoint
    {
        get => model.CurrentHitPoint;
        set
        {
            if (model.CurrentHitPoint != value)
            {
                Card.Front.GetNode<IconCounter>("LifeCounter").SetCurrent(value);
                model.CurrentHitPoint = value;
            }
        }
    }

    public List<ItemModel> Items => model.Items;

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
        set
        {
            if (model.HitPoint != value)
            {
                Card.Front.GetNode<IconCounter>("LifeCounter").SetMax(value);
                model.HitPoint = value;
            }
        }
    }

    public string Name
    {
        get { return model.Name; }
    }

    public int Power
    {
        get => model.Power;
        set
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

    public override void DisableButtons()
    {
        base.DisableButtons();
        CardButtonBase button = Card.Front.GetNode<CardButtonBase>("CombatDeckButton");
        button.Disable();

        button = Card.Front.GetNode<CardButtonBase>("FieldDeckButton");
        button.Disable();

        button = Card.Front.GetNode<CardButtonBase>("InventoryButton");
        button.Disable();
    }

    public override void EnableButtons()
    {
        base.EnableButtons();
        CardButtonBase button = Card.Front.GetNode<CardButtonBase>("CombatDeckButton");
        button.Enable();

        button = Card.Front.GetNode<CardButtonBase>("FieldDeckButton");
        button.Enable();

        button = Card.Front.GetNode<CardButtonBase>("InventoryButton");
        button.Enable();
    }

    private void UpdateBonusDeckCards()
    {
        fieldDeckManager.UpdateBonusCards(Items);
        combatDeckManager.UpdateBonusCards(Items);
    }
}
