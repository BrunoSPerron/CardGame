using System;
using System.Collections.Generic;

public class LocationModel : BaseCardModel
{
    public int ExploreCost = 1;
    public int FieldActionCost = 1;
    public new string Image = "";
    public new string Name = "core_desolation";
    public int TravelCost = 0;

    public string ExploreDeck;

    public List<EncounterModel> Encounters = new List<EncounterModel>();
}
