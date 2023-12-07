using Godot;
using System;
using System.Drawing;

public class Panel : Area2D
{
    [Export]
    public Vector2 SizeOnReady = new Vector2(10,10);

    private Vector2 size;

    private CollisionShape2D collider;
    private Sprite topLeft;
    private Sprite top;
    private Sprite topRight;
    private Sprite left;
    private Sprite middle;
    private Sprite right;
    private Sprite bottomLeft;
    private Sprite bottom;
    private Sprite bottomRight;

    private Vector2 topLeftSize;
    private Vector2 topRightSize;
    private Vector2 botRightSize;
    private Vector2 botLeftSize;
    private Vector2 leftSize;
    private Vector2 rightSize;
    private Vector2 topSize;
    private Vector2 bottomSize;

    private Vector2 halftopLeftSize;
    private Vector2 halftopRightSize;
    private Vector2 halfBotRightSize;
    private Vector2 halfBotLeftSize;
    private Vector2 halfLeftSize;
    private Vector2 halfRightSize;
    private Vector2 halfTopSize;
    private Vector2 halfBottomSize;

    public override void _Ready()
    {
        collider = GetNode<CollisionShape2D>("Collider");

        topLeft = GetNode<Sprite>("TopLeft");
        top = GetNode<Sprite>("Top");
        topRight = GetNode<Sprite>("TopRight");
        left = GetNode<Sprite>("Left");
        middle = GetNode<Sprite>("Center");
        right = GetNode<Sprite>("Right");
        bottomLeft = GetNode<Sprite>("BottomLeft");
        bottom = GetNode<Sprite>("Bottom");
        bottomRight = GetNode<Sprite>("BottomRight");

        SetPanelSize(SizeOnReady);
    }

    public Vector2 GetSize()
    {
        return size;
    }

    public void SetPanelSize(Vector2 newSize)
    {
        UpdateDataFromTextures();
        QuickSetPanelSize(newSize);
    }

    private void UpdateDataFromTextures()
    {
        topLeftSize = topLeft.Texture.GetSize();
        halftopLeftSize = topLeftSize / 2;

        topRightSize = topRight.Texture.GetSize();
        halftopRightSize = topRightSize / 2;

        botRightSize = bottomRight.Texture.GetSize();
        halfBotRightSize = botRightSize / 2;

        botLeftSize = bottomLeft.Texture.GetSize();
        halfBotLeftSize = botLeftSize / 2;

        leftSize = left.Texture.GetSize();
        halfLeftSize = leftSize / 2;

        rightSize = right.Texture.GetSize();
        halfRightSize = rightSize / 2;

        topSize = top.Texture.GetSize();
        halfTopSize = topSize / 2;

        bottomSize = bottom.Texture.GetSize();
        halfBottomSize = bottomSize / 2;
    }

    internal void QuickSetPanelSize(Vector2 newSize)
    {
        size = newSize;
        Vector2 halfsize = newSize / 2;
        if (collider?.Shape is RectangleShape2D shape)
            shape.Extents = halfsize;
        else
            GD.PrintErr("Panel Error: missing RectangleShape2D in panel collider");

        // Corners
        topLeft.Position = new Vector2(-halfsize + halftopLeftSize);

        topRight.Position = new Vector2(halfsize.x - halftopRightSize.x,
                                        -halfsize.y + halftopRightSize.y);

        bottomRight.Position = new Vector2(halfsize - halfBotRightSize);

        bottomLeft.Position = new Vector2(-halfsize.x + halfBotLeftSize.x,
                                          halfsize.y - halfBotLeftSize.y);

        // Sides
        left.Position = new Vector2(-halfsize.x + halfLeftSize.x, 0);
        left.Scale = new Vector2(1, (newSize.y - botLeftSize.y - topLeftSize.y));

        right.Position = new Vector2(halfsize.x - halfRightSize.x, 0);
        right.Scale = new Vector2(1, (newSize.y - botRightSize.y - topRightSize.y));

        top.Position = new Vector2(0, -halfsize.y + halfTopSize.y);
        top.Scale = new Vector2((newSize.x - topLeftSize.x - topRightSize.x), 1);

        bottom.Position = new Vector2(0, halfsize.y - halfBottomSize.y);
        bottom.Scale = new Vector2((newSize.x - botLeftSize.x - botRightSize.x), 1);

        // Center
        middle.Scale = new Vector2(newSize.x - leftSize.x - rightSize.x,
                                   newSize.y - topSize.y - bottomSize.y);
    }
}
