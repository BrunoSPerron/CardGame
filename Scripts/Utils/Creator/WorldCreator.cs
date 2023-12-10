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
    public static WorldModel CreateFromInstructions(WorldCreationModel model)
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
            string[] splittedArguments = arguments.Split('/');

            try
            {
                for (int i = 0; i < splittedArguments.Length; i++)
                    splittedArguments[i] = splittedArguments[i].Trim();

                MethodInfo theMethod = factoryType.GetMethod(name);
                theMethod.Invoke(factory, splittedArguments);
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
        public WorldModel model = new WorldModel();

        public void addlocation(string locationName = "random")
        {
            HexLocation hexLocation = new HexLocation();
            hexLocation.Location = JsonLoader.GetLocationModel(model.Mod, locationName);

            model.Locations.Add(hexLocation);
        }
    }
}
