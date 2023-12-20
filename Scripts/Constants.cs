using System.IO;
using System.Reflection;

public static class CONSTS
{
    // Not sure how to get the actual setting from GODOT
    public const int MAX_Z_INDEX = 4096;

    //TODO replace with dynamic and delete this
    public static readonly Godot.Vector2 SCREEN_CENTER = new Godot.Vector2(338, 200);
}

public static class PATHS
{
    public static string ModFolderPath { get; } = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonMods");
}

