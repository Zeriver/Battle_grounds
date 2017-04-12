using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tuber : Enemy
{

    void Start()
    {

    }

    override public void createTuber(int x, int z, string facingDirection)
    {
        standardInitialization(x, z, facingDirection);
        positions.Add(transform.position);
        type = 1;
        anim = GetComponent<Animator>();
        name = "Tuber";

        maxHealth = 40;
        health = maxHealth;
        maxMovement = 3;
        movesLeft = maxMovement;
        moveSpeed = 1.5f;
        currentItem = new Weapon(10, 5, "melee");
    }

    void Update()
    {
        if (turnInProgress && !_battleGroundController.menuController.menu.enabled)
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
                anim.SetBool("isWalk", true);
                moveToNextStep(0);
                if (positionQueue.Count == 0)
                {
                    if (attackUnitIfInRange())
                    {
                        turnDone = false;
                    }
                    else
                    {
                        anim.SetBool("isWalk", false);
                        turnDone = true;
                    }
                }
            }
            if (turningToTarget)
            {
                anim.SetBool("isWalk", false);
                anim.SetBool("isAttack", true);
                turnToEnemy();
                if (weaponHighlights.Count == 0)
                {
                    highlightTiles(weaponHighlights, attackTilesInRange, false);
                }
            }
            if (attack && weaponHighlights.Count == 0 && !anim.GetBool("isAttack"))
            {
                attackUnit();
            }
        }
        else if (anim != null && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !anim.IsInTransition(0))
        {
            float random = Random.Range(0.0f, 3.0f);
            if (random > 2.993f)
            {
                anim.SetBool("isIdle2", true);
            }
            else
            {
                anim.SetBool("isIdle3", true);
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
