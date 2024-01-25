
using Godot;
using System;

public abstract class BaseModel
{
    public virtual string ID { get; set; } = Guid.NewGuid().ToString();
    public virtual string JsonFilePath { get; set; }
    public virtual string Mod { get; set; } = "";
}
