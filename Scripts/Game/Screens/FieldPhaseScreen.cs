using Godot;

public class FieldPhaseScreen : BaseGameScreen
{
    public CharacterWrapper Character { get; set; }
    public FieldPhaseManager Manager { get; set; }

    public override void Destroy()
    {
        GD.PrintErr("TODO: Destroy the FieldPhaseScreen");
        QueueFree();
    }
}