public class CombatCardModel : BaseCardModel
{
    public new string Name = "Punch";
    public new string ImageFileName = "punch.png";

    public CombatAction Type = CombatAction.ATTACK;
    public int FlatDamageModifier = 0;
}
