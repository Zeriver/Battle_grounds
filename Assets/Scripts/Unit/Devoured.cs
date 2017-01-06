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
        type = 2;
        name = "Devoured";
        maxMovement = 3;
        movesLeft = maxMovement;
        moveSpeed = 3f;
        attackRange = 1;
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
                moveToNextStep();
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
                turnToEnemy();
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
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, attackRange, true, false);
        movementTilesInRange = TileHighlight.FindHighlight(currentTile, movesLeft, false, false);
        //movementToAttackTilesInRange = TileHighlight.FindHighlight(currentTile, moves + attackRange, false);

        //Attack if in range
        if (attackUnitIfInRange())
        {
            return;
        }

        //Move and attack unit if in range
        for (int i = 0; i < movementTilesInRange.Count; i++)
        {
            attackTilesInRange = TileHighlight.FindHighlight(movementTilesInRange[i], attackRange, true, false);
            for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
            {
                if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getPlayerUnitTile()))
                {
                    TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
                    List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), movementTilesInRange[i]);
                    for (int x = 0; x < path.Count; x++)
                    {
                        positionQueue.Add(new Vector3(path[x].PosX, coordinates.y, path[x].PosY));
                    }
                    movesLeft -= positionQueue.Count;
                    highlightTiles(movementHighlights, movementTilesInRange, true);
                    return;
                }
            }
        }



        //move toward nearest opponent
        targetPosition = new Vector3(1000, 1000, 1000);
        for (int i = 0; i < movementTilesInRange.Count; i++)
        {
            for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
            {
                Vector3 movementTileVector = new Vector3(movementTilesInRange[i].PosX, transform.position.y, movementTilesInRange[i].PosY);
                float distance = Vector3.Distance(movementTileVector, _battleGroundController.playerUnits[j].coordinates);
                float oldDistance = 100000;
                if (unitToAttack != null)
                {
                    oldDistance = Vector3.Distance(targetPosition, unitToAttack.coordinates);
                }
                if (distance < oldDistance)
                {
                    targetPosition = movementTileVector;
                    unitToAttack = _battleGroundController.playerUnits[j];
                }
            }
        }
        if (targetPosition != new Vector3(1000, 1000, 1000))
        {
            Tile destination = TileMap.getTile((int)targetPosition.x, (int)targetPosition.z);
            if (movementTilesInRange.Contains(destination))
            {
                TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
                List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), destination);
                for (int i = 0; i < path.Count; i++)
                {
                    positionQueue.Add(new Vector3(path[i].PosX, coordinates.y, path[i].PosY));
                }
                unitToAttack = null;
                movesLeft -= positionQueue.Count;
                highlightTiles(movementHighlights, movementTilesInRange, true);
                return;
            }
        }

        Debug.Log("Warning: Enemy unit did not perform any action!");
        turnDone = true;
    }

}
