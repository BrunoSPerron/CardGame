using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public static class WorldCreator
{

    /// <summary>
    /// Create a character by calling 'factory' methods name in an array of strings.
    /// Arguments are limited to string arguments
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.." (limited to string arguments)</param>
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
                    "World creator error: Invalid instruction from json. "
                    + model.Mod + " \"" + instruction + "\"");
            }
        }

        return factory.model;
    }

#pragma warning disable IDE1006
    // Methods in this class must be public and use lowercase names to be invokable.
    // They must take a single argument, which is an array of string.
    public class Factory
    {
        public WorldModel model = new WorldModel();

        /// <param name="args">
        /// 0: Location Name
        /// 1: x = 0
        /// 2: y = 0
        /// </param>
        public void addlocation(string[] args)
        {
            string locationName = args.Length > 0 ? args[0] : null;
            int x = args.Length > 1 ? int.Parse(args[1]) : 0;
            int y = args.Length > 2 ? int.Parse(args[2]) : 0;

            Vector2Int positionVector = new Vector2Int(x, y);
            bool newLocation = true;
            for (int i = 0; i < model.Locations.Count; i++)
            {
                HexLocationModel location = model.Locations[i];
                if (location.HexPosition == positionVector)
                {
                    location.Location = JsonLoader.GetLocationModel(model.Mod, locationName);
                    newLocation = false;
                    break;
                }
            }

            if (newLocation)
            {
                model.Locations.Add(new HexLocationModel
                {
                    Location = JsonLoader.GetLocationModel(model.Mod, locationName),
                    HexPosition = positionVector
                });
            }
        }

        /// <param name="args">
        /// 0: x = 0
        /// 1: y = 0
        /// 2+: hex links (enum key)
        /// </param>
        public void openposition(string[] args)
        {
            int x = args.Length > 0 ? int.Parse(args[0]) : 0;
            int y = args.Length > 1 ? int.Parse(args[1]) : 0;

            Vector2Int positionVector = new Vector2Int(x, y);
            HexLocationModel hexlocation = new HexLocationModel() { HexPosition = positionVector };
            bool newLocation = true;
            foreach (HexLocationModel location in model.Locations)
            {
                if (location.HexPosition == positionVector)
                {
                    hexlocation = location;
                    newLocation = false;
                    break;
                }
            }

            if (args.Length < 3)
            {
                foreach (HexLink link in Enum.GetValues(typeof(HexLink)))
                    if (!hexlocation.Openings.Contains(link))
                        hexlocation.Openings.Add(link);
            }
            else
            {
                for (int i = 2; i < args.Length; i++)
                {
                    string arg = args[i].Replace(" ", "");

                    if (Enum.TryParse(arg, true, out HexLink link))
                    {
                        if (!hexlocation.Openings.Contains(link))
                            hexlocation.Openings.Add(link);
                    }
                    else
                    {
                        GD.PrintErr("World creation warning: Unreconized link \""
                            + arg + "\" from: " + model.Mod); //TODO log file
                    }
                }
            }

            if (newLocation)
                model.Locations.Add(hexlocation);
        }
    }
}
