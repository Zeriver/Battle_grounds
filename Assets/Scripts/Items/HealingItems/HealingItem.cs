using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealingItem : Item
{
    protected int amount;

    protected HealingItem(int amount)
    {
        this.amount = amount;
    }

    public List<Tile> getHealingHighlights(int x, int y)
    {
        return TileHighlight.FindHighlight(TileMap.getTile((int)x, (int)y), 1, true);
    }

    public void use()
    {
        subtractAmount(1);
    }

    protected void addAmount(int value)
    {
        amount += value;
    }

    protected void subtractAmount(int value)
    {
        amount -= value;
    }
}
