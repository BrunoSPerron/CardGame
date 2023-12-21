using Godot;
using System;
using System.Collections.Generic;

/*
 * World use an hexagonal grid
 * Hex coord are stored in 2d table graph with these possible connexions:
 *  | |X|X|  \ \X\X\     y
 *  |X|O|X|   \X\O\X\    |
 *  |X|X| |    \X\X\ \   o--x
 */

public class WorldModel : BaseModel
{
    public List<WorldHexModel> Locations = new List<WorldHexModel>();
}