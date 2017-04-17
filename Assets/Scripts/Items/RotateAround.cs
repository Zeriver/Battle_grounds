using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour {

    public float speed = 20.0f;
    


	void Update () {
        // transform.RotateAround(transform.position, transform.up, Time.deltaTime * speed);
        transform.Rotate(0, 0, Time.deltaTime * speed);
    }
}
