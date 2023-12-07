using System;

public class UnlockedLocationModel : BaseModel
{
    public string Name = "desolation";
    public DestinationInfo[] Destinations = new DestinationInfo[0];
}

public class DestinationInfo
{
    public int TravelCost = 0;
    public string Name;
}
