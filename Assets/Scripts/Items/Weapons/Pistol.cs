using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pistol : Weapon {


    public Pistol(int ammunition) : base(ammunition)
    {
        name = "Pistol";
        description = "Shoot stuff";
        cooldown = 0;
        this.ammunition = ammunition;
        range = 4;
        isDiagonal = false;
    }

}
