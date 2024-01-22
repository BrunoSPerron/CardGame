using Godot;
using System;
using System.Collections.Generic;


public class ExplorationManager : BaseGameScreen
{
    private readonly List<Vector2Int> positionsWithSurvivor = new List<Vector2Int>();

    private ExplorationScreen currentScreen;

    //===Overrides===

    public override void _Ready()
    {
        AddNavigationButtons();
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

    public override void Destroy()
    {
        currentScreen.Destroy();
    }

    public override void DisableScreen()
    {
        base.DisableScreen();

        if (currentScreen != null)
        {
            float alpha = 0.25f;
            foreach (LocationWrapper destination in currentScreen.Destinations)
            {
                destination.Card.IsStackTarget = false;
                destination.Card.SetAlpha(alpha);
            }

            foreach (CharacterWrapper character in currentScreen.Survivors)
            {
                character.Card.IsDraggable = false;
                character.Card.SetAlpha(alpha);
            }

            if (currentScreen.SurviveTarget != null)
            {
                currentScreen.SurviveTarget.IsStackTarget = false;
                currentScreen.SurviveTarget.SetAlpha(alpha);
            }

            if (currentScreen.ExploreTarget != null)
            {
                currentScreen.ExploreTarget.IsStackTarget = false;
                currentScreen.ExploreTarget.SetAlpha(alpha);
            }

            if (currentScreen.Location != null)
            {
                currentScreen.Location.Card.IsStackTarget = false;
                currentScreen.Location.Card.SetAlpha(alpha);
            }
        }
    }

    public override void EnableScreen()
    {
        base.EnableScreen();
        if (currentScreen != null)
        {

            foreach (LocationWrapper destination in currentScreen.Destinations)
            {
                destination.Card.IsStackTarget = true;
                destination.Card.SetAlpha(1f);
            }

            foreach (CharacterWrapper character in currentScreen.Survivors)
            {
                character.Card.IsDraggable = true;
                character.Card.SetAlpha(1f);
            }

            if (currentScreen.SurviveTarget != null)
            {
                currentScreen.SurviveTarget.IsStackTarget = true;
                currentScreen.SurviveTarget.SetAlpha(1f);
            }

            if (currentScreen.ExploreTarget != null)
            {
                currentScreen.ExploreTarget.IsStackTarget = true;
                currentScreen.ExploreTarget.SetAlpha(1f);
            }

            if (currentScreen.Location != null)
            {
                currentScreen.Location.Card.IsStackTarget = true;
                currentScreen.Location.Card.SetAlpha(1f);
            }
        }
    }

    //===Unique Methods===

    public void AddNavigationButtons()
    {
        int xOffset = 100;
        int yOffset = 140;
        Vector2 leftPosition = new Vector2(CONSTS.SCREEN_CENTER.x - xOffset, CONSTS.SCREEN_CENTER.y - yOffset);
        Vector2 rightPosition = new Vector2(CONSTS.SCREEN_CENTER.x + 60, CONSTS.SCREEN_CENTER.y - yOffset);
        AddButton("←", "OnLeftNavigationButtonPress", Cardinal.NW, leftPosition);
        AddButton("→", "OnRightNavigationButtonPress", Cardinal.NW, rightPosition);
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

        List<bool> locationToKeepMask = new List<bool>(new bool[positionsWithSurvivor.Count]);
        for (int i = 0; i < Game.Survivors.Count; i++)
        {
            CharacterWrapper character = Game.Survivors[i];
            Vector2Int position = character.WorldPosition;
            if (!positionsWithSurvivor.Contains(position))
            {
                positionsWithSurvivor.Add(position);
                locationToKeepMask.Add(false);
            }
            locationToKeepMask[positionsWithSurvivor.IndexOf(position)] = true;
        }

        for (int i = locationToKeepMask.Count - 1; i > -1; i--)
        {
            if (!locationToKeepMask[i])
            {
                positionsWithSurvivor.RemoveAt(i);
            }
        }

        currentScreen.QueueFree();
        SetCurrentScreen(worldPosition);
    }

    public void OnLeftNavigationButtonPress()
    {
        int i = 0;
        while (i < positionsWithSurvivor.Count)
        {
            Vector2Int position = positionsWithSurvivor[i];
            if (currentScreen.Location.WorldPosition.Coord == position)
            {
                if (i == 0)
                    i = positionsWithSurvivor.Count - 1;
                else
                    i--;
                MoveToHex(positionsWithSurvivor[i]);
                break;
            }
            i++;
        }
    }

    public void OnRightNavigationButtonPress()
    {
        int i = 0;
        int nbOfPositions = positionsWithSurvivor.Count;
        while (i < nbOfPositions)
        {
            Vector2Int position = positionsWithSurvivor[i];
            if (currentScreen.Location.WorldPosition.Coord == position)
            {
                if (i == nbOfPositions - 1)
                    i = 0;
                else
                    i++;
                MoveToHex(positionsWithSurvivor[i]);
                break;
            }
            i++;
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
}
