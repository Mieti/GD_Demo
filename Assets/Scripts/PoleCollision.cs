using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleCollision : MonoBehaviour
{
    // variable used by external objects to trigger events
    public bool active = false;
    private int colliderCount = 0;

    private Dictionary<string, Color> tagColorMapping = new Dictionary<string, Color>
    {
        { "CorrectPole", Color.green },
        { "WrongPole", Color.red }
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        colliderCount++;
        ChangeColorIfNeeded();
    }
    // Maybe not needed with the counter
    private void OnTriggerStay2D(Collider2D other)
    {
        ChangeColorIfNeeded();
        /*
        // use tag to discriminate what is currently touching the collider
        if (other.tag == "cable")
        {
            print("Staying");
        }
        */
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        colliderCount--;
        if (colliderCount <= 0)
        {
            colliderCount = 0;
            active = false;
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void ChangeColorIfNeeded()
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
    }

}
