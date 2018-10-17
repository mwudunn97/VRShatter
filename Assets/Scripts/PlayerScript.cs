using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public GameObject projectilePrefab;
    private float reloadTime = 0.5f;
    private bool reload = false;
	
	// Update is called once per frame
	void Update () {
        bool is_pressed = false;
        is_pressed = OVRInput.Get(OVRInput.Button.One);

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
        //mousePos = Vector3.Scale(mousePos, new Vector3( (float) 1 / (Screen.width / 2), (float) 1 / (Screen.height / 2), 1.0f));
        //Debug.Log(mousePos);
        //var mouseMag = mousePos.magnitude;
        //var mouseVec = (rotation * mousePos.normalized) * mousePos.magnitude;


        position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        //var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, convergingDistance));

        // Create the Projectile from the Projectile Prefab
        var projectile = (GameObject)Instantiate(
            projectilePrefab);
        projectile.transform.position = position;
        projectile.transform.rotation = rotation;
         
        // Add velocity to the bullet
        //projectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(mouseVec * 4.0f);
        //projectile.transform.LookAt(mousePos);
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 15.0f);

        // Destroy the bullet after 2 seconds
        Destroy(projectile, 1.2f);
    }
}
