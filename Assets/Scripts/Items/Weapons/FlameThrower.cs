using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameThrower : Weapon
{


    public FlameThrower(int ammunition) : base(ammunition)
    {
        name = "Flame Thrower";
        description = "Light them up!";
        this.ammunition = ammunition;

        damageType = "fire";
        damage = 20;
        cooldown = 2;
        range = 1;
        isDiagonal = false;
        nextTurnsDamage = new[] { 15, 10, 5 };

        pattern = generatePattern();
        areOfEffect = generateAreaOfEffect();
    }

    private List<Vector3> generatePattern()
    {
        List<Vector3> pattern = new List<Vector3>();
        for (int i = -range; i < range + 1; i++)
        {
            if (i != 0)
            {
                pattern.Add(new Vector3(i, 0.0f, 0.0f));
            }
        }
        for (int i = -range; i < range + 1; i++)
        {
            if (i != 0)
            {
                pattern.Add(new Vector3(0.0f, 0.0f, i));
            }
        }
        return pattern;
    }

    private List<Vector3> generateAreaOfEffect()
    {
        List<Vector3> areOfEffect = new List<Vector3>();
        areOfEffect.Add(new Vector3(-1.0f, 0.0f, 0.0f));
        areOfEffect.Add(new Vector3(0.0f, 0.0f, 0.0f));
        areOfEffect.Add(new Vector3(1.0f, 0.0f, 0.0f));
        return areOfEffect;
    }

    public static List<Tile> getAreaOfEffect(int x, int z, int x2, int z2)
    {
        List<Tile> area = new List<Tile>();
        if (x2 > x)
        {
            area.Add(TileMap.getTile(x2 + 1, z));
            area.Add(TileMap.getTile(x2 + 1, z - 1));
            area.Add(TileMap.getTile(x2 + 1, z + 1));
        } else if (x2 < x)
        {
            area.Add(TileMap.getTile(x2 - 1, z));
            area.Add(TileMap.getTile(x2 - 1, z - 1));
            area.Add(TileMap.getTile(x2 - 1, z + 1));
        }
        else if (z2 >= z)
        {
            area.Add(TileMap.getTile(x, z2 + 1));
            area.Add(TileMap.getTile(x + 1, z2 + 1));
            area.Add(TileMap.getTile(x - 1, z2 + 1));
        }
        else
        {
            area.Add(TileMap.getTile(x, z2 - 1));
            area.Add(TileMap.getTile(x + 1, z2 - 1));
            area.Add(TileMap.getTile(x - 1, z2 - 1));
        }
        return area;
    }


    /*
      for (int i = -range; i<range + 1; i++)
        {
            if (i != 0)
            {
                pattern.Add(new Vector3(i, 0.0f, 0.0f));
            }
            if (i == range || i == -range)
            {
                pattern.Add(new Vector3(i, 0.0f, 1.0f));
                pattern.Add(new Vector3(i, 0.0f, -1.0f));
            }
        }
        for (int i = -range; i<range + 1; i++)
        {
            if (i != 0)
            {
                pattern.Add(new Vector3(0, 0.0f, i));
            }
            if (i == range || i == -range)
            {
                pattern.Add(new Vector3(1.0f, 0.0f, i));
                pattern.Add(new Vector3(-1.0f, 0.0f, i));
            }
        }
    */
}

