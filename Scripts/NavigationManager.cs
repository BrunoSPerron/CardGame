using Godot;
using System;

public class NavigationManager : Node2D
{
    [Export]
    public PackedScene GameScene { get; set; }

    private Node2D currentScreen;

    public override void _Ready()
    {
        LoadMainMenu();
        //LoadTestScreen();
    }

    public void LoadMainMenu()
    {
        currentScreen?.QueueFree();
        currentScreen = new MainMenu() { NavigationManager = this };
        AddChild(currentScreen);
    }

    public void StartNewGame(string mod, string scenario)
    {
        currentScreen?.QueueFree();
        Game newGameScreen = GameScene.Instance<Game>();
        currentScreen = newGameScreen;
        AddChild(newGameScreen);
        newGameScreen.InitializeScenario(mod, scenario);
    }

    public void LoadTestScreen()
    {
        currentScreen?.QueueFree();
        currentScreen = new TestScreen();
        AddChild(currentScreen);
    }
}
