
using System;
#pragma warning disable CS0660 // Missing Object.Equals override
#pragma warning disable CS0661 // Missing Object.GetHashCode override

public struct Vector2Int
{
    public int x;
    public int y;
    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Vector2Int first, Vector2Int second)
    {
        return first.x == second.x && first.y == second.y;
    }

    public static bool operator !=(Vector2Int first, Vector2Int second)
    {
        return first.x != second.x || first.y != second.y;
    }
}