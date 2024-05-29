using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleAppear : MonoBehaviour
{
    GameObject appearingPole;
    private void Start()
    {
        // The appearing pole should be called "Hidden" + the same name as THIS pole.
        appearingPole = GameObject.Find("Hidden" + gameObject.name);
        appearingPole.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        print("Staying");
        appearingPole.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        print("Exiting");
        appearingPole.SetActive(false);
    }

    /*
    // Collider version (needs animation for object appearing)
    BoxCollider appearingPoleCollider;
    private void Start()
    {
        // The appearing pole should be called "Hidden" + the same name as THIS pole.
        appearingPoleCollider = GameObject.Find("Hidden" + gameObject.name).GetComponent<BoxCollider>();
        appearingPoleCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        print("Staying");
        appearingPoleCollider.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        print("Exiting");
        appearingPoleCollider.enabled = false;
    }
    */
}
