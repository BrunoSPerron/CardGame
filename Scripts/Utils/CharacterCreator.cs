using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


// IMPORTANT NOTES:
//      - Methods with lowercase names here may be invoked via json files.
//      - Arguments are limited to string.
public static class CharacterCreator
{

    /// <summary>
    /// Create a character by calling 'factory' methods name in an array of strings.
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    /// <returns></returns>
    public static CharacterModel CreateFromInstructions(string[] instructions)
    {
        Factory factory = new Factory();
        Type factoryType = factory.GetType();

        foreach (string instruction in instructions)
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

        return factory.characterInfo;
    }


    #pragma warning disable IDE1006
    // Methods in this Factory class must by public and use lowercase
    //  names to be invokable.
    public class Factory
    {
        public CharacterModel characterInfo = new CharacterModel();

        public void rename(string newName = "random")
        {
            switch (newName)
            {
                case "random":
                    characterInfo.Name = NameGenerator.GetRandomName();
                    break;
                case "random female":
                    characterInfo.Name = NameGenerator.GetRandomFemaleName();
                    break;
                case "random male":
                    characterInfo.Name = NameGenerator.GetRandomMaleName();
                    break;
                default:
                    characterInfo.Name = newName;
                    break;
            }
        }
    }
}
