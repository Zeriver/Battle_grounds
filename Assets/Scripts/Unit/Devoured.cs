using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Devoured : Enemy {


    void Start()
    {

    }

    override public void createDevoured(int x, int z, string facingDirection)
    {
        standardInitialization(x, z, facingDirection);
        positions.Add(transform.position);
        type = 2;
        name = "Devoured";

        health = 80;
        maxMovement = 3;
        movesLeft = maxMovement;
        moveSpeed = 3f;
        currentItem = new Weapon(15, 1, "melee");
        poisonResistance = 100;
    }

    void Update()
    {
        if (turnInProgress)
        {
            if (positionQueue.Count > 0 && !moving && movementHighlights.Count == 0)
            {
                setNextStep(new Vector3[]  {            //Temporary fix until proper units models will be in game TODO
                     new Vector3(0.0f, 180.0f, 0.0f),
                     new Vector3(0.0f, 0.0f, 0.0f),
                     new Vector3(0.0f, 90.0f, 0.0f),
                     new Vector3(0.0f, -90.0f, 0.0f)
                 });
            }
            if (moving)
            {
                moveToNextStep(0);
                if (positionQueue.Count == 0)
                {
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
            if (turningToTarget)
            {
                turnToEnemy();
                if (weaponHighlights.Count == 0)
                {
                    highlightTiles(weaponHighlights, attackTilesInRange, false);
                }
            }
            if (attack && weaponHighlights.Count == 0)
            {
                attackUnit();
            }
        }
    }

    override public void performTurn()
    {
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, currentItem.range, true, false);
        movementTilesInRange = TileHighlight.FindHighlight(currentTile, movesLeft, false, false);
        //movementToAttackTilesInRange = TileHighlight.FindHighlight(currentTile, moves + attackRange, false);

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
