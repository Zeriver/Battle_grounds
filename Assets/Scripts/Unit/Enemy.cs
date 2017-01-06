using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Enemy :  Unit {

    public int type;
    protected new string name;
    protected int attackRange;

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
            movementTilesInRange = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), maxMovement, false, false);
            Color tileColor = new Color(0.85f, 0.85f, 0.0f, 0.5f);
            for (int i = 0; i < movementTilesInRange.Count; i++)
            {
                int x = Mathf.FloorToInt(movementTilesInRange[i].PosX / _tileMapBuilder.tileSize);
                int z = Mathf.FloorToInt(movementTilesInRange[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
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
            if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getPlayerUnitTile()))
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
        yield return new WaitForSeconds(waitTime);
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
