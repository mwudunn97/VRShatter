using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public GameObject projectilePrefab;
	
	// Update is called once per frame
	void Update () {
        bool is_pressed = false;
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote)) {
            is_pressed = OVRInput.GetDown(OVRInput.Button.One); 
        }

        if (is_pressed) {
            Fire();
        }
	}

    void Fire()
    {
        var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        var rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        // Create the Projectile from the Projectile Prefab
        var projectile = (GameObject)Instantiate(
            projectilePrefab,
            position,
            rotation);

        // Add velocity to the bullet
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * 6;

        // Destroy the bullet after 2 seconds
        Destroy(projectile, 2.0f);
    }
}
