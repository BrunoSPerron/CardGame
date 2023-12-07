using Godot;
using System;

public abstract class UIElement : Area2D
{
    protected Cardinal UIAnchor = Cardinal.NW;
    protected Vector2 UIOffset = Vector2.Zero;

    public abstract Vector2 UIElementSize { get; set; }

    public override void _Ready()
    {
        // Viewport RootNode = GetTree().Root;
        // RootNode.Connect("size_changed", this, "OnRootSizeChanged");
    }

    public void UIAnchorToScreenBorder(Cardinal cardinal, Vector2? offset = null)
    {
        UIAnchor = cardinal;
        UIOffset = offset ?? Vector2.Zero;
        UpdatePosition();
    }

    public abstract Vector2 GetUIElementSize();

    private void UpdatePosition()
    {
        Vector2 halfElementSize = GetUIElementSize() / 2;
        var size = GetTree().Root.GetVisibleRect().Size;
        Vector2 anchor = Vector2.Zero;
        switch (UIAnchor)
        {
            case Cardinal.NW:
                anchor = halfElementSize;
                break;
            case Cardinal.NONE:
                anchor = new Vector2(size / 2);
                break;
            case Cardinal.N:
                anchor = new Vector2(size.x / 2, halfElementSize.y);
                break;
            case Cardinal.W:
                anchor = new Vector2(halfElementSize.x, size.y / 2);
                break;
            case Cardinal.SW:
                anchor = new Vector2(halfElementSize.x, size.y - halfElementSize.y);
                break;
            case Cardinal.S:
                anchor = new Vector2(size.x / 2, size.y - halfElementSize.y);
                break;
            case Cardinal.SE:
                anchor = new Vector2(size.x - halfElementSize.x, size.y - halfElementSize.y);
                break;
            case Cardinal.E:
                anchor = new Vector2(size.x - halfElementSize.x, size.y / 2);
                break;
            case Cardinal.NE:
                anchor = new Vector2(size.x - halfElementSize.x, halfElementSize.y);
                break;
        }
        GlobalPosition = anchor + UIOffset;
    }

    public void OnRootSizeChanged()
    {
        if (UIAnchor != Cardinal.NW)
            UpdatePosition();
    }
}
