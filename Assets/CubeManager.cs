﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public enum CubeType { Red, Blue, Green, Glass };
public class CubeManager : MonoBehaviour
{
    public Material blue;
    public Material red;
    public Material green;
    public Material glass;
    public GameObject cubePrefab;
    public TextAsset textFile;
    public Game game;


    bool shouldSpawn = true;
    float waitTime = 0.3f;
    float zDist = 5.0f;
    int cubeSpread = 40;
    private GameObject[] cubeRow;

    private void Start()
    {
        cubeRow = new GameObject[cubeSpread];
    }

    public Material GetMaterial(CubeType type)
    {
        switch (type)
        {
            case CubeType.Blue:
                return blue;
            case CubeType.Red:
                return red;
            case CubeType.Green:
                return green;
            default:
                return glass;
        }
    }

    public void HandleCubeCollision(GameObject cube)
    {
        List<GameObject> matchingCubes = FindMatchingCubes(cube);
        if (matchingCubes.Count >= 3 && cube.GetComponent<CubeScript>().type != CubeType.Glass)
        {
            game.AdjustScore(matchingCubes.Count);
            Debug.Log(game.GetScore());
            while (matchingCubes.Count > 0) {
                GameObject go = matchingCubes[0];
                matchingCubes.RemoveAt(0);
                Destroy(go);
            }
        }
    }

    public int GetCubeSpread() {
        return cubeSpread;
    }

    public void AdjustCubeRow(GameObject cube) {
        List<GameObject> matchingCubes = FindMatchingCubes(cube);
        if (matchingCubes.Count >= 3 && cube.GetComponent<CubeScript>().type != CubeType.Glass)
        {
            foreach (GameObject cubeGO in matchingCubes)
            {
                var location = cube.GetComponent<CubeScript>().GetCubeIndex();
                if (cube == cubeRow[location])
                {
                    GameObject nonmatchingCube = SearchAboveForMismatch(cube);
                    if (nonmatchingCube != null)
                    {
                        cubeRow[location] = nonmatchingCube;
                        SetCubeBottomRow(nonmatchingCube, location);
                    }

                }
            }
        }
    }

    public GameObject SearchAboveForMismatch(GameObject go) {
        var originalType = go.GetComponent<CubeScript>().type;
        CubeScript nextCube = go.GetComponent<CubeScript>();
        while (nextCube.adjacencies[2] != null) {
            if (nextCube.type != originalType || nextCube.type == CubeType.Glass) {
                return nextCube.gameObject;
            }
            nextCube = nextCube.adjacencies[2].GetComponent<CubeScript>();
        }

        return null;
    }

    public GameObject GetCubeAtIndex(int col, int row)
    {
        GameObject cube = cubeRow[col];
        while (cube != null && row >= 0) {
            cube = gameObject.GetComponent<CubeScript>().adjacencies[2];
            row -= 1;
        }
        return cube;
    }

    public int FindCubeIndex(GameObject target)
    {
        GameObject cube = cubeRow[target.GetComponent<CubeScript>().GetCubeIndex()];
        int counter = 0;
        while (cube != null && cube != target)
        {
            CubeScript cubeScript = gameObject.GetComponent<CubeScript>();
            if (cubeScript == null) {
                break;
            }
            cube = cubeScript.adjacencies[2];
            counter++;
        }
        return counter;
    }

    public List<GameObject> FindMatchingCubes(GameObject cube)
    {
        CubeScript cubeScript = cube.GetComponent<CubeScript>();
        HashSet<GameObject> matchingCubes = new HashSet<GameObject>();
        List<GameObject> candidateCubes = new List<GameObject>();
        matchingCubes.Add(cube);

        AddRangeNonNull(candidateCubes, matchingCubes, cubeScript.adjacencies);

        while (candidateCubes.Count > 0)
        {
            GameObject nextCubeGO = candidateCubes[0];
            candidateCubes.RemoveAt(0);
            CubeScript nextCube = nextCubeGO.GetComponent<CubeScript>();

            if (nextCube.type == cubeScript.type)
            {
                matchingCubes.Add(nextCubeGO);
                AddRangeNonNull(candidateCubes, matchingCubes, nextCube.adjacencies);
            }
        }
        var arr = new GameObject[matchingCubes.Count];
        matchingCubes.CopyTo(arr);
        return new List<GameObject>(arr);
    }

    public void AddRangeNonNull(List<GameObject> candidates, HashSet<GameObject> matches, GameObject[] adjacencies) {
        foreach (GameObject go in adjacencies)
        {
            if (go != null && !matches.Contains(go))
            {
                candidates.Add(go);
            }
        }
    }

    public CubeType ChooseType()
    {
        float fRand = Random.Range(0.0f, 1.0f);
        if (fRand >= 0.3f)
        {
            return CubeType.Green;
        }
        //else if (fRand >= 0.7f)
        //{
        //    return CubeType.Red;
        //}
        //else if (fRand >= 0.001f)
        //{
        //    return CubeType.Blue;
        //}
        else
        {
            return CubeType.Glass;
        }
    }

    public void Spawn()
    {
        var cube = (GameObject)Instantiate(
            cubePrefab);
        cube.GetComponent<CubeScript>().cubeManager = this;
        int index = Random.Range(0, cubeSpread);
        CubeType nextType = ChooseType();
        cube.GetComponent<CubeScript>().SetType(nextType);

        placeCube(cube, index);

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldSpawn)
        {
            Spawn();
            shouldSpawn = false;
            StartCoroutine(wait());
        }

    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(waitTime);
        shouldSpawn = true;
    }

    private void placeCube(GameObject cube, int locationIndex)
    {
        var position = cubePrefab.transform.position;
        var rotation = cubePrefab.transform.rotation;

        cube.transform.Rotate(0.0f, locationIndex * (360.0f / cubeSpread), 0.0f);
        cube.transform.position = position + cube.transform.forward * zDist;

        SetCubeBottomRow(cube, locationIndex);

        cube.GetComponent<CubeScript>().SetCubeIndex(locationIndex);
    }

    public void SetCubeBottomRow(GameObject cube, int locationIndex) {
        if (cubeRow[locationIndex] == null)
        {
            cubeRow[locationIndex] = cube;
            var leftIndex = (locationIndex + cubeSpread - 1) % cubeSpread;
            var rightIndex = (locationIndex + 1) % cubeSpread;

           

            cube.GetComponent<CubeScript>().SetAdjacency(cubeRow[leftIndex], 0);
            cube.GetComponent<CubeScript>().SetAdjacency(cubeRow[rightIndex], 1);
        }
    }

    public void WriteString(string str)
    {
        string path = "Assets/Resources/data.txt";

        //Write some text to the data.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(str);
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = (TextAsset)Resources.Load("data");

        //Print the text from the file
        Debug.Log(str);
    }


}