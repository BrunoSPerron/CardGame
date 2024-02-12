public class CombatCardModel : BaseCardModel
{
    public override string Name { get; set; } = "Punch";
    public override string Image { get; set; } = "core__punch";

    public int Cost = 3;

    public string[] Effects = new string[0];

    public string TextBox = "";
}
