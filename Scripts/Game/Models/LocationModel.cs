using System;

public class LocationModel : BaseModel    
{
    public string Name = "core_desolation";
    public string ImageFileName = "";

    public string[] destinations = new string[0];

    public SceneActionModel[] OnEnter = new SceneActionModel[0];
    public SceneActionModel[] OnLeave = new SceneActionModel[0];

    public string[] ExplorationDeck = new string[0];
}
