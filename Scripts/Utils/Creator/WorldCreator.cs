using Godot;
using System;
using System.Reflection;


public static class WorldCreator
{

    /// <summary>
    /// Create a character by calling 'factory' methods name in an array of strings.
    /// Arguments are limited to string arguments
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.." (limited to string arguments)</param>
    /// <returns></returns>
    public static WorldModel CreateFromModel(WorldCreationModel model)
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
                : "";
            string[] splittedArguments = arguments?.Split('/') ?? new string[0];

            try
            {
                for (int i = 0; i < splittedArguments.Length; i++)
                    splittedArguments[i] = splittedArguments[i].Trim();

                MethodInfo theMethod = factoryType.GetMethod(name);
                theMethod.Invoke(factory, new[] { splittedArguments });
            }
            catch
            {
                GD.PrintErr(
                    "World creator error: Invalid instruction from json." + model.Mod
                    + " \"" + instruction + "\"");
            }
        }

        return factory.model;
    }

#pragma warning disable IDE1006
    // Methods in this class must be public and use lowercase names to be invokable.
    // They must receive a single argument, which is an array of string.
    public class Factory
    {
        public WorldModel model = new WorldModel();

        /// <param name="args">
        /// 0: Location Name
        /// 1: x
        /// 2: y
        /// </param>
        public void addlocation(string[] args)
        {
            string locationName = args.Length > 0 ? args[0]: "random";
            int x = args.Length > 1 ? int.Parse(args[1]) : 0;
            int y = args.Length > 2 ? int.Parse(args[2]) : 0;

            var hexLocation = new HexLocation
            {
                Location = JsonLoader.GetLocationModel(model.Mod, locationName),
                HexPosition = new Vector2Int(x, y)
            };
            model.Locations.Add(hexLocation);
        }
    }
}
