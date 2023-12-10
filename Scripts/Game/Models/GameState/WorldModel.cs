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
    public List<Tuple<Vector2, Vector2>> Links = new List<Tuple<Vector2, Vector2>>();
}

public struct HexLocation
{
    public List<HexLink> Openings;
    public LocationModel Location;
    public Vector2 HexPosition;
}
