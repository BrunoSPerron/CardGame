public class CharacterModel : BaseModel
{
    public string Name = "Survivor";
    public int HitPoint = 3;
    public int CurrentHitPoint = 3;
    public int Power = 1;
    public int ActionPoint = 2;
    public int CurrentActionPoint = 2;

    public string CurrentLocationID;

    public DeckModel CombatDeck = new DeckModel();
    public DeckModel FieldDeck = new DeckModel();

    public ItemModel[] items = new ItemModel[0];
}
