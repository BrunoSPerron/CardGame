using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseGameScreen : Node2D
{
    public PackedScene ButtonScene => ResourceLoader.Load<PackedScene>(
        "res://Assets/UI/Button.tscn/");

    public PackedScene DeckPanel => ResourceLoader.Load<PackedScene>(
        "res://Assets/UI/DeckPanel.tscn/");
     
    public Game Game;

    private readonly List<Button> buttons = new List<Button>();
    private CardDealer cardDealer;
    private DeckPanel deckPanel;

    public Button AddButton(string label,
                         string callbackMethodName,
                         Cardinal anchor = Cardinal.NW,
                         Vector2? offset = null)
    {
        Button button = ButtonScene.Instance<Button>();
        button.Label = label;
        button.Connect("OnClick", this, callbackMethodName);
        buttons.Add(button);
        AddChild(button);
        button.UIAnchorToScreenBorder(anchor, offset);
        return button;
    }

    public void DealOnBoard(Card card, Vector2 target, Boolean priority = false)
    {
        card.MoveToPosition(target);
        if (cardDealer == null || cardDealer.IsQueuedForDeletion())
        {
            cardDealer = new CardDealer()
            {
                cardsParent = this,
                Position = new Vector2(-card.GetSize().x / 2, card.GetSize().y / 2)
            };

            cardDealer.AddCard(card, false);
            AddChild(cardDealer);
        }
        else
        {
            if (priority)
                cardDealer.AddPriorityCard(card);
            else
                cardDealer.AddCard(card);
        }
    }

    public void OpenDeckModificationPanel(BaseDeckManager deck)
    {
        DisableScreen();
        deckPanel = DeckPanel.Instance<DeckPanel>();
        deckPanel.DeckWrapper = deck;
        deckPanel.GameScreen = this;
        AddChild(deckPanel);
    }

    public void OnDeckPanelClosed()
    {
        EnableScreen();
    }

    public virtual void DisableScreen()
    {
        foreach (Button button in buttons)
            button.Visible = false;
    }

    public virtual void EnableScreen()
    {
        foreach (Button button in buttons)
            button.Visible = true;
    }

    public abstract void Destroy();
}
