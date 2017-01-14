using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Enemy :  Unit {

    protected new string name;
    protected int attackRange;
    protected int attackStrength;

    public bool turnDone;
    public bool turnInProgress;
    protected PlayerUnitController unitToAttack = null;
    protected List<Tile> attackTilesInRange = new List<Tile>();
    protected List<Tile> movementToAttackTilesInRange = new List<Tile>();
    protected List<Tile> movementTilesInRange = new List<Tile>();

    void Start()
    {
        
    }

    protected void standardInitialization(int x, int z, string facingDirection)
    {
        BattleGroundObject = GameObject.Find("BattleGrounds");
        _battleGroundController = BattleGroundObject.GetComponent("BattleGroundController") as BattleGroundController;
        _tileMapBuilder = BattleGroundObject.GetComponent("TileMapBuilder") as TileMapBuilder;

        coordinates = new Vector3(x, transform.position.y, z);
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, -coordinates.z + 0.5f);
        TileMap.setTileNotWalkable(x, z);

        positionQueue = new List<Vector3>();
        movementHighlights = new List<GameObject>();
        weaponHighlights = new List<GameObject>();

        switch (facingDirection)
        {
            case "up":
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f));
                break;
            case "right":
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
                break;
            case "down":
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, -90.0f, 0.0f));
                break;
            case "left":
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
                break;
        }
        setPositions();
    }

    public void createEnemy(int x, int z, int type, string facingDirection)
    {   

    }

    virtual public void createImpaler(int x, int z, string facingDirection)
    {

    }

    virtual public void createDevoured(int x, int z, string facingDirection)
    {

    }

    virtual public void createSalamand(int x, int z, string facingDirection)
    {

    }

    void Update()
    {

    }

    abstract public void performTurn();

    public void resetAfterTurn()
    {
        unitToAttack = null;
        turnDone = false;
        turnInProgress = false;
        movesLeft = maxMovement;
    }

    public void showPossibleMovement()
    {
        if (movementHighlights.Count == 0)
        {
            movementTilesInRange.Clear();
            Color tileColor = new Color(0.85f, 0.85f, 0.0f, 0.5f);
            for (int i = 0; i < positions.Count; i++)
            {
                List<Tile> tempList = TileHighlight.FindHighlight(TileMap.getTile((int)positions[i].x, (int)positions[i].z - 1), maxMovement, false, false);
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (!movementTilesInRange.Contains(tempList[j]))  //maybe hashsets would be more efficient
                    {
                        movementTilesInRange.Add(tempList[j]);
                    }
                }
            }
            for (int j = 0; j < movementTilesInRange.Count; j++)
            {
                int x = Mathf.FloorToInt(movementTilesInRange[j].PosX / _tileMapBuilder.tileSize);
                int z = Mathf.FloorToInt(movementTilesInRange[j].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
                movementHighlights.Add(createPlane(x, z, tileColor));
            }
        }
    }

    public void destroyMovementHighlights()
    {
        for (int i = 0; i < movementHighlights.Count; i++)
        {
            Destroy(movementHighlights[i]);
        }
        movementHighlights.Clear();
    }

    protected bool attackUnitIfInRange()
    {
        for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
        {
            if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getUnitTile()))
            {
                unitToAttack = _battleGroundController.playerUnits[j];
                if (unitToAttack.coordinates.x > coordinates.x)
                    targetRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                else if (unitToAttack.coordinates.x < coordinates.x)
                    targetRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                else if (unitToAttack.coordinates.z < coordinates.z)
                    targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                else if (unitToAttack.coordinates.z > coordinates.z)
                    targetRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                turningToAttack = true;
                return true;
            }
        }
        return false;
    }

    protected bool moveAndAttackIfInRange()
    {
        for (int i = 0; i < movementTilesInRange.Count; i++)
        {
            attackTilesInRange = TileHighlight.FindHighlight(movementTilesInRange[i], attackRange, true, false);
            for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
            {
                if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getUnitTile()))
                {
                    for (int x = 0; x < positions.Count; x++)
                    {
                        TileMap.setTileWalkable((int)positions[x].x, (int)positions[x].z - 1);
                    }
                    List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), movementTilesInRange[i]);
                    for (int x = 0; x < path.Count; x++)
                    {
                        positionQueue.Add(new Vector3(path[x].PosX, coordinates.y, path[x].PosY));
                    }
                    movesLeft -= positionQueue.Count;
                    highlightTiles(movementHighlights, movementTilesInRange, true);
                    return true;
                }
            }
        }
        return false;
    }

    protected bool moveTowardNearestEnemy()
    {
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
                for (int x = 0; x < positions.Count; x++)
                {
                    TileMap.setTileWalkable((int)positions[x].x, (int)positions[x].z - 1);
                }
                List<Tile> path = TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), destination);
                for (int i = 0; i < path.Count; i++)
                {
                    positionQueue.Add(new Vector3(path[i].PosX, coordinates.y, path[i].PosY));
                }
                unitToAttack = null;
                movesLeft -= positionQueue.Count;
                highlightTiles(movementHighlights, movementTilesInRange, true);
                return true;
            }
        }
        return false;
    }

    protected void highlightTiles(List<GameObject> highlights, List<Tile> tiles, bool isMovement)
    {
        Color tileColor;
        if (isMovement)
        {
            tileColor = new Color(0.5f, 0.85f, 0.0f, 0.5f);
        }
        else
        {
            tileColor = new Color(1.0f, 0.0f, 0.05f, 0.5f);
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            int x = Mathf.FloorToInt(tiles[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(tiles[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            highlights.Add(createPlane(x, z, tileColor));
        }
        StartCoroutine(DestroyObjectsDelayed(1.4f, highlights));
    }

    IEnumerator DestroyObjectsDelayed(float waitTime, List<GameObject> objects)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < objects.Count; i++)
        {
            Destroy(objects[i]);
        }
        objects.Clear();
    }

    public string getEnemyName()
    {
        return name;
    }

}
