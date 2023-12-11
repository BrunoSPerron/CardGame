using Godot;
using System;
using System.Collections.Generic;

/*
 * World use an hexagonal grid
 * Hex coord are stored in a Vector2 corresponding to a 2d table with
 *  these possible connexions:
 *  | |X|X|  \ \X\X\
 *  |X|O|X|   \X\X\X\
 *  |X|X| |    \X\X\ \
 *  
 *  y
 *  |
 *  o--x
 *  
 */

public class WorldModel : BaseModel
{
    public List<HexLocation> Locations = new List<HexLocation>();
}

public struct HexLocation
{
    public List<HexLink> Openings;
    public LocationModel Location;
    public Vector2Int HexPosition;

    public HexLocation(LocationModel location, Vector2Int position) 
    { 
        Openings = new List<HexLink>();
        Location = location;
        HexPosition = position;
    }
}
