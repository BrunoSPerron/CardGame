using Godot;
using System;

public class FieldDeckWrapper : BaseDeckWrapper
{
    public new FieldDeckModel Model { get; private set; }

    public FieldDeckWrapper(FieldDeckModel deckInfo): base(deckInfo)
    {
        Model = deckInfo;
    }

    public FieldCardWrapper[] GetFieldDeck()
    {
        if (Model.BaseDeck == null || Model.BaseDeck.Length < 1)
        {
            Model.FieldDeck = DeckFactory.CreateNewFieldDeck();
            GD.Print("Field deck wrapper error: No field deck. Using bad field deck");
        }

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

    public override BaseCardWrapper[] GetBaseDeck()
    {
        FieldCardWrapper[] fieldDeck = GetFieldDeck();
        BaseCardWrapper[] baseDeck = new BaseCardWrapper[fieldDeck.Length];
        for (int i = 0; i < baseDeck.Length; i++)
            baseDeck[i] = fieldDeck[i];
        return baseDeck;
    }
}
