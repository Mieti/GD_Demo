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
    private bool hasSolvedRoom = false;

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
        string checkTagCorrect = gameObject.tag.Replace("HiddenPole", "CorrectPole");
        string checkTagWrong = gameObject.tag.Replace("HiddenPole", "WrongPole");
        GameObject[] correctPoleObjects = GameObject.FindGameObjectsWithTag(checkTagCorrect);
        GameObject[] wrongPoleObjects = GameObject.FindGameObjectsWithTag(checkTagWrong);
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
        if (hasSolvedRoom == false)
        {
            GameObject wireControllerObjectPlayer1 = GameObject.FindGameObjectWithTag("Player1L");
            GameObject wireControllerObjectPlayer2 = GameObject.FindGameObjectWithTag("Player2L");
            if (wireControllerObjectPlayer1 != null && wireControllerObjectPlayer2 != null)
            {
                WireController wireControllerPlayer1 = wireControllerObjectPlayer1.GetComponent<WireController>();
                WireController wireControllerPlayer2 = wireControllerObjectPlayer2.GetComponent<WireController>();
                if (wireControllerPlayer2 != null)
                {
                    wireControllerPlayer2.AddSegment();
                    wireControllerPlayer2.AddEnd();
                }

                if (wireControllerPlayer1 != null)
                {
                    Destroy(wireControllerObjectPlayer1);
                }
            }
            hasSolvedRoom = true;

        }



    }
}
