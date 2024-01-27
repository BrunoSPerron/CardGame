using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ExplorationScreen : BaseGameScreen
{
    public readonly List<CharacterWrapper> Survivors = new List<CharacterWrapper>();
    public readonly List<LocationWrapper> Destinations = new List<LocationWrapper>();

    public Card ExploreTarget;
    public Card SurviveTarget;

    public LocationWrapper Location;
    public ExplorationManager Manager;

    private BaseGameScreen eventScreen;
    private CharacterWrapper survivorDragged;

    public override void _Ready()
    {
        AddLocation();
        if (Location.Model.Encounters.Count != 0)
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
        Destinations.Add(location);
        if (Game.RemoveFromCleaner(location.Card) == CardCleanResponse.RECENT)
        {
            AddChild(location.Card);
            location.Card.LerpDeltaMultiplier = 3;
            location.Card.MoveToPosition(position);
        }
        else
        {
            DealOnBoard(location.Card, position, true);
        }
    }

    private void AddExploreOption()
    {
        ExploreTarget = CardFactory.CreateExploreCard();

        //TODO Placement based on window size
        DealOnBoard(ExploreTarget, new Vector2(450, 200), true);
        ExploreTarget.CostCounter.SetMax(Location.Model.SurviveActionCost);
    }

    private void AddLocation()
    {
        if (Game.RemoveFromCleaner(Location.Card) == CardCleanResponse.RECENT)
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
        DealOnBoard(SurviveTarget, new Vector2(226, 200), true);
        SurviveTarget.CostCounter.SetMax(Location.Model.SurviveActionCost);
    }

    private void AddSurvivors()
    {
        List<CharacterWrapper> survivorsToAdd = new List<CharacterWrapper>();
        List<CharacterWrapper> preDealtSurvivors = new List<CharacterWrapper>();

        foreach (CharacterWrapper survivor in Game.Survivors)
        {
            if (survivor.WorldPosition == Location.WorldPosition.Coord)
            {
                if (Game.RemoveFromCleaner(survivor.Card) == CardCleanResponse.RECENT)
                {
                    preDealtSurvivors.Add(survivor);
                }
                else
                {
                    survivorsToAdd.Add(survivor);
                    Survivors.Add(survivor);
                }
            }
        }

        foreach (CharacterWrapper survivor in preDealtSurvivors)
            Survivors.Add(survivor);

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

        foreach (CharacterWrapper survivor in Survivors)
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

        foreach (LocationWrapper location in Destinations)
        {
            RemoveChild(location.Card);
            Game.CleanCard(location.Card);
        }
    }

    public override void Destroy()
    {
        eventScreen?.Destroy();
        eventScreen = null;
        Clean();
        QueueFree();
    }

    public override void DisableScreen()
    {
        Manager.DisableScreen();
    }

    public override void EnableScreen()
    {
        Manager.EnableScreen();
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

    public void OnCarddragEnd(Card OriginCard, Card StackTarget)
    {
        if (StackTarget == null)
        {
            Survivors.Add(survivorDragged);
            StackSurvivors();
            survivorDragged = null;
            return;
        }
        CardManager.StackCards(new List<Card> { OriginCard }, StackTarget.Position);

        if (StackTarget == ExploreTarget)
        {
            Survivors.Add(survivorDragged);
            CharacterWrapper character = Game.charactersByCardId[OriginCard.GetInstanceId()];
            SurvivorEvent_Explore(character);
        }
        else if (StackTarget == SurviveTarget)
        {
            Survivors.Add(survivorDragged);
            CharacterWrapper character = Game.charactersByCardId[OriginCard.GetInstanceId()];
            SurvivorEvent_Field(character);
        }
        else if (Destinations.Exists(d => d.Card == StackTarget))
        {
            LocationWrapper destination = Game.locationsByCardId[StackTarget.GetInstanceId()];
            CharacterWrapper character = Game.charactersByCardId[OriginCard.GetInstanceId()];
            SurvivorEvent_Move(character, destination);
        }
        else
        {
            Survivors.Add(survivorDragged);
            StackSurvivors();
        }
        survivorDragged = null;
    }

    public void OnCarddragStart(Card card)
    {
        survivorDragged = Game.charactersByCardId[card.GetInstanceId()];
        Survivors.Remove(survivorDragged);
        StackSurvivors();
    }

    public void StackSurvivors()
    {
        List<Card> cards = CardManager.GetCardsInCharacterWrappers(Survivors);
        CardManager.StackCards(cards, CONSTS.SCREEN_CENTER);
    }

    private void SurvivorEvent_Explore(CharacterWrapper character)
    {
        if (character.CurrentActionPoint >= Location.Model.ExploreCost)
        {
            DisableScreen();
            character.CurrentActionPoint -= Location.Model.ExploreCost;
            Random rand = new Random();
            List<EncounterModel> encounters = Location.Model.Encounters;
            EncounterModel encounter = encounters[rand.Next(0, encounters.Count)];
            EventPlayer eventPlayer = new EventPlayer(character, encounter.Steps, encounter);
            eventPlayer.Connect("OnEventEnd", this, "SurvivorEvent_Field_End");
            AddChild(eventPlayer);
        }
        else
        {
            StackSurvivors();
        }
    }

    private void SurvivorEvent_Field(CharacterWrapper character)
    {
        if (character.CurrentActionPoint >= Location.Model.SurviveActionCost)
        {
            character.CurrentActionPoint -= Location.Model.SurviveActionCost;
            DisableScreen();
            eventScreen = new PlayFieldScreen() 
            {
                Character = character,
                Parent = this,
                Game = Game
            };
            AddChild(eventScreen);
        }
        else
        {
            StackSurvivors();
        }
    }

    public override void SurvivorEvent_Field_End()
    {
        base.SurvivorEvent_Field_End();
        eventScreen?.Destroy();
        eventScreen = null;
        EnableScreen();
        StackSurvivors();
        Manager.OnCharacterActionOver();
    }

    public void SurvivorEvent_Move(CharacterWrapper character, LocationWrapper destination)
    {
        if (character.CurrentActionPoint >= destination.Model.TravelCost)
        {
            character.CurrentActionPoint -= destination.Model.TravelCost;
            character.WorldPosition = destination.WorldPosition.Coord;
            Manager.MoveToHex(destination.WorldPosition.Coord);
            Manager.OnCharacterActionOver();
        }
        else
        {
            Survivors.Add(survivorDragged);
            StackSurvivors();
        }
    }
}
