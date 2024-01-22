
using System.Collections.Generic;

public abstract class BaseDeckModel : BaseModel
{
    public abstract List<BonusFieldCardModel> BaseBonusCards { get; }
    public abstract List<BaseCardModel> BaseDeck { get; }
}
