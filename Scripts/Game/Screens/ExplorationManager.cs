using Godot;
using System.Collections.Generic;


public class ExplorationManager : BaseGameScreen
{
    private readonly Dictionary<Vector2Int, ExplorationScreen> subscreens
        = new Dictionary<Vector2Int, ExplorationScreen>();

    public override void _Ready()
    {
        foreach (CharacterWrapper character in Game.Survivors)
        {
            Vector2Int position = character.Model.WorldPosition;

            if (!subscreens.ContainsKey(position))
                AddSubScreen(position, Game.LocationsByPosition[position]);

            subscreens[position].AddSurvivor(character);
        }

        if (Game.Survivors.Count > 0)
        {
            //TODO screen priority logic
            AddChild(subscreens[Game.Survivors[0].Model.WorldPosition]);
        }
        else
        {
            GD.PrintErr("Exploration Manager Error: No survivors in game.");
        }
    }

    public void AddSubScreen(Vector2Int position, LocationWrapper location)
    {
        ExplorationScreen screen = new ExplorationScreen { Game = Game };
        screen.SetLocation(location);
        subscreens.Add(position, screen);
    }

    public override void Destroy()
    {
        foreach (KeyValuePair<Vector2Int, ExplorationScreen> kvp in subscreens)
            kvp.Value.Destroy();
    }

    protected override void UpdateButtons()
    {
        //throw new System.NotImplementedException();
    }
}
