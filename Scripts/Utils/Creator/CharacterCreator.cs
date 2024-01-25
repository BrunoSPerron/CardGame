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

        SetCombatDeck(assemblyLine, model);
        SetFieldDeck(assemblyLine, model);

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
            combatDeckToUse = "core__bad";
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
            fieldDeckToUse = "core__bad";
        }
        assemblyLine.replacefielddeck(new string[] { fieldDeckToUse });
    }


    // Methods in this class must be public and use lowercase names to be invokable.
    // They accept a single argument, which must be an array of string.
    public class AssemblyLine
    {
        #pragma warning disable IDE1006
        #pragma warning disable IDE0060
        public CharacterModel model = new CharacterModel();

        /// <param name="args">
        /// 0: Name or Command
        /// </param>
        public void rename(string[] args)
        {
            if (args.Length > 0)
                model.Name = args[0];
            else
                renamerandom(args);
        }

        public void renamerandom(string[] args)
        {
            model.Name = NameGenerator.GetRandomName();
        }

        public void renamerandomfemale(string[] args)
        {
            model.Name = NameGenerator.GetRandomFemaleName();
        }

        public void renamerandommale(string[] args)
        {
            model.Name = NameGenerator.GetRandomMaleName();
        }

        /// <param name="args">
        /// 0: CombatDeckCreation FileName
        /// </param>
        public void replacecombatdeck(string[] args)
        {
            (string name, string mod) = PathHelper.GetNameAndMod(args[0], model);
            CombatDeckCreationModel combatModel = JsonLoader.GetCombatDeckCreationModel(
                mod, name);
            model.CombatDeck = CombatDeckCreator.CreateFromModel(combatModel);
        }

        /// <param name="args">
        /// 0: FieldDeckCreation FileName
        /// </param>
        public void replacefielddeck(string[] args)
        {
            (string name, string mod) = PathHelper.GetNameAndMod(args[0], model);
            FieldDeckCreationModel fieldModel = JsonLoader.GetFieldDeckCreationModel(
                mod, name);
            model.FieldDeck = FieldDeckCreator.CreateFromModel(fieldModel);
        }
    }
}
