using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeScript : MonoBehaviour {
    public CubeType type;
    private Material material;
    public GameObject[] adjacencies = new GameObject[4];
    public CubeManager cubeManager;
    private int cubeIndex;
    public Matrix4x4 transformationMat;
    public bool destroyable = false;
    private GameObject lastCollidedWith = null;


	public void SetType(CubeType cubeType)
	{
        this.type = cubeType;
        this.material = cubeManager.GetMaterial(cubeType);

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material = this.material;
        }
        if (cubeType != CubeType.Glass) {
            gameObject.GetComponent<Rigidbody>().mass = 1;
        }
	}

    public void SetCubeIndex(int index) {
        this.cubeIndex = index;
    }

    public int GetCubeIndex()
    {
        return this.cubeIndex;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == lastCollidedWith) {
            return;
        }
        lastCollidedWith = collision.gameObject;

        if (collision.gameObject.tag == "Projectile")
        {
            if (type == CubeType.Glass)
            {
                Vector3 collPos = collision.gameObject.transform.position + gameObject.transform.forward.normalized *
                                       collision.gameObject.GetComponent<SphereCollider>().radius;
                Vector3 localCollPos = transform.InverseTransformPoint(collPos);
                Vector2 projectedPos = new Vector2(localCollPos.x, localCollPos.y);
                cubeManager.WriteString(projectedPos.ToString());
                cubeManager.AdjustCubeRow(gameObject);
                setNeighborsDestroyable();
                Destroy(gameObject);
            } else {
                SetType(GetNextType());
            }
        }
        else if (collision.gameObject.tag == "Box")
        {
            if (IsCubeBelow(gameObject, collision.gameObject))
            {
                SetAdjacency(collision.gameObject, 2);
            }
            else
            {
                SetAdjacency(collision.gameObject, 3);
                SetLeftRightAdjacencies();
            }


            cubeManager.AdjustCubeRow(gameObject);

            if (type != CubeType.Glass && destroyable)
            {
                cubeManager.HandleCubeCollision(gameObject);
            }
            destroyable = false;
           
        } else {
            cubeManager.SetCubeBottomRow(gameObject, cubeIndex);
            cubeManager.AdjustCubeRow(gameObject);

            if (type != CubeType.Glass && destroyable)
            {
                cubeManager.HandleCubeCollision(gameObject);
            }
            destroyable = false;

        }
    }

    public void setNeighborsDestroyable() {
        int rowIndex = cubeManager.FindCubeIndex(gameObject);
        if (adjacencies[2] != null) {
            adjacencies[2].GetComponent<CubeScript>().destroyable = true;
        }

    }

    //If first cube is below the second cube, return true
    public bool IsCubeBelow(GameObject cube, GameObject otherCube) {
        if (cube.transform.position.y < otherCube.transform.position.y) {
            return true;
        }

        return false;
    }

    private CubeType GetNextType() {
        switch (type)
        {
            case CubeType.Blue:
                return CubeType.Red;
            case CubeType.Red:
                return CubeType.Green;
            case CubeType.Green:
                return CubeType.Blue;
            default:
                return type;
        }
    }

    public void SetLeftRightAdjacencies() {
        GameObject cubeBelow = adjacencies[3];
        if (cubeBelow == null) {
            return;
        } else {
            GameObject cubeDiagLeft = cubeBelow.GetComponent<CubeScript>().adjacencies[0];
            GameObject cubeDiagRight = cubeBelow.GetComponent<CubeScript>().adjacencies[1];
            if (cubeDiagLeft != null) {
                SetAdjacency(cubeDiagLeft.GetComponent<CubeScript>().adjacencies[2], 0);
            }
            if (cubeDiagRight != null) {
                SetAdjacency(cubeDiagRight.GetComponent<CubeScript>().adjacencies[2], 1);
            }
        }
    }

    public void SetAdjacency(GameObject go, int index) {
        if (go == null) {
            return;
        }

        adjacencies[index] = go;
        int otherIndex;
        switch (index) {
            case 0:
                otherIndex = 1;
                break;
            case 1:
                otherIndex = 0;
                break;
            case 2:
                otherIndex = 3;
                break;
            case 3:
                otherIndex = 2;
                break;
            default:
                otherIndex = 0;
                break;
        }
        go.GetComponent<CubeScript>().adjacencies[otherIndex] = gameObject;
    }


}
