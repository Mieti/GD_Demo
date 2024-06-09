using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlugController : MonoBehaviour
{
    public bool isConected = false;
    public UnityEvent OnWirePlugged;
    public UnityEvent OnWireUnplugged;
    public Transform plugPosition;

    [HideInInspector]
    public Transform endAnchor;
    [HideInInspector]
    public Rigidbody endAnchorRB;
    [HideInInspector]

    public WireController wireController;

    private bool wasConnectedLastFrame = false;

    public void OnPlugged()
    {
        OnWirePlugged.Invoke();
        CheckCorrectPoles();
    }

    public void OnUnplugged()
    {
        OnWireUnplugged.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == endAnchor.gameObject)
        {
            isConected = true;
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            endAnchor.transform.rotation = transform.rotation;
            wasConnectedLastFrame = true;

            OnPlugged();
        }
    }

    private void Update()
    {
        if (isConected)
        {
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            Vector3 eulerRotation = new Vector3(this.transform.eulerAngles.x + 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            endAnchor.transform.rotation = Quaternion.Euler(eulerRotation);

            CheckCorrectPoles();

            // Handle disconnection if player tried to move
            isConected = false;
            endAnchorRB.isKinematic = false;
            OnUnplugged();
        }

        wasConnectedLastFrame = isConected;
    }

    private void CheckCorrectPoles()
    {
        GameObject[] correctPoleObjects = GameObject.FindGameObjectsWithTag("CorrectPole1L");
        GameObject[] wrongPoleObjects = GameObject.FindGameObjectsWithTag("WrongPole1L");
        foreach (GameObject poleObject in correctPoleObjects)
        {
            PoleCollision pole = poleObject.GetComponent<PoleCollision>();
            if (pole != null && !pole.active)
            {
                Debug.Log("Wrong");
                return;
            }
        }

        foreach (GameObject poleObject in wrongPoleObjects)
        {
            PoleCollision pole = poleObject.GetComponent<PoleCollision>();

            if (pole != null && pole.active)
            {
                Debug.Log("Wrong");
                return;
            }
        }

        Debug.Log("Right");
        GameObject wireControllerObject = GameObject.FindGameObjectWithTag("Player2L");
        if (wireControllerObject != null)
        {
            WireController wireController = wireControllerObject.GetComponent<WireController>();
            if (wireController != null)
            {
                wireController.AddSegment();
                wireController.AddEnd();
            }
        }

        
    }
}
