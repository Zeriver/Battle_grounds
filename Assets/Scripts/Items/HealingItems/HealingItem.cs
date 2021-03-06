﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingItem : Item
{
    public int healingPoints;
    public int amount;

    protected HealingItem(int amount)
    {
        this.amount = amount;
    }

    public List<Tile> getHealingHighlights(int x, int y)
    {
        return TileHighlight.FindHighlight(TileMap.getTile(x, y), 1, true, true);
    }

    public bool use()
    {
        if (amount > 0)
        {
            subtractAmount(1);
            return true;
        }
        return false;
    }

    public bool isLastStackable()
    {
        if (amount == 0 && isStackable)
        {
            return true;
        }
        return false;
    }

    public void addAmount(int value)
    {
        amount += value;
    }

    protected void subtractAmount(int value)
    {
        amount -= value;
    }
}
