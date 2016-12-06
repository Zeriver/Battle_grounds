using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private float cameraSpeed;
    private bool movingToActive;
    private Vector3 target;

    private float minCameraHeight;
    private float maxCameraHeight;
    private float cameraEdgeSpeed, cameraEdgeOffset;

    void Start () {
        movingToActive = false;
        cameraSpeed = 3.0f;
        Camera.main.transform.rotation = Quaternion.Euler(55.0f, 47.0f, 0.0f);
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 12.0f, Camera.main.transform.position.z);

        minCameraHeight = 9.0f;
        maxCameraHeight = 18.0f;
        cameraEdgeOffset = 35.0f;
        cameraEdgeSpeed = 0.08f;
    }
	

	void Update () {
                    // Camera needs to follow unit if it is on the edge of camera vision or outside view TODO


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
        } else
        {
            //Something fucky is going on here, event thougth it should not entering this code while camera is moving to unit, it somehow passes the condition and it allows moving camera via scroll or edge mouse TODO

            //Mouse on edge
            if (Input.mousePosition.x >= Screen.width - cameraEdgeOffset || Input.GetKey("right"))  //Need to detect Edge of the map and not allowing pass it TODO
            {
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraEdgeSpeed, 0.0f, -cameraEdgeSpeed);
            }
            if (Input.mousePosition.x <= 0 + cameraEdgeOffset || Input.GetKey("left"))
            {
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraEdgeSpeed, 0.0f, cameraEdgeSpeed);
            }
            if (Input.mousePosition.y >= Screen.height - cameraEdgeOffset || Input.GetKey("up"))
            {
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraEdgeSpeed, 0.0f, cameraEdgeSpeed);
            }
            if (Input.mousePosition.y <= 0 + cameraEdgeOffset || Input.GetKey("down"))
            {
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraEdgeSpeed, 0.0f, -cameraEdgeSpeed);
            }


            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + (Input.GetAxis("Mouse ScrollWheel") * 10), Camera.main.transform.position.z);
                if (Camera.main.transform.position.y > maxCameraHeight)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, maxCameraHeight, Camera.main.transform.position.z);
                }
                else if (Camera.main.transform.position.y < minCameraHeight)
                {
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, minCameraHeight, Camera.main.transform.position.z);
                }
            }
        }
    }


    public void setCameraToActiveUnit(Vector3 unitPosition)
    {
        target = new Vector3(unitPosition.x - 4.5f, Camera.main.transform.position.y, unitPosition.z - 4.5f);
        movingToActive = true;
    }

}
