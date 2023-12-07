using Godot;
using System;

public class NavigationManager : Node2D
{
    [Export]
    public PackedScene GameScene { get; set; }

    [Export]
    public PackedScene TestScene { get; set; }

    private Node2D currentScreen;

    public override void _Ready()
    {
        //OpenTestScreen();
        StartNewGame();
    }

    public void StartNewGame()
    {
        currentScreen?.QueueFree();
        Game newGameScreen = GameScene.Instance<Game>();
        currentScreen = newGameScreen;
        AddChild(newGameScreen);
        newGameScreen.StartNew();
    }

    public void OpenTestScreen()
    {
        currentScreen?.QueueFree();
        Node2D newScreen = TestScene.Instance<Node2D>();
        currentScreen = newScreen;
        AddChild(newScreen);
    }
}
