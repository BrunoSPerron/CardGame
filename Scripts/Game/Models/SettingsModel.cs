using System;


public class SettingsModel: BaseModel
{
    public bool ClickToPlay = false;
    public bool ClickToPay = true;

    public float DealingDelay = 0.04f;
    public float CleaningDelay = 0.04f;

    public float DealingSpeed = 8;
}
