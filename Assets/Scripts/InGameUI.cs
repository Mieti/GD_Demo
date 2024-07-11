using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Image wireBar;
    public float wireAmount = 100f;
    //GameObject wc;
    float maxLen = 0;
    List<Transform> segments;

    private void Start()
    {
        /*
        string findTag = gameObject.tag.Replace("UI", "Player");
        wc = GameObject.FindGameObjectWithTag(findTag);
        maxLen = wc.GetComponent<WireController2D>().limitMax;
        */

        maxLen = GetComponentInParent<WireController2D>().limitMax;
        segments = GetComponentInParent<WireController2D>().segments;
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

    public void appear()
    {
        Canvas canvasObject = gameObject.GetComponentInChildren<Canvas>();
        canvasObject.enabled = true;
    }

    public void disappear()
    {
        Canvas canvasObject = gameObject.GetComponentInChildren<Canvas>();
        canvasObject.enabled = false;
    }
}
