using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsUI : MonoBehaviour
{
    private UILightSwitch[] lights;
    /*
    GameObject roomUI = GameObject.FindGameObjectWithTag("UI Completed Rooms");
        if (roomUI != null)
        {
            roomUI.GetComponent<RoomsUI>().lightUp(_level);
        }
    */
    private void Awake()
    {
        lights = GetComponentsInChildren<UILightSwitch>();
    }

    public void lightUp(int room)
    {
        lights[room].active = true;
    }
}
