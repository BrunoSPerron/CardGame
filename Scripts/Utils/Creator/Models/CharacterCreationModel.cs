﻿using Godot;
using System;

public class CharacterCreationModel: BaseModel
{
    public int ActionPoint = 1;
    public int CurrentActionPoint = -1;
    public int CurrentHitPoint = -1;
    public int HitPoint = 1;
    public int Power = 1;

    public string CombatDeck = "core__bad";
    public string FieldDeck = "core__bad";
    public string[] Instructions = new string[0];
}
