using Godot;
using System;
using System.Reflection;


public static class FieldDeckCreator
{

    /// <summary>
    /// Create a character by calling the 'AssemblyLine' methods Listed in an array of string
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    public static FieldDeckModel CreateFromModel(FieldDeckCreationModel model)
    {
        AssemblyLine assemblyLine = new AssemblyLine();
        assemblyLine.model.Mod = model.Mod;
        assemblyLine.JSONCreationPath = model.JsonFilePath;
        Type factoryType = assemblyLine.GetType();

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
                    "Field deck creator error: Invalid instruction from json. \"" +
                    instruction + "\"");
            }
        }


        while (assemblyLine.model.FieldDeck.Count < 4)
            assemblyLine.addcard(new string[0]);

        return assemblyLine.model;
    }


    // Methods in this class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class AssemblyLine
    {
        #pragma warning disable IDE1006

        public FieldDeckModel model = new FieldDeckModel();
        public string JSONCreationPath;

        /// <param name="args">
        /// 0: Card Name
        /// 1: amount = 1
        /// </param>
        public void addcard(string[] args)
        {
            string fullCardName = args.Length > 0 ? args[0] : "core__drool";
            int amount = args.Length > 1 ? Int32.Parse(args[1]) : 1;

            FileToLoad fileToLoad = PathHelper.GetFileToLoadInfo(fullCardName, model);
            FieldCardModel card = JsonLoader.GetFieldCardModel(fileToLoad);

            for (int i = 0; i < amount; i++)
                model.FieldDeck.Add(card.CloneViaJSON());
        }
    }
}
