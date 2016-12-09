using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlameThrower : Weapon
{


    public FlameThrower(int ammunition) : base(ammunition)
    {
        name = "Flame Thrower";
        cooldown = 3;
        this.ammunition = ammunition;
        range = 2;
        isDiagonal = false;
        pattern = generatePattern();
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
            if (i == range || i == -range)
            {
                pattern.Add(new Vector3(i, 0.0f, 1.0f));
                pattern.Add(new Vector3(i, 0.0f, -1.0f));
            }
        }
        for (int i = -range; i < range + 1; i++)
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
        return pattern;
    }



}

