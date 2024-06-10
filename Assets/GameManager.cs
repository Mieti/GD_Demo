using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;
    // Start is called before the first frame update
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    void Start()
    {
        PlayerInputManager.instance.JoinPlayer(0, -1, null);
        PlayerInputManager.instance.JoinPlayer(1, -1, null);
    }

    // Update is called once per frame
    void Update()
    {
    }
}


// CHANGE PREFAB ON JOIN 

//void OnPlayerJoined(PlayerInput input)
//{
//    if (playerA == null)
//    {
//        playerA = input.gameObject;
//        inputManager.playerPrefab = playerPrefabB;
//    }
//    else
//    {
//        playerB = input.gameObject;
//    }
//}