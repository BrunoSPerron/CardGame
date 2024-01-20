using System.Collections.Generic;

public class FieldDeckModel : BaseDeckModel
{
    public new List<FieldCardModel> BaseDeck = new List<FieldCardModel>();

    public List<BonusFieldCardModel> BonusCards = new List<BonusFieldCardModel>();
}
