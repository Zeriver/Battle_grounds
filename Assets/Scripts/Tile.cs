﻿using UnityEngine;
using System.Collections;

public class Tile {

    /*
     * 0 - normalFloor
     * 1 - normalFloor
     * 87 - slowField
     * 3 - not walkable
     * 49 - not walkable, blocking weapons
     * 
     * PREFABS
     * 
     * 5 - Wooden crate - Obstacle pushable, NOT blocking weapos
     * 4 - Column - Obstacle pushable, blocking weapos
     * 22 - fountain model
     * 81 - tree2
     * 82 - tree1
     * 41 - park light
     * 
    */


    private int moveCost;
    private int id;
    private int posX, posY;
    private bool isWalkable;
    private bool isUnitOnIt;
    private bool isPushable;
    private bool isBlockingWeapons;

    public Tile(int id, int x, int y)
    {
        posX = x;
        posY = y;
        this.id = id;
        isWalkable = true;
        isUnitOnIt = false;
        isPushable = false;
        switch (id)
        {
            case 1:
                moveCost = 1;
                break;
            case 87:
                moveCost = 3;
                break;
            case 3:
                moveCost = 999;  // not walkable
                isWalkable = false;
                break;
            case 5:
                moveCost = 1;
                isWalkable = false;
                isPushable = true;
                isUnitOnIt = true;
                break;
            case 4:
                moveCost = 1;
                isWalkable = false;
                isPushable = true;
                isBlockingWeapons = true;
                break;
            case 49:
                moveCost = 999;
                isWalkable = false;
                isBlockingWeapons = true;
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

    public bool IsWalkable
    {
        get { return isWalkable; }
        set { isWalkable = value; }
    }

    public bool IsUnitOnIt
    {
        get { return isUnitOnIt; }
        set { isUnitOnIt = value; }
    }

    public bool IsPushable
    {
        get { return isPushable; }
        set { isPushable = value; }
    }

    public bool IsBlockingWeapons
    {
        get { return isBlockingWeapons; }
        set { isBlockingWeapons = value; }
    }
}
