public class CombatCardModel : BaseCardModel
{
    public new string Name = "Punch";
    public new string Image = "core__punch";

    public int Cost = 3;
    public int FlatDamageModifier = 0;
    public CombatAction Type = CombatAction.ATTACK;
}
