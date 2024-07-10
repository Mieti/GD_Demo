using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [SerializeField] public GameObject endLevelCanvas;
    [SerializeField] public int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (count%2 >= 3)
        {
            endLevelCanvas.SetActive(true);
        }
        
    }

    public void updateCount() { count++; Debug.Log(count); }
}
