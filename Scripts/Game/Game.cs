using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    public TurnCounter TurnCounter { get; private set; }

    private BaseGameScreen currentScene;

    public GameStateModel State { get; private set; }

    // ===== Wrappers Storage =====

    public readonly List<CharacterWrapper> Survivors = new List<CharacterWrapper>();
    public readonly List<LocationWrapper> Locations = new List<LocationWrapper>();

    public readonly Dictionary<Vector2Int, LocationWrapper> LocationsByPosition
        = new Dictionary<Vector2Int, LocationWrapper>();
    public readonly Dictionary<Vector2Int, HexLocationModel> HexLocationsByPosition
        = new Dictionary<Vector2Int, HexLocationModel>();

    public Dictionary<ulong, CharacterWrapper> charactersByCardId
        = new Dictionary<ulong, CharacterWrapper>();

    public Dictionary<ulong, LocationWrapper> locationsByCardId
        = new Dictionary<ulong, LocationWrapper>();

    // ===== Methods =====

    public void AddSurvivor(CharacterModel survivor)
    {
        CharacterWrapper character = CardFactory.CreateCardFromCharacter(survivor);
        Survivors.Add(character);
        charactersByCardId.Add(character.Card.GetInstanceId(), character);
    }

    public void AddLocation(HexLocationModel hexLocation)
    {
        LocationModel location = hexLocation.Location;
        LocationWrapper wrapper = null;
        if (location != null)
        {
            wrapper = CardFactory.CreateCardFromLocation(State.Scenario, hexLocation);
            Locations.Add(wrapper);
            ulong instanceId = wrapper.Card.GetInstanceId();
            if (!locationsByCardId.ContainsKey(instanceId))
                locationsByCardId.Add(instanceId, wrapper);
        }

        if (LocationsByPosition.ContainsKey(hexLocation.HexPosition))
        {
            HexLocationsByPosition[hexLocation.HexPosition] = hexLocation;
            LocationsByPosition[hexLocation.HexPosition] = wrapper;
        }
        else
        {
            HexLocationsByPosition.Add(hexLocation.HexPosition, hexLocation);
            LocationsByPosition.Add(hexLocation.HexPosition, wrapper);
        }
    }

    public void InitializeScenario(string mod, string scenario)
    {
        State = new GameStateModel
        {
            Turn = 0,
            Scenario = mod
        };
        OutsetModel outsetInfo = JsonLoader.GetOnsetModel(mod, scenario);

        WorldCreationModel worldCreationModel = JsonLoader.GetWorldCreationModel(
            mod, outsetInfo.World);
        State.Map = WorldCreator.CreateFromModel(worldCreationModel);

        foreach (HexLocationModel hexLocation in State.Map.Locations)
            AddLocation(hexLocation);

        foreach (string characterCreatorLogic in outsetInfo.StartingCharacters)
        {
            CharacterCreationModel model = JsonLoader.GetCharacterCreationModel(
                State.Scenario, characterCreatorLogic);
            CharacterModel character = CharacterCreator.CreateFromModel(model);
            character.CurrentActionPoint = Mathf.Clamp(
                character.CurrentActionPoint, 1, character.ActionPoint);
            character.CurrentHitPoint = Mathf.Clamp(
                character.CurrentHitPoint, 1, character.HitPoint);

            //TODO from x / y instead
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
