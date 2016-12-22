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
        List<Tile> pathsWithUnits = new List<Tile>();
        bool shouldSkip = false;

        TilePath originPath = new TilePath();
        originPath.addTile(originTile, isWeapon);

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
            if (isWeapon)
            {
                shouldSkip = false;
                for (int i = 0; i < pathsWithUnits.Count; i++)
                {
                    if (current.lastTile.PosY == pathsWithUnits[i].PosY)
                    {
                        if ((pathsWithUnits[i].PosX > originTile.PosX && current.lastTile.PosX > originTile.PosX) || (pathsWithUnits[i].PosX < originTile.PosX && current.lastTile.PosX < originTile.PosX))
                        {
                            shouldSkip = true;
                        }
                    }
                    else if (current.lastTile.PosX == pathsWithUnits[i].PosX)
                    {
                        if ((pathsWithUnits[i].PosY > originTile.PosY && current.lastTile.PosY > originTile.PosY) || (pathsWithUnits[i].PosY < originTile.PosY && current.lastTile.PosY < originTile.PosY))
                        {
                            shouldSkip = true;
                        }
                    }
                }
            }
            if (shouldSkip)
            {
                continue;
            }
            if (current.lastTile != originTile && current.lastTile.IsUnitOnIt)
            {
                pathsWithUnits.Add(current.lastTile);
            }

            closed.Add(current.lastTile);

            List<Tile> adjacentTiles = TileMap.GetListOfAdjacentTiles(current.lastTile.PosX, current.lastTile.PosY);
            for (int i = 0; i < adjacentTiles.Count; i ++)
            {
                if (isWeapon)
                {
                    if (originTile.PosX == adjacentTiles[i].PosX || originTile.PosY == adjacentTiles[i].PosY)
                    {
                        TilePath newTilePath = new TilePath(current);
                        newTilePath.addTile(adjacentTiles[i], isWeapon);
                        open.Add(newTilePath);
                    }
                }
                else if (adjacentTiles[i].IsWalkable)
                {
                    TilePath newTilePath = new TilePath(current);
                    newTilePath.addTile(adjacentTiles[i], isWeapon);
                    open.Add(newTilePath);
                }
            }
        }
        closed.Remove(originTile);
        return closed;
    }
}
