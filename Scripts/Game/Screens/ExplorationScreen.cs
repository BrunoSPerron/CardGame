using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ExplorationScreen : BaseGameScreen
{
    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();
    private readonly List<LocationWrapper> destinations = new List<LocationWrapper>();

    private CharacterWrapper survivorDragged;

    public Card ExploreTarget;
    public Card SurviveTarget;

    public LocationWrapper Location;
    public ExplorationManager Manager;

    public override void _Ready()
    {
        AddLocation();
        if (Location.Model.ExplorationDeck.Length != 0)
            AddExploreOption();
        AddSurviveOption();

        AddDestinations();
        AddSurvivors();
    }

    private void AddDestinations()
    {
        foreach (HexLink link in Location.WorldPosition.Openings)
        {
            Vector2Int worldPosition = Location.WorldPosition.Coord;
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

            if (Game.LocationsByPosition.ContainsKey(worldPosition))
            {
                LocationWrapper destination = Game.LocationsByPosition[worldPosition];
                HexLink reverseLink = (HexLink)(((int)link + 3) % 6);
                if (destination.WorldPosition.Openings.Contains(reverseLink))
                {
                    Vector2 postion = GetDestinationScreenPosition(link);
                    AddDestination(destination, postion);
                }
            }
        }
    }

    private void AddDestination(LocationWrapper location, Vector2 position)
    {
        destinations.Add(location);
        if (Game.RemoveCardFromCleaner(location.Card))
        {
            AddChild(location.Card);
            location.Card.LerpDeltaMultiplier = 3;
            location.Card.MoveToPosition(position);
        }
        else
        {
            DealOnBoard(location.Card, position, 0, true);
        }
    }

    private void AddExploreOption()
    {
        ExploreTarget = CardFactory.CreateExploreCard();

        //TODO Placement based on window size
        DealOnBoard(ExploreTarget, new Vector2(450, 200), 0, true);
    }

    private void AddLocation()
    {
        //TODO Position based on screen size
        if (Game.RemoveCardFromCleaner(Location.Card))
        {
            AddChild(Location.Card);
            Location.Card.LerpDeltaMultiplier = 3;
            Location.Card.MoveToPosition(CONSTS.SCREEN_CENTER);
        }
        else
        {
            DealOnBoard(Location.Card, CONSTS.SCREEN_CENTER);
        }
    }

    private void AddSurviveOption()
    {
        SurviveTarget = CardFactory.CreateSurviveCard();
        DealOnBoard(SurviveTarget, new Vector2(226, 200), 0, true);
    }

    private void AddSurvivors()
    {
        // Note: The CardDealer need each card `Target` to be the correct one
        //  at the time CardDealer.AddSurvivors is called. This is done in
        //  this.StackSurvivors() and is why they are first added to this list
        List<CharacterWrapper> survivorsToAdd = new List<CharacterWrapper>();
        List<CharacterWrapper> preDealtSurvivors = new List<CharacterWrapper>();

        foreach (CharacterWrapper survivor in Game.Survivors)
        {
            if (survivor.WorldPosition == Location.WorldPosition.Coord)
            {
                if (Game.RemoveCardFromCleaner(survivor.Card))
                {
                    preDealtSurvivors.Add(survivor);
                }
                else
                {
                    survivorsToAdd.Add(survivor);
                    survivors.Add(survivor);
                }
            }
        }

        foreach (CharacterWrapper survivor in preDealtSurvivors)
            survivors.Add(survivor);

        StackSurvivors();

        foreach (CharacterWrapper survivor in survivorsToAdd)
            AddSurvivor(survivor);

        foreach (CharacterWrapper survivor in preDealtSurvivors)
            AddSurvivor(survivor, false);
    }

    private void AddSurvivor(CharacterWrapper survivor, bool dealOnBoard = true)
    {
        survivor.Card.Connect("OnDragStart", this, "OnCarddragStart");
        survivor.Card.Connect("OnDragEnd", this, "OnCarddragEnd");
        if (dealOnBoard)
        {
            DealOnBoard(survivor.Card, survivor.Card.Target);
        }
        else
        {
            AddChild(survivor.Card);
            survivor.Card.ZIndex = CONSTS.MAX_Z_INDEX;
        }
    }

    public void Clean()
    {
        RemoveChild(Location.Card);
        Game.CleanCard(Location.Card);

        foreach (CharacterWrapper survivor in survivors)
        {
            survivor.Card.Disconnect("OnDragStart", this, "OnCarddragStart");
            survivor.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
            RemoveChild(survivor.Card);
            Game.CleanCard(survivor.Card);
        }

        if (survivorDragged != null)
        {
            RemoveChild(survivorDragged.Card);
            Game.CleanCard(survivorDragged.Card);
        }

        foreach (LocationWrapper location in destinations)
        {
            RemoveChild(location.Card);
            Game.CleanCard(location.Card);
        }

        if (SurviveTarget != null)
        {
            RemoveChild(SurviveTarget);
            Game.CleanCard(SurviveTarget);
        }

        if (ExploreTarget != null)
        {
            RemoveChild(ExploreTarget);
            Game.CleanCard(ExploreTarget);
        }
    }

    public override void Destroy()
    {
        foreach (CharacterWrapper survivor in survivors)
        {
            survivor.Card.Disconnect("OnDragStart", this, "OnCarddragStart");
            survivor.Card.Disconnect("OnDragEnd", this, "OnCarddragEnd");
            RemoveChild(survivor.Card);
        }

        foreach (LocationWrapper location in destinations)
            RemoveChild(location.Card);

        RemoveChild(Location.Card);
        QueueFree();
    }

    public override void DisableScreen()
    {
        base.DisableScreen();
        foreach (CharacterWrapper character in survivors)
            character.Card.IsDraggable = false;
    }

    public override void EnableScreen()
    {
        base.EnableScreen();
        foreach (CharacterWrapper character in survivors)
            character.Card.IsDraggable = true;
    }

    private Vector2 GetDestinationScreenPosition(HexLink link)
    {
        // TODO dynamic based on window size
        switch (link)
        {
            case HexLink.TOPLEFT:
                return new Vector2(170, 70);
            case HexLink.TOPRIGHT:
                return new Vector2(504, 70);
            case HexLink.LEFT:
                return new Vector2(114, 200);
            case HexLink.RIGHT:
                return new Vector2(562, 200);
            case HexLink.BOTTOMLEFT:
                return new Vector2(170, 330);
            case HexLink.BOTTOMRIGHT:
                return new Vector2(504, 330);
        }
        return new Vector2(70, 70);
    }

    public void Move(CharacterWrapper character, LocationWrapper destination)
    {
        if (character.CurrentActionPoint >= destination.Model.TravelCost)
        {
            character.CurrentActionPoint -= destination.Model.TravelCost;
            character.WorldPosition = destination.WorldPosition.Coord;
            Manager.MoveToHex(destination.WorldPosition.Coord);
        }
        else
        {
            survivors.Add(survivorDragged);
            StackSurvivors();
        }
    }

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        if (StackTarget == null)
        {
            survivors.Add(survivorDragged);
            StackSurvivors();
            survivorDragged = null;
            return;
        }
        CardManager.StackCards(new List<Card> { OriginCard }, StackTarget.Position);

        if (StackTarget == ExploreTarget)
        {
            survivors.Add(survivorDragged);
            //TODO Trigger Exploration Event
        }
        else if (StackTarget == SurviveTarget)
        {
            survivors.Add(survivorDragged);
            //TODO Trigger Field Event
        }
        else if (destinations.Exists(d => d.Card == StackTarget))
        {
            LocationWrapper destination = Game.locationsByCardId[StackTarget.GetInstanceId()];
            CharacterWrapper character = Game.charactersByCardId[OriginCard.GetInstanceId()];
            Move(character, destination);
        }
        else
        {
            survivors.Add(survivorDragged);
            StackSurvivors();
        }
        survivorDragged = null;
    }

    public void OnCarddragStart(Card card)
    {
        survivorDragged = Game.charactersByCardId[card.GetInstanceId()];
        survivors.Remove(survivorDragged);
        StackSurvivors();
    }

    public void StackSurvivors()
    {
        List<Card> cards = CardManager.GetCardsInCharacterWrappers(survivors);
        CardManager.StackCards(cards, CONSTS.SCREEN_CENTER);
    }
}
