using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class BaseDeckManager
{
    public abstract BaseDeckModel BaseModel { get; }
    public abstract List<BaseBonusCardWrapper> GetBonusCards();
    public abstract List<BaseCardWrapper> GetSortedBaseDeck();
}
