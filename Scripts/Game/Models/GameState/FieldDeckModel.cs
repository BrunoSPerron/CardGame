using System.Collections.Generic;

public class FieldDeckModel : BaseDeckModel
{
    public new BaseCardModel[] BaseDeck => BaseFieldDeck.ToArray();

    public List<FieldCardModel> BaseFieldDeck = new List<FieldCardModel>();

    public List<ItemFieldCard> BonusCards = new List<ItemFieldCard>();
}

public class ItemFieldCard : BaseDeckModel
{
    public FieldCardModel Card;
    public string ItemSourceId;
    public int OverrideCardIndex;
    public int Priority;
}
