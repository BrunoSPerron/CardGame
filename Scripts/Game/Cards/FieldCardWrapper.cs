using System;


public class FieldCardWrapper : BaseCardWrapper
{
    private FieldCardModel info;
    public FieldCardModel Info
    {
        get => info;
        private set
        {
            info = value;
            Card.SetLabel(value.Name);
        }
    }

    public FieldCardWrapper(Card card, FieldCardModel cardInfo)
    {
        Card = card;
        Info = cardInfo;
    }
}
