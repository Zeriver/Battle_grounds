using UnityEngine;
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
        originPath.addTile(originTile);

        open.Add(originPath);

        while (open.Count > 0)
        {
            //open = open.OrderBy(x => x.costOfPath).ToList(); //Might remove / check performance
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

            foreach (Tile t in TileMap.GetListOfAdjacentTiles(current.lastTile.PosX, current.lastTile.PosY))
            {
                if (!t.IsWalkable) continue;
                TilePath newTilePath = new TilePath(current);
                newTilePath.addTile(t);
                open.Add(newTilePath);
            }
        }
        return null;
    }
}
