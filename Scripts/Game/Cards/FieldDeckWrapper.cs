using Godot;
using System;

public class FieldDeckWrapper : BaseDeckWrapper
{
    public new FieldDeckModel Model { get; private set; }

    public FieldDeckWrapper(FieldDeckModel deckInfo): base(deckInfo)
    {
        Model = deckInfo;
    }

    public FieldCardWrapper[] GenerateBaseFieldDeck()
    {
        if (Model.BaseDeck == null)
            Model.FieldDeck = DeckFactory.CreateNewFieldDeck();

        FieldCardWrapper[] wrappedDeck = new FieldCardWrapper[Model.FieldDeck.Length];
        for (int i = 0; i < Model.BaseDeck.Length; i++)
        {
            FieldCardModel model = Model.FieldDeck[i];
            FieldCardWrapper wrapper = CardFactory.CreateCardFromFieldCardModel(
                model);
            wrappedDeck[i] = wrapper;
        }

        return wrappedDeck;
    }

    public override void GenerateBaseDeck()
    {
        GenerateBaseFieldDeck();
    }
}
