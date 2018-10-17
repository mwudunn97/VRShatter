using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class CubeScript : MonoBehaviour {
    public CubeType type;
    private Material material;
    public GameObject[] adjacencies = new GameObject[4];
    public CubeManager cubeManager;
    private int cubeIndex;

    public TextAsset textFile;

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
        if (collision.gameObject.tag == "Projectile" && type == CubeType.Glass) {
            
            cubeManager.AdjustCubeRow(gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Box" && type != CubeType.Glass) {
            
            var coType = collision.gameObject.GetComponent<CubeScript>().type;
            //Compare Y locations to determine top or bot
            if (IsCubeBelow(gameObject, collision.gameObject)) {
                SetAdjacency(collision.gameObject, 2);
            } else {
                SetAdjacency(collision.gameObject, 3);
                SetLeftRightAdjacencies();
            }
            cubeManager.HandleCubeCollision(gameObject);
        } else {
            cubeManager.HandleCubeCollision(gameObject);
        }
    }

    public bool IsCubeBelow(GameObject cube, GameObject otherCube) {
        if (cube.transform.position.y < otherCube.transform.position.y) {
            return true;
        }

        return false;
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

    static void WriteString(string str)
    {
        string path = "Assets/Resources/data.txt";

        //Write some text to the data.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(str);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset) Resources.Load("data");

        //Print the text from the file
        Debug.Log(asset.text);
    }
}
