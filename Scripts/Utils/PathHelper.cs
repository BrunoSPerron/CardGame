using System;
using System.Collections.Generic;
using System.IO;


public static class PathHelper
{
    public static FileToLoad GetFileToLoadInfo(string name, BaseModel model)
    {
        return GetFileToLoadInfo(name, model.Mod);
    }

    public static FileToLoad GetFileToLoadInfo(string name, string baseMod)
    {
        string mod = baseMod;
        string[] splittedName = name.Split(
            new string[] { "__" }, StringSplitOptions.None);
        string cardName = splittedName[0];
        if (splittedName.Length > 1)
        {
            mod = splittedName[0];
            cardName = splittedName[1];
        }
        FileToLoad fileToLoad = new FileToLoad()
        {
            FileName = cardName,
            Mod = mod
        };
        if (mod != baseMod)
            fileToLoad.FallbackMod = baseMod;

        return fileToLoad;
    }

    public static List<string> GetTopDirectoriesInFolder(string folderPath)
    {
        string[] unfiltered = Directory.GetDirectories(
            folderPath, "*", SearchOption.TopDirectoryOnly);
        List<string> filtered = new List<string>();

        foreach (string s in unfiltered)
            if(!s.StartsWith("__"))
                filtered.Add(s);

        return filtered;
    }
}
