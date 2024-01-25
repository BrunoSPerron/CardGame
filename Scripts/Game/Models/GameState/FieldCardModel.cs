public class FieldCardModel : BaseCardModel
{
    public override string Name { get; set; } = "drool";
    public override string Image { get; set; } = "core__drool";

    public int Cost = 2;

    public string[] Effects = new string[0];
    public string TextBox = "";
}
