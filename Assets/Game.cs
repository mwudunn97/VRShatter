using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Game : MonoBehaviour {

    public static Game instance = null;
    [SerializeField] private GameObject gameOverPanel;
    private CubeManager cubeManager;
    private Score score;
    bool gameOver = false;
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
        score = new Score();

    }

	private void Update()
	{
        if (gameOver && (Input.GetKeyDown(KeyCode.R) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))) {
            Restart();
        }
	}

	public CubeManager GetCubeManager() {
        return cubeManager;
    } 

    public void AdjustScore(int deltaScore) {
        score.AdjustScore(deltaScore);
    }

    public int GetScore() {
        return score.GetScore();
    }

    public void GameOver() {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        gameOver = true;
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
