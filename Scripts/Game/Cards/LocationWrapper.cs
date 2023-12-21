using Godot;
using System;
using System.Collections.Generic;

public class LocationWrapper : BaseCardWrapper
{
    private WorldHexModel worldPosition;
    public WorldHexModel WorldPosition
    {
        get => worldPosition;
        set
        {
            worldPosition = value;
            Card?.SetLabel(value.Location?.Name ?? "");
        }
    }

    public LocationModel Model
    {
        get => worldPosition.Location;
    }

    // ===== Methods =====

    public LocationWrapper(Card card, WorldHexModel location)
    {
        Card = card;
        WorldPosition = location;
    }
}
