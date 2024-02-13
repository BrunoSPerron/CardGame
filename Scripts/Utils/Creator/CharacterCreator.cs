using Godot;
using System;
using System.Reflection;


public static class CharacterCreator
{
    /// <summary>
    /// Create a character by calling the 'AssemblyLine' methods Listed in an array of string
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    public static CharacterModel CreateFromModel(CharacterCreationModel model)
    {
        AssemblyLine assemblyLine = new AssemblyLine();
        assemblyLine.model.Mod = model.Mod;

        Type factoryType = assemblyLine.GetType();

        SetInitialValues(assemblyLine, model);

        foreach (string instruction in model.Instructions)
        {
            string[] splittedInstruction = instruction.Split(
                new string[] { "->" }, StringSplitOptions.None);

            string name = splittedInstruction[0].Replace(" ", "").ToLowerInvariant();
            string arguments = splittedInstruction.Length > 1
                ? splittedInstruction[1].ToLowerInvariant()
                : null;
            string[] splittedArguments = arguments?.Split('/') ?? new string[0];
            for (int i = 0; i < splittedArguments.Length; i++)
                splittedArguments[i] = splittedArguments[i].Trim();
            try
            {
                MethodInfo theMethod = factoryType.GetMethod(name);
                theMethod.Invoke(assemblyLine, new[] { splittedArguments });
            }
            catch
            {
                GD.PrintErr(
                    "Character creator error: Invalid instruction from json. \"" +
                    instruction + "\" at: " + model.JsonFilePath);
            }
        }

        return assemblyLine.model;
    }

    private static void SetCombatDeck(
        AssemblyLine assemblyLine, CharacterCreationModel model)
    {
        string combatDeckToUse = model.CombatDeck;
        if (model.CombatDeck == null || model.CombatDeck.Trim() == "")
        {
            GD.PrintErr("Character creator error: CombatDeck missing in file "
                + model.JsonFilePath);
            combatDeckToUse = "core__simple";
        }
        assemblyLine.replacecombatdeck(new string[] { combatDeckToUse });
    }

    private static void SetFieldDeck(
        AssemblyLine assemblyLine, CharacterCreationModel model)
    {
        string fieldDeckToUse = model.FieldDeck;
        if (model.CombatDeck == null || model.CombatDeck.Trim() == "")
        {
            GD.PrintErr("Character creator error: CombatDeck missing in file "
                + model.JsonFilePath);
            fieldDeckToUse = "core__simple";
        }
        assemblyLine.replacefielddeck(new string[] { fieldDeckToUse });
    }

    public static void SetInitialValues(
        AssemblyLine assemblyLine, CharacterCreationModel origin)
    {

        SetCombatDeck(assemblyLine, origin);
        SetFieldDeck(assemblyLine, origin);
        CharacterModel target = assemblyLine.model;
        target.Name = origin.Name;

        target.ActionPoint = origin.ActionPoint;
        target.CurrentActionPoint = origin.CurrentActionPoint == -1
            ? origin.ActionPoint : origin.CurrentActionPoint;

        target.HitPoint = origin.HitPoint;
        target.CurrentHitPoint = origin.CurrentHitPoint == -1
            ? origin.HitPoint : origin.CurrentHitPoint;

        target.Power = origin.Power;

        if (origin.Image != null)
            assemblyLine.addimagelayers(new string[] { origin.Image });
    }


    // Methods in this class must be public and use lowercase names to be invokable.
    // They take a single argument, which is an array of string.
    public class AssemblyLine
    {
#pragma warning disable IDE1006
#pragma warning disable IDE0060
        public CharacterModel model = new CharacterModel();

        /// <param name="args"> 0: card image name </param>
        public void addimagelayers(string[] args)
        {
            string imageFolderPath = System.IO.Path.Combine(
                PathHelper.ModFolder, model.Mod, "Images\\Cards");

            foreach (string s in args)
            {
                string filePath = System.IO.Path.Combine(imageFolderPath, s);
                if (System.IO.Directory.Exists(filePath))
                {
                    string[] files = System.IO.Directory.GetFiles(filePath, "*.png");
                    filePath = files[RANDOM.rand.Next(files.Length)];
                }
                model.ImageLayers.Add(filePath);
            }
        }

        /// <param name="args"> 0: amount </param>
        public void addrandomstats(string[] args)
        {
            int amount = args[0].ToInt();

            for (int i = 0; i < amount; i++)
            {
                int randomValue = RANDOM.rand.Next(0, 3);
                switch (randomValue)
                {
                    case 0:
                        model.Power++;
                        break;
                    case 1:
                        model.HitPoint++;
                        model.CurrentHitPoint++;
                        break;
                    case 2:
                        model.ActionPoint++;
                        model.CurrentActionPoint++;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <param name="args"> 0: Name or Command </param>
        public void rename(string[] args)
        {
            if (args.Length > 0)
                model.Name = args[0];
            else
                renamerandom(args);
        }

        /// <param name="args"> N/A </param>
        public void renamerandom(string[] args)
        {
            model.Name = NameGenerator.GetRandomName();
        }

        /// <param name="args"> N/A </param>
        public void renamerandomfemale(string[] args)
        {
            model.Name = NameGenerator.GetRandomFemaleName();
        }

        /// <param name="args"> N/A </param>
        public void renamerandommale(string[] args)
        {
            model.Name = NameGenerator.GetRandomMaleName();
        }

        /// <param name="args"> 0: CombatDeckCreation FileName </param>
        public void replacecombatdeck(string[] args)
        {
            FileToLoad fileToLoad = PathHelper.GetFileToLoadInfo(args[0], model);
            CombatDeckCreationModel combatModel = JsonLoader.GetCombatDeckCreationModel(
                fileToLoad);
            model.CombatDeck = CombatDeckCreator.CreateFromModel(combatModel);
        }

        /// <param name="args"> 0: FieldDeckCreation FileName </param>
        public void replacefielddeck(string[] args)
        {
            FileToLoad fileToLoad = PathHelper.GetFileToLoadInfo(args[0], model);
            FieldDeckCreationModel fieldModel = JsonLoader.GetFieldDeckCreationModel(
                fileToLoad);
            model.FieldDeck = FieldDeckCreator.CreateFromModel(fieldModel);
        }
    }
}
