using UnityEngine;
using System.Collections.Generic;
using System;

public class TileMap
{
    int size_x;
    int size_y;

    static int[,] map_data;
    static List<Tile> tile_data;
    static List<Obstacle> obstacles;

    private const string normalTile = "0";
    private const string slowTile = "87";
    private const string notWalkable = "3"; //but shootable
    private const string notWalkableNotShootable = "49";
    private const string woodenCrate = "5";
    private const string column = "4";
    private const string fountain = "22";
    private const string tree2 = "81";
    private const string tree1 = "82";
    private const string parkLight = "41";
    private const string bench = "65";
    private const string trashbin = "61";
    private const string parkTable = "76";


    public TileMap()
    {

    }

    public TileMap(int size_x, int size_y)
    {
        this.size_x = size_x;
        this.size_y = size_y;
        tile_data = new List<Tile>();
        obstacles = new List<Obstacle>();

        map_data = new int[size_x, size_y];

        UnityEngine.Object obstaclePrefab = Resources.Load("Prefabs/Obstacle_02");
        UnityEngine.Object obstaclePrefab2 = Resources.Load("Prefabs/Obstacle_03");
        UnityEngine.Object fountain1 = Resources.Load("Prefabs/FountainBiggetT");
        UnityEngine.Object tree01 = Resources.Load("Prefabs/Tree1T");
        UnityEngine.Object tree02 = Resources.Load("Prefabs/Tree2T");
        UnityEngine.Object parkLight1 = Resources.Load("Prefabs/StreetLight2T");
        UnityEngine.Object bench1 = Resources.Load("Prefabs/Bench1T");
        UnityEngine.Object trashbin1 = Resources.Load("Prefabs/TrashBin1T");
        UnityEngine.Object parkTable1 = Resources.Load("Prefabs/ParkTable1T");

        string fileLevel = "MP2";
        string[][] levelMap = FileReader.readMapFile(Application.dataPath + "/Maps/" + fileLevel +"/map.txt");

        for (int y = 0; y < levelMap.Length; y++)
        {
            for (int x = 0; x < levelMap[0].Length; x++)
            {
                switch (levelMap[y][x])
                {
                    case normalTile:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(0, x, y));
                        break;
                    case slowTile:
                        map_data[x, y] = 2;
                        tile_data.Add(new Tile(87, x, y));
                        break;
                    case notWalkable:
                        map_data[x, y] = 1;
                        tile_data.Add(new Tile(3, x, y));
                        break;
                    case notWalkableNotShootable:
                        map_data[x, y] = 0; ///1
                        tile_data.Add(new Tile(49, x, y));
                        break;
                    case woodenCrate:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(5, x, y));
                        GameObject obstacle = (GameObject)GameObject.Instantiate(obstaclePrefab);
                        Obstacle obstacleController = obstacle.GetComponent<Obstacle>();
                        obstacleController.createObstacle(x, y);
                        obstacles.Add(obstacleController);
                        break;
                    case column:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(5, x, y));
                        GameObject obstacle2 = (GameObject)GameObject.Instantiate(obstaclePrefab2);
                        Obstacle obstacleController2 = obstacle2.GetComponent<Obstacle>();
                        obstacleController2.createObstacle(x, y);
                        obstacles.Add(obstacleController2);
                        break;
                    case fountain:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject fountainPrefab = (GameObject)GameObject.Instantiate(fountain1, new Vector3(x + 0.5f, 0, -y + 0.5f), Quaternion.identity);
                        break;
                    case tree1:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject treePrefab1 = (GameObject)GameObject.Instantiate(tree01, new Vector3(x + 0.5f, 0, -y + 0.5f), Quaternion.identity);
                        break;
                    case tree2:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject treePrefab2 = (GameObject)GameObject.Instantiate(tree02, new Vector3(x + 0.5f, 0, -y + 0.5f), Quaternion.identity);
                        break;
                    case parkLight:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject parkLightPrefab = (GameObject)GameObject.Instantiate(parkLight1, new Vector3(x + 0.5f, 0, -y + 0.5f), Quaternion.identity);
                        break;
                    case trashbin:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject trashbinPrefab = (GameObject)GameObject.Instantiate(trashbin1, new Vector3(x + 0.5f, 0, -y + 0.5f), Quaternion.identity);
                        break;
                    case bench:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        if (levelMap[y + 1][x].Equals("66")){  //facing down 
                            levelMap[y + 1][x] = "49";
                            GameObject benchPrefab = (GameObject)GameObject.Instantiate(bench1, new Vector3(x + 0.5f, 0, -y), Quaternion.Euler(0, 270, 0));
                        }
                        else if (levelMap[y - 1][x].Equals("66")){ //facing up
                            map_data[x, y - 1] = 0;
                            setTileNotWalkable(x, y - 1);
                            GameObject benchPrefab = (GameObject)GameObject.Instantiate(bench1, new Vector3(x + 0.5f, 0, -y + 1f), Quaternion.Euler(0, 90, 0));
                        }
                        else if (levelMap[y][x + 1].Equals("66")) //facing right
                        {
                            levelMap[y][x + 1] = "49";
                            GameObject benchPrefab = (GameObject)GameObject.Instantiate(bench1, new Vector3(x + 1f, 0, -y + 0.5f), Quaternion.Euler(0, 180, 0));
                        }
                        else if (levelMap[y][x - 1].Equals("66")) //facing left
                        {
                            map_data[x - 1, y] = 0;
                            setTileNotWalkable(x - 1, y);
                            GameObject benchPrefab = (GameObject)GameObject.Instantiate(bench1, new Vector3(x, 0, -y + 0.5f), Quaternion.Euler(0, 0, 0));
                        }
                        break;
                    case parkTable:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(49, x, y));
                        GameObject parkTablePrefab = (GameObject)GameObject.Instantiate(parkTable1, new Vector3(x + 1f, 0, -y), Quaternion.Euler(0, 90, 0));
                        break;
                    default:
                        map_data[x, y] = 0;
                        tile_data.Add(new Tile(0, x, y));
                        break;
                }
            }
        }

    }

    public int getTileAt(int x, int y)
    {
        return map_data[x, y];
    }

    public static Obstacle getObstacleAt(int x, int z)
    {
        Obstacle obstacle = null;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].id.Equals(x + "|" + z))
            {
                return obstacles[i];
            }
        }
        return obstacle;
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

    public static void resetObstacleTile(Tile tile)
    {
        tile.IsWalkable = true;
        tile.IsPushable = false;
        tile.IsUnitOnIt = false;
        tile.IsBlockingWeapons = false;
        tile.Id = 1;
    }
}
