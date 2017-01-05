using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

    protected GameObject BattleGroundObject;
    protected Canvas InventoryCanvas;
    protected bool isActionUsed;
    protected bool showMoves;
    protected bool turningToAttack;
    protected bool attack;
    protected float moveSpeed;
    protected int maxMovement, movesLeft;

    public Vector3 coordinates;
    public bool moving;
    public List<Weapon> weapons;
    public List<HealingItem> healingItems;
    public Item currentItem;
    public string facingDirection;

    protected List<GameObject> movementHighlights;
    protected List<GameObject> weaponHighlights;
    protected List<GameObject> weaponAreaEffectHighlights;
    protected List<Vector3> positionQueue;
    protected Quaternion targetRotation;
    protected Vector3 targetPosition;
    protected TileMapBuilder _tileMapBuilder;
    protected BattleGroundController _battleGroundController;

    protected int health;
    
    void Start () {
	
	}
	
	void Update () {
	
	}


    protected GameObject createPlane(int x, int z, Color color)   // needs to create mesh from sratch for better performance TODO
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
        plane.transform.position = new Vector3(x, 0.05f, z) * _tileMapBuilder.tileSize;
        plane.transform.position = new Vector3(plane.transform.position.x + 0.5f, plane.transform.position.y, plane.transform.position.z + 0.5f);
        plane.GetComponent<Renderer>().material.color = color;
        plane.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
        return plane;
    }

    protected void moveToNextStep()
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
        }
    }


    protected void setNextStep(Vector3 [] rotations)
    {
        if (positionQueue[0].x > coordinates.x)
        {
            turnCorrectWay(facingDirection = "up", rotations);
            targetPosition = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        }
        else if (positionQueue[0].x < coordinates.x)
        {
            turnCorrectWay(facingDirection = "down", rotations);
            targetPosition = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
        }
        else if (positionQueue[0].z < coordinates.z)
        {
            turnCorrectWay(facingDirection = "right", rotations);
            targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
        }
        else if (positionQueue[0].z > coordinates.z)
        {
            turnCorrectWay(facingDirection = "left", rotations);
            targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
        }
        moving = true;
    }

    protected void turnCorrectWay(string direction, Vector3[] rotations)
    {
        switch (direction)
        {
            case "up":
                targetRotation = Quaternion.Euler(rotations[0]);
                break;
            case "right":
                targetRotation = Quaternion.Euler(rotations[2]);
                break;
            case "down":
                targetRotation = Quaternion.Euler(rotations[1]);
                break;
            case "left":
                targetRotation = Quaternion.Euler(rotations[3]);
                break;
        }
    }

    protected void turnToEnemy()
    {
        Debug.Log("ass" + targetRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveSpeed * 80f * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, targetRotation) < 5.0f)
        {
            transform.rotation = targetRotation;
            turningToAttack = false;
            attack = true;
        }
    }

}
