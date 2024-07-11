using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsUI : MonoBehaviour
{
    private UILightSwitch[] lights;
    private void Awake()
    {
        lights = GetComponentsInChildren<UILightSwitch>();
    }

    public void lightUp(int room)
    {
        foreach (var light in lights)
        {
            if (light.num == room)
            {
                light.active = true;
            }
        }
    }
}
