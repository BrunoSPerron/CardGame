using System;
using Godot;


public abstract class CardButtonBase : Area2D
{

    [Export]
    public Texture ButtonTextureOnReady = null;

    [Export]
    public bool VerticalSplit = true;

    [Signal]
    public delegate void OnClick(CardButtonBase nodeClicked);

    private Sprite[] sprites = new Sprite[0];


    private bool mouseEnteredDown = false;
    private bool isDisabled = false;

    private PressableState currentButtonState = PressableState.STAND_BY;

    public override void _Ready()
    {
        foreach (object node in GetChildren())
        {
            if (node is Sprite sprite)
            {
                Array.Resize(ref sprites, sprites.Length + 1);
                sprites[sprites.Length - 1] = sprite;
            }
        }

        if (ButtonTextureOnReady != null)
            ChangeTexture(ButtonTextureOnReady);
    }


    public void ChangeTexture(Texture texture)
    {
        Sprite sprite = GetNode<Sprite>("Sprite");
        sprite.Texture = texture;
    }

    private void SetImage(PressableState state)
    {
        if (VerticalSplit)
        {
            foreach (Sprite sprite in sprites)
            {
                float xOffset = (int)state * sprite.Texture.GetSize().x / 3;
                Vector2 regionPosition = new Vector2(xOffset, 0);
                sprite.RegionRect = new Rect2(regionPosition, sprite.RegionRect.Size);
            }
        }
        else
        {
            foreach (Sprite sprite in sprites)
            {
                float yOffset = (int)state * sprite.Texture.GetSize().y / 3;
                Vector2 regionPosition = new Vector2(0, yOffset);
                sprite.RegionRect = new Rect2(regionPosition, sprite.RegionRect.Size);
            }
        }

    }

    public void UpdateSpriteRect()
    {
        SetImage(currentButtonState);
    }


    // ===== Mouse events methods =====
    // To manage overlaps these methods are called by the CardHandlingControl
    //      instead of connecting to signals.

    public void OnMouseDown()
    {
        if (!isDisabled)
        {
            currentButtonState = PressableState.DOWN;
            UpdateSpriteRect();
        }
    }

    public void OnMouseUp()
    {
        if (mouseEnteredDown)
        {
            mouseEnteredDown = false;
        }
        else if (!isDisabled)
        {
            currentButtonState = PressableState.MOUSE_OVER;
            UpdateSpriteRect();
            EmitSignal("OnClick");
        }
    }

    public void OnMouseEnter(InputEventMouseMotion mouseMotion)
    {
        if (!isDisabled)
        {
            currentButtonState = PressableState.MOUSE_OVER;
            UpdateSpriteRect();
            mouseEnteredDown = mouseMotion.IsPressed();
        }
    }

    public void OnMouseExit()
    {
        if (!isDisabled)
        {
            currentButtonState = PressableState.STAND_BY;
            UpdateSpriteRect();
        }
    }
}

