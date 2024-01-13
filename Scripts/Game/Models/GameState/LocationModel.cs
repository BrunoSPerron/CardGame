using System;

public class LocationModel : BaseCardModel
{
    public int ExploreCost = 1;
    public int FieldActionCost = 1;
    public new string Image = "";
    public new string Name = "core_desolation";
    public int TravelCost = 0;

    public string[] ExplorationDeck = new string[0];
}
