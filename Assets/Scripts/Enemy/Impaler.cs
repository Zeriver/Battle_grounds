using UnityEngine;
using System.Collections;

public class Impaler : Enemy {


    public Impaler(int x, int y)
    {
        name = "Impaler";
        type = 1;
        movesLeft = 4;


        coordinates = new Vector3(x, transform.position.y, y);
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, -coordinates.z + 0.5f);
        TileMap.setTileNotWalkable(x, y);
    }

    private void setBehaviour()
    {

    }

    public void performTurn()
    {
        Debug.Log(name + " " + movesLeft);
    }

}
