using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrigger : MonoBehaviour {

    public GameObject parentObj;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            parentObj.GetComponent<CubeScript>().ShatterCube(other);
        }
    }
}
