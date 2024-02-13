using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public static class JsonLoader
{
    public static CharacterCreationModel GetCharacterCreationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new CharacterCreationModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\CharacterCreation\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path)) {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\CharacterCreation\\" + nameWithExtension);
        }

        CharacterCreationModel model =
            GetOfType<CharacterCreationModel>.FromJson(path)
            ?? new CharacterCreationModel();
        model.Mod = modName;
        return model;
    }

    public static CombatCardModel GetCombatCardModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new CombatCardModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\CombatCards\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\CombatCards\\" + nameWithExtension);
        }

        CombatCardModel model =
            GetOfType<CombatCardModel>.FromJson(path) ?? new CombatCardModel();
        model.Mod = modName;
        return model;
    }

    public static CombatDeckCreationModel GetCombatDeckCreationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new CombatDeckCreationModel() {};
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\CombatDeckCreation\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\CombatDeckCreation\\" + nameWithExtension);
        }

        CombatDeckCreationModel model =
            GetOfType<CombatDeckCreationModel>.FromJson(path)
            ?? new CombatDeckCreationModel();
        model.Mod = modName;
        return model;
    }

    public static EncounterModel GetEncounterModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new EncounterModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\Encounters\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\Encounters\\" + nameWithExtension);
        }

        EncounterModel model = 
            GetOfType<EncounterModel>.FromJson(path) ?? new EncounterModel();
        model.Mod = modName;
        return model;
    }

    public static ExploreDeckCreationModel GetExploreDeckCreationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new ExploreDeckCreationModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\ExploreDeckCreation\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\ExploreDeckCreation\\" + nameWithExtension);
        }

        ExploreDeckCreationModel model = GetOfType<ExploreDeckCreationModel>.FromJson(path)
            ?? new ExploreDeckCreationModel();
        model.Mod = modName;
        return model;
    }

    public static FieldCardModel GetFieldCardModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new FieldCardModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\FieldCards\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\FieldCards\\" + nameWithExtension);
        }

        FieldCardModel model =
            GetOfType<FieldCardModel>.FromJson(path) ?? new FieldCardModel();
        model.Mod = modName;
        return model;
    }

    public static FieldDeckCreationModel GetFieldDeckCreationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new FieldDeckCreationModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\FieldDeckCreation\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\FieldDeckCreation\\" + nameWithExtension);
        }

        FieldDeckCreationModel model =
            GetOfType<FieldDeckCreationModel>.FromJson(path)
            ?? new FieldDeckCreationModel();
        model.Mod = modName;
        return model;
    }

    public static ItemModel GetItemModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new ItemModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\Items\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\Items\\" + nameWithExtension);
        }

        ItemModel model = GetOfType<ItemModel>.FromJson(path) ?? new ItemModel();
        model.Mod = modName;
        return model;
    }

    public static LocationModel GetLocationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new LocationModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\Locations\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\Locations\\" + nameWithExtension);
        }

        LocationModel model =
            GetOfType<LocationModel>.FromJson(path) ?? new LocationModel();
        model.Mod = modName;
        return model;
    }

    public static OutsetModel GetOnsetModel(string modName, string scenario)
    {
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\Scenarios\\" + scenario + "\\Outset.json");
        OutsetModel model = GetOfType<OutsetModel>.FromJson(path) ?? new OutsetModel();
        model.Mod = modName;
        return model;
    }

    public static WorldCreationModel GetWorldCreationModel(FileToLoad fileToLoad)
    {
        if (fileToLoad.Mod == "core")
        {
            return new WorldCreationModel();
        }

        string nameWithExtension = Path.ChangeExtension(fileToLoad.FileName, "json");
        string modName = fileToLoad.Mod;
        string path = Path.Combine(PathHelper.ModFolder,
            modName + "\\Data\\World\\" + nameWithExtension);
        if (fileToLoad.FallbackMod != null && !File.Exists(path))
        {
            Godot.GD.PrintErr("Json loader warning: Dependency file not find at " + path
                + " requested by mod \"" + fileToLoad.FallbackMod
                + "\". Using fallback provided by that mod, if any.");
            modName = fileToLoad.FallbackMod;
            path = Path.Combine(PathHelper.ModFolder,
                modName + "\\Data\\World\\" + nameWithExtension);
        }

        WorldCreationModel model =
            GetOfType<WorldCreationModel>.FromJson(path) ?? new WorldCreationModel();
        model.Mod = modName;
        return model;
    }


    // ===== helper class =====


    public static class GetOfType<T>
    {
        //TODO Manage cache
        private static readonly Dictionary<string, T> cache
            = new Dictionary<string, T>();
        private static string GetJSONStringFromFile(string pathWithExtension)
        {
            if (!File.Exists(pathWithExtension))
            {
                Godot.GD.PrintErr("JsonLoader error: File not found at "
                    + pathWithExtension);
                return null;
            }
            return File.ReadAllText(pathWithExtension);
        }

        public static T FromJson(string path)
        {
            if (!cache.ContainsKey(path))
            {
                string jsonString = GetJSONStringFromFile(path);
                if (jsonString == null)
                    return default;

                T deserialized;
                try
                {
                    deserialized = JsonConvert.DeserializeObject<T>(jsonString);
                }
                catch
                {
                    Godot.GD.PrintErr(
                        "JsonLoader error: File does not contain valid JSON at "
                        + path);
                    T instance = default;
                    if (instance is BaseModel model)
                        model.JsonFilePath = path;
                    cache[path] = instance;
                    return instance;
                }

                if (deserialized is BaseModel baseModel)
                    baseModel.JsonFilePath = path;

                cache.Add(path, deserialized);
                return deserialized;
            }
            T clone = cache[path].CloneViaJSON();
            if (clone is BaseModel baseModelClone)
                baseModelClone.ID = Guid.NewGuid().ToString();
            return clone;
        }
    }
}