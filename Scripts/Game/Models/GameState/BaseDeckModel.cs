
using System.Collections.Generic;

public abstract class BaseDeckModel : BaseModel
{
    public abstract List<BaseBonusCardModel> BaseBonusCards { get; }
    public abstract List<BaseCardModel> BaseDeck { get; }
}
