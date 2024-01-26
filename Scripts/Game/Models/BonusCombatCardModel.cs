using System;


public class BonusCombatCardModel : BaseBonusCardModel
{
    public override BaseCardModel Card => CombatCard;

    public CombatCardModel CombatCard;
}
