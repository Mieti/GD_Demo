using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject endLevelCanvas;
    [SerializeField] public int count = 0;

    RoomsUI ingameUI;
    // Start is called before the first frame update
    void Start()
    {
        ingameUI = GameObject.Find("UI Ingame").GetComponent<RoomsUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (count%2 >= 3)
        //{
        //    endLevelCanvas.SetActive(true);
        //}
        if (count%2 == 0)
        {
            ingameUI.lightUp(count/2);
        }
        
        
    }

    public void updateCount()
    { 
        count++;
        //Debug.Log(count);
    }
    public void levelCompleted() { Debug.Log("Level ended"); endLevelCanvas.SetActive(true); }
}
