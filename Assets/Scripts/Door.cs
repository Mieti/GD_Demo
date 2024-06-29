using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    // [SerializeField] public SpriteRenderer leftLight;
    // [SerializeField] public SpriteRenderer rightLight;

    private bool playerLCompleted = false;
    private bool playerRCompleted = false;
    
    private int _level;
    private string _side;

    public bool isFakePlayer = false;


    private void Awake(){
        // tag ex. "Plug1L" -> _level="1", _side="L"
        _level = int.Parse(tag[4..^1]);
        _side = tag[^1..];

    }

    // plugSide is "R" or "L"
    public void PlayerCompletedRoom(string plugSide)
    {
        if (plugSide.Contains('L'))
        {
            playerLCompleted = true;
            // leftLight.color = Color.green;
        }
        else if (plugSide.Contains('R'))
        {
            playerRCompleted = true;
            // rightLight.color = Color.green;
        }

        CheckCompletion();
    }
    public void PlayerDetached(string plugTag)
    {
        if (plugTag.Contains('L'))
        {
            playerLCompleted = false;
            // leftLight.color = Color.white;
        }
        else if (plugTag.Contains('R'))
        {
            playerRCompleted = false;
            // rightLight.color = Color.white;
        }

    }


    private void CheckCompletion()
    {
        if (playerLCompleted && playerRCompleted)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // Implement door opening logic (e.g., animation or enabling/disabling objects)
        Debug.Log("Door opens!");
        MoveToNextRoom();
    }

    public void MoveToNextRoom(){
        if(isFakePlayer){
            return;
        }
        GameObject currentWireObject = GameObject.FindGameObjectWithTag($"Player{_level}{_side}");
        GameObject nextWireObject = GameObject.FindGameObjectWithTag($"Player{_level+1}{_side}");
        if (currentWireObject != null && nextWireObject != null)
        {
            WireController currentWire = currentWireObject.GetComponent<WireController>();
            WireController nextWire = nextWireObject.GetComponent<WireController>();
            if (currentWire != null && nextWire != null)
            {
                // detach the joint connencted body
                Transform p = currentWire.DetachEnd();
                nextWire.AddSegment();
                nextWire.AddEndPlayer(p);
                // make sure the player can move
                p.GetComponent<PlayerController>().freeze = false;

                // destroy the current wire
                Destroy(currentWireObject);
            }
            else
            {
                Debug.Log($"Unable to find a wire at level {_level}");
            }
        }
        else
        {
                Debug.Log($"Unable to find a wire object at level {_level}");
        }

    }
    
}
