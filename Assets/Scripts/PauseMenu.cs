using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : NetworkBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    private GameObject[] menu;

    // Start is called before the first frame update
 
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
                //resume();
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
        Time.timeScale = 1f;
        GameIsPaused = false;
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestReplayServerRpc()
    {
        Debug.Log("reuqested replay from client");
        replay();

    }
    public void options()
    {
        Debug.Log("Options");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestQuitServerRpc()
    {
        Debug.Log("reuqested quit from client");
        quit();
    }

    public void quit()
    {
        Debug.Log("Quit");
        //Application.Quit();
        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        //NetworkManager.Singleton.Shutdown();
    }
}
