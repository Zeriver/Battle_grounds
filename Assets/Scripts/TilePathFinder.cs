﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TilePathFinder : MonoBehaviour
{

    public static List<Tile> FindPath(Tile originTile, Tile destinationTile)
    {       
        List<Tile> closed = new List<Tile>();
        List<TilePath> open = new List<TilePath>();

        TilePath originPath = new TilePath();
        originPath.addTile(originTile, false);

        open.Add(originPath);

        while (open.Count > 0)
        {
            open = open.OrderBy(x => x.costOfPath).ToList();
            TilePath current = open[0];
            open.Remove(open[0]);

            if (closed.Contains(current.lastTile))
            {
                continue;
            }
            if (current.lastTile == destinationTile)
            {
                current.listOfTiles.Remove(originTile);
                return current.listOfTiles;
            }

            closed.Add(current.lastTile);

            List<Tile> adjacentTiles = TileMap.GetListOfAdjacentTiles(current.lastTile.PosX, current.lastTile.PosY);
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                if (!adjacentTiles[i].IsWalkable) continue;
                TilePath newTilePath = new TilePath(current);
                newTilePath.addTile(adjacentTiles[i], false);
                open.Add(newTilePath);
            }
        }
        Debug.Log("Warning: path finder returned null");
        return null;
    }
}
