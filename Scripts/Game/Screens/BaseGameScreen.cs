using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseGameScreen : Node2D
{
    public PackedScene ButtonScene => ResourceLoader.Load<PackedScene>(
        "res://Assets/UI/Button.tscn/");
     
    public Game Game;

    public readonly List<Button> Buttons = new List<Button>();
    private CardDealer cardDealer;

    public Button AddButton(string label,
                         string callbackMethodName,
                         Cardinal anchor = Cardinal.NW,
                         Vector2? offset = null)
    {
        Button button = ButtonScene.Instance<Button>();
        button.Label = label;
        button.Connect("OnClick", this, callbackMethodName);
        Buttons.Add(button);
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

    public abstract void Destroy();

    public virtual void DisableScreen()
    {
        foreach (Button button in Buttons)
            button.Visible = false;
    }

    public virtual void EnableScreen()
    {
        foreach (Button button in Buttons)
            button.Visible = true;
    }


    public void OnDeckPanelClosed()
    {
        EnableScreen();
    }

    public void OnInventoryScreenClosed()
    {
        EnableScreen();
    }
    public void OpenDeckModificationPanel(BaseDeckManager manager)
    {
        DisableScreen();
        DeckPanel deckPanel = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/DeckPanel.tscn/").Instance<DeckPanel>();

        deckPanel.Manager = manager;
        deckPanel.GameScreen = this;
        AddChild(deckPanel);
    }

    public void OpenInventoryScreen(CharacterWrapper wrapper)
    {
        DisableScreen();

        InventoryPanel panel = new InventoryPanel
        {
            GameScreen = this,
            CharacterWrapper = wrapper
        };
        AddChild(panel);
    }
}
