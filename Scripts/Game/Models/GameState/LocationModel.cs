using System;
using System.Collections.Generic;

public class LocationModel : BaseCardModel
{
    public override string Name { get; set; } = "";
    public override string Image { get; set; } = "core_desolation";

    public int ExploreCost = 1;
    public int SurviveActionCost = 1;
    public int TravelCost = 0;

    public string ExploreDeck;

    public List<EncounterModel> Encounters = new List<EncounterModel>();
}
