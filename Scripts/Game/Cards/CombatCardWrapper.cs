using System;


public class CombatCardWrapper : BaseCardWrapper
{
    private CombatCardModel model;
    public CombatCardModel Model
    {
        get => model;
        private set
        {
            model = value;
            Card.SetLabel(value.Name);
        }
    }

    public CombatCardWrapper(Card card, CombatCardModel model)
    {
        Card = card;
        Model = model;
    }
}
