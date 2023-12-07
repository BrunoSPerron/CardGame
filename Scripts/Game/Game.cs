using Godot;
using System;
using System.Collections.Generic;

public class Game : Node2D
{
    [Export]
    public PackedScene CampScene { get; set; } //DELETE ME
    public TurnCounter TurnCounter { get; private set; }

    private BaseGameScreen currentScene;

    public GameStateModel State { get; private set; }

    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();
    private readonly List<LocationWrapper> locations = new List<LocationWrapper>();

    // ===== GD Methods Override =====

    public override void _Ready()
    {
        //GD.Print(JsonLoader.GetCombatCardInfo("BaseGame", "Stab").Serialize());
    }

    // ===== Methods unique to this class =====

    public void InitializeScenario(string mod, string scenario)
    {
        State = new GameStateModel() { Turn = 0 };
        State.Scenario = mod;
        OutsetModel outsetInfo = JsonLoader.GetOnsetModel(mod, scenario);
        LocationWrapper startingLocation = null;

        foreach (UnlockedLocationModel unlockedLocation in outsetInfo.UnlockedLocations)
        {
            LocationModel location = JsonLoader.GetLocationModel(
                State.Scenario, unlockedLocation.Name);
            LocationWrapper wrapper = CardFactory.CreateCardFromLocation(
                State.Scenario, location);
            if (location.ID.StartsWith(mod + '_' +outsetInfo.StartingLocation))
                startingLocation = wrapper;
            locations.Add(wrapper);
        }
        if (startingLocation == null)
        {
            LocationModel location = JsonLoader.GetLocationModel(
                State.Scenario, outsetInfo.StartingLocation);
            if (location.ID != "core_desolation")
            {
                LocationWrapper wrapper = CardFactory.CreateCardFromLocation(
                    State.Scenario, location);
                locations.Add(wrapper);
                startingLocation = wrapper;
            }
        }

        if (locations.Count == 0)
        {
            LocationWrapper locationWrapper = CardFactory.CreateDefaultWrappedLocation();
            locations.Add(locationWrapper);
            startingLocation = locationWrapper;
        }

        foreach (string characterCreatorLogic in outsetInfo.StartingCharacters)
        {
            string[] instructions = JsonLoader.GetCharacterCreationInstructions(
                State.Scenario, characterCreatorLogic);
            CharacterModel info = CharacterCreator.CreateFromInstructions(instructions);
            info.CurrentActionPoint = Mathf.Clamp(
                info.CurrentActionPoint, 1, info.ActionPoint);
            info.CurrentHitPoint = Mathf.Clamp(info.CurrentHitPoint, 1, info.HitPoint);
            info.CurrentLocationID = startingLocation.Model.ID;
            survivors.Add(CardFactory.CreateCardFromCharacter(info));
        }

        TurnCounter = GetNode<TurnCounter>("TurnCounter");
        TurnCounter.UpdateFromGameState(State);
    }

    public void StartExplorationPhase()
    {
        currentScene?.Destroy();
        ExplorationManager explorationManager = new ExplorationManager();
        foreach (CharacterWrapper character in survivors)
            explorationManager.AddSurvivor(character);
        foreach (LocationWrapper location in locations)
            explorationManager.AddLocation(location);
        currentScene = explorationManager;
        AddChild(explorationManager);
    }

    public void StartFieldPhase()
    {
        currentScene?.Destroy();
    }


    //DELETE ME
    public void MoveToCamp()
    {
        currentScene?.Destroy();

        DispatchScreen campScene = CampScene.Instance<DispatchScreen>();
        currentScene = campScene;
        campScene.Game = this;
        AddChild(campScene);

        foreach (LocationWrapper location in locations)
            campScene.AddLocation(location);
        foreach (CharacterWrapper survivor in survivors)
            campScene.AddSurvivor(survivor);
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
        foreach (LocationWrapper location in locations)
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
