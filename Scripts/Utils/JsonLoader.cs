using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public static class JsonLoader
{
    ///<returns>The object serialized in a Json encoded string.</returns>
    public static string Serialize<T>(this T source) // .Serialize() extension
    {
        return JsonConvert.SerializeObject(source);
    }

    // ===== Getters =====


    public static CharacterCreationModel GetCharacterCreationModel(string modName,
                                                                   string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\CharacterCreation\\" + jsonFileName);
        CharacterCreationModel model =
            GetOfType<CharacterCreationModel>.FromJson(path)
            ?? new CharacterCreationModel();
        model.Mod = modName;
        return model;
    }

    public static CombatCardModel GetCombatCardModel(string modName,string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\CombatCards\\" + jsonFileName);
        CombatCardModel model =
            GetOfType<CombatCardModel>.FromJson(path) ?? new CombatCardModel();
        model.Mod = modName;
        return model;
    }

    public static FieldCardModel GetFieldCardModel(string modName, string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\FieldCards\\" + jsonFileName);
        FieldCardModel model =
            GetOfType<FieldCardModel>.FromJson(path) ?? new FieldCardModel();
        model.Mod = modName;
        return model;
    }

    public static ItemModel GetItemModel(string modName, string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\Items\\" + jsonFileName);
        ItemModel model = GetOfType<ItemModel>.FromJson(path) ?? new ItemModel();
        model.Mod = modName;
        return model;
    }

    public static LocationModel GetLocationModel(string modName, string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\Locations\\" + jsonFileName);
        LocationModel model =
            GetOfType<LocationModel>.FromJson(path) ?? new LocationModel();
        model.Mod = modName;
        return model;
    }

    public static OutsetModel GetOnsetModel(string modName, string scenario)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\Scenarios\\" + scenario + "\\Outset");
        OutsetModel model = GetOfType<OutsetModel>.FromJson(path) ?? new OutsetModel();
        model.Mod = modName;
        return model;
    }

    public static WorldCreationModel GetWorldCreationModel(string modName,
                                                           string jsonFileName)
    {
        string path = Path.Combine(PATHS.ModFolderPath,
            modName + "\\Data\\World\\" + jsonFileName);
        WorldCreationModel model =
            GetOfType<WorldCreationModel>.FromJson(path) ?? new WorldCreationModel();
        model.Mod = modName;
        return model;
    }


    // ===== helper class =====


    private static class GetOfType<T>
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
            string pathWithExtension = Path.ChangeExtension(path, "json");
            if (!cache.ContainsKey(pathWithExtension))
            {
                string jsonString = GetJSONStringFromFile(pathWithExtension);
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
                        + pathWithExtension);
                    T instance = default;
                    if (instance is BaseModel model)
                        model.JsonFilePath = pathWithExtension;
                    cache[pathWithExtension] = instance;
                    return instance;
                }

                if (deserialized is BaseModel baseModel)
                    baseModel.JsonFilePath = pathWithExtension;

                cache.Add(pathWithExtension, deserialized);
                return deserialized;

            }
            return cache[pathWithExtension];
        }
    }
}