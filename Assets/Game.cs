using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Game instance = null;
    private CubeManager cubeManager;
    public int totalScore;
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        cubeManager = new CubeManager();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public CubeManager GetCubeManager() {
        return cubeManager;
    } 

    public void AdjustScore(int score) {
        totalScore += score;
    }

    public int GetScore() {
        return totalScore;
    }
}
