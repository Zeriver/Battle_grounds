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
    protected bool isDiagonal;
    protected bool isMelee = false;
    // TODO add texture/sprite
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

}
