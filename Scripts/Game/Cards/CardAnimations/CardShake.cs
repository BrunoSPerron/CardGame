using Godot;
using System;

public class CardShake : CardAnimationBase
{
    private float trauma = 0;

    private static OpenSimplexNoise noise = new OpenSimplexNoise()
    {
        Seed = 12,
        Lacunarity = 2.5f,
        Octaves = 3,
        Period = 0.25f,
        Persistence = 0.18f,
    };

    private float noiseIndexA;
    private float noiseIndexB;
    private float noiseIndexC;

    public float DoubleMaxAngle = 120;
    public float DoubleMaxOffsetX = 250;
    public float DoubleMaxOffsetY = 150;

    private Vector2 lastOffset;

    public CardShake(Card card, float trauma) : base(card)
    {
        this.trauma = trauma;
        noiseIndexA = (float)RANDOM.rand.NextDouble();
        noiseIndexB = (float)RANDOM.rand.NextDouble();
        noiseIndexC = (float)RANDOM.rand.NextDouble();
    }

    public override void _Process(float delta)
    {
        if (trauma > 0)
        {
            noiseIndexA += delta;
            noiseIndexB += delta;
            noiseIndexC += delta;
            float shake = trauma * trauma;
            float angle = DoubleMaxAngle * shake * noise.GetNoise1d(noiseIndexA);
            float offsetX = DoubleMaxOffsetX * shake * noise.GetNoise1d(noiseIndexB);
            float offsetY = DoubleMaxOffsetY * shake * noise.GetNoise1d(noiseIndexC);

            Card.RotationDegrees = angle;
            Card.Position = Card.Position + new Vector2(offsetX, offsetY) - lastOffset;

            lastOffset = new Vector2(offsetX, offsetY);
            trauma -= 1.25f * delta;
        }
        else
        {
            Destroy();
        }
    }

    public override void Destroy()
    {
        Card.Rotation = 0;
        Card.MoveToPosition(Card.Target);
        QueueFree();
    }

    public void AddTrauma(float trauma)
    {
        this.trauma += trauma;
        if (this.trauma > 1)
            this.trauma = 1;
    }
}
