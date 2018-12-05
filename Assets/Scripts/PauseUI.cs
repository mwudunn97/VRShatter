using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour {

    public Transform location;
	// Use this for initialization
	void Start () {
        gameObject.transform.position = location.position + location.forward * 2.0f;
        gameObject.transform.rotation = location.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = location.position + location.forward * 2.0f;
        gameObject.transform.rotation = location.rotation;
    }
}
