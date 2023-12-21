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

            CombatDeck = new CombatDeckWrapper(value.CombatDeck);
            FieldDeck = new FieldDeckWrapper(value.FieldDeck);
        }
    }

    private CombatDeckWrapper combatDeck;
    public CombatDeckWrapper CombatDeck
    {
        get
        {
            if (combatDeck == null)
                combatDeck = CreateNewCombatDeck();
            return combatDeck;
        }
        set => combatDeck = value;
    }

    private FieldDeckWrapper fieldDeck;
    public FieldDeckWrapper FieldDeck
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

    private CombatDeckWrapper CreateNewCombatDeck()
    {
        GD.PrintErr("Character wrapper error: No combat deck for character '"
            + model.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewCombatDeckWrapper();
    }

    private FieldDeckWrapper CreateNewFieldDeck()
    {
        GD.PrintErr("Character wrapper error: No field deck for character '"
            + model.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewFieldDeckWrapper();
    }
}
