using System.Collections.Generic;

public class CharacterModel : BaseModel
{
    public string Name = "Survivor";
    public int HitPoint = 3;
    public int CurrentHitPoint = 3;
    public int Power = 1;
    public int ActionPoint = 2;
    public int CurrentActionPoint = 2;

    public Vector2Int WorldPosition;

    public CombatDeckModel CombatDeck = new CombatDeckModel();
    public FieldDeckModel FieldDeck = new FieldDeckModel();

    public List<ItemModel> Items = new List<ItemModel>();
}
