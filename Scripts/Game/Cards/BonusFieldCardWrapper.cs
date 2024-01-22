using Godot;
using System;


public class BonusFieldCardWrapper : BaseBonusCardWrapper
{
    public override BaseBonusCardModel BaseModel => Model;

    public BonusFieldCardModel model;
    public BonusFieldCardModel Model
    {
        get => model;
        private set
        {
            model = value;
            Card.SetLabel(value.Card.Name);
        }
    }

    public BonusFieldCardWrapper(Card card, BonusFieldCardModel model)
    {
        Card = card;
        Model = model;
    }
}
