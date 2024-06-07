using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Levels to load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;
    public void NewGame()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedLevel1"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel1");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
