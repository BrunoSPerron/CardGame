using Godot;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class EventPlayer: BaseGameScreen
{
    [Signal]
    public delegate void OnEventEnd();

    private readonly Backstage_base backStage;
    private readonly Type backstageType;
    private readonly string[] instructions;
    private int instructionIndex = 0;
    private readonly BaseModel originModel;
    private bool waitingForInput = false;

    public EventPlayer(CharacterWrapper character, 
        string[] instructions, BaseModel eventOrigin)
    {
        originModel = eventOrigin;
        this.instructions = instructions;
        backStage = new BackStage_Solo(this, character, eventOrigin.Mod);
        backstageType = backStage.GetType();
        ZIndex = CardManager.NumberOfCards;
    }


    public override void _Process(float delta)
    {
        if (!waitingForInput)
            FollowNextInstruction();
    }

    public override void _Input(InputEvent @event)
    {
        if (waitingForInput && !(@event is InputEventMouseMotion))
            waitingForInput = false;
    }

    public override void Destroy()
    {
        //TODO make sure there's no card as child somehow
        QueueFree();
    }

    public void FollowInstruction(string instruction)
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
            MethodInfo theMethod = backstageType.GetMethod(name);
            theMethod.Invoke(backStage, new[] { splittedArguments });
        }
        catch
        {
            GD.PrintErr(
                "Event player error: Invalid instruction from json. \"" +
                instruction + "\" at: " + originModel.JsonFilePath);
        }
    }

    public void FollowNextInstruction()
    {
        if (instructionIndex >= instructions.Length)
        {
            EmitSignal("OnEventEnd");
            Destroy();
            return;
        }
        FollowInstruction(instructions[instructionIndex]);
        instructionIndex++;
    }

    public string ReplaceRegex(string text)
    {
        string replaced = text;
        MatchCollection matches = Regex.Matches(text, @"\[~.*?\]");
        foreach (Match match in matches)
        {
            switch(match.Value)
            {
                case "[~character]":
                    replaced = replaced.Replace(match.Value, backStage.Characters[0].Name);
                    break;
                default:
                    GD.PrintErr("Event player error: Tag " + match.Value
                        + " not valid, in message \"" + text
                        + "\" from " + originModel.JsonFilePath);
                    break;
            }
        }
        return replaced;
    }

    public void WaitForInput()
    {
        waitingForInput = true;
    }


    // Methods in these class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public abstract class Backstage_base
    {
        public abstract CharacterWrapper[] Characters { get; }

        protected readonly string mod;
        protected readonly EventPlayer player;

        public Backstage_base(EventPlayer player, string mod)
        {
            this.player = player;
            this.mod = mod;
        }
    }

    public class BackStage_Solo : Backstage_base
    {
#pragma warning disable IDE0060
#pragma warning disable IDE1006

        public override CharacterWrapper[] Characters => new CharacterWrapper[]
        {
            character
        };

        private readonly CharacterWrapper character;

        public BackStage_Solo(EventPlayer player, CharacterWrapper character, string mod)
            : base(player, mod)
        {
            this.character = character;
        }

        /// <param name="args">
        /// 0: Name
        /// </param>
        public void additem(string[] args)
        {
            foreach (string itemName in args)
            {
                FileToLoad fileToload = PathHelper.GetFileToLoadInfo(itemName, mod);
                ItemModel item = JsonLoader.GetItemModel(fileToload);
                character.AddItem(item);
            }
        }

        /// <param name="args">
        /// 0+: Line to display
        /// </param>
        public void display(string[] args)
        {
            string path = "res://Assets/UI/PixelText.tscn";
            PackedScene packedScene = ResourceLoader.Load<PackedScene>(path);
            float offsetY = 10;

            for (int i = 0; i < args.Length; i++)
            {
                PixelText text = packedScene.Instance<PixelText>();
                player.AddChild(text);
                text.Position = new Vector2(225, 100 + offsetY * i);
                text.SetLabel(player.ReplaceRegex(args[i]));
            }
            player.WaitForInput();
        }

        /// <param name="args">
        /// 0+: Opponent (Character creation logic file name)
        /// </param>
        public void fight(string[] args)
        {
            CombatScreen screen = new CombatScreen();
            player.AddChild(screen);
        }
    }
}
