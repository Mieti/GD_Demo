using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Image wireBar;
    public float wireAmount = 100f;
    private float maxLen = 0;
    private List<Transform> segments;
    private WireController2D wc;

    public UILightSwitch[] lights;

    private void Start()
    {
        /*
        string findTag = gameObject.tag.Replace("UI", "Player");
        wc = GameObject.FindGameObjectWithTag(findTag);
        maxLen = wc.GetComponent<WireController2D>().limitMax;
        */
        wc = GameObject.Find("Room 1L").GetComponentInChildren<WireController2D>();
        maxLen = wc.limitMax;
        segments = wc.segments;

        lights = GameObject.Find("UI Rooms").GetComponentsInChildren<UILightSwitch>();
    }

    private void Update()
    {
        UpdateWireBar(maxLen - segments.Count);
    }

    public void UpdateWireBar(float len)
    {
        wireAmount = len;
        wireBar.fillAmount = wireAmount / maxLen;
    }

    public void Appear()
    {
        Canvas canvasObject = gameObject.GetComponentInChildren<Canvas>();
        canvasObject.enabled = true;
    }

    public void Disappear()
    {
        Canvas canvasObject = gameObject.GetComponentInChildren<Canvas>();
        canvasObject.enabled = false;
    }

    public void UpdateWC(WireController2D newWC)
    {
        wc = newWC;
        maxLen = wc.limitMax;
        segments = wc.segments;
    }

    //Lights
    public void LightUp(int room)
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
