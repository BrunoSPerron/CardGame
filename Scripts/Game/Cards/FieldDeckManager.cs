using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class FieldDeckManager : BaseDeckManager
{
    public List<FieldCardWrapper> deck;
    public List<FieldCardWrapper> Deck {
        get
        {
            if (deck == null)
                GenerateDeck();
            return deck;
        }
    }
    private List<FieldCardWrapper> discardPile;
    public List<FieldCardWrapper> DiscardPile
    {
        get
        {
            if (discardPile == null)
                GenerateDeck();
            return discardPile;
        }
    }
    public new FieldDeckModel Model { get; private set; }

    // Constructor

    public FieldDeckManager(FieldDeckModel deckInfo): base(deckInfo)
    {
        Model = deckInfo;
    }

    // Overrides

    public override BaseCardWrapper[] GetBaseDeck()
    {
        BaseCardWrapper[] baseDeck = new BaseCardWrapper[Deck.Count];
        for (int i = 0; i < baseDeck.Length; i++)
            baseDeck[i] = deck[i];
        return baseDeck;
    }

    // Unique Methods

    public FieldCardWrapper Draw()
    {
        if (Deck.Count == 0)
            Shuffle();
        FieldCardWrapper wrapper = deck[0];
        deck.RemoveAt(0);
        DiscardPile.Add(wrapper);
        return wrapper;
    }

    public FieldCardWrapper[] DrawMultiple(int amount)
    {
        FieldCardWrapper[] cards = new FieldCardWrapper[amount];
        int i = 0;
        while (i < amount)
        {
            if (Deck.Count == 0)
                Shuffle();
            cards[i] = deck[0];
            deck.RemoveAt(0);
            i++;
        }

        foreach (FieldCardWrapper card in cards)
            DiscardPile.Add(card);

        return cards;
    }

    public void GenerateDeck()
    {
        if (Model.BaseDeck == null || Model.BaseDeck.Length < 1)
        {
            Model.BaseFieldDeck = DeckFactory.CreateNewFieldDeck();
            GD.PrintErr("Field deck wrapper error: No field deck. Using bad field deck");
        }

        FieldCardWrapper[] wrappedDeck = new FieldCardWrapper[Model.BaseFieldDeck.Count];
        for (int i = 0; i < Model.BaseDeck.Length; i++)
        {
            FieldCardModel model = Model.BaseFieldDeck[i];
            FieldCardWrapper wrapper = CardFactory.CreateCardFromFieldCardModel(
                model);
            wrappedDeck[i] = wrapper;
        }
        deck = wrappedDeck.ToList();
        discardPile = new List<FieldCardWrapper>();
    }

    public void Shuffle()
    {
        Deck.AddRange(DiscardPile);
        discardPile.Clear();
        deck.Shuffle();
    }
}
