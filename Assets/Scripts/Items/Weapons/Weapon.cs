﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : Item
{
    protected int cooldown;
    public int ammunition;
    protected List<Vector3> pattern;
    protected List<Vector3> areOfEffect;
    protected int range;
    protected bool isDiagonal;
    protected bool isMelee = false;
    // type (energetic, melee etc)
    // damage
    // modifiers agains types of armor (piercing, good against light armor etc)


    protected Weapon(int ammunition)
    {
        this.ammunition = ammunition;
    }


    protected void addAmmunition(int value)
    {
        ammunition += value;
    }

    protected void subtractAmmunition(int value)
    {
        ammunition -= value;
    }

    public string getName()
    {
        return name;
    }


    public List<Tile> getWeaponHighlights(int x, int y)
    {
        if (pattern != null)
        {
            return getWeaponHighlightsFromPattern(x, y);
        } else
        {
            return TileHighlight.FindHighlight(TileMap.getTile((int)x, (int)y), range, true);
        }
    }

    private List<Tile> getWeaponHighlightsFromPattern(int x, int y) //TODO needs improvements
    {
        List<Tile> highligts = new List<Tile>();
        for (int i = 0; i < pattern.Count; i++)
        {
            Tile tile = TileMap.getTile(x + (int)pattern[i].x, y + (int)pattern[i].z);
            if (tile != null && !highligts.Contains(tile) && tile.IsWalkable)
            {
                highligts.Add(tile);
            }
        }
        return highligts;
    }

    public void useWeapon()
    {
        if (!isMelee)
        {
            subtractAmmunition(1);
        }

        //TODO
    }

    public List<Tile> getAreaEffect(int x, int z, int x2 , int z2) //refactoring TODO
    {
        List<Tile> area = new List<Tile>();
        if (name.Equals("Flame Thrower"))
        {
            area = FlameThrower.getAreaOfEffect(x, z, x2 , z2);
        }
        else if (name.Equals("Pistol"))
        {
            area.Add(TileMap.getTile(x2, z2));
        }

        return area;
    }

}