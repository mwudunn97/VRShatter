using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public GameObject projectilePrefab;
    private float reloadTime = 0.3f;
    private bool reload = false;
    private float projectileForce = 40.0f;
	
	// Update is called once per frame
	void Update () {
        bool is_pressed = false;
        is_pressed = OVRInput.Get(OVRInput.Button.One) || Input.GetMouseButtonDown(0);
        if (is_pressed && !reload) {
            Fire();
            reload = true;
            StartCoroutine(waitReload());
        }
    }

    public IEnumerator waitReload()
    {
        yield return new WaitForSeconds(reloadTime);
        reload = false;
    }

    void Fire()
    {
        var position = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        position = position + transform.forward.normalized * 0.5f;
        var rotation = transform.rotation;

        var convergingDistance = 5.0f;

        // Create the Projectile from the Projectile Prefab
        var projectile = (GameObject)Instantiate(
                projectilePrefab);

        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch)) {
            position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

        } else {
            var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 
                                                                      Input.mousePosition.y - Screen.height / 2, convergingDistance));
            projectile.transform.position = position;
            projectile.transform.rotation = rotation;

            mousePos = rotation * mousePos;
            projectile.transform.LookAt(mousePos);

        }


        // Add force to the bullet
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * projectileForce);

        // Destroy the bullet after 2 seconds
        Destroy(projectile, 6.0f);
    }
}
