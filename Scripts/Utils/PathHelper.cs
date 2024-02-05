using System;
using System.Collections.Generic;
using System.IO;


public static class PathHelper
{
    public static Tuple<string, string> GetNameAndMod(string name, BaseModel model)
    {
        string mod = model.Mod;
        string[] splittedName = name.Split(
            new string[] { "__" }, StringSplitOptions.None);
        string cardName = splittedName[0];
        if (splittedName.Length > 1)
        {
            mod = splittedName[0];
            cardName = splittedName[1];
        }
        return new Tuple<string, string>(cardName, mod);
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
