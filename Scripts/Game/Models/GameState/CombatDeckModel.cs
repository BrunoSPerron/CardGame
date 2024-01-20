using System.Collections.Generic;

public class CombatDeckModel : BaseDeckModel
{
    public new List<CombatCardModel> BaseDeck = new List<CombatCardModel>();

    public List<BonusCombatCardModel> BonusCards = new List<BonusCombatCardModel>();
}

