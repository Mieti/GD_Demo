using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleCollision : MonoBehaviour
{
    // variable used by external objects to trigger events
    public bool active = false;
    private void OnTriggerStay(Collider other)
    {
        if (gameObject.tag == "Correct Pole")
        {
            print("Staying");
            active = true;
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (gameObject.tag == "Wrong Pole")
        {
            print("Staying");
            active = true;
            GetComponent<Renderer>().material.color = Color.red;
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
