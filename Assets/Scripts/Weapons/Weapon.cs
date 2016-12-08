using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon
{
    protected string name;
    protected int cooldown;
    public int ammunition;
    protected List<Vector3> pattern;
    protected int range;
    protected bool isDiagnal;
    protected bool isMelee = false;
    // TODO add texture/sprite
    // type (energetic, melee etc)
    // damage
    // modifiers agains types of armor (piercing, good against light armor etc)


    protected Weapon(int ammunition)
    {
        this.ammunition = ammunition;
    }

    protected List<Vector3> createGenericPattern()  // generating simple Patterns for basic weapons here instead in specific weapons TODO
    {
        List<Vector3> pattern = new List<Vector3>();


        return pattern;
    }

    protected void addAmmunition(int value)
    {
        ammunition += value;
    }

    protected void subtractAmmunition(int value)
    {
        ammunition -= value;
    }

    public List<Tile> getWeaponHighlights(int x, int y)
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

}
