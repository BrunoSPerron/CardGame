using Godot;
using System;

public class Button : UIElement
{
    ///<summary> Loaded from _ready. Using SetLabel(value) to change it afterwards.</summary>
    [Export]
    public string Label;
    [Export]
    public float WidthOverride;
    [Signal]
    public delegate void OnClick();

    private Vector2 uiElementSize = Vector2.Zero;
    public override Vector2 UIElementSize {
        get => uiElementSize;
        set
        {
            SetWidth(value.x);
            uiElementSize.y = value.y;
        }
    }

    private Sprite leftBackground;
    private Sprite centerBackground;
    private Sprite rightBackground;

    private float leftTextureWidth = 0;
    private float centerTextureWidth = 0;
    private float rightTextureWidth = 0;

    private bool enteredDown = false;
    private bool isDisabled = false;

    public override void _Ready()
    {
        leftBackground = GetNode<Sprite>("BackgroundLeft");
        centerBackground = GetNode<Sprite>("BackgroundMiddle");
        rightBackground = GetNode<Sprite>("BackgroundRight");
        leftTextureWidth = leftBackground.RegionRect.Size.x;
        centerTextureWidth = centerBackground.RegionRect.Size.x;
        rightTextureWidth = rightBackground.RegionRect.Size.x;
        UIElementSize = new Vector2(
            leftTextureWidth + centerTextureWidth + rightTextureWidth,
            centerBackground.RegionRect.Size.y);
        SetLabel(Label ?? "");
    }

    public void Disable()
    {
        SetButtonImage(PressableState.STAND_BY);
        isDisabled = true;
        Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, 0.35f);
    }

    public void Enable()
    {
        isDisabled = false;
        Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, 1f);
    }

    public override Vector2 GetUIElementSize()
    {
        return UIElementSize;
    }

    private void SetButtonImage(PressableState state)
    {
        if (!isDisabled)
        {
            float yOffset = (int)state * UIElementSize.y;
            leftBackground.RegionRect = new Rect2(
                0, yOffset, leftTextureWidth, UIElementSize.y);
            centerBackground.RegionRect = new Rect2(
                0, yOffset, centerTextureWidth, UIElementSize.y);
            rightBackground.RegionRect = new Rect2(
                0, yOffset, rightTextureWidth, UIElementSize.y);
        }
    }

    public void SetLabel(string value)
    {
        PixelText label = GetNode<PixelText>("Label");
        label.AlignCenter();
        label.Value = value;
        SetWidth(label.GetWidth() + 22);
    }

    private void SetWidth(float width)
    {
        uiElementSize.x = width;
        float halfHeight = UIElementSize.y / 2f;
        float halfWidth = width / 2f;
        leftBackground.Position = new Vector2(-halfWidth + leftTextureWidth / 2f, 0);
        centerBackground.Scale = new Vector2(
            width - leftTextureWidth - rightTextureWidth, 1);
        rightBackground.Position = new Vector2(halfWidth - rightTextureWidth / 2f, 0);
        CollisionShape2D hitbox = GetNode<CollisionShape2D>("Hitbox");
        RectangleShape2D shape = hitbox.Shape as RectangleShape2D;
        shape.Extents = new Vector2(halfWidth, halfHeight);
    }


    // ===== Signals =====


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
                    SetButtonImage(PressableState.DOWN);
                }
                else if (!enteredDown)
                {
                    SetButtonImage(PressableState.MOUSE_OVER);
                    EmitSignal("OnClick");
                }
            }
        }
    }

    public void OnMouseEnter()
    {
        SetButtonImage(PressableState.MOUSE_OVER);
        enteredDown = Input.IsMouseButtonPressed((int)MouseButton.LEFT);
    }

    public void OnMouseExit()
    {
        SetButtonImage(PressableState.STAND_BY);
        enteredDown = false;
    }
}
