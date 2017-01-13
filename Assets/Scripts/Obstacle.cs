using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{

    public string id;
    private bool moving;
    private float moveSpeed;
    private Vector3 targetPosition;

    void Start()
    {
        moving = false;
    }

    void Update()
    {
        if (moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed/2 * Time.deltaTime);
            if (Vector3.Distance(targetPosition, transform.position) <= 0.01f)
            {
                transform.position = targetPosition;
                moving = false;
            }
        }
    }

    public void createObstacle(int x, int z)
    {
        transform.position = new Vector3(x + 0.5f, transform.position.y, -z + 0.5f);
        id = x + "|" + z;
    }

    public void moveOneTile(Tile target, float speed)
    {
        targetPosition = new Vector3(target.PosX + 0.5f, transform.position.y, -target.PosY + 0.5f);
        moveSpeed = speed;
        id = target.PosX + "|" + target.PosY;
        moving = true;
    }
}
