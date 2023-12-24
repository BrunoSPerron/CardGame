using System;

public class LocationModel : BaseModel
{
    public int ExploreCost = 1;
    public int FieldActionCost = 1;
    public string ImageFileName = "";
    public string Name = "core_desolation";
    public int TravelCost = 0;

    public string[] ExplorationDeck = new string[0];
}
