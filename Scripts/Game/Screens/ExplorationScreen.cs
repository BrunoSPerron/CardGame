using Godot;
using System;
using System.Collections.Generic;

public class ExplorationScreen : BaseGameScreen
{
    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();

    private CharacterWrapper survivorDragged;

    public LocationWrapper LocationWrapper;

    public Card ExploreTarget;

    public override void _Ready()
    {
        if (LocationWrapper.HexLocation.Location.ExplorationDeck.Length != 0)
            AddExploreOption();
        StackSurvivors();
    }

    private void AddExploreOption()
    {
        ExploreTarget = CardFactory.CreateExploreCard();

        //TODO Placement based on window size
        DealOnBoard(ExploreTarget, new Vector2(450, 200), 0, true);
    }

    public void AddSurvivor(CharacterWrapper cardWrapper)
    {
        cardWrapper.Card.Connect("OnDragStart", this, "OnCarddragStart");
        cardWrapper.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
        survivors.Add(cardWrapper);
        DealOnBoard(cardWrapper.Card, Vector2.Zero);
    }

    public override void Destroy()
    {
        foreach (CharacterWrapper character in survivors)
            RemoveChild(character.Card);
        RemoveChild(LocationWrapper.Card);
        QueueFree();
    }

    public override void DisableScreen()
    {
        base.DisableScreen();
    }

    public override void EnableScreen()
    {
        base.EnableScreen();
    }

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        survivors.Add(survivorDragged);
        survivorDragged = null;

        if (StackTarget != null)
        {
            if (StackTarget == ExploreTarget)
            {
                CardManager.StackCards(new List<Card> { OriginCard }, StackTarget.Position);

                //TODO Trigger Exploration Event
            }
            /*else if (StackTarget == FieldTarget)
            {
                TODO
                    - Create FieldTarget
                    - Create FieldTarget
            }
            */
            else
            {
                StackSurvivors();
            }
        }
        else
        {
            StackSurvivors();
        }
    }

    public void OnCarddragStart(Card card)
    {
        survivorDragged = Game.charactersByCardId[card.GetInstanceId()];
        survivors.Remove(survivorDragged);
        StackSurvivors();
    }

    public void SetLocation(LocationWrapper location)
    {
        if (LocationWrapper != null)
            RemoveChild(LocationWrapper.Card);
        LocationWrapper = location;

        //TODO Position based on screen size
        DealOnBoard(LocationWrapper.Card, new Vector2(338, 200));
    }

    public void StackSurvivors()
    {
        List<Card> cards = CardManager.GetCardsInCharacterWrappers(survivors);
        CardManager.StackCards(cards, LocationWrapper.Card.Target);
    }
}
