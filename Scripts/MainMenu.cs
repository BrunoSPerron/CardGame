using System;
using System.Collections.Generic;
using Godot;

public class MainMenu : BaseGameScreen
{
    public NavigationManager NavigationManager;
    public Card NewGameTarget;
    public Card Survivor;

    private CardCleaner cardCleaner;
    private Dictionary<ulong, ScenarioTarget> scenarioTargets
        = new Dictionary<ulong, ScenarioTarget>();

    public override void _Ready()
    {
        Survivor = CardFactory.CreateBlankSurvivor();
        Survivor.Connect("OnDragEnd", this, "OnCarddragEnd");
        DealOnBoard(Survivor, CONSTS.SCREEN_CENTER + Vector2.Down * 75);

        AddTopLevelTargets();
    }

    private void AddScenarioTargets()
    {
        List<ScenarioTarget> newTargets = CardFactory.CreateScenarioTargets();

        for (int i = 0; i < newTargets.Count; i++)
        {
            ScenarioTarget target = newTargets[i];
            scenarioTargets.Add(target.Card.GetInstanceId(), target);
            DealOnBoard(target.Card, new Vector2(125 + i * 100, 125));
        }
    }

    private void AddTopLevelTargets()
    {
        NewGameTarget = CardFactory.CreateNewGameTarget();
        DealOnBoard(NewGameTarget, CONSTS.SCREEN_CENTER + Vector2.Up * 75);
    }

    /// <summary>
    /// Flip cards out of the screen
    /// </summary>
    /// <param name="card">Card with no godot parent</param>
    private void CleanCard(Card card)
    {
        RemoveChild(card);
        card.ClearAnimations();
        if (cardCleaner == null)
        {
            cardCleaner = new CardCleaner();
            AddChild(cardCleaner);
            cardCleaner.Position = new Vector2(0, 0);
        }
        cardCleaner.AddCardToClean(card);
    }

    private void RemoveTopLevelTargets()
    {
        CleanCard(NewGameTarget);
        NewGameTarget = null;
    }

    public override void Destroy()
    {
        CleanCard(Survivor);
        CleanCard(NewGameTarget);
        DisableScreen();
        if (cardCleaner == null)
            QueueFree();
        else
            cardCleaner.Connect("tree_exited", this, "OnScreenCleaned");
    }

    public override void DisableScreen()
    {
        base.DisableScreen();
        Survivor.IsDraggable = false;
        if (NewGameTarget != null)
            NewGameTarget.IsStackTarget = false;
    }

    public override void EnableScreen()
    {
        base.EnableScreen();
        Survivor.IsDraggable = true;
        if (NewGameTarget != null)
            NewGameTarget.IsStackTarget = true;
    }

    // === Signals Listened ===

    public void OnCarddragEnd(Card originCard, Card stackTarget)
    {
        if (stackTarget == null)
            return;

        if (stackTarget == NewGameTarget)
        {
            RemoveTopLevelTargets();
            AddScenarioTargets();
        }
        else if (scenarioTargets.ContainsKey(stackTarget.GetInstanceId()))
        {
            ScenarioTarget target = scenarioTargets[stackTarget.GetInstanceId()];
            NavigationManager.StartNewGame(target.Mod, target.Scenario);
        }
    }

    public void OnScreenCleaned()
    {
        QueueFree();
    }
}
