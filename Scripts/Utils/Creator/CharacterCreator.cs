using Godot;
using System;
using System.Reflection;


public static class CharacterCreator
{

    /// <summary>
    /// Create a character by calling 'factory' methods name in an array of strings.
    /// Arguments are limited to string arguments
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.." (limited to string arguments)</param>
    /// <returns></returns>
    public static CharacterModel CreateFromModel(CharacterCreationModel model)
    {
        Factory factory = new Factory();
        factory.model.Mod = model.Mod;
        Type factoryType = factory.GetType();

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
                theMethod.Invoke(factory, new[] { splittedArguments });
            }
            catch
            {
                GD.PrintErr(
                    "Character creator error: Invalid instruction from json. \"" +
                    instruction + "\"");
            }
        }

        return factory.model;
    }


    #pragma warning disable IDE1006
    // Methods in this class must be public and use lowercase
    //  names to be invokable.
    public class Factory
    {
        public CharacterModel model = new CharacterModel();

        /// <param name="args">
        /// 0: New name
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
