using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    public Game game;
    private bool wait = false;
    void Start()
    {
        pausePanel.SetActive(false);
    }
    void Update()
    {
        if (!wait && (Input.GetKeyDown(KeyCode.Space) || OVRInput.Get(OVRInput.Button.Two)))
        {
            if (!pausePanel.activeInHierarchy)
            {
                PauseGame();
            }
            else if (pausePanel.activeInHierarchy)
            {
                ContinueGame();
            }

            StartCoroutine(Wait());
        }
        else if (Input.GetKeyDown(KeyCode.R) || OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.1f) 
        {
            game.Restart();
        }
    }
    private void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    }
    private void ContinueGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        //enable the scripts again
    }

    public IEnumerator Wait()
    {
        wait = true;
        yield return new WaitForSecondsRealtime(0.2f);
        wait = false;
    }
}
