using System;


public abstract class BaseBonusCardModel
{
    public abstract BaseCardModel Card { get; }

    public string ItemSourceId;
    public int OverrideCardIndex;
    public int Priority;
}
