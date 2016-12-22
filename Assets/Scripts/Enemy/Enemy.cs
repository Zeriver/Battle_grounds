using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MonoBehaviour {  //Need to create more abstract unit class so playerUnit and enemies may inherit  from it TODO

    private GameObject BattleGroundObject;
    private BattleGroundController _battleGroundController;
    private TileMapBuilder _tileMapBuilder;

    private volatile List<GameObject> movementHighlights = new List<GameObject>();
    private volatile List<GameObject> weaponHighlights = new List<GameObject>();

    public int type;
    protected new string name;
    protected Vector3 coordinates;
    private float moveSpeed;
    private List<Vector3> positionQueue;
    private bool moving;
    protected int movesLeft, maxMovement;
    private int attackRange = 2;

    public bool turnDone;
    public bool turnInProgress;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private PlayerUnitController unitToAttack = null;
    private List<Tile> attackTilesInRange = new List<Tile>();
    private List<Tile> movementToAttackTilesInRange = new List<Tile>();
    private List<Tile> movementTilesInRange = new List<Tile>();

    void Start()
    {
        BattleGroundObject = GameObject.Find("BattleGrounds");
        _battleGroundController = BattleGroundObject.GetComponent("BattleGroundController") as BattleGroundController;
        _tileMapBuilder = BattleGroundObject.GetComponent("TileMapBuilder") as TileMapBuilder;
    }

    public void createEnemy(int x, int z, int type)
    {
        this.type = type;
        name = "Impaler";
        maxMovement = 4;
        movesLeft = maxMovement;
        moveSpeed = 3f;
        coordinates = new Vector3(x, transform.position.y, z);
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, -coordinates.z + 0.5f);
        TileMap.setTileNotWalkable(x, z);

        positionQueue = new List<Vector3>();
    }

    void Update()
    {
        if (turnInProgress)
        {
            if (unitToAttack != null && weaponHighlights.Count == 0)
            {
                //ATTACK TODO  
                //turning to attacked unit
                unitToAttack = null;
                turnDone = true;
            }
        }

        if (positionQueue.Count > 0 && !moving && movementHighlights.Count == 0)
        {
            if (positionQueue[0].x > coordinates.x)
            {
                targetRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                targetPosition = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
            }
            else if (positionQueue[0].x < coordinates.x)
            {
                targetRotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                targetPosition = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);

            }
            else if (positionQueue[0].z < coordinates.z)
            {
                targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);

            }
            else if (positionQueue[0].z > coordinates.z)
            {
                targetRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);

            }
            moving = true;
        }
        if (moving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * 2.2f * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(targetPosition, transform.position) <= 0.1f)
            {
                coordinates = positionQueue[0];
                transform.position = new Vector3(positionQueue[0].x + 0.5f, positionQueue[0].y, -positionQueue[0].z + 0.5f);
                positionQueue.RemoveAt(0);
                moving = false;
            }
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
    }

    public void performTurn()
    {
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, attackRange, true);
        movementTilesInRange = TileHighlight.FindHighlight(currentTile, movesLeft, false);
        //movementToAttackTilesInRange = TileHighlight.FindHighlight(currentTile, moves + attackRange, false);

        //Attack if in range
        if (attackUnitIfInRange())
        {
            return;
        }

        //Move and attack unit if in range
        for (int i = 0; i < movementTilesInRange.Count; i++)
        {
            attackTilesInRange = TileHighlight.FindHighlight(movementTilesInRange[i], attackRange, true);
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
        targetPosition = new Vector3 (1000,1000,1000);
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

    public void resetAfterTurn()
    {
        unitToAttack = null;
        turnDone = false;
        turnInProgress = false;
        movesLeft = maxMovement;
    }

    private bool attackUnitIfInRange()
    {
        for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
        {
            if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getPlayerUnitTile()))
            {
                unitToAttack = _battleGroundController.playerUnits[j];
                highlightTiles(weaponHighlights, attackTilesInRange, false);
                return true;
            }
        }
        return false;
    }

    private void highlightTiles(List<GameObject> highlights, List<Tile> tiles, bool isMovement)
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

    private GameObject createPlane(int x, int z, Color color)   // needs to create mesh from sratch for better performance TODO
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
        plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
        plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
        plane.GetComponent<Renderer>().material.color = color;
        plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        return plane;
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

}
