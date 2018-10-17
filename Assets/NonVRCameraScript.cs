using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVRCameraScript : MonoBehaviour {

    public GameObject playerObject;
    Vector3 offset;
	// Use this for initialization
	void Start () {
        offset = new Vector3(0.0f, 0.1f, 0.0f);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = playerObject.transform.position + offset;
        if (Input.GetKey(KeyCode.D)) {
            playerObject.transform.Rotate(0,2,0);
            transform.rotation = playerObject.transform.rotation;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerObject.transform.Rotate(0, -2, 0);
            transform.rotation = playerObject.transform.rotation;
        }

	}
}
