using Godot;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

public class EventPlayer : BaseGameScreen
{
    public override void Destroy()
    {
    }

    /// <summary>
    /// Create a character by calling the 'AssemblyLine' methods Listed in an array of string
    /// </summary>
    /// <param name="instructions">format: "METHOD_NAME -> ARG1 / ARG2 /.."</param>
    public void PlayEvent(CharacterWrapper character,
        string[] instructions, BaseModel model)
    {
        BackStage_Solo backStage = new BackStage_Solo(this, character, model.Mod);
        Type factoryType = backStage.GetType();

        foreach (string instruction in instructions)
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
                theMethod.Invoke(backStage, new[] { splittedArguments });
            }
            catch
            {
                GD.PrintErr(
                    "PlayEventError: Invalid instruction from json. \"" +
                    instruction + "\" at: " + model.JsonFilePath);
            }
        }
    }


    // Methods in this class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class BackStage_Solo
    {
        #pragma warning disable IDE1006

        private readonly BaseGameScreen stage;
        private readonly CharacterWrapper character;
        private readonly string mod;

        public BackStage_Solo(BaseGameScreen stage, CharacterWrapper character, string mod)
        {
            this.stage = stage;
            this.character = character;
            this.mod = mod; 
        }

        /// <param name="args">
        /// 0: Name
        /// </param>
        public void additem(string[] args)
        {
            foreach (string itemName in args)
            {
                ItemModel item = JsonLoader.GetItemModel(mod, itemName);
                character.AddItem(item);
            }
        }
    }
}
