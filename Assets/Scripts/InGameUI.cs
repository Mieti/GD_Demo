using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Image wireBar;
    public float wireAmount = 100f;
    private float maxLen = 0;
    private List<Transform> segments;
    [SerializeField] private WireController2D wc;
    [SerializeField] private string wcName;

    public UILightSwitch[] lights;

    private void Start()
    {
        /*
        wc = GameObject.Find("WireBuilder(Clone)").GetComponentInChildren<WireController2D>();
        maxLen = wc.limitMax;
        segments = wc.segments;
        */

        lights = GameObject.Find("UI Rooms").GetComponentsInChildren<UILightSwitch>();
    }

    private void Update()
    {
        if (wc != null)
        {
            UpdateWireBar(maxLen - segments.Count);
        }
        else if (wcName != "")
        {
            InitializeWC(wcName);
        }
    }

    public void UpdateWireBar(float len)
    {
        wireAmount = len;
        wireBar.fillAmount = wireAmount / maxLen;
    }

    /*
    // deprecated
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
    */

    public void InitializeWC(string newName)
    {
        if (newName != "")
        {
            wcName = newName;
            wc = GameObject.Find(wcName).GetComponent<WireController2D>();
            if (wc != null)
            {
                maxLen = wc.limitMax;
                segments = wc.segments;
            }
        }
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
