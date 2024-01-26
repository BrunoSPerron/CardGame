using Godot;
using System;

public class FieldPhaseManager : BaseGameScreen
{
    private FieldPhaseScreen currentScreen;

    public override void _Ready()
    {
        //AddNavigationButtons();

        if (Game.Survivors.Count > 0)
            SetCurrentScreen(Game.Survivors[0]);
        else
            GD.PrintErr("Field Phase Manager Error: No survivors in game.");
    }

    public void SetCurrentScreen(CharacterWrapper characterWrapper)
    {
        currentScreen?.Destroy();
        currentScreen = new FieldPhaseScreen()
        {
            Game = Game,
            Character = characterWrapper,
            Manager = this,
        };
        AddChild(currentScreen);
    }

    public override void Destroy()
    {
        GD.PrintErr("TODO: Destroy the FieldPhaseManager");
        QueueFree();
    }
}
