using Godot;
using System;
using System.Collections.Generic;

public class DeckPanel : Node2D
{
    private const float INNER_PADDING_X = 10;
    private const float INNER_PADDING_Y = 19;
    private const float OUTER_PADDING = 5;
    private const int VERTICAL_CARD_SPACING = 30;

    [Export]
    public PackedScene ButtonScene;
    [Export]
    public PackedScene PanelScene;
    [Export]
    public PackedScene PixelTextScene;

    public BaseDeckWrapper DeckWrapper;
    public BaseGameScreen GameScreen;

    private readonly List<Button> buttons = new List<Button>();
    private CardDealer cardDealer;
    private Panel panel;

    public override void _Ready()
    {
        panel = PanelScene.Instance<Panel>();
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

    private void DealBaseDeck()
    {
        //TODO calc multiplier based on available space
        float mult = 0.75f;

        Vector2 panelSize = panel.GetSize() - new Vector2(
            INNER_PADDING_X * 2, INNER_PADDING_Y * 2);
        DeckWrapper.GenerateBaseDeck();
        BaseCardWrapper[] deck = new BaseCardWrapper[0]; //TODO Fix this

        Vector2 cardSize = deck[0].Card.GetSize() * mult;

        Vector2 viewSize = GetTree().Root.GetVisibleRect().Size;

        Vector2 topLeft = new Vector2(OUTER_PADDING + INNER_PADDING_X,
            OUTER_PADDING + INNER_PADDING_Y) + cardSize / 2;

        Vector2 bottomRight = new Vector2(
            viewSize.x - OUTER_PADDING - INNER_PADDING_X,
            viewSize.y - OUTER_PADDING - INNER_PADDING_Y) - cardSize / 2;

        int numberOfRows = Mathf.CeilToInt(
            panelSize.y / (cardSize.y + VERTICAL_CARD_SPACING * mult));
        int cardsPerRow = Mathf.CeilToInt(deck.Length / (float)numberOfRows);

        float xSpacing = (bottomRight.x - topLeft.x) / (cardsPerRow - 1);
        float ySpacing = cardSize.y + VERTICAL_CARD_SPACING * mult;

        for (int i = 0; i < deck.Length; i++)
        {
            int x = i % cardsPerRow;
            int y = i / cardsPerRow;
            deck[i].Card.Scale = new Vector2(mult, mult);
            deck[i].Card.IsDraggable = false;
            deck[i].Card.IsStackTarget = false;

            DealOnBoard(deck[i].Card, topLeft + new Vector2(x * xSpacing, y * ySpacing));
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
