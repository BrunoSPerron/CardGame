using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CombatScreen : BaseGameScreen
{
    public CombatCardWrapper CardBeingPaidFor = null;
    public readonly List<CombatCardWrapper> CardsUsedAsPayment = new List<CombatCardWrapper>();
    public CharacterWrapper Character;
    public CombatDeckManager Deck;
    public Hand Hand;
    public BaseGameScreen Parent;
    public bool ResetDeckOnReady = true;

    private Card playTarget;
    private readonly Dictionary<ulong, CombatCardWrapper> wrapperByCardIds
        = new Dictionary<ulong, CombatCardWrapper>();

    //===Overrides===

    public override void _Ready()
    {
        ZIndex = CardManager.NumberOfCards;
        Hand = new Hand()
        {
            Game = Game,
        };
        Deck = Character.CombatDeckManager;
        AddChild(Hand);
        Hand.Position = new Vector2(CONSTS.SCREEN_CENTER.x, CONSTS.SCREEN_SIZE.y - 25);

        playTarget = CardFactory.CreatePlayTarget();
        AddChild(playTarget);
        if (ResetDeckOnReady)
        {
            Deck.Reset();
            DrawNewHand();
        }
        else
        {
            UpdateHandFromManager();
        }
    }

    public override void Destroy()
    {
        if (CardBeingPaidFor != null)
        {
            RemoveChild(CardBeingPaidFor.Card);
            Game.CleanCard(CardBeingPaidFor.Card, true);
        }
        foreach (CombatCardWrapper wrapper in CardsUsedAsPayment)
        {
            RemoveChild(wrapper.Card);
            Game.CleanCard(wrapper.Card, true);
        }
        Hand.Destroy();
        QueueFree();
    }

    //===Uniques Methods===

    private void DrawNewHand()
    {
        foreach (BaseCardWrapper wrapper in Hand)
        {
            wrapper.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
            wrapper.Card.Disconnect("OnDragStart", this, "OnCarddragStart");
        }
        Hand.DiscardHand();
        List<CombatCardWrapper> cards = Deck.DrawMultiple(CONSTS.COMBAT_HAND_SIZE);

        foreach (CombatCardWrapper wrapper in cards)
        {
            if (wrapper == null) continue;

            wrapperByCardIds.Add(wrapper.Card.GetInstanceId(), wrapper);
            wrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
            wrapper.Card.Connect("OnDragStart", this, "OnCarddragStart");
            Hand.AddCard(wrapper);
        }
    }

    public void PlayCardBeingPaid()
    {
        EventPlayer eventPlayer = new EventPlayer(Character,
            CardBeingPaidFor.Model.Effects, CardBeingPaidFor.Model);
        eventPlayer.Connect("OnEventEnd", this, "OnCardEffectOver");
        AddChild(eventPlayer);
    }

    public void ReturnCardBeingPaidForToHand()
    {
        ReturnCardToHand(CardBeingPaidFor);
        CardBeingPaidFor = null;
    }

    public void ReturnCardsUsedAsCostToHand()
    {
        foreach (CombatCardWrapper wrapper in CardsUsedAsPayment)
            ReturnCardToHand(wrapper);
        CardsUsedAsPayment.Clear();
    }

    private void ReturnCardToHand(BaseCardWrapper wrapper)
    {
        wrapper.Card.Position -= Hand.Position;
        RemoveChild(wrapper.Card);
        Hand.AddCard(wrapper);
    }

    private void TakeCardFromHand(BaseCardWrapper wrapper)
    {
        Hand.RemoveCard(wrapper);
        wrapper.Card.Position += Hand.Position;
        AddChild(wrapper.Card);
    }

    private void UpdateHandFromManager()
    {
        foreach (BaseCardWrapper wrapper in Hand)
        {
            wrapper.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
            wrapper.Card.Disconnect("OnDragStart", this, "OnCarddragStart");
        }

        Hand.Clear();
        Hand.AddCards(Deck.GetHand());

        foreach (CombatCardWrapper wrapper in Hand.Cast<CombatCardWrapper>())
        {
            wrapperByCardIds.Add(wrapper.Card.GetInstanceId(), wrapper);
            wrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
            wrapper.Card.Connect("OnDragStart", this, "OnCarddragStart");
        }
    }

    private void UseCardInHand(CombatCardWrapper wrapper)
    {
        if (CardBeingPaidFor == null && wrapper.Model.Cost < Hand.Size)
        {
            CardBeingPaidFor = wrapper;
            TakeCardFromHand(CardBeingPaidFor);
            CardBeingPaidFor.Card.MoveToPosition(new Vector2(200, 150));
        }
        else
        {
            CardsUsedAsPayment.Add(wrapper);
            TakeCardFromHand(wrapper);
            wrapper.Card.MoveToPosition(new Vector2(450, 200));
            if (CardsUsedAsPayment.Count >= CardBeingPaidFor.Model.Cost)
                PlayCardBeingPaid();
        }
    }

    //===Triggers===

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        if (StackTarget == null)
            return;

        if (StackTarget == playTarget)
        {
            CombatCardWrapper wrapper = wrapperByCardIds[OriginCard.GetInstanceId()];
            UseCardInHand(wrapper);
        }
    }

    public void OnCarddragStart(Card card)
    {
        CombatCardWrapper wrapper = wrapperByCardIds[card.GetInstanceId()];
        if (CardBeingPaidFor == null)
        {
            if (Settings.Current.ClickToPlay)
            {
                CardHandlingControl.StopDragging(false);
                UseCardInHand(wrapper);
            }
        }
        else if (CardBeingPaidFor.Card == card)
        {
            CardHandlingControl.StopDragging(false);
            ReturnCardBeingPaidForToHand();
            ReturnCardsUsedAsCostToHand();
        }
        else if (CardsUsedAsPayment.Contains(wrapper))
        {
            CardHandlingControl.StopDragging(false);
            ReturnCardToHand(wrapper);
            CardsUsedAsPayment.Remove(wrapper);
        }
        else if (Settings.Current.ClickToPay)
        {
            CardHandlingControl.StopDragging(false);
            UseCardInHand(wrapper);
        }
    }

    public void OnCardEffectOver()
    {
        //TODO Check if another card can be played
        Parent.SurvivorEvent_Combat_End();
    }
}
