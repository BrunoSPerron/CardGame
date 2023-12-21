using System.Collections.Generic;

public class WorldHexModel : BaseModel
{
    public Vector2Int Coord;
    public LocationModel Location;
    public HashSet<HexLink> Openings = new HashSet<HexLink>()
    {
        HexLink.TOPLEFT,
        HexLink.TOPRIGHT,
        HexLink.LEFT,
        HexLink.RIGHT,
        HexLink.BOTTOMLEFT,
        HexLink.BOTTOMRIGHT
    };
}
