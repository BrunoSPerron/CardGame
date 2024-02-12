using Godot;
using System;

public class CharacterCreationModel: BaseModel
{
    public string Name = "core__survivor";

    public int ActionPoint = 1;
    public int CurrentActionPoint = -1;
    public int CurrentHitPoint = -1;
    public int HitPoint = 1;
    public int Power = 1;

    public string Image;

    public string CombatDeck = "core__simple";
    public string FieldDeck = "core__simple";
    public string[] Instructions = new string[0];
}
