using System;

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

    public static Vector2Int operator -(Vector2Int first, Vector2Int second)
    {
        return new Vector2Int(first.x - second.x, first.y - second.y);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector2Int @int &&
               x == @int.x &&
               y == @int.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}