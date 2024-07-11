using UnityEngine;
using UnityEngine.UI;

public class UILightSwitch : MonoBehaviour
{
    public bool active = false;

    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;

    private Image img;

    // Start is called before the first frame update
    private void Awake()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (active && img.sprite != on)
        {
            img.sprite = on;
        }
        else if (!active && img.sprite != off)
        {
            img.sprite = off;
        }
    }
}
