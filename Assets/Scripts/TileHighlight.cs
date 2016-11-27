using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePath
{
    public List<Tile> listOfTiles = new List<Tile>();

    public int costOfPath = 0;

    public Tile lastTile;

    public TilePath() { }

    public TilePath(TilePath tp)
    {
        listOfTiles = tp.listOfTiles;
        costOfPath = tp.costOfPath;
        lastTile = tp.lastTile;
    }

    public void addTile(Tile t)
    {
        costOfPath += t.MoveCost;
        listOfTiles.Add(t);
        lastTile = t;
    }
}

public class TileHighlight
{



    public TileHighlight()
    {

    }

    public static List<Tile> FindHighlight(Tile originTile, int movementPoints)
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

            foreach (Tile t in TileMap.GetListOfAdjacentTiles(current.lastTile.PosX, current.lastTile.PosY))
            {
                TilePath newTilePath = new TilePath(current);
                newTilePath.addTile(t);
                open.Add(newTilePath);
            }
        }
        closed.Remove(originTile);
        return closed;
    }
}
