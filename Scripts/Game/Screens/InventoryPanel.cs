using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryPanel: Node2D
{
    private const float INNER_PADDING_X = 10;
    private const float INNER_PADDING_Y = 19;
    private const float OUTER_PADDING = 5;

    public BaseGameScreen GameScreen;
    public CharacterWrapper CharacterWrapper;

    private readonly List<Button> buttons = new List<Button>();
    private Panel panel;

    public override void _Ready()
    {
        panel = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/Panel.tscn/").Instance<Panel>();
        float paddingX2 = OUTER_PADDING * 2;
        Vector2 viewSize = GetTree().Root.GetVisibleRect().Size;
        Vector2 halfViewSize = viewSize / 2;
        panel.SizeOnReady = viewSize - new Vector2(paddingX2, paddingX2);
        panel.Position = halfViewSize;
        AddChild(panel);
        panel.ZIndex = CardManager.NumberOfCards;
        AddButton("Finish", "OnDone", Cardinal.SE,
            new Vector2(-10 - INNER_PADDING_X, -10 - INNER_PADDING_Y));
        ListItems();
    }
    private Button AddButton(string label,
                         string callbackMethod,
                         Cardinal anchor = Cardinal.NW,
                         Vector2? offset = null)
    {
        Button button = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/Button.tscn/").Instance<Button>();

        button.Label = label;
        button.Connect("OnClick", this, callbackMethod);
        buttons.Add(button);
        AddChild(button);
        button.UIAnchorToScreenBorder(anchor, offset);
        return button;
    }

    private void ListItems()
    {
        PackedScene textScene = ResourceLoader.Load<PackedScene>(
            "res://Assets/UI/PixelText.tscn/");
        float offsetY = 10;
        List<ItemModel> items = CharacterWrapper.Items;
        for (int i = 0; i < items.Count; i++)
        {
            PixelText t = textScene.Instance<PixelText>();
            t.SetLabel(items[i].Name);
            t.ZIndex = 4092;
            AddChild(t);
            t.Position = new Vector2(INNER_PADDING_X + OUTER_PADDING,
                INNER_PADDING_Y + OUTER_PADDING + i * offsetY);
        }
    }

    public void OnDone()
    {
        GameScreen.OnDeckPanelClosed();
        QueueFree();
    }
}

