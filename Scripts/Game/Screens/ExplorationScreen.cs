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
        AddFieldDeckOption();

        AddDestinations();
        StackSurvivors();
    }

    private void AddDestinations()
    {
        foreach (HexLink link in LocationWrapper.HexLocation.Openings)
        {
            Vector2Int worldPosition = LocationWrapper.HexLocation.HexPosition;
            switch (link)
            {
                case HexLink.TOPLEFT:
                    worldPosition.y++;
                    break;
                case HexLink.TOPRIGHT:
                    worldPosition.x++;
                    worldPosition.y++;
                    break;
                case HexLink.LEFT:
                    worldPosition.x--;
                    break;
                case HexLink.RIGHT:
                    worldPosition.x++;
                    break;
                case HexLink.BOTTOMLEFT:
                    worldPosition.x--;
                    worldPosition.y--;
                    break;
                case HexLink.BOTTOMRIGHT:
                    worldPosition.y--;
                    break;
            }

            if (Game.LocationsByPosition.ContainsKey(worldPosition)                )
            {
                LocationWrapper destination = Game.LocationsByPosition[worldPosition];
                HexLink reverseLink = (HexLink)(((int)link + 3) % 6);
                if (destination.HexLocation.Openings.Contains(reverseLink))
                {
                    Vector2 postion = GetDestinationScreenPosition(link);
                    AddDestination(destination, postion);
                }
            }
        }
    }

    private void AddDestination(LocationWrapper locationWrapper, Vector2 position)
    {
        DealOnBoard(locationWrapper.Card, position, 0, true);
    }

    private void AddExploreOption()
    {
        ExploreTarget = CardFactory.CreateExploreCard();

        //TODO Placement based on window size
        DealOnBoard(ExploreTarget, new Vector2(450, 200), 0, true);
    }

    private void AddFieldDeckOption()
    {
        ExploreTarget = CardFactory.CreateUseFieldDeckCard();
        DealOnBoard(ExploreTarget, new Vector2(226, 200), 0, true);

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

    private Vector2 GetDestinationScreenPosition(HexLink link)
    {
        // TODO dynamic based on window size
        switch (link)
        {
            case HexLink.TOPLEFT:
                return new Vector2(176, 70);
            case HexLink.TOPRIGHT:
                return new Vector2(500, 70);
            case HexLink.LEFT:
                return new Vector2(114, 200);
            case HexLink.RIGHT:
                return new Vector2(562, 200);
            case HexLink.BOTTOMLEFT:
                return new Vector2(176, 330);
            case HexLink.BOTTOMRIGHT:
                return new Vector2(500, 330);
        }
        return new Vector2(70, 70);
    }

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        survivors.Add(survivorDragged);
        survivorDragged = null;

        if (StackTarget != null)
        {
            if (StackTarget == ExploreTarget)
            {
                CardManager.StackCards(
                    new List<Card> { OriginCard }, StackTarget.Position);

                //TODO Trigger Exploration Event
            }
            /*else if (StackTarget == FieldTarget)
            {
                TODO
                    - Create FieldTarget
                    - Trigger Rest Event
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
