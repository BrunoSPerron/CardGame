using Godot;
using System;
using System.Collections.Generic;

public class GameStateModel : BaseModel
{
    public Phase PhaseOfDay = Phase.MORNING;
    public string Scenario = "BaseGame";
    public int Turn = 0;

    public WorldModel Map = new WorldModel();
    public List<CharacterModel> Survivors = new List<CharacterModel>();
    public List<LocationModel> Locations = new List<LocationModel>();
}
