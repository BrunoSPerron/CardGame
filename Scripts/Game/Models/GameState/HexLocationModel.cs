using System.Collections.Generic;

public class HexLocationModel : BaseModel
{
    public HashSet<HexLink> Openings = new HashSet<HexLink>();
    public LocationModel Location;
    public Vector2Int HexPosition;
}
