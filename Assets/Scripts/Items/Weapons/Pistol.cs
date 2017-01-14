using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon {


    public Pistol(int ammunition) : base(ammunition)
    {
        name = "Pistol";
        description = "Shoot stuff";
        this.ammunition = ammunition;

        damageType = "firearm";
        damage = 10;
        cooldown = 0;
        range = 4;
        isDiagonal = false;
    }

}
