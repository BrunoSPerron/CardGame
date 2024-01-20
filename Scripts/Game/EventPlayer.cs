using Godot;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        {
            FollowNextInstruction();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (waitingForInput)
        {
            if (!(@event is InputEventMouseMotion))
            {
                waitingForInput = false;
            }
        }
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

    public void WaitForInput()
    {
        waitingForInput = true;
    }


    // Methods in these class must be public and use lowercase names to be invokable.
    // They must accept a single argument, which is an array of string.
    public class Backstage_base{ }

    public class BackStage_Solo : Backstage_base
    {
        #pragma warning disable IDE1006

        private readonly EventPlayer player;
        private readonly CharacterWrapper character;
        private readonly string mod;

        public BackStage_Solo(EventPlayer player, CharacterWrapper character, string mod)
        {
            this.player = player;
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

        public void display(string[] args)
        {
            string path = "res://Assets/UI/PixelText.tscn";
            PackedScene packedScene = ResourceLoader.Load<PackedScene>(path);
            float offsetY = 10;

            for (int i = 0; i < args.Length; i++)
            {
                PixelText text = packedScene.Instance<PixelText>();
                player.AddChild(text);
                text.Position = new Vector2(100, 100 + offsetY * i);
                text.SetLabel(args[i]);
            }
            player.WaitForInput();
        }
    }
}
