using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon {


    public Pistol(int ammunition) : base(ammunition)
    {
        name = "Pistol";
        cooldown = 0;
        this.ammunition = ammunition;
        range = 4;
        isDiagnal = false;
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
        }
        for (int i = -range; i < range + 1; i++)
        {
            if (i != 0)
            {
                pattern.Add(new Vector3(0, 0.0f, i));
            }
        }
        return pattern;
    }



}
