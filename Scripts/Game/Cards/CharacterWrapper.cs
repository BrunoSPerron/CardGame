using Godot;
using System;
using System.Collections.Generic;

public class CharacterWrapper : BaseCardWrapper
{
    private DeckWrapper combatDeck;
    public DeckWrapper CombatDeck
    {
        get
        {
            if (combatDeck == null)
                combatDeck = CreateNewCombatDeck();
            return combatDeck;
        }
        set => combatDeck = value;
    }

    private DeckWrapper fieldDeck;
    public DeckWrapper FieldDeck
    {
        get
        {
            if (fieldDeck == null)
                fieldDeck = CreateNewFieldDeck();
            return fieldDeck;
        }
        set => fieldDeck = value;
    }

    private CharacterModel info;
    public CharacterModel Info
    {
        get => info;
        private set
        {
            info = value;
            ActionPoint = value.ActionPoint;
            CombatDeck = new DeckWrapper(value.CombatDeck);
            HitPoint = value.CurrentHitPoint;
            Power = value.Power;
            Card.SetLabel(value.Name);
        }
    }

    // ===== IconCounter related attributes =====

    private int actionPoint;
    public int ActionPoint
    {
        get => actionPoint;
        set
        {
            if (actionPoint != value)
            {
                Card.Front.GetNode<IconCounter>("ActionCounter").SetAmount(value);
                actionPoint = value;
            }
        }
    }

    private int hitPoint;
    public int HitPoint
    {
        get => hitPoint;
        set
        {
            if (hitPoint != value)
            {
                Card.Front.GetNode<IconCounter>("LifeCounter").SetAmount(value);
                hitPoint = value;
            }
        }
    }

    private int power;
    public int Power
    {
        get => power;
        set
        {
            if (power != value)
            {
                Card.Front.GetNode<IconCounter>("PowerCounter").SetAmount(value);
                power = value;
            }
        }
    }

    // ===== Methods unique to this class =====

    public CharacterWrapper(Card card, CharacterModel character)
    {
        Card = card;
        Info = character;
    }

    private DeckWrapper CreateNewCombatDeck()
    {
        GD.PrintErr("Character wrapper error: No combat deck for character '"
            + info.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewCombatDeckWrapper();
    }

    private DeckWrapper CreateNewFieldDeck()
    {
        GD.PrintErr("Character wrapper error: No field deck for character '"
            + info.Name + "'. Received bad generic deck.");
        return DeckFactory.CreateNewFieldDeckWrapper();
    }
}
