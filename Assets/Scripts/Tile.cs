using UnityEngine;
using System.Collections;

public class Tile {

    /*
     * 0 - normalFloor
     * 1 - swamps
     * 2 - wall
     * 3 - ...
    */


    private int moveCost;
    private int id;
    private int posX, posY;

    public Tile(int id, int x, int y)
    {
        posX = x;
        posY = y;
        this.id = id;
        switch (id)
        {
            case 1:
                moveCost = 1;
                break;
            case 2:
                moveCost = 1;
                break;
            case 3:
                moveCost = 999;  // not walkable
                break;
            default:
                moveCost = 1;
                break;
        }
    }

    public int PosX
    {
        get { return posX; }
        set { posX = value; }
    }

    public int PosY
    {
        get { return posY; }
        set { posY = value; }
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public int MoveCost
    {
        get { return moveCost; }
        set { moveCost = value; }
    }
}
