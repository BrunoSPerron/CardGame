using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    public TurnCounter TurnCounter { get; private set; }

    private BaseGameScreen currentScene;

    public GameStateModel State { get; private set; }

    public readonly List<CharacterWrapper> Survivors = new List<CharacterWrapper>();
    public readonly List<LocationWrapper> Locations = new List<LocationWrapper>();
    public readonly Dictionary<Vector2Int, HexLocation> LocationsByPosition
        = new Dictionary<Vector2Int, HexLocation>();


    // ===== GD Methods Override =====

    public override void _Ready()
    {
        //GD.Print(JsonLoader.GetCombatCardInfo("BaseGame", "Stab").Serialize());
    }

    // ===== Methods unique to this class =====

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

        LocationWrapper startingLocation = null;

        foreach (HexLocation hexLocation in State.Map.Locations)
        {
            LocationModel location = hexLocation.Location;
            if (location != null)
            {
                LocationWrapper wrapper = CardFactory.CreateCardFromLocation(
                    State.Scenario, location);
                if (location.ID.StartsWith(mod + '_' + outsetInfo.StartingLocation))
                    startingLocation = wrapper;
                Locations.Add(wrapper);
            }
            if (LocationsByPosition.ContainsKey(hexLocation.HexPosition))
                LocationsByPosition[hexLocation.HexPosition] = hexLocation;
            else
                LocationsByPosition.Add(hexLocation.HexPosition, hexLocation);
        }

        if (startingLocation == null)
        {
            LocationModel location = JsonLoader.GetLocationModel(
                State.Scenario, outsetInfo.StartingLocation);
            if (location.ID != "core_desolation")
            {
                LocationWrapper wrapper = CardFactory.CreateCardFromLocation(
                    State.Scenario, location);
                Locations.Add(wrapper);
                startingLocation = wrapper;
            }
        }

        if (Locations.Count == 0)
        {
            LocationWrapper locationWrapper = CardFactory.CreateDefaultWrappedLocation();
            Locations.Add(locationWrapper);
            startingLocation = locationWrapper;
        }

        foreach (string characterCreatorLogic in outsetInfo.StartingCharacters)
        {
            CharacterCreationModel model = JsonLoader.GetCharacterCreationModel(
                State.Scenario, characterCreatorLogic);
            CharacterModel info = CharacterCreator.CreateFromModel(model);
            info.CurrentActionPoint = Mathf.Clamp(
                info.CurrentActionPoint, 1, info.ActionPoint);
            info.CurrentHitPoint = Mathf.Clamp(info.CurrentHitPoint, 1, info.HitPoint);
            info.CurrentLocationID = startingLocation.Model.ID;
            Survivors.Add(CardFactory.CreateCardFromCharacter(info));
        }

        TurnCounter = GetNode<TurnCounter>("TurnCounter");
        TurnCounter.UpdateFromGameState(State);
    }

    public void StartExplorationPhase()
    {
        currentScene?.Destroy();
        ExplorationManager explorationManager = new ExplorationManager();
        foreach (CharacterWrapper character in Survivors)
            explorationManager.AddSurvivor(character);
        foreach (LocationWrapper location in Locations)
            explorationManager.AddLocation(location);
        currentScene = explorationManager;
        AddChild(explorationManager);
    }

    public void StartFieldPhase()
    {
        currentScene?.Destroy();
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

    public void OnDispatch()
    {
        List<LocationWrapper> locationsWithSurvivors = new List<LocationWrapper>();
        foreach (LocationWrapper location in Locations)
        {
            if (location.Characters.Count > 0)
            {
                locationsWithSurvivors.Add(location);
            }
        }
    }

    public void StartNew() 
    { 
        InitializeScenario("BaseGame", "Endless");
        NextPhase();
    }
}
