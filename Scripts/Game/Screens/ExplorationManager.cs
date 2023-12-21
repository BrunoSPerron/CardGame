using Godot;
using System.Collections.Generic;


public class ExplorationManager : BaseGameScreen
{
    private readonly HashSet<Vector2Int> positionsWithSurvivor = new HashSet<Vector2Int>();

    private ExplorationScreen currentScreen;

    public override void _Ready()
    {
        foreach (CharacterWrapper character in Game.Survivors)
        {
            Vector2Int position = character.WorldPosition;

            if (!positionsWithSurvivor.Contains(position))
                positionsWithSurvivor.Add(position);
        }

        if (Game.Survivors.Count > 0)
        {
            SetCurrentScreen(Game.Survivors[0].WorldPosition);
        }
        else
        {
            GD.PrintErr("Exploration Manager Error: No survivors in game.");
        }
    }

    public void SetCurrentScreen(Vector2Int position)
    {
        currentScreen = new ExplorationScreen()
        {
            Game = Game,
            Location = Game.LocationsByPosition[position],
            Manager = this,
        };
        AddChild(currentScreen);
    }

    public override void Destroy()
    {
        currentScreen.Destroy();
    }

    public void MoveToHex(Vector2Int worldPosition)
    {
        if (!Game.LocationsByPosition.ContainsKey(worldPosition))
        {
            GD.PrintErr("Exploration manager error: Non-existant destination");
            return;
        }

        if (worldPosition == currentScreen.Location.WorldPosition.Coord)
        {
            GD.PrintErr("Exploration manager error: Destination is current position");
            return;
        }

        currentScreen.Clean();

        bool removeOldLocation = true;
        foreach (CharacterWrapper character in Game.Survivors)
        {
            Vector2Int position = character.WorldPosition;

            if (!positionsWithSurvivor.Contains(position))
            {
                positionsWithSurvivor.Add(position);
                removeOldLocation = false;
                break;
            }
        }
        if (removeOldLocation)
        {
            positionsWithSurvivor.Remove(currentScreen.Location.WorldPosition.Coord);
        }
        currentScreen.QueueFree();
        SetCurrentScreen(worldPosition);
    }
}
