using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        readEscInputForPauseMenu();
    }


    private void readEscInputForPauseMenu()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                resume();
            }
            else
            {
                pause();
            }
        }
    }

    public void resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    private void pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    
    public void replay()
    {
        Debug.Log("Replay");
    }

    public void options()
    {
        Debug.Log("Options");
    }

    public void quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
