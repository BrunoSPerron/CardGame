using System.IO;
using System.Reflection;

public static class PATHS
{
    public static string ModFolderPath { get; } = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "JsonMods");
}

