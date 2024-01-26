using System.Collections.Generic;
using System.Linq;

public class CombatDeckModel : BaseDeckModel
{
    public override List<BaseCardModel> BaseDeck
        => CombatDeck.Cast<BaseCardModel>().ToList();
    public override List<BaseBonusCardModel> BaseBonusCards
        => BonusCards.Cast<BaseBonusCardModel>().ToList();


    public List<BonusCombatCardModel> BonusCards = new List<BonusCombatCardModel>();
    public List<CombatCardModel> CombatDeck = new List<CombatCardModel>();
}

