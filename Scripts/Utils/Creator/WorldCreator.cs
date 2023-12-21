using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public static class WorldCreator
{

    /// <summary>
    /// Create a character by calling the 'AssemblyLine' methods Listed in an array of string
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    public static WorldModel CreateFromModel(WorldCreationModel model)
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
                    "World creator error: Invalid instruction from json. "
                    + model.Mod + " \"" + instruction + "\"");
            }
        }

        return assemblyLine.model;
    }

#pragma warning disable IDE1006
    // Methods in this class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class AssemblyLine
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
                WorldHexModel location = model.Locations[i];
                if (location.Coord == positionVector)
                {
                    location.Location = JsonLoader.GetLocationModel(model.Mod, locationName);
                    newLocation = false;
                    break;
                }
            }

            if (newLocation)
            {
                model.Locations.Add(new WorldHexModel
                {
                    Location = JsonLoader.GetLocationModel(model.Mod, locationName),
                    Coord = positionVector
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
            WorldHexModel hexlocation = new WorldHexModel() { Coord = positionVector };
            bool newLocation = true;
            foreach (WorldHexModel location in model.Locations)
            {
                if (location.Coord == positionVector)
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
