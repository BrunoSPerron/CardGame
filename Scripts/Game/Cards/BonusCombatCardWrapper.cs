using Godot;
using System;


public class BonusCombatCardWrapper : BaseBonusCardWrapper
{
    public override BaseBonusCardModel BaseModel => Model;

    public BonusCombatCardModel model;
    public BonusCombatCardModel Model
    {
        get => model;
        private set
        {
            model = value;
            Card.SetLabel(value.Card.Name);
        }
    }

    public BonusCombatCardWrapper(Card card, BonusCombatCardModel model)
    {
        Card = card;
        Model = model;
    }
}
