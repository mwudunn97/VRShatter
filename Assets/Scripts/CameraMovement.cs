using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

	public Transform target;
	Camera cam;
	public float M_SPEED;
 
	// Use this for initialization
	void Start () {
        float TARGET_ASPECT = 16.0f / 9.0f;
        float currentAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = currentAspect / TARGET_ASPECT;
        cam = GetComponent<Camera>();

        float scaleWidth = 1.0f / scaleHeight;
		M_SPEED = 0.2f;

        Rect rect = cam.rect;
        rect.width = scaleWidth;
        rect.height = 1.0f;
        rect.x = (1.0f - scaleWidth) / 2.0f;
        rect.y = 0;

        cam.rect = rect;
	}
	
	// Update is called once per frame
	void Update () {

        //linearly interpolate camera view
		if (target) {
			transform.position = Vector3.Lerp (transform.position, target.position, M_SPEED) + new Vector3(0,0,-1);
		}
	}
}
