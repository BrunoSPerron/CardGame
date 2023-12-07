using Godot;
using System;
using System.Collections.Generic;

public class LocationWrapper : BaseCardWrapper
{
    //REMOVE ME
    public List<CharacterWrapper> Characters = new List<CharacterWrapper>();
    //

    private LocationModel model;
    public LocationModel Model
    {
        get => model;
        set
        {
            model = value;
            Card.SetLabel(value.Name);
        }
    }

    // ===== Methods =====

    public LocationWrapper(Card card, LocationModel location)
    {
        Card = card;
        Model = location;
    }
}
