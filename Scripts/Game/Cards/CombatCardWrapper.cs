using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CombatCardWrapper : BaseCardWrapper
{
    private CombatCardModel info;
    public CombatCardModel Info
    {
        get => info;
        private set
        {
            info = value;
            Card.SetLabel(value.Name);
        }
    }

    public CombatCardWrapper(Card card, CombatCardModel cardInfo)
    {
        Card = card;
        Info = cardInfo;
    }
}
