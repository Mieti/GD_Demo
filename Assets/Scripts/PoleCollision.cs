using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleCollision : MonoBehaviour
{
    // variable used by external objects to trigger events
    public bool active = false;

    private Dictionary<string, Color> tagColorMapping = new Dictionary<string, Color>
    {
        { "CorrectPole", Color.green },
        { "WrongPole", Color.red }
    };

    private void OnTriggerStay(Collider other)
    {
        foreach (var tagColor in tagColorMapping)
        {
            if (gameObject.tag.Contains(tagColor.Key))
            {
                active = true;
                GetComponent<Renderer>().material.color = tagColor.Value;
                return;
            }
        }
        /*
        // use tag to discriminate what is currently touching the collider
        if (other.tag == "cable")
        {
            print("Staying");
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        active = false;
        GetComponent<Renderer>().material.color = Color.white;
    }

}
