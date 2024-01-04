using Godot;
using System.Reflection;

public static class CONSTS
{
    // TODO Get the actual setting from GODOT
    public const int MAX_Z_INDEX = 4096;

    //TODO Find a way to have this dynamic
    public static Vector2 SCREEN_CENTER => SCREEN_SIZE / 2;
    public static readonly Vector2 SCREEN_SIZE = new Vector2(682f, 400f);

    public static readonly Color BLACK = new Color(0.0784313725f, 0.1098039216f, 0.1450980392f);
    public static readonly Color PALE_BROWN = new Color(0.6117647059f, 0.3686274510f, 0.2823529412f);
}

public static class PATHS
{
    public static string ModFolderPath { get; } = System.IO.Path.Combine(
        System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonMods");
}
