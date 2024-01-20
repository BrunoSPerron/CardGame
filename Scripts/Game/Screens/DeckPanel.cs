using Godot;
using System;
using System.Collections.Generic;

public class DeckPanel : Node2D
{
    private const float INNER_PADDING_X = 10;
    private const float INNER_PADDING_Y = 19;
    private const float OUTER_PADDING = 5;
    private const int VERTICAL_CARD_SPACING = 30;

    public List<BaseCardWrapper> Deck;
    public BaseGameScreen GameScreen;

    private readonly List<Button> buttons = new List<Button>();
    private CardDealer cardDealer;
    private Panel panel;

    public override void _Ready()
    {
        panel = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/Panel.tscn/").Instance<Panel>();
        float paddingX2 = OUTER_PADDING * 2;
        Vector2 viewSize = GetTree().Root.GetVisibleRect().Size;
        Vector2 halfViewSize = viewSize / 2;
        panel.SizeOnReady = viewSize - new Vector2(paddingX2, paddingX2);
        panel.Position = halfViewSize;
        AddChild(panel);
        panel.ZIndex = CardManager.NumberOfCards;
        AddButton("Finish", "OnDone", Cardinal.SE,
            new Vector2(-10 - INNER_PADDING_X, -10 - INNER_PADDING_Y));
        DealBaseDeck();
    }

    private Button AddButton(string label,
                             string callbackMethod,
                             Cardinal anchor = Cardinal.NW,
                             Vector2? offset = null)
    {
        Button button = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/Button.tscn/").Instance<Button>();

        button.Label = label;
        button.Connect("OnClick", this, callbackMethod);
        buttons.Add(button);
        AddChild(button);
        button.UIAnchorToScreenBorder(anchor, offset);
        return button;
    }

    private void DealBaseDeck()
    {
        //TODO Placement based on available space
        float mult = 0.75f;

        Vector2 panelSize = panel.GetSize() - new Vector2(
            INNER_PADDING_X * 2, INNER_PADDING_Y * 2);

        Vector2 cardSize = Deck[0].Card.GetSize() * mult;
        Vector2 viewSize = GetTree().Root.GetVisibleRect().Size;
        Vector2 topLeft = new Vector2(OUTER_PADDING + INNER_PADDING_X,
            OUTER_PADDING + INNER_PADDING_Y) + cardSize / 2;
        Vector2 bottomRight = new Vector2(
            viewSize.x - OUTER_PADDING - INNER_PADDING_X,
            viewSize.y - OUTER_PADDING - INNER_PADDING_Y) - cardSize / 2;

        int numberOfRows = Mathf.CeilToInt(
            panelSize.y / (cardSize.y + VERTICAL_CARD_SPACING * mult));
        int cardsPerRow = Mathf.CeilToInt(Deck.Count / (float)numberOfRows);

        float xSpacing = (bottomRight.x - topLeft.x) / (cardsPerRow - 1);
        float ySpacing = cardSize.y + VERTICAL_CARD_SPACING * mult;

        for (int i = 0; i < Deck.Count; i++)
        {
            int x = i % cardsPerRow;
            int y = i / cardsPerRow;
            Deck[i].Card.Scale = new Vector2(mult, mult);
            Deck[i].Card.IsDraggable = false;
            Deck[i].Card.IsStackTarget = false;

            DealOnBoard(Deck[i].Card, topLeft + new Vector2(x * xSpacing, y * ySpacing));
        }
    }

    public void DealOnBoard(Card card, Vector2 target)
    {
        card.MoveToPosition(target);
        if (cardDealer == null || cardDealer.IsQueuedForDeletion())
        {
            cardDealer = new CardDealer()
            {
                cardsParent = this,
                Position = Position - new Vector2(card.GetSize().x / 2, 300),
                SecondsBetweenDraws = 0.03f
            };

            cardDealer.AddCard(card, false);
            AddChild(cardDealer);
        }
        else
        {
            cardDealer.AddCard(card);
        }
    }

    public void OnDone()
    {
        GameScreen.OnDeckPanelClosed();
        QueueFree();
    }
}
