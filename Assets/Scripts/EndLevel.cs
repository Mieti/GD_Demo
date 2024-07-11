using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void next()
    {
        // implement simplematchamaking
        try
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } catch
        {
            Debug.Log("It was the last level");
            SceneManager.LoadScene(0);
        }
    }

    public void replay()
    {
        Debug.Log("Replay");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void menu()
    {
        Debug.Log("Quit");
        SceneManager.LoadScene(0);
    }
}
