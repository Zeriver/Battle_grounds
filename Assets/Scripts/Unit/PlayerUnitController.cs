using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerUnitController : Unit
{
    // mouse events MAYBE should be moved to BattleGroundController TODO

    public bool isSelected;
    public bool isActionMode;
    public List<GameObject> giveHighlights;
    public List<GameObject> pushHighlights;
    private List<Tile> validTiles;
    private Vector3 actionMouseHighlight;
    private MouseHighlight _mouseHiglight;
    private Inventory inventory;
   
    void Start()
    {

    }

    public void createPlayerUnit(int x, int y, int moves, string facingDirection)
    {
        BattleGroundObject = GameObject.Find("BattleGrounds");
        _mouseHiglight = BattleGroundObject.GetComponent("MouseHighlight") as MouseHighlight;
        _tileMapBuilder = BattleGroundObject.GetComponent("TileMapBuilder") as TileMapBuilder;
        _battleGroundController = BattleGroundObject.GetComponent("BattleGroundController") as BattleGroundController;
        InventoryCanvas = GameObject.Find("EquipmentCanvas").GetComponent<Canvas>();
        inventory = InventoryCanvas.GetComponent("Inventory") as Inventory;

        isSelected = false;
        showMoves = false;
        isActionMode = false;
        isActionUsed = false;
        moving = false;
        coordinates = new Vector3(x, 2.5f, y); //in battle map vertices   // TEMPORARY 2.5f on y because model pivot is incorrect TODO
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, -coordinates.z + 0.5f);
        targetRotation = transform.rotation;
        validTiles = new List<Tile>();
        movementHighlights = new List<GameObject>();
        weapons = new List<Weapon>();
        healingItems = new List<HealingItem>();
        weaponHighlights = new List<GameObject>();
        giveHighlights = new List<GameObject>();
        pushHighlights = new List<GameObject>();
        weaponAreaEffectHighlights = new List<GameObject>();
        positionQueue = new List<Vector3>();
        targets = new List<Unit>();
        obstaclesToAttack = new List<Obstacle>();
        weaponSkills = new List<WeaponSkill>();
        TileMap.setTileNotWalkable(x, y);

        maxHealth = 100;
        health = 15;
        maxMovement = moves;
        movesLeft = maxMovement;
        moveSpeed = 6.0f;
        currentEffect = "none";
        healthEffects = new List<int>();

        switch (facingDirection)
        {
            case "up":
                transform.rotation = Quaternion.Euler(new Vector3(-90.0f, 90.0f, 0.0f));
                break;
            case "right":
                transform.rotation = Quaternion.Euler(new Vector3(-90.0f, 180.0f, 0.0f));
                break;
            case "down":
                transform.rotation = Quaternion.Euler(new Vector3(-90.0f, -90.0f, 0.0f));
                break;
            case "left":
                transform.rotation = Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f));
                break;
        }
        this.facingDirection = facingDirection;

        //EQ
        weapons.Add(new Pistol(5));
        weapons.Add(new FlameThrower(5));
        healingItems.Add(new MediumHealingKit(2));
        currentItem = healingItems[0];
    }

    void Update()
    {
        if (isSelected && !inventory.equipment.enabled)
        {
            if (showMoves && positionQueue.Count == 0)
            {
                showAllowedMovements();
            }
            if (positionQueue.Count > 0 && !moving)
            {
                setNextStep(new Vector3[]  {            //Temporary fix until proper units models will be in game TODO
                     new Vector3(-90.0f, 90.0f, 0.0f),
                     new Vector3(-90.0f, -90.0f, 0.0f),
                     new Vector3(-90.0f, 0.0f, 0.0f),
                     new Vector3(-90.0f, 180.0f, 0.0f)
                 });
                showMoves = true;
            }
            if (moving)
            {
                moveToNextStep(0);
            }
            if (turningToTarget)
            {
                turnToEnemy();
            }
            if (attack)
            {
                if (currentItem is Weapon)
                {
                    if (((Weapon)currentItem).useWeapon())
                    {
                        for (int i = 0; i < targets.Count; i++)
                        {
                            targets[i].getAttacked(((Weapon)currentItem), this, getBonusDamageFromWeaponSkill());
                        }
                        for (int i = 0; i < obstaclesToAttack.Count; i++)
                        {
                            obstaclesToAttack[i].getDamaged(((Weapon)currentItem), getBonusDamageFromWeaponSkill());
                        }
                        if (targets.Count > 0)
                        {
                            weaponSkillUpgrade();
                        }
                        isActionUsed = true;
                        switchActionMode();
                    }
                    targets.Clear();
                    obstaclesToAttack.Clear();
                    attack = false;
                }
                else if (currentItem is HealingItem)
                {
                    if (((HealingItem)currentItem).use())
                    {
                        for (int i = 0; i < targets.Count; i++)
                        {
                            targets[i].getHealed(((HealingItem)currentItem));
                        }
                        switchActionMode();
                        isActionUsed = true;
                    }
                    targets.Clear();
                    attack = false;
                }
            }
            if (Input.GetMouseButtonDown(0) && positionQueue.Count == 0 && !turningToTarget) //LEFT CLICK
            {
                Vector3 mousePosition = Input.mousePosition;
                if (mousePosition.y < Screen.height * 0.1681271) //divided by player ui anchor
                {
                    return;
                }
                Tile clickedTile = TileMap.getTile((int)_mouseHiglight.getHighlightSelection().position.x, (int)_mouseHiglight.getHighlightSelection().position.z);
                if (!isActionMode && giveHighlights.Count == 0 && pushHighlights.Count == 0)
                {
                    if (validTiles.Contains(clickedTile))
                    {
                        TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
                        destroyMovementHiglights();
                        List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), clickedTile);
                        for (int i = 0; i < path.Count; i++)
                        {
                            positionQueue.Add(new Vector3(path[i].PosX, coordinates.y, path[i].PosY));
                        }
                        int pathCost = 0;
                        for (int i = 0; i < path.Count; i++)
                        {
                            pathCost += path[i].MoveCost;
                        }
                        movesLeft -= pathCost;
                    }
                }
                else if (isActionMode && validTiles != null)
                {
                    if (validTiles.Contains(clickedTile))
                    {
                        if (currentItem is Weapon)
                        {
                            List<Tile> weaponEffect = ((Weapon)currentItem).getAreaEffect(Math.Abs((int)coordinates.x), Math.Abs((int)coordinates.z), clickedTile.PosX, clickedTile.PosY);
                            List<Unit> unitsInArea = new List<Unit>();
                            unitsInArea.AddRange(_battleGroundController.enemyUnits.Cast<Unit>());
                            unitsInArea.AddRange(_battleGroundController.playerUnits.Cast<Unit>());
                            for (int j = 0; j < weaponEffect.Count; j++)
                            {
                                for (int i = 0; i < unitsInArea.Count; i++)
                                {
                                    for (int x = 0; x < unitsInArea[i].positions.Count; x++)
                                    {
                                        if (TileMap.getTile((int)unitsInArea[i].positions[x].x, (int)unitsInArea[i].positions[x].z - 1).Equals(weaponEffect[j]))
                                        {
                                            targets.Add(unitsInArea[i]);
                                            break;
                                        }
                                    }
                                }
                                if (TileMap.getObstacleAt(weaponEffect[j].PosX, weaponEffect[j].PosY) != null)
                                {
                                    obstaclesToAttack.Add(TileMap.getObstacleAt(weaponEffect[j].PosX, weaponEffect[j].PosY));
                                }
                            }
                        }
                        else if (currentItem is HealingItem)
                        {
                            List<Unit> unitsInArea = new List<Unit>();
                            unitsInArea.AddRange(_battleGroundController.playerUnits.Cast<Unit>());
                            for (int i = 0; i < unitsInArea.Count; i++)
                            {
                                for (int j = 0; j < unitsInArea[i].positions.Count; j++)
                                {
                                    if (TileMap.getTile((int)unitsInArea[i].positions[j].x, (int)unitsInArea[i].positions[j].z - 1).Equals(clickedTile))
                                    {
                                        targets.Add(unitsInArea[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        if (clickedTile.PosX > coordinates.x)
                            targetRotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                        else if (clickedTile.PosX < coordinates.x)
                            targetRotation = Quaternion.Euler(-90.0f, -90.0f, 0.0f);
                        else if (clickedTile.PosY < coordinates.z)
                            targetRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
                        else if (clickedTile.PosY > coordinates.z)
                            targetRotation = Quaternion.Euler(-90.0f, 180.0f, 0.0f);
                        turningToTarget = true;
                    }
                }
                else if (giveHighlights.Count != 0)
                {
                    for (int i = 0; i < _battleGroundController.playerUnits.Count; i++)
                    {
                        if (_battleGroundController.playerUnits[i].getUnitTile().Equals(clickedTile))
                        {
                            if (currentItem is Weapon)
                            {
                                _battleGroundController.playerUnits[i].weapons.Add((Weapon)currentItem);
                                weapons.Remove((Weapon)currentItem);
                                currentItem = null;
                            }
                            else if (currentItem is HealingItem)
                            {
                                _battleGroundController.playerUnits[i].healingItems.Add((HealingItem)currentItem);
                                healingItems.Remove((HealingItem)currentItem);
                                currentItem = null;
                            }
                            giveMode();
                        }
                    }
                }
                else if (pushHighlights.Count != 0)
                {
                    if (clickedTile.IsPushable)
                    {
                        Tile pushableTile;
                        if (coordinates.x > clickedTile.PosX)
                            pushableTile = TileMap.getTile(clickedTile.PosX - 1, clickedTile.PosY);
                        else if (coordinates.x < clickedTile.PosX)
                            pushableTile = TileMap.getTile(clickedTile.PosX + 1, clickedTile.PosY);
                        else if (coordinates.z > clickedTile.PosY)
                            pushableTile = TileMap.getTile(clickedTile.PosX, clickedTile.PosY - 1);
                        else
                            pushableTile = TileMap.getTile(clickedTile.PosX, clickedTile.PosY + 1);
                        if (pushableTile.IsWalkable)
                        {
                            pushableTile.IsWalkable = false;
                            pushableTile.IsPushable = true;
                            pushableTile.Id = 4;
                            pushableTile.IsUnitOnIt = clickedTile.IsUnitOnIt;
                            pushableTile.IsBlockingWeapons = clickedTile.IsBlockingWeapons;

                            TileMap.setTileWalkable((int)coordinates.x, (int)coordinates.z);
                            TileMap.resetObstacleTile(clickedTile);

                            TileMap.getObstacleAt(clickedTile.PosX, clickedTile.PosY).moveOneTile(pushableTile, moveSpeed);
                            List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), clickedTile);
                            for (int i = 0; i < path.Count; i++)
                            {
                                positionQueue.Add(new Vector3(path[i].PosX, coordinates.y, path[i].PosY));
                            }
                            movesLeft -= positionQueue.Count;
                            isActionUsed = true;
                            pushMode();
                        }
                    }
                }
            }
            if (Input.GetMouseButtonDown(1) && positionQueue.Count == 0 && !isActionUsed) //RIGHT CLICK
            {
                switchActionMode();
            }
            if (isActionMode && validTiles != null) //HOVER
            {
                Tile hoverTile = TileMap.getTile((int)_mouseHiglight.getHighlightSelection().position.x, (int)_mouseHiglight.getHighlightSelection().position.z);
                if (validTiles.Contains(hoverTile) && currentItem is Weapon)
                {
                    actionMouseHighlight = new Vector3((int)_mouseHiglight.getHighlightSelection().position.x + 0.5f, 0.0f, (int)_mouseHiglight.getHighlightSelection().position.z + 0.5f);
                    for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
                    {
                        Destroy(weaponAreaEffectHighlights[i]);
                    }
                    int mouseX = Math.Abs((int)actionMouseHighlight.x);
                    int mouseY = Math.Abs((int)actionMouseHighlight.z - 1);
                    List<Tile> weaponAreaEffect = ((Weapon)currentItem).getAreaEffect(Math.Abs((int)coordinates.x), Math.Abs((int)coordinates.z), mouseX, mouseY);
                    highlightWeaponAreaEffect(weaponAreaEffect);
                }
                else if (!(validTiles.Contains(hoverTile) && actionMouseHighlight != null))
                {
                    for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
                    {
                        Destroy(weaponAreaEffectHighlights[i]);
                    }
                }
            }
        }
    }

    public void showAllowedMovements()
    {
        destroyMovementHiglights();
        validTiles = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), movesLeft, false, false);
        for (int i = 0; i < validTiles.Count; i++)
        {
            int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            movementHighlights.Add(createPlane(x, z, new Color(0.5f, 0.85f, 0.0f, 0.5f)));
        }
        showMoves = false;
    }

    private void highlightWeaponRange()
    {
        if (currentItem is Weapon)
        {
            validTiles = ((Weapon)currentItem).getWeaponHighlights((int)coordinates.x, (int)coordinates.z);
        }
        else if (currentItem is HealingItem)
        {
            validTiles = ((HealingItem)currentItem).getHealingHighlights((int)coordinates.x, (int)coordinates.z);
        }
        else
        {
            validTiles = null;
        }
        if (validTiles != null)
        {
            for (int i = 0; i < validTiles.Count; i++)
            {
                int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
                int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
                weaponHighlights.Add(createPlane(x, z, new Color(1.0f, 0.0f, 0.05f, 0.5f)));
            }
        }
    }

    private void highlightWeaponAreaEffect(List<Tile> weaponAreaEffect)
    {
        for (int i = 0; i < weaponAreaEffect.Count; i++)
        {
            int x = Mathf.FloorToInt(weaponAreaEffect[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(weaponAreaEffect[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            weaponAreaEffectHighlights.Add(createPlane(x, z, new Color(8.0f, 8.0f, 0.0f, 0.5f)));
        }
    }

    private void setActionMode(bool value)
    {
        isActionMode = !value;
        switchActionMode();
    }

    public void switchActionMode()
    {
        if (giveHighlights.Count > 0)
        {
            destroyGiveHiglights();
        }
        if (pushHighlights.Count > 0)
        {
            destroyPushHiglights();
        }
        for (int i = 0; i < _battleGroundController.enemyUnits.Count; i++)
        {
            _battleGroundController.enemyUnits[i].destroyMovementHighlights();
        }

        isActionMode = !isActionMode;
        if (isActionMode && !isActionUsed)
        {
            destroyMovementHiglights();
            highlightWeaponRange();
        }
        else
        {
            DestroyWeaponHiglights();
            for (int i = 0; i < weaponAreaEffectHighlights.Count; i++)
            {
                Destroy(weaponAreaEffectHighlights[i]);
            }
            showMoves = true;
        }
    }

    public void DestroyWeaponHiglights()
    {
        destroyTiles(weaponHighlights, validTiles);
    }

    public void destroyMovementHiglights()
    {
        destroyTiles(movementHighlights, validTiles);
    }

    private void destroyGiveHiglights()
    {
        destroyTiles(giveHighlights, validTiles);
        giveHighlights.Clear();
    }

    private void destroyPushHiglights()
    {
        destroyTiles(pushHighlights, validTiles);
        pushHighlights.Clear();
    }

    private void destroyTiles(List<GameObject> highlights, List<Tile> tiles)
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            Destroy(highlights[i]);
        }
        if (tiles != null)
        {
            tiles.Clear();
        }
    }

    public void giveMode()
    {
        if (giveHighlights.Count > 0)
        {
            destroyGiveHiglights();
            setActionMode(false);
        }
        else
        {
            destroyMovementHiglights();
            DestroyWeaponHiglights();

            validTiles = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), 1, true, false);
            for (int i = 0; i < validTiles.Count; i++)
            {
                int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
                int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
                giveHighlights.Add(createPlane(x, z, new Color(0.85f, 0.85f, 0.0f, 0.5f)));
            }
        }
    }

    public void pushMode()
    {
        if (pushHighlights.Count > 0)
        {
            destroyPushHiglights();
            setActionMode(false);
        }
        else
        {
            destroyMovementHiglights();
            DestroyWeaponHiglights();

            validTiles = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), 1, true, false);
            for (int i = 0; i < validTiles.Count; i++)
            {
                int x = Mathf.FloorToInt(validTiles[i].PosX / _tileMapBuilder.tileSize);
                int z = Mathf.FloorToInt(validTiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
                pushHighlights.Add(createPlane(x, z, new Color(0.85f, 0.85f, 0.5f, 0.5f)));
            }
        }
    }


    public void defendingPosition()
    {
        deactivatePlayerUnit();
        isSelected = true;
        movesLeft = 0;
        isActionUsed = true;
        defending = true;
    }

    public void setPlayerUnitActive()
    {
        setActionMode(false);
        isSelected = true;
        showMoves = true;
    }

    public void deactivatePlayerUnit()
    {
        isSelected = false;
        showMoves = false;
        setActionMode(false);
        destroyMovementHiglights();
    }

    public void resetAfterTurn()
    {
        destroyMovementHiglights();
        standardReset();
        calculateDefendBonus();
        isActionUsed = false;
        defending = false;
        setActionMode(false);
    }
}
