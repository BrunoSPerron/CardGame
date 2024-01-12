using System.Collections.Generic;

public class FieldDeckModel : BaseDeckModel
{
    public new BaseCardModel[] BaseDeck => FieldDeck.ToArray();

    public List<FieldCardModel> FieldDeck = new List<FieldCardModel>();
}

