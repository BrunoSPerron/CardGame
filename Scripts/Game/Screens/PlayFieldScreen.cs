using Godot;
using System;
using System.Collections.Generic;

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

    public override void _Ready()
    {
        Hand = new Hand()
        {
            Game = Game,
        };
        Deck = Character.FieldDeck;
        AddChild(Hand);
        Hand.Position = new Vector2(CONSTS.SCREEN_CENTER.x, CONSTS.SCREEN_SIZE.y - 25);

        playTarget = CardFactory.CreatePlayTarget();
        AddChild(playTarget);
        DrawNewHand();
    }

    private void DrawNewHand()
    {
        foreach (BaseCardWrapper wrapper in Hand.Cards)
            wrapper.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
        Hand.DiscardHand();
        FieldCardWrapper[] cards = Deck.DrawMultiple(5);
        foreach (FieldCardWrapper wrapper in cards)
        {
            wrapperByCardIds.Add(wrapper.Card.GetInstanceId(), wrapper);
            wrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
        }

        Hand.AddCards(cards);
    }

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
                Hand.RemoveCard(CardBeingPaidFor);
                CardBeingPaidFor.Card.Position += Hand.Position;
                AddChild(CardBeingPaidFor.Card);
                CardBeingPaidFor.Card.MoveToPosition(new Vector2(200, 150));
            }
            else
            {
                Hand.RemoveCard(wrapper);
                CardsUsedAsPayment.Add(wrapper);
                wrapper.Card.Position += Hand.Position;
                AddChild(wrapper.Card);
                wrapper.Card.MoveToPosition(new Vector2(450, 200));
                if (CardsUsedAsPayment.Count >= CardBeingPaidFor.Model.Cost)
                    PlayCardBeingPaid();
            }
        }
    }

    private void PlayCardBeingPaid()
    {
        Parent.SurvivorEvent_Field_End();
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
}
