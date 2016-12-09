using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileHighlight
{
    public TileHighlight()
    {

    }

    public static List<Tile> FindHighlight(Tile originTile, int movementPoints, bool isWeapon)
    {
        List<Tile> closed = new List<Tile>();
        List<TilePath> open = new List<TilePath>();

        TilePath originPath = new TilePath();
        originPath.addTile(originTile);

        open.Add(originPath);

        while (open.Count > 0)
        {
            TilePath current = open[0];
            open.Remove(open[0]);

            if (closed.Contains(current.lastTile))
            {
                continue;
            }
            if (current.costOfPath > movementPoints + 1)
            {
                continue;
            }

            closed.Add(current.lastTile);

            List<Tile> adjacentTiles = TileMap.GetListOfAdjacentTiles(current.lastTile.PosX, current.lastTile.PosY);
            for (int i = 0; i < adjacentTiles.Count; i ++)
            {
                if (adjacentTiles[i].IsWalkable)
                {
                    if (isWeapon)
                    {
                        if (originTile.PosX == adjacentTiles[i].PosX || originTile.PosY == adjacentTiles[i].PosY)
                        {
                            TilePath newTilePath = new TilePath(current);
                            newTilePath.addTile(adjacentTiles[i]);
                            open.Add(newTilePath);
                        }
                    }
                    else
                    {
                        TilePath newTilePath = new TilePath(current);
                        newTilePath.addTile(adjacentTiles[i]);
                        open.Add(newTilePath);
                    }
                    
                }
            }
        }
        closed.Remove(originTile);
        return closed;
    }
}
