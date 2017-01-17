using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSkill {

    public string name;
    public int level;
    public int experience;
    public int[] levelRequirements;

    public WeaponSkill(string name)
    {
        this.name = name;
        level = 0;
        experience = 0;
        levelRequirements = new int[] { 2, 5, 10 };
    }
}
