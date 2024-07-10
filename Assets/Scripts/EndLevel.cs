using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    
    public void next()
    {
        //SceneManager.LoadScene();
        Debug.Log("next level");
    }

    public void replay()
    {
        Debug.Log("Replay");
    }

    public void menu()
    {
        Debug.Log("Menu");
    }
}
