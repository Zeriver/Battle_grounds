using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy : MonoBehaviour {  //Need to create more abstract unit class so playerUnit and enemies may inherit  from it TODO

    private GameObject BattleGroundObject;
    private BattleGroundController _battleGroundController;
    private TileMapBuilder _tileMapBuilder;

    private volatile List<GameObject> movementHighlights;
    private volatile List<GameObject> weaponHighlights = new List<GameObject>();

    public int type;
    protected new string name;
    protected Vector3 coordinates;
    private float moveSpeed;
    private List<Vector3> positionQueue;
    private Vector3 targetPosition;
    private bool moving;
    protected int moves;
    private int attackRange = 2;

    public bool turnDone;
    public bool turnInProgress;
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
        moves = 4;
        moveSpeed = 8f;
        coordinates = new Vector3(x, transform.position.y, z);
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, -coordinates.z + 0.5f);
        TileMap.setTileNotWalkable(x, z);
    }

    void Update()
    {
        if (turnInProgress)
        {
            if (unitToAttack != null && weaponHighlights.Count == 0)
            {
                //ATTACK TODO
                unitToAttack = null;
                turnDone = true;
            }
        }


       
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * 2.2f * Time.deltaTime);
        /*
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
        }
       */
        //showMoves = true;
    }

    public void performTurn()
    {
        turnInProgress = true;
        Tile currentTile = TileMap.getTile((int)coordinates.x, (int)coordinates.z);
        attackTilesInRange = TileHighlight.FindHighlight(currentTile, attackRange, true);
        movementToAttackTilesInRange = TileHighlight.FindHighlight(currentTile, moves + attackRange, false);
        movementTilesInRange = TileHighlight.FindHighlight(currentTile, moves + 1000, false);

        //attack if in range and with lowest HP

        for (int j = 0; j < _battleGroundController.playerUnits.Count; j++)
        {
            if (attackTilesInRange.Contains(_battleGroundController.playerUnits[j].getPlayerUnitTile()))
            {
                //Attack
                unitToAttack = _battleGroundController.playerUnits[j];
                highlightAttackTiles(attackTilesInRange);
                return;
            }
        }


        turnDone = true;


        //move toward nearest attack range of opponent
        /*
        else if (!moving && movementToAttackTilesInRange.Where(x => GameManager.instance.players.Where(y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count() > 0)
        {
            var opponentsInRange = movementToAttackTilesInRange.Select(x => GameManager.instance.players.Where(y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0 ? GameManager.instance.players.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
            Player opponent = opponentsInRange.OrderBy(x => x != null ? -x.HP : 1000).ThenBy(x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First();

            GameManager.instance.removeTileHighlights();
            moving = true;
            attacking = false;
            GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);

            List<Tile> path = TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.players.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
            GameManager.instance.moveCurrentPlayer(path[(int)Mathf.Max(0, path.Count - 1 - attackRange)]);
        }
        */
        //move toward nearest opponent
        /*
        else if (!moving && movementTilesInRange.Where(x => GameManager.instance.players.Where(y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0).Count() > 0)
        {
            var opponentsInRange = movementTilesInRange.Select(x => GameManager.instance.players.Where(y => y.GetType() != typeof(AIPlayer) && y.HP > 0 && y != this && y.gridPosition == x.gridPosition).Count() > 0 ? GameManager.instance.players.Where(y => y.gridPosition == x.gridPosition).First() : null).ToList();
            Player opponent = opponentsInRange.OrderBy(x => x != null ? -x.HP : 1000).ThenBy(x => x != null ? TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], GameManager.instance.map[(int)x.gridPosition.x][(int)x.gridPosition.y]).Count() : 1000).First();

            GameManager.instance.removeTileHighlights();
            moving = true;
            attacking = false;
            GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);

            List<Tile> path = TilePathFinder.FindPath(GameManager.instance.map[(int)gridPosition.x][(int)gridPosition.y], GameManager.instance.map[(int)opponent.gridPosition.x][(int)opponent.gridPosition.y], GameManager.instance.players.Where(x => x.gridPosition != gridPosition && x.gridPosition != opponent.gridPosition).Select(x => x.gridPosition).ToArray());
            GameManager.instance.moveCurrentPlayer(path[(int)Mathf.Min(Mathf.Max(path.Count - 1 - 1, 0), movementPerActionPoint - 1)]);
        }
        */
    }

    public void resetAfterTurn()
    {
        unitToAttack = null;
        turnDone = false;
        turnInProgress = false;
    }


    private void highlightAttackTiles(List<Tile> attacktilesInRange)
    {
        for (int i = 0; i < attacktilesInRange.Count; i++)
        {
            int x = Mathf.FloorToInt(attacktilesInRange[i].PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(attacktilesInRange[i].PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            weaponHighlights.Add(createPlane(x, z, new Color(1.0f, 0.0f, 0.05f, 0.5f)));
        }
        StartCoroutine(DestroyObjectsDelayed(1.4f, weaponHighlights));
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
