using UnityEngine;
using System.Collections.Generic;

public class PlayerUnitController : MonoBehaviour {

    // mouse events must be moved to BattleGroundController TODO

    GameObject BattleGroundObject; 
    private bool isSelected, showMoves;
    private int maxMovement, movesLeft;
    private Vector3 coordinates;

    private List<Tile> validMoves;
    private List<GameObject> highlights;
    private List<Vector3> positionQueue;

    TileMapBuilder _tileMapBuilder;
    MouseHighlight _mouseHiglight;


    void Start() {

    }   

    public void createPlayerUnit(int x, int y, int moves)
    {
        BattleGroundObject = GameObject.Find("BattleGrounds");
        _mouseHiglight = BattleGroundObject.GetComponent("MouseHighlight") as MouseHighlight;
        _tileMapBuilder = BattleGroundObject.GetComponent("TileMapBuilder") as TileMapBuilder;

        isSelected = true;
        showMoves = true;
        maxMovement = moves;
        movesLeft = maxMovement;
        coordinates = new Vector3(x, 0.0f, y); //in battle map vertices
        transform.position = new Vector3(coordinates.x + 0.5f, coordinates.y, coordinates.z + 0.5f);
        validMoves = new List<Tile>();
        highlights = new List<GameObject>();
        positionQueue = new List<Vector3>();
        //showAllowedMovements();
    }
	
	void Update () {
	    if (isSelected)
        {
            if (showMoves && positionQueue.Count == 0)
            {
                showAllowedMovements();
            }
            if (Input.GetMouseButtonDown(0))
            {
                Transform mousePositin = _mouseHiglight.getHighlightSelection();
                Tile destinationTile = TileMap.getTile((int)mousePositin.position.x, (int)mousePositin.position.z);
                if (validMoves.Contains(destinationTile))
                {
                    foreach (GameObject plane in highlights)
                    {
                        Destroy(plane);
                    }
                    highlights.Clear();
                    foreach (Tile t in TilePathFinder.FindPath(TileMap.getTile((int)coordinates.x, (int)coordinates.z), destinationTile))
                    {
                        positionQueue.Add(new Vector3(t.PosX, 0.0f, t.PosY));
                    }
                }
            }
            if (positionQueue.Count > 0)
            {
                foreach(Vector3 v in positionQueue)
                {
                    Debug.Log(v);
                }
                 
                coordinates += (positionQueue[0] - coordinates).normalized * 15.0f * Time.deltaTime;
                if (Vector3.Distance(positionQueue[0], coordinates) <= 0.1f)
                {
                    coordinates = positionQueue[0];
                    transform.position = new Vector3(positionQueue[0].x + 0.5f, positionQueue[0].y, -positionQueue[0].z + 0.5f);
                    positionQueue.RemoveAt(0);
                }
                //movesLeft--;
                showMoves = true;
            }
        }
	}

    private void showAllowedMovements()
    {
        foreach (GameObject plane in highlights)
        {
            Destroy(plane);
        }
        highlights.Clear();
        validMoves.Clear();
        validMoves = TileHighlight.FindHighlight(TileMap.getTile((int)coordinates.x, (int)coordinates.z), movesLeft);
        highlightAvailableMoves();
        showMoves = false;
    }

    private void highlightAvailableMoves()
    {
        foreach(Tile tile in validMoves)
        {
            int x = Mathf.FloorToInt(tile.PosX / _tileMapBuilder.tileSize);
            int z = Mathf.FloorToInt(tile.PosY * (-1) / _tileMapBuilder.tileSize);  //* -1 because battleGround generates on negative z TODO
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
            plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
            plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
            plane.GetComponent<Renderer>().material.color = new Color(0.5f, 0.85f, 0.0f, 0.5f);
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            highlights.Add(plane);
        }
    }


    /*                                      WORKS ONLY IF NOT NEAR EDGE OF THE MAP
    private void getValidMoves(Tile currentTile, int movePoints)
    {
        validMoves.Add(currentTile);
        foreach(Tile tile in TileMap.GetListOfAdjacentTiles(currentTile.getX(), currentTile.getY()))
        {
            if (tile != null)
            {
                int nextMoveCost = movePoints - tile.getMoveCost();
                if (nextMoveCost >= 0 && !validMoves.Contains(tile))
                {
                    getValidMoves(tile, nextMoveCost);
                }
            } else
            {
                return;
            }

        }
    }*/

}
