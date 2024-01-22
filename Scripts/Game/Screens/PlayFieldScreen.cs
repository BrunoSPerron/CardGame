using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayFieldScreen : BaseGameScreen
{
    public FieldCardWrapper CardBeingPaidFor = null;
    public readonly List<FieldCardWrapper> CardsUsedAsPayment = new List<FieldCardWrapper>();
    public CharacterWrapper Character;
    public FieldDeckManager Deck;
    public ExplorationScreen Parent;
    public Hand Hand;

    private Card playTarget;
    private readonly Dictionary<ulong, FieldCardWrapper> wrapperByCardIds
        = new Dictionary<ulong, FieldCardWrapper>();

    //===Overrides===

    public override void _Ready()
    {
        ZIndex = CardManager.NumberOfCards;
        Hand = new Hand()
        {
            Game = Game,
        };
        Deck = Character.FieldDeckManager;
        AddChild(Hand);
        Hand.Position = new Vector2(CONSTS.SCREEN_CENTER.x, CONSTS.SCREEN_SIZE.y - 25);

        playTarget = CardFactory.CreatePlayTarget();
        AddChild(playTarget);
        Deck.Reset();
        DrawNewHand();
    }

    public override void Destroy()
    {
        if (CardBeingPaidFor != null)
            RemoveChild(CardBeingPaidFor.Card);
        foreach (FieldCardWrapper wrapper in CardsUsedAsPayment)
            RemoveChild(wrapper.Card);
        Hand.Destroy();
        QueueFree();
    }

    //===Uniques Methods===

    private void DrawNewHand()
    {
        foreach (BaseCardWrapper wrapper in Hand.Cards)
        {
            wrapper.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
            wrapper.Card.Disconnect("OnDragStart", this, "OnCarddragStart");
        }
        Hand.DiscardHand();
        List<FieldCardWrapper> cards = Deck.DrawMultiple(5);

        foreach (FieldCardWrapper wrapper in cards)
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
        foreach (FieldCardWrapper wrapper in CardsUsedAsPayment)
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

    //===Triggers===

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        if (StackTarget == null)
            return;

        if (StackTarget == playTarget)
        {
            FieldCardWrapper wrapper = wrapperByCardIds[OriginCard.GetInstanceId()];
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
    }

    public void OnCarddragStart(Card card)
    {
        if (CardBeingPaidFor?.Card == card)
        {
            CardHandlingControl.StopDragging(false);
            ReturnCardBeingPaidForToHand();
            ReturnCardsUsedAsCostToHand();
        }
        else
        {
            FieldCardWrapper wrapper = wrapperByCardIds[card.GetInstanceId()];
            if (CardsUsedAsPayment.Contains(wrapper))
            {
                CardHandlingControl.StopDragging(false);
                ReturnCardToHand(wrapper);
                CardsUsedAsPayment.Remove(wrapper);
            }
        }
    }

    public void OnCardEffectOver()
    {
        //TODO Check if another card can be played
        Destroy();
        Parent.SurvivorEvent_Field_End();
    }
}
