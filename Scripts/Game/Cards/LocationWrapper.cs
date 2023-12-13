using Godot;
using System;
using System.Collections.Generic;

public class LocationWrapper : BaseCardWrapper
{
    private HexLocationModel hexLocation;
    public HexLocationModel HexLocation
    {
        get => hexLocation;
        set
        {
            hexLocation = value;
            Card?.SetLabel(value.Location?.Name ?? "");
        }
    }

    // ===== Methods =====

    public LocationWrapper(Card card, HexLocationModel location)
    {
        Card = card;
        HexLocation = location;
    }
}
