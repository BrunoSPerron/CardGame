using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    public GameStateModel State { get; private set; }
    public TurnCounter TurnCounter { get; private set; }

    private CardCleaner cardCleaner;
    private BaseGameScreen currentScene;

    // ===== Wrappers Storage =====

    public readonly List<CharacterWrapper> Survivors = new List<CharacterWrapper>();
    public readonly List<LocationWrapper> Locations = new List<LocationWrapper>();

    public readonly Dictionary<Vector2Int, LocationWrapper> LocationsByPosition
        = new Dictionary<Vector2Int, LocationWrapper>();
    public readonly Dictionary<ulong, CharacterWrapper> charactersByCardId
        = new Dictionary<ulong, CharacterWrapper>();
    public readonly Dictionary<ulong, LocationWrapper> locationsByCardId
        = new Dictionary<ulong, LocationWrapper>();

    // ===== Methods =====

    public void AddSurvivor(CharacterModel model)
    {
        CharacterWrapper wrapper = CardFactory.CreateCardFromCharacter(model);
        Survivors.Add(wrapper);
        charactersByCardId.Add(wrapper.Card.GetInstanceId(), wrapper);
        wrapper.Card.Connect("OnCombatDeckClick", this, "OnCombatDeckClick");
        wrapper.Card.Connect("OnFieldDeckClick", this, "OnFieldDeckClick");
        wrapper.Card.Connect("OnInventoryButtonClick", this, "OnInventoryClick");
    }

    public void AddLocation(WorldHexModel hexLocation)
    {
        LocationModel location = hexLocation.Location;
        LocationWrapper wrapper = null;
        if (location != null)
        {
            wrapper = CardFactory.CreateCardFromLocation(State.Mod, hexLocation);
            Locations.Add(wrapper);
            ulong instanceId = wrapper.Card.GetInstanceId();
            if (!locationsByCardId.ContainsKey(instanceId))
                locationsByCardId.Add(instanceId, wrapper);
        }

        if (LocationsByPosition.ContainsKey(hexLocation.Coord))
            LocationsByPosition[hexLocation.Coord] = wrapper;
        else
            LocationsByPosition.Add(hexLocation.Coord, wrapper);
    }

    /// <summary>
    /// Flip cards out of the screen
    /// </summary>
    /// <param name="card">Card with no godot parent</param>
    public void CleanCard(Card card)
    {
        card.ClearAnimations();
        if (cardCleaner == null)
        {
            cardCleaner = new CardCleaner();
            AddChild(cardCleaner);
            cardCleaner.Position = new Vector2(0, 0);
        }
        var instanceId = card.GetInstanceId();
        bool preserveCard = locationsByCardId.ContainsKey(instanceId)
            || charactersByCardId.ContainsKey(instanceId);

        cardCleaner.AddCardToClean(card, preserveCard);
    }

    public CardCleanResponse RemoveFromCleaner(Card card)
    {
        return cardCleaner?.RemoveCard(card) ?? CardCleanResponse.NONE;
    }

    public HexLink? GetHexDirection(LocationWrapper from, LocationWrapper to)
    {
        Vector2Int offset = to.WorldPosition.Coord - from.WorldPosition.Coord;

        if (offset.x == 1)
        {
            if (offset.y == 0)
                return HexLink.RIGHT;
            if (offset.y == 1)
                return HexLink.TOPRIGHT;
        }
        else if (offset.x == 0)
        {
            if (offset.y == 1)
                return HexLink.TOPLEFT; 
            if (offset.y == -1)
                return HexLink.BOTTOMRIGHT;
        }
        else if (offset.x == -1)
        {
            if (offset.y == -1)
                return HexLink.BOTTOMLEFT;
            if (offset.y == 0)
                return HexLink.LEFT;
        }

        return null;
    }

    public void InitializeScenario(string mod, string scenario)
    {
        State = new GameStateModel
        {
            Mod = mod,
            Scenario = scenario,
            Turn = 0,
        };
        OutsetModel outsetInfo = JsonLoader.GetOnsetModel(mod, scenario);

        WorldCreationModel worldCreationModel = JsonLoader.GetWorldCreationModel(
            mod, outsetInfo.World);
        State.Map = WorldCreator.CreateFromModel(worldCreationModel);

        foreach (WorldHexModel hexLocation in State.Map.Locations)
            AddLocation(hexLocation);

        foreach (string characterCreatorLogic in outsetInfo.StartingCharacters)
        {
            CharacterCreationModel model = JsonLoader.GetCharacterCreationModel(
                State.Mod, characterCreatorLogic);
            CharacterModel character = CharacterCreator.CreateFromModel(model);
            character.CurrentActionPoint = Mathf.Clamp(
                character.CurrentActionPoint, 1, character.ActionPoint);
            character.CurrentHitPoint = Mathf.Clamp(
                character.CurrentHitPoint, 1, character.HitPoint);

            character.WorldPosition = outsetInfo.StartingPosition;
            AddSurvivor(character);
        }

        TurnCounter = GetNode<TurnCounter>("TurnCounter");
        TurnCounter.UpdateFromGameState(State);
    }

    public void NextPhase()
    {
        int nextPhase = (int)State.PhaseOfDay + 1;
        if (nextPhase > 5)
        {
            State.Turn++;
            State.PhaseOfDay = Phase.DAWN;
        }
        switch (State.PhaseOfDay)
        {
            case Phase.DAWN:
            case Phase.NOON:
            case Phase.DUSK:
                StartFieldPhase();
                break;
            case Phase.MORNING:
            case Phase.AFTERNOON:
                StartExplorationPhase();
                break;
            case Phase.NIGHT:
                //TODO Night Phase
                NextPhase();
                break;
            default:
                GD.PrintErr("Game error: Phase unknown");
                break;
        }
        TurnCounter.UpdateFromGameState(State);
    }

    public void OnCombatDeckClick(Card card)
    {
        CombatDeckManager wrapper = charactersByCardId[card.GetInstanceId()].CombatDeck;
        currentScene?.OpenDeckModificationPanel(wrapper);
    }

    public void OnFieldDeckClick(Card card)
    {
        FieldDeckManager wrapper = charactersByCardId[card.GetInstanceId()].FieldDeck;
        currentScene?.OpenDeckModificationPanel(wrapper);
    }

    public void OnInventoryClick(Card card)
    {
        CharacterWrapper wrapper = charactersByCardId[card.GetInstanceId()];
        currentScene?.OpenInventoryScreen(wrapper);
    }

    public void StartExplorationPhase()
    {
        currentScene?.Destroy();
        ExplorationManager explorationManager = new ExplorationManager() { Game = this };
        currentScene = explorationManager;
        AddChild(explorationManager);
    }

    public void StartFieldPhase()
    {
        currentScene?.Destroy();
    }

    public void StartNew()
    {
        InitializeScenario("BaseGame", "Endless");
        NextPhase();
    }
}
