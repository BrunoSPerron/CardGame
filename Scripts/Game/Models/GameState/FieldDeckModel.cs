using System.Collections.Generic;
using System.Linq;

public class FieldDeckModel : BaseDeckModel
{
    public override List<BaseCardModel> BaseDeck
        => FieldDeck.Cast<BaseCardModel>().ToList();
    public override List<BonusFieldCardModel> BaseBonusCards
        => BonusCards.Cast<BonusFieldCardModel>().ToList();


    public List<BonusFieldCardModel> BonusCards = new List<BonusFieldCardModel>();
    public List<FieldCardModel> FieldDeck = new List<FieldCardModel>();
}
