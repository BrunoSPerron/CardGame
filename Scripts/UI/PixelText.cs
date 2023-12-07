using Godot;
using System;

//Simple bitmap text simulacrum expecting a specific texture.
//TODO Replace with bitmap font (Supported in Godot 4)
public class PixelText : Node2D
{
    [Export]
    public Texture AtlasTexture;

    [Export]
    public int TexCharWidth = 6;

    [Export]
    public int TexHeight = 8;

    private string value;
    [Export]
    public string Value {
        get => value;
        set
        {
            if (value != this.value)
            {
                this.value = value;
                SetText(value);
            }
        }
    }


    private int characterAmount = 0;
    private Alignement textAlign = Alignement.LEFT;


    private void AddChar(char c)
    {
        if (c == ' ')
        {
            characterAmount++;
            return;
        }

        int textureOffsetX;
        if (c > 96)
            textureOffsetX = c - 97;
        else if (c < 65)
            textureOffsetX = c - 22;
        else
            textureOffsetX = c - 65;
        Sprite sprite = new Sprite();
        AtlasTexture atlasInstance = new AtlasTexture
        {
            Atlas = AtlasTexture,
            Region = new Rect2(textureOffsetX * TexCharWidth, 0, TexCharWidth, TexHeight)
        };
        sprite.Texture = atlasInstance;
        GetNode<Position2D>("SpriteContainer").AddChild(sprite);
        sprite.Position = new Vector2(characterAmount * TexCharWidth, 0);
        characterAmount++;
    }

    public void AlignCenter()
    {
        textAlign = Alignement.CENTER;
        UpdateTextPosition();
    }

    public void AlignLeft()
    {
        textAlign = Alignement.LEFT;
        UpdateTextPosition();
    }

    public void AlignRight()
    {
        textAlign = Alignement.RIGHT;
        UpdateTextPosition();
    }

    public void Clear()
    {
        Godot.Collections.Array sprites = GetNode<Position2D>("SpriteContainer").GetChildren();
        foreach (Node child in sprites)
        {
            child.QueueFree();
        }
        sprites.Clear();
        characterAmount = 0;
    }

    public float GetWidth()
    {
        return (TexCharWidth * characterAmount) * Scale.x;
    }

    /// <param name="text">No sanitization</param>
    private void SetText(string text)
    {
        Clear();
        foreach (char c in text)
            AddChar(c);
        UpdateTextPosition();
    }

    private void UpdateTextPosition()
    {
        Position2D spriteContainer = GetNode<Position2D>("SpriteContainer");
        switch (textAlign)
        {
            case Alignement.LEFT:
                spriteContainer.Position = Vector2.Zero;
                break;
            case Alignement.CENTER:
                spriteContainer.Position = new Vector2(
                    -characterAmount / 2f * (TexCharWidth - 1), 0);
                break;
            case Alignement.RIGHT:
                spriteContainer.Position = new Vector2(
                    -characterAmount * (TexCharWidth - 1), 0);
                break;
        }

    }

    private enum Alignement { LEFT, CENTER, RIGHT }
}
