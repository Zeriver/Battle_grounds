using UnityEngine;
using System.Collections;

public class MediumHealingKit : HealingItem
{

	public MediumHealingKit(int amount) : base(amount)
    {
        name = "Medium Healing kit";
        description = "Oh shit gettup!";
        this.amount = amount;
    }
}
