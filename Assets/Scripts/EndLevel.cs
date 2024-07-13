using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : NetworkBehaviour
{
    // Start is called before the first frame 
    public void next()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the valid range
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
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

    public void replay()
    {
        Debug.Log("Replay");
        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
    public void menu()
    {
        Debug.Log("Quit");
        NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
