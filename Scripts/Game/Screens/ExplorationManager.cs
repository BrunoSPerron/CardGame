using Godot;
using System.Collections.Generic;


public class ExplorationManager : BaseGameScreen
{
    private readonly List<CharacterWrapper> survivors = new List<CharacterWrapper>();
    private readonly List<LocationWrapper> locations = new List<LocationWrapper>();

    private readonly Dictionary<string, ExplorationScreen> subscreens
        = new Dictionary<string, ExplorationScreen>();

    public Dictionary<ulong, CharacterWrapper> charactersByCardId
        = new Dictionary<ulong, CharacterWrapper>();

    public Dictionary<ulong, LocationWrapper> locationsByCardId
        = new Dictionary<ulong, LocationWrapper>();

    public override void _Ready()
    {
        foreach (LocationWrapper location in locations)
            foreach (CharacterWrapper character in survivors)
                if (character.Info.CurrentLocationID == location.Model.ID)
                {
                    AddSubScreen(location);
                    break;
                }

        foreach (CharacterWrapper character in survivors)
        {
            if (subscreens.ContainsKey(character.Info.CurrentLocationID))
                subscreens[character.Info.CurrentLocationID].AddSurvivor(character);
        }
        AddChild(subscreens[survivors[0].Info.CurrentLocationID]);
        //TODO Introduce screen priority logic
    }

    public void AddSubScreen(LocationWrapper location)
    {
        ExplorationScreen screen = new ExplorationScreen { Parent = this };
        screen.SetLocation(location);
        subscreens.Add(location.Model.ID, screen);
    }

    public void AddLocation(LocationWrapper cardWrapper)
    {
        locations.Add(cardWrapper);
        locationsByCardId.Add(cardWrapper.Card.GetInstanceId(), cardWrapper);
    }

    public void AddSurvivor(CharacterWrapper cardWrapper)
    {
        survivors.Add(cardWrapper);
        charactersByCardId.Add(cardWrapper.Card.GetInstanceId(), cardWrapper);
    }

    public override void Destroy()
    {
        foreach (KeyValuePair<string, ExplorationScreen> kvp in subscreens)
            kvp.Value.Destroy();

        foreach (CharacterWrapper character in survivors)
            RemoveChild(character.Card);

        foreach (LocationWrapper location in locations)
            RemoveChild(location.Card);
    }

    protected override void UpdateButtons()
    {
        //throw new System.NotImplementedException();
    }
}
