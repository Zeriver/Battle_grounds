using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private float cameraSpeed;
    private bool movingToActive;
    private Vector3 target;
    private TileMapBuilder _tileMapBuilder;

    private float minCameraHeight;
    private float maxCameraHeight;
    private float cameraEdgeSpeed, cameraEdgeOffset;

    private float targetAngle = 0;
    private const float rotationAmount = 1.5f;
    private float rDistance = 1.0f;
    private float rSpeed = 1.0f;
    private Vector3 mapCenter;

    public GameObject _inventory;
    private Inventory inventory;

    void Start () {
        movingToActive = false;
        cameraSpeed = 3.0f;
        Camera.main.transform.rotation = Quaternion.Euler(38.5f, 47.0f, 0.0f);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 7.0f, Camera.main.transform.position.z);

        minCameraHeight = 6.0f;
        maxCameraHeight = 18.0f;
        cameraEdgeOffset = 5.0f;
        cameraEdgeSpeed = 0.2f;

        inventory = _inventory.GetComponent("Inventory") as Inventory;
        _tileMapBuilder = GetComponent<TileMapBuilder>();

        mapCenter = new Vector3(_tileMapBuilder.size_x / 2, Camera.main.transform.position.y, -_tileMapBuilder.size_z / 2);
    }
	

	void Update () {
        // Camera needs to follow unit if it is on the edge of camera vision or outside view TODO

        Vector3 oldPos = Camera.main.transform.position;
        if (movingToActive)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, target, cameraSpeed * Time.deltaTime);
            float distance = Vector3.Distance(Camera.main.transform.position, target);
            if (distance < 0.5f)
            {
                movingToActive = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Camera.main.transform.position = target;
                movingToActive = false;
            }
        }

        //Mouse on edge
        if (Input.mousePosition.x >= Screen.width - cameraEdgeOffset || Input.GetKey("right"))
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraEdgeSpeed, 0.0f, -cameraEdgeSpeed);
            movingToActive = false;
        }
        if (Input.mousePosition.x <= 0 + cameraEdgeOffset || Input.GetKey("left"))
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraEdgeSpeed, 0.0f, cameraEdgeSpeed);
            movingToActive = false;
        }
        if (Input.mousePosition.y >= Screen.height - cameraEdgeOffset || Input.GetKey("up"))
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraEdgeSpeed, 0.0f, cameraEdgeSpeed);
            movingToActive = false;
        }
        if (Input.mousePosition.y <= 0 + cameraEdgeOffset || Input.GetKey("down"))
        {
            Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraEdgeSpeed, 0.0f, -cameraEdgeSpeed);
            movingToActive = false;
        }
        if (Camera.main.transform.position.x > _tileMapBuilder.size_x/1.4f || Camera.main.transform.position.x < -2
            || Camera.main.transform.position.z > -5 || Camera.main.transform.position.z < -_tileMapBuilder.size_z)
        {
            Camera.main.transform.position = oldPos;
            movingToActive = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f && !inventory.equipment.enabled)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + (Input.GetAxis("Mouse ScrollWheel") * 10), Camera.main.transform.position.z);
            movingToActive = false;
            if (Camera.main.transform.position.y > maxCameraHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, maxCameraHeight, Camera.main.transform.position.z);
            }
            else if (Camera.main.transform.position.y < minCameraHeight)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, minCameraHeight, Camera.main.transform.position.z);
            }
            mapCenter = new Vector3(mapCenter.x, Camera.main.transform.position.y, mapCenter.z);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //targetAngle -= 45.0f;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //targetAngle += 45.0f;
        }

        if (targetAngle != 0)
        {
            Rotate();
        }
    }

    protected void Rotate()
    {

        float step = rSpeed * Time.deltaTime;
        float orbitCircumfrance = 2F * rDistance * Mathf.PI;
        float distanceDegrees = (rSpeed / orbitCircumfrance) * 360;
        float distanceRadians = (rSpeed / orbitCircumfrance) * 2 * Mathf.PI;

        if (targetAngle > 0)
        {
            Camera.main.transform.RotateAround(target, Vector3.up, -rotationAmount);
            targetAngle -= rotationAmount;
        }
        else if (targetAngle < 0)
        {
            Camera.main.transform.RotateAround(target, Vector3.up, rotationAmount);
            targetAngle += rotationAmount;
        }

    }


    public void setCameraToActiveUnit(Vector3 unitPosition)
    {
        target = new Vector3(unitPosition.x - 7.5f, Camera.main.transform.position.y, unitPosition.z - 7.5f);
        movingToActive = true;
    }

}
