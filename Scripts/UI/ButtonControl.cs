using Godot;
using System;

public class ButtonControl : Node
{
    [Export]
    public bool VerticalSplit = true;

    [Export]
    public bool useMouseSignal = true;

    private PressableState currentButtonState = PressableState.STAND_BY;

    private Sprite sprite;
    private Node2D node2D;
    private Vector2 RegionSize
    {
        get => sprite.RegionRect.Size;
        set => sprite.RegionRect = new Rect2(RegionPosition, value);
    }

    private Vector2 RegionPosition
    {
        get => sprite.RegionRect.Position;
        set => sprite.RegionRect = new Rect2(value, RegionSize);
    }

    private Vector2 ImageSize => sprite.Texture.GetSize();

    private bool mouseEnteredDown = false;
    private bool isDisabled = false;

    public override void _Ready()
    {
        try
        {
            sprite = GetParent<Sprite>();
            node2D = sprite.GetParent<Node2D>();

            if (useMouseSignal)
            {
                node2D.Connect("mouse_entered", this, "OnMouseEnter");
                node2D.Connect("mouse_exited", this, "OnMouseExit");
                node2D.Connect("input_event", this, "OnMouseInputEvent");
            }
        }
        catch
        {
            sprite = new Sprite();
            node2D = new Node2D();
            GD.PrintErr("Button control error: Node tree structure is not compatible with this script.");
        }
    }

    public void Disable()
    {
        currentButtonState = PressableState.STAND_BY;
        Update();
        isDisabled = true;
        node2D.Modulate = new Color(
            node2D.Modulate.r, node2D.Modulate.g, node2D.Modulate.b, 0.35f);
    }

    public void Enable()
    {
        isDisabled = false;
        node2D.Modulate = new Color(
            node2D.Modulate.r, node2D.Modulate.g, node2D.Modulate.b, 1f);
    }

    private void SetImage(PressableState state)
    {
        if (!isDisabled)
        {
            if (VerticalSplit)
            {
                float xOffset = (int)state * ImageSize.x / 3;
                RegionPosition = new Vector2(xOffset, 0);
            }
            else
            {
                float yOffset = (int)state * ImageSize.y / 3;
                RegionPosition = new Vector2(0, yOffset);
            }
        }
    }

    // === Signals ===
#pragma warning disable IDE0060

    public void OnMouseInputEvent(
        Viewport viewport, InputEvent evt, int shapeIdx)
    {
        if (isDisabled)
            return;

        if (evt.GetType() == typeof(InputEventMouseButton))
        {
            InputEventMouseButton mouseEvent = evt as InputEventMouseButton;
            if (mouseEvent.ButtonIndex == (int)MouseButton.LEFT)
            {
                if (mouseEvent.Pressed)
                {
                    currentButtonState = PressableState.DOWN;
                    Update();
                }
                else if (!mouseEnteredDown)
                {
                    currentButtonState = PressableState.OVER;
                    Update();
                    EmitSignal("OnClick");
                }
            }
        }
    }

    public void OnMouseEnter()
    {
        currentButtonState = PressableState.OVER;
        Update();
        mouseEnteredDown = Input.IsMouseButtonPressed((int)MouseButton.LEFT);
    }

    public void OnMouseExit()
    {
        currentButtonState = PressableState.STAND_BY;
        Update();
    }

    public void Update()
    {
        SetImage(currentButtonState);
    }
}
