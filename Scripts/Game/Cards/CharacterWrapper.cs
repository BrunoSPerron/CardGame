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

            CombatDeck = new CombatDeckManager(value.CombatDeck);
            FieldDeck = new FieldDeckManager(value.FieldDeck);
        }
    }

    private CombatDeckManager combatDeck;
    public CombatDeckManager CombatDeck
    {
        get
        {
            if (combatDeck == null)
                combatDeck = CreateNewCombatDeck();
            return combatDeck;
        }
        set => combatDeck = value;
    }

    private FieldDeckManager fieldDeck;
    public FieldDeckManager FieldDeck
    {
        get
        {
            if (fieldDeck == null)
                fieldDeck = CreateNewFieldDeck();
            return fieldDeck;
        }
        set => fieldDeck = value;
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
                AddBonusCardsToFieldDeck(item);
                AddBonusCardsToCombatDeck(item);
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

    private void AddBonusCardsToCombatDeck(ItemModel item)
    {
        //TODO
    }

    private void AddBonusCardsToFieldDeck(ItemModel item)
    {
        for (int i = 0; i < item.FieldCards.Length; i++)
        {
            string cardName = item.FieldCards[i];
            FieldCardModel cardModel = JsonLoader.GetFieldCardModel(item.Mod, cardName);
            model.FieldDeck.BonusCards.Add(new ItemFieldCard()
            {
                Card = cardModel,
                ItemSourceId = item.ID,
                OverrideCardIndex = 0,
                Priority = i
            });
        }
    }

    public void AddItem(ItemModel item)
    {
        model.Items.Add(item);
    }

    private CombatDeckManager CreateNewCombatDeck()
    {
        //TODO Check if there's a deck in the model
        GD.PrintErr("Character wrapper error: No combat deck for character '"
            + model.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewCombatDeckWrapper();
    }

    private FieldDeckManager CreateNewFieldDeck()
    {
        //TODO Check if there's a deck in the model
        GD.PrintErr("Character wrapper error: No field deck for character '"
            + model.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewFieldDeckWrapper();
    }

    private void UpdateBonusDeckCards()
    {

    }
}
