using UnityEngine;
using UnityEngine.UI;

public class UILightSwitch : MonoBehaviour
{
    public bool active = false;

    [SerializeField] private Sprite off;
    [SerializeField] private Sprite on;

    public int num;
    private Image img;

    // Start is called before the first frame update
    private void Awake()
    {
        img = GetComponent<Image>();
        // object name should be LightN (n being the number referring to the room)
        num = int.Parse(gameObject.name[5..]);

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
