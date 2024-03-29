﻿public enum Cardinal
{
    NONE, N, NW, W, SW, S, SE, E, NE
}

public enum CombatAction
{
    ATTACK
}

/// <summary> Note: These keys are also the resource names. </summary>
public enum CardIcon
{
    HEART,
    SWORD,
    TIME
}

//Note: Some code use (HexLink)(((int)MyHexLink + 3) % 6) to get the reverse
public enum HexLink
{
    TOPLEFT,
    TOPRIGHT,
    RIGHT,
    BOTTOMRIGHT,
    BOTTOMLEFT,
    LEFT,
}

public enum MouseButton
{
    LEFT = 1,
    RIGHT = 2,
    MIDDLE = 3
}

public enum Phase
{
    DAWN = 0,
    MORNING = 1,
    NOON = 2,
    AFTERNOON = 3,
    DUSK = 4,
    NIGHT = 5,
}

/// <summary> Values are cast as integer to get atlas texture 'index' </summary>
public enum PressableState
{
    STAND_BY = 0,
    OVER = 1,
    DOWN = 2
}

public enum CardCleanResponse
{
    NONE,
    RECENT,
    OLD
}