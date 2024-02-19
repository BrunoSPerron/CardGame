using Godot;
using System;


internal class CardFlip : CardAnimationBase
{

    private float timer = 0;
    private readonly float flipTime;
    private readonly float halfFlipTime = -1;
    private bool halfFlipUpdated = false;

    internal CardFlip(Card card, float timeInSeconds = 0.4f) : base(card)
    {
        flipTime = timeInSeconds;
        halfFlipTime = timeInSeconds / 2;
    }

    public override void _Process(float delta)
    {
        if (reversed)
            ReverseProcess(delta);
        else
            RegularProcess(delta);
    }

    public override void _EnterTree()
    {
        Card.IsFaceDown = !Card.IsFaceDown;

        AudioStreamMP3 sound = ResourceLoader.Load<AudioStreamMP3>(
            "res://Audio/Fx/cardFlip.mp3");

        AudioStreamPlayer player = new AudioStreamPlayer
        {
            Stream = sound,
            PitchScale = .7f
        };
        AddChild(player);
        player.Play();
    }

    private void RegularProcess(float delta)
    {
        if (timer > halfFlipTime)
        {
            float currentTime = timer - halfFlipTime;
            if (!halfFlipUpdated)
            {
                Card.Front.Visible = !Card.IsFaceDown;
                Card.Back.Visible = Card.IsFaceDown;
                halfFlipUpdated = true;
            }
            if (Card.IsFaceDown)
                Card.Back.Scale = new Vector2(currentTime / halfFlipTime, 1);
            else
                Card.Front.Scale = new Vector2(currentTime / halfFlipTime, 1);
        }
        else
        {
            if (halfFlipUpdated)
            {
                Card.Front.Visible = Card.IsFaceDown;
                Card.Back.Visible = !Card.IsFaceDown;
                halfFlipUpdated = false;
            }
            if (Card.IsFaceDown)
                Card.Front.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
            else
                Card.Back.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
        }

        timer += delta;
        if (flipTime < timer)
            Destroy();
    }

    private void ReverseProcess(float delta)
    {
        if (timer > halfFlipTime)
        {
            float currentTime = timer - halfFlipTime;
            if (halfFlipUpdated)
            {
                Card.Front.Visible = Card.IsFaceDown;
                Card.Back.Visible = !Card.IsFaceDown;
                halfFlipUpdated = false;
            }
            if (Card.IsFaceDown)
                Card.Front.Scale = new Vector2(currentTime / halfFlipTime, 1);
            else
                Card.Back.Scale = new Vector2(currentTime / halfFlipTime, 1);
        }
        else
        {
            if (!halfFlipUpdated)
            {
                Card.Front.Visible = !Card.IsFaceDown;
                Card.Back.Visible = Card.IsFaceDown;
                halfFlipUpdated = true;
            }
            if (Card.IsFaceDown)
                Card.Back.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
            else
                Card.Front.Scale = new Vector2(1 - (timer / halfFlipTime), 1);
        }

        timer -= delta;
        if (timer < 0)
            Destroy();
    }

    public override void Destroy()
    {
        ScaleToOne();
        QueueFree();
    }
}
