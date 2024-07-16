using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndLevel : NetworkBehaviour
{
    public TextMeshProUGUI levelText;
    // Start is called before the first frame update
    void Start()
    {
        int sceneNumber = SceneManager.GetActiveScene().buildIndex - 1;
        levelText.text = "LEVEL " + sceneNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void next()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Time.timeScale = 1f;
            // Get the path of the next scene
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            // Extract the scene name from the path
            string nextSceneName = Path.GetFileNameWithoutExtension(nextScenePath);

            // Load the next scene using NetworkManager's SceneManager
            NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("It was the last level");
            // Load the main menu or first scene
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestNextServerRpc()
    {
        Debug.Log("reuqested replay from client");
        next();


    }
    public void replay()
    {
        Debug.Log("Replay");
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestReplayServerRpc()
    {
        Debug.Log("reuqested replay from client");
        replay();

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
