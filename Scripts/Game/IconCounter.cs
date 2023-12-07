using Godot;
using System;
using System.Collections.Generic;

public class IconCounter : Node2D
{
    [Export] public CardIcon icon;

    private List<Sprite> icons = new List<Sprite>();

    private StreamTexture iconTexture;
    public StreamTexture IconTexture
    {
        get
        {
            if (iconTexture == null)
                iconTexture = ResourceLoader.Load<StreamTexture>(
                    "res://Art/Cards/Icons/" + icon + ".png");
            return iconTexture;
        }
        set => iconTexture = value;
    }

    // ===== Methods unique to this class =====

    public void AddIcon()
    {
        int offsetX = icons.Count * (IconTexture.GetWidth() + 1);
        Sprite sprite = new Sprite();
        sprite.Texture = IconTexture;
        icons.Add(sprite);
        AddChild(sprite);
        sprite.Position = new Vector2(offsetX, 0);
    }

    public void RemoveIcon()
    {
        int index = icons.Count - 1;
        RemoveChild(icons[index]);
        icons.RemoveAt(index);
    }

    public void SetAmount(int amount)
    {
        var current = icons.Count;
        while (current != amount)
        {
            if (current > amount)
            {
                RemoveIcon();
                current--;
            }
            else
            {
                AddIcon();
                current++;
            }
        }
    }
}
