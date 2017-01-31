using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Weapon {

    public Axe() : base()
    {
        name = "Axe";
        description = "Chop chop!";
        damageType = "cut";
        damage = 10;
        cooldown = 0;
        range = 1;
        isDiagonal = false;
        isFlankingBonus = true;
        isMelee = true;
    }
}
