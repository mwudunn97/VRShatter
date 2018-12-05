using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour {

    public Transform location;
	// Use this for initialization
	void Start () {
        GetComponent<RectTransform>().position = location.position + location.forward * 2.0f;
        GetComponent<RectTransform>().rotation = location.rotation;
        this.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        GetComponent<RectTransform>().position = location.position + location.forward * 2.0f;
        GetComponent<RectTransform>().rotation = location.rotation;
    }
}
