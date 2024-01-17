using Godot;
using System;

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

    public void PopulateEncounters()
    {
        if (Model?.ExploreDeck != null) 
        {
            ExploreDeckCreationModel creationModel = JsonLoader.GetExploreDeckCreationModel(
                Model.Mod, Model.ExploreDeck);
            Model.Encounters = ExploreDeckCreator.CreateFromModel(creationModel);
        }
    }
}
