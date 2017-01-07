using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Salamand : Enemy {


    void Start()
    {

    }

    override public void createSalamand(int x, int z, string facingDirection)
    {
        standardInitialization(x, z, facingDirection);
        transform.position = new Vector3(coordinates.x + 1.0f, coordinates.y, -coordinates.z + 0.5f);
        additionalPositions.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
        setPositions();
        type = 3;
        name = "Salamandar";
        maxMovement = 3;
        movesLeft = maxMovement;
        moveSpeed = 3f;
        attackRange = 10;
    }

    void Update()
    {
        if (turnInProgress)
        {
            if (positionQueue.Count > 0 && !moving && movementHighlights.Count == 0)
            {
                setNextStep(new Vector3[]  {            //Temporary fix until proper units models will be in game TODO
                     new Vector3(0.0f, 90.0f, 0.0f),
                     new Vector3(0.0f, -90.0f, 0.0f),
                     new Vector3(0.0f, 0.0f, 0.0f),
                     new Vector3(0.0f, 180.0f, 0.0f)
                 });
            }
            if (moving)
            {
                moveToNextStep(0.5f);
                if (positionQueue.Count == 0)
                {
                    TileMap.setTileNotWalkable((int)coordinates.x, (int)coordinates.z);
                    if (attackUnitIfInRange())
                    {
                        turnDone = false;
                    }
                    else
                    {
                        turnDone = true;
                    }
                }
            }
            if (turningToAttack)
            {
                //turnToEnemy();
                turningToAttack = false;
                attack = true;
                if (weaponHighlights.Count == 0)
                {
                    highlightTiles(weaponHighlights, attackTilesInRange, false);
                }
            }
            if (attack && weaponHighlights.Count == 0)
            {
                //ATTACK TODO  
                //turning to attacked unit
                unitToAttack = null;
                attack = false;
                turnDone = true;
            }
        }
    }

    override public void performTurn()
    {
        attackTilesInRange.Clear();
        movementTilesInRange.Clear();
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, attackRange, true, false);
        for (int i = 0; i < positions.Count; i++)
        {
            List<Tile> temp = TileHighlight.FindHighlight(TileMap.getTile((int)positions[i].x, (int)positions[i].z - 1), movesLeft, false, false);
            for (int j = 0; j < temp.Count; j++)
            {
                if (!movementTilesInRange.Contains(temp[j]))  //maybe hashsets would be more efficient
                {
                    movementTilesInRange.Add(temp[j]);
                }
            }
        }

        if (attackUnitIfInRange())
        {
            return;
        }

        if (moveAndAttackIfInRange())
        {
            return;
        }

        if (moveTowardNearestEnemy())
        {
            return;
        }

        Debug.Log("Warning: Enemy unit did not perform any action!");
        turnDone = true;
    }

}
