public class ExplorationEncounterModel : BaseModel
{
    public string Name { get; set; }

    public SceneActionModel[] Steps { get; set; }
}
