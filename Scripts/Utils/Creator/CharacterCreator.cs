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
                    instruction + "\"");
            }
        }

        return assemblyLine.model;
    }


    #pragma warning disable IDE1006
    // Methods in this class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class AssemblyLine
    {
        public CharacterModel model = new CharacterModel();

        /// <param name="args">
        /// 0: Name or Command
        /// </param>
        public void rename(string[] args)
        {
            string newName = args.Length > 0 ? args[0] : "random";
            switch (newName)
            {
                case "random":
                    model.Name = NameGenerator.GetRandomName();
                    break;
                case "random female":
                    model.Name = NameGenerator.GetRandomFemaleName();
                    break;
                case "random male":
                    model.Name = NameGenerator.GetRandomMaleName();
                    break;
                default:
                    model.Name = newName;
                    break;
            }
        }
    }
}
