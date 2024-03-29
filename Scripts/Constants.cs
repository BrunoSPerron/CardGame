﻿using System;
using Godot;
using System.Reflection;

public static class CONSTS
{
    public const int COMBAT_HAND_SIZE = 5;
    public const int FIELD_HAND_SIZE = 5;

    // TODO Get the actual setting from GODOT
    public const int MAX_Z_INDEX = 4096;

    //TODO Find a way to have this dynamic
    public static Vector2 SCREEN_CENTER => SCREEN_SIZE / 2;
    public static readonly Vector2 SCREEN_SIZE = new Vector2(682f, 400f);

    public static readonly Color BLACK = new Color(0.0784313725f, 0.1098039216f, 0.1450980392f);
    public static readonly Color PALE_BROWN = new Color(0.6117647059f, 0.3686274510f, 0.2823529412f);
}

public static class RANDOM
{
    public static Random rand = new Random();

    public static float GetNegOneToOne()
    {
        return (float)rand.NextDouble() * 2 - 1;
    }
}
