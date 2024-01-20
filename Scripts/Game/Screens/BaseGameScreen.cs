using Godot;
using System;
using System.Collections.Generic;

public abstract class BaseGameScreen : Node2D
{
    public PackedScene ButtonScene => ResourceLoader.Load<PackedScene>(
        "res://Assets/UI/Button.tscn/");
     
    public Game Game;

    private readonly List<Button> buttons = new List<Button>();
    private CardDealer cardDealer;

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

    public abstract void Destroy();

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


    public void OnDeckPanelClosed()
    {
        EnableScreen();
    }

    public void OnInventoryScreenClosed()
    {
        EnableScreen();
    }
    public void OpenDeckModificationPanel(List<BaseCardWrapper> deck)
    {
        DisableScreen();
        DeckPanel deckPanel = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/DeckPanel.tscn/").Instance<DeckPanel>();

        deckPanel.Deck = deck;
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
