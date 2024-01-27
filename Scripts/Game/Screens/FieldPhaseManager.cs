using Godot;
using System;

public class FieldPhaseManager : BaseGameScreen
{
    private PlayFieldScreen currentScreen;

    public override void _Ready()
    {
        //AddNavigationButtons();

        foreach (CharacterWrapper survivor in Game.Survivors)
        {
            survivor.FieldDeckManager.Reset();
            survivor.FieldDeckManager.DrawMultiple(CONSTS.FIELD_HAND_SIZE);
        }

        if (Game.Survivors.Count > 0)
            SetCurrentScreen(Game.Survivors[0]);
        else
            GD.PrintErr("Field Phase Manager Error: No survivors in game.");
    }

    public void SetCurrentScreen(CharacterWrapper characterWrapper)
    {
        currentScreen?.Destroy();
        currentScreen = new PlayFieldScreen()
        {
            Character = characterWrapper,
            Game = Game,
            Parent = this,
            ResetDeckOnReady = false,
        };
        AddChild(currentScreen);
    }

    public override void Destroy()
    {
        currentScreen.Destroy();
        QueueFree();
    }

    public override void SurvivorEvent_Field_End()
    {
        Game.NextPhase();
    }
}
