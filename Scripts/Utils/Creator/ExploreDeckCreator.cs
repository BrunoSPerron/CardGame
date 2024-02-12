using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public static class ExploreDeckCreator
{

    /// <summary>
    /// Create a character by calling the 'AssemblyLine' methods Listed in an array of string
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    public static List<EncounterModel> CreateFromModel(ExploreDeckCreationModel model)
    {
        AssemblyLine assemblyLine = new AssemblyLine(model.Mod);
        Type factoryType = assemblyLine.GetType();

        assemblyLine.addencounters(model.Encounters);

        foreach (string instruction in model.Instructions)
        {
            string[] splittedInstruction = instruction.Split(
                new string[] { "->" }, StringSplitOptions.None);

            string name = splittedInstruction[0].Replace(" ", "").ToLowerInvariant();
            string arguments = splittedInstruction.Length > 1
                ? splittedInstruction[1].ToLowerInvariant()
                : "";
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
                    "Explore deck creator error: Invalid instruction from json. "
                    + model.Mod + " \"" + instruction + "\"");
            }
        }

        return assemblyLine.models;
    }

    // Methods in this class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class AssemblyLine
    {
        #pragma warning disable IDE1006

        public List<EncounterModel> models = new List<EncounterModel>();
        private readonly string mod;

        public AssemblyLine (string mod)
        {
            this.mod = mod;
        }

        /// <param name="args">
        /// 0+: encounter(s)
        /// </param>
        public void addencounter(string[] args)
        {
            foreach (string arg in args)
            {
                FileToLoad fileToLoad = PathHelper.GetFileToLoadInfo(arg, mod);
                EncounterModel model = JsonLoader.GetEncounterModel(fileToLoad);
                models.Add(model);
            }
        }


        //===Aliases===

        public void addcard(string[] args) => addencounter(args);
        public void addencounters(string[] args) => addencounter(args);
    }
}
