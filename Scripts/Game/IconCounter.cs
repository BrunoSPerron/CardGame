using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class IconCounter : Node2D
{
    [Export] public CardIcon defaultIcon;

    private readonly List<IconInfo> icons = new List<IconInfo>();

    private int max = 0;
    private int current = 0;

    // ===== Methods unique to this class =====
    // === public ===

    private void AddIcon(CardIcon? icon = null)
    {
        CardIcon iconToUse = icon ?? defaultIcon;
        StreamTexture texture = GetIconTexture(iconToUse);

        int offsetX = icons.Count * (texture.GetWidth() + 1);
        Sprite sprite = new Sprite { Texture = texture };
        IconInfo info = new IconInfo()
        {
            Icon = iconToUse,
            Sprite = sprite,
            Alpha = 1
        };
        icons.Add(info);
        AddChild(sprite);
        sprite.Position = new Vector2(offsetX, 0);
        current++;
    }

    private void RemoveIcon()
    {
        int index = icons.Count - 1;
        RemoveChild(icons[index].Sprite);
        icons.RemoveAt(index);
        current--;
    }

    public void SetMax(int amount)
    {
        //TODO update correctly when gaining max hp while damaged

        if (current > max)
            current = max;

        max = amount;
        var i = icons.Count;
        while (i != amount)
        {
            if (i > amount)
            {
                RemoveIcon();
                i--;
            }
            else
            {
                AddIcon();
                i++;
            }
        }
    }

    public void SetCurrent(int amount)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            IconInfo iconInfo = icons[i];
            Color modulate = iconInfo.Sprite.Modulate;
            modulate.a = i < amount ? 1f : 0.5f;
            iconInfo.Sprite.Modulate = modulate;
        }
    }

    // === private ===

    private StreamTexture GetIconTexture(CardIcon icon)
    {
        return ResourceLoader.Load<StreamTexture>(
            "res://Art/Cards/Icons/" + icon + ".png");
    }


    // ===== Private struct =====


    private struct IconInfo
    {
        public CardIcon Icon;
        public Sprite Sprite;
        public float Alpha;
    }
}
