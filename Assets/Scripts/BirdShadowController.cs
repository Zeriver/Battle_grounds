using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdShadowController : MonoBehaviour {

    public float speed = 10;

	void Start () {
        Destroy(gameObject, 20.0f);
    }
	
	void Update () {
        transform.Translate(Time.deltaTime * -speed, 0, Time.deltaTime * speed/10f);
    }
}
