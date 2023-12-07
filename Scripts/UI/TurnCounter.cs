using Godot;
using System;

public class TurnCounter : Area2D
{
    PixelText Label { get; set; }

    public override void _Ready()
    {
        Label = GetNode<PixelText>("Label");
    }

    public void UpdateFromGameState(GameStateModel gameState)
    {
        Label.Value = "Day " + gameState.Turn.ToString() + " " + gameState.PhaseOfDay;
    }
}
