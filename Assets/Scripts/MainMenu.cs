using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
   
    public void play()
    {
        // implement simplematchamaking
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void join()
    {
        // implement simplematchamaking
    }

    public void quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
