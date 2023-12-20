
using System;

public abstract class BaseModel
{
    public string ID = Guid.NewGuid().ToString();
    public string Mod;
    public string JsonFilePath;
}
    