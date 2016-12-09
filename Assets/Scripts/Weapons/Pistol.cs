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
        isDiagonal = false;
    }

}
