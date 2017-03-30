using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Salamand : Enemy {

    Animator anim;

    void Start()
    {

    }

    override public void createSalamand(int x, int z, string facingDirection)
    {
        standardInitialization(x, z, facingDirection);
        transform.position = new Vector3(coordinates.x, coordinates.y, -coordinates.z + 0.5f);
        additionalPositions.Add(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
        setPositions();
        anim = GetComponent<Animator>();
        type = 3;
        name = "Salamandar";

        Debug.Log(anim);
        maxHealth = 40;
        health = maxHealth;
        moveSpeed = 3f;
        maxMovement = 10;
        movesLeft = maxMovement;
        currentItem = new Weapon(15, 1, "melee");
        fireResistance = 25;
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
                anim.Play("Walk");
                moveToNextStep(0.5f);
                if (positionQueue.Count == 0)
                {
                    if (attackUnitIfInRange())
                    {
                        turnDone = false;
                    }
                    else
                    {
                        playRandomIdleAnimation();
                        turnDone = true;
                    }
                }
            }
            if (turningToTarget)
            {
                
                anim.Play("Attack");
                //anim.Stop();
                turnToEnemy();
                if (weaponHighlights.Count == 0)
                {
                    highlightTiles(weaponHighlights, attackTilesInRange, false);
                }
            }
            if (attack && weaponHighlights.Count == 0)
            {
                //anim.Play("Attack");
                attackUnit();
            }
        }
        else if(anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle 2"))
        {
            playRandomIdleAnimation();
        }
    }

    private void playRandomIdleAnimation()
    {
        float random = Random.Range(0.0f, 3.0f);
        if (random > 1.2f)
        {
            anim.Play("Idle 2");
        }
        else
        {
            anim.Play("Idle");
        }
    }

    override public void performTurn()
    {
        attackTilesInRange.Clear();
        movementTilesInRange.Clear();
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, currentItem.range, true, false);
        //movementTilesInRange = TileHighlight.FindHighlight(currentTile, movesLeft, false, false);
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
