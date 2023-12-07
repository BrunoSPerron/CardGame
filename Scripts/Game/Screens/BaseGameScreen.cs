using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public abstract class BaseGameScreen : Node2D
{
    [Export]
    public PackedScene ButtonScene;
    [Export]
    public PackedScene DeckPanel;

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

    public void DealOnBoard(Card card, Vector2 target, int zIndex = -1)
    {
        card.MoveToPosition(target);
        if (cardDealer == null || cardDealer.IsQueuedForDeletion())
        {
            cardDealer = new CardDealer()
            {
                cardsParent = this,
                Position = Position - new Vector2(card.GetSize().x / 2, 300)
            };

            cardDealer.cards.Add(card);
            AddChild(cardDealer);
        }
        else
        {
            cardDealer.cards.Add(card);
        }
    }

    public void OpenDeckModificationPanel(DeckWrapper deck)
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
        UpdateButtons();
    }

    protected abstract void UpdateButtons();

    public abstract void Destroy();
}