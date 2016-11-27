using UnityEngine;
using System.Collections.Generic;

public class PlayerUnitController : MonoBehaviour {


    GameObject Mouse; // Must be moved to BattleGroundController TODO

    class Coordinates
    {
        int posX, posY;

        public Coordinates(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }

        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }
    }


    private bool isSelected, showMoves;
    private int maxMovement, movesLeft;
    private Coordinates coordinates;

    private List<Tile> validMoves;
    private List<GameObject> highlights;

    TileMapBuilder _tileMapBuilder;


    void Start() {

    }   

    public void createPlayerUnit(int x, int y, int moves)
    {
        _tileMapBuilder = GetComponent<TileMapBuilder>();
        isSelected = true;
        showMoves = true;
        maxMovement = moves;
        movesLeft = maxMovement;
        coordinates = new Coordinates(x, y); //in battle map vertices
        validMoves = new List<Tile>();
        highlights = new List<GameObject>();
    }
	
	void Update () {
	    if (isSelected)
        {
            if (showMoves)
            {
                showAllowedMovements();
            }
            if (Input.GetMouseButtonDown(0))
            {
                foreach(GameObject plane in highlights)
                {
                    Destroy(plane);
                }
                highlights.Clear();
                Mouse = GameObject.Find("BattleGrounds");
                MouseHighlight ass = Mouse.GetComponent("MouseHighlight") as MouseHighlight;
                Transform mousePositin = ass.getHighlightSelection();
                coordinates.PosX = ((int)mousePositin.position.x);
                coordinates.PosY = ((int)mousePositin.position.z);
                showMoves = true;
            }
        }
	}

    private void showAllowedMovements()
    {
        validMoves.Clear();
        validMoves = TileHighlight.FindHighlight(TileMap.getTile(coordinates.PosX, coordinates.PosY), movesLeft);
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
