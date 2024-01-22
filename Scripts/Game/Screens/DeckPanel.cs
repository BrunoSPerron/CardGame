using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public class DeckPanel : Node2D
{
    private const float INNER_PADDING_X = 10;
    private const float INNER_PADDING_Y = 19;
    private const float OUTER_PADDING = 5;
    private const int VERTICAL_CARD_SPACING = 30;

    public BaseDeckManager Manager;
    public BaseGameScreen GameScreen;

    private BaseCardWrapper[] baseCards;
    private readonly Dictionary<ulong, BaseCardWrapper> baseCardWrapperById
        = new Dictionary<ulong, BaseCardWrapper>();

    private BaseBonusCardWrapper[] bonusCards;
    private List<BaseBonusCardWrapper>[] bonusCardsAtIndex;
    private Dictionary<ulong, BaseBonusCardWrapper> bonusCardWrapperById
        = new Dictionary<ulong, BaseBonusCardWrapper>();

    private readonly List<Button> buttons = new List<Button>();
    private CardDealer cardDealer;
    private float cardSizeMultiplier = 1f;

    private Panel panel;
    private Vector2[] positionsOnScreen;

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
        DealBonusCards();
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
        cardSizeMultiplier = 0.75f;

        Vector2 panelSize = panel.GetSize() - new Vector2(
            INNER_PADDING_X * 2, INNER_PADDING_Y * 2);

        baseCards = Manager.GetSortedBaseDeck().ToArray();
        positionsOnScreen = new Vector2[baseCards.Length];

        Vector2 cardSize = baseCards[0].Card.GetSize() * cardSizeMultiplier;
        Vector2 viewSize = GetTree().Root.GetVisibleRect().Size;
        Vector2 topLeft = new Vector2(OUTER_PADDING + INNER_PADDING_X,
            OUTER_PADDING + INNER_PADDING_Y) + cardSize / 2;
        Vector2 bottomRight = new Vector2(
            viewSize.x - OUTER_PADDING - INNER_PADDING_X,
            viewSize.y - OUTER_PADDING - INNER_PADDING_Y) - cardSize / 2;

        int numberOfRows = Mathf.CeilToInt(
            panelSize.y / (cardSize.y + VERTICAL_CARD_SPACING * cardSizeMultiplier));
        int cardsPerRow = Mathf.CeilToInt(baseCards.Length / (float)numberOfRows);

        float xSpacing = (bottomRight.x - topLeft.x) / (cardsPerRow - 1);
        float ySpacing = cardSize.y + VERTICAL_CARD_SPACING * cardSizeMultiplier;

        for (int i = 0; i < baseCards.Length; i++)
        {
            int x = i % cardsPerRow;
            int y = i / cardsPerRow;
            Card card = baseCards[i].Card;
            card.Scale = new Vector2(cardSizeMultiplier, cardSizeMultiplier);
            card.IsDraggable = false;
            card.IsStackTarget = true;
            var targetPosition = topLeft + new Vector2(x * xSpacing, y * ySpacing);
            positionsOnScreen[i] = targetPosition;
            baseCardWrapperById[card.GetInstanceId()] = baseCards[i];
            DealOnBoard(card, targetPosition);
        }
    }

    private void DealBonusCards()
    {
        bonusCards = Manager.GetBonusCards().ToArray();

        bonusCardsAtIndex = new List<BaseBonusCardWrapper>[baseCards.Length];
        for (int i = 0; i < bonusCardsAtIndex.Length; i++)
            bonusCardsAtIndex[i] = new List<BaseBonusCardWrapper>();

        for (int i = 0; i < bonusCards.Length; i++)
        {
            BaseBonusCardWrapper bonusCardWrapper = bonusCards[i];
            int overrideIndex = bonusCardWrapper.BaseModel.OverrideCardIndex;
            bonusCardsAtIndex[overrideIndex].Add(bonusCardWrapper);
        }

        for (int i = 0; i < bonusCardsAtIndex.Length; i++)
        {
            List<BaseBonusCardWrapper> cardsAtPosition = bonusCardsAtIndex[i];
            if (cardsAtPosition.Count != 0)
            {
                List<Card> cards = new List<Card>();
                foreach (BaseBonusCardWrapper bonusCardWrapper in cardsAtPosition)
                    cards.Add(bonusCardWrapper.Card);
                CardManager.StackCards(cards, positionsOnScreen[i]);
            }
        }

        for (int i = 0; i < bonusCards.Length; i++)
        {
            Card card = bonusCards[i].Card;
            card.Scale = new Vector2(cardSizeMultiplier, cardSizeMultiplier);
            card.IsDraggable = true;
            card.Connect("OnDragStart", this, "OnCardDragStart");
            card.Connect("OnDragEnd", this, "OnCardDragEnd");
            bonusCardWrapperById[card.GetInstanceId()] = bonusCards[i];
            DealOnBoard(card, card.Target);
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

    public void OnCardDragEnd(Card originCard, Card stackTarget)
    {
        BaseBonusCardWrapper originWrapper = bonusCardWrapperById[originCard.GetInstanceId()];
        int oldIndex = originWrapper.BaseModel.OverrideCardIndex;
        bonusCardsAtIndex[oldIndex].Remove(originWrapper);

        if (stackTarget == null)
        {
            bonusCardsAtIndex[oldIndex].Add(originWrapper);
            RestackPosition(oldIndex);
            return;
        }

        BaseCardWrapper stackWrapper = baseCardWrapperById[stackTarget.GetInstanceId()];
        int newIndex = Array.IndexOf(baseCards, stackWrapper);
        GD.Print(newIndex);
        originWrapper.BaseModel.OverrideCardIndex = newIndex;
        bonusCardsAtIndex[newIndex].Add(originWrapper);

        RestackPosition(oldIndex);
        RestackPosition(newIndex);
    }

    public void OnCardDragStart(Card originCard)
    {
        BaseBonusCardWrapper wrapper = bonusCardWrapperById[originCard.GetInstanceId()];
        RestackPosition(
            wrapper.BaseModel.OverrideCardIndex,
            new List<Card>() { originCard } );
    }

    public void RestackPosition(int index, List<Card> exclude = null)
    {
        if (exclude == null)
            exclude = new List<Card>();

        List<Card> cardsToStack = new List<Card>();

        foreach (BaseBonusCardWrapper wrapper in bonusCardsAtIndex[index])
            if (!exclude.Contains(wrapper.Card))
                cardsToStack.Add(wrapper.Card);

        CardManager.StackCards(cardsToStack, positionsOnScreen[index]);
    }
}
