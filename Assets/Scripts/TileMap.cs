using UnityEngine;
using System.Collections.Generic;
using System;

public class TileMap
{
    int size_x;
    int size_y;

    static int[,] map_data;
    static List<Tile> tile_data;

    public TileMap()
    {
        //Exception TODO
    }

    public TileMap(int size_x, int size_y)
    {
        this.size_x = size_x;
        this.size_y = size_y;
        tile_data = new List<Tile>();

        map_data = new int[size_x, size_y];

        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                // Add logic for different kind of tiles TODO
                if (x == 7 && y == 7)
                {
                    map_data[x, y] = 3;
                    tile_data.Add(new Tile(3, x, y));
                }
                else if (x == 4 && (y == 5 || y == 6))
                {
                    map_data[x, y] = 4;  // Instead of making special texture in this place, create preffab object with it's own script that allows object to be pushed TODO
                    tile_data.Add(new Tile(4, x, y));
                }
                else if (x == 9 && (y == 9 || y == 10 || y == 11))
                {
                    map_data[x, y] = 2;
                    tile_data.Add(new Tile(2, x, y));
                }
                else if (x == 0 || y == 0 || x == size_x-1 || y == size_y-1)  //not walkable tile at the edge of the map
                {
                    map_data[x, y] = 1;
                    tile_data.Add(new Tile(3, x, y));
                }
                else
                {
                    map_data[x, y] = 0;
                    tile_data.Add(new Tile(0, x, y));
                }

                
            }
        }
    }

    public int getTileAt(int x, int y)
    {
        return map_data[x, y];
    }

    static public List<Tile> GetListOfAdjacentTiles(int x, int y)
    {
        List<Tile> adjacentTile = new List<Tile>();

        Tile tile1 = getTile(x + 1, y);
        Tile tile2 = getTile(x - 1, y);
        Tile tile3 = getTile(x, y + 1);
        Tile tile4 = getTile(x, y - 1);
        if (tile1 != null)
            adjacentTile.Add(tile1);
        if (tile2 != null)
            adjacentTile.Add(tile2);
        if (tile3 != null)
            adjacentTile.Add(tile3);
        if (tile4 != null)
            adjacentTile.Add(tile4);

        return adjacentTile;
    }

    public static Tile getTile(int x, int y)
    {
        Tile tile = null;
        x = Math.Abs(x);
        y = Math.Abs(y);
        for (int i = 0; i < tile_data.Count; i++)
        {
            if (tile_data[i].PosX == x && tile_data[i].PosY == y)
            {
                tile = tile_data[i];
            }
        }
        if (tile == null)
        {
            Debug.Log("Warning: returned tile is null!");
        }
        return tile;
    }

    public static void setTileNotWalkable(int x, int y)
    {
        getTile(x, y).IsWalkable = false;
        getTile(x, y).IsUnitOnIt = true;
    }

    public static void setTileWalkable(int x, int y)
    {
        getTile(x, y).IsWalkable = true;
        getTile(x, y).IsUnitOnIt = false;
    }
}
