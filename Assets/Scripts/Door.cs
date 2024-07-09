using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private SpriteRenderer leftLight;
    [SerializeField] private SpriteRenderer rightLight;
    [SerializeField] private Sprite lightOff;
    [SerializeField] private Sprite lightOn;

    [SerializeField] private Sprite close;
    [SerializeField] private Sprite open;

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
            leftLight.sprite = lightOn;
        }
        else if (plugSide.Contains('R'))
        {
            playerRCompleted = true;
            // rightLight.color = Color.green;
            rightLight.sprite = lightOn;
        }

        CheckCompletion();
    }
    public void PlayerDetached(string plugTag)
    {
        if (plugTag.Contains('L'))
        {
            playerLCompleted = false;
            // leftLight.color = Color.white;
            leftLight.sprite = lightOff;
        }
        else if (plugTag.Contains('R'))
        {
            playerRCompleted = false;
            // rightLight.color = Color.white;
            rightLight.sprite = lightOff;
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
        GetComponent<SpriteRenderer>().sprite = open;        
        MoveToNextRoom();
        GetComponent<SpriteRenderer>().sprite = close; 
    }

    protected virtual void MoveToNextRoom(){
        if(isFakePlayer){
            Plug currentP = GameObject.FindGameObjectWithTag($"Plug{_level}{_side}").GetComponent<Plug>();
            currentP.isFakePlayer = false;
            Plug nextP = GameObject.FindGameObjectWithTag($"Plug{_level+1}{_side}").GetComponent<Plug>();
            Door nextD = GameObject.FindGameObjectWithTag($"Door{_level+1}{_side}").GetComponent<Door>();
            nextP.isFakePlayer=true;
            nextD.isFakePlayer=true;
            return;
        }
        GameObject currentWireObject = GameObject.FindGameObjectWithTag($"Player{_level}{_side}");
        GameObject nextWireObject = GameObject.FindGameObjectWithTag($"Player{_level+1}{_side}");
        if (currentWireObject != null && nextWireObject != null)
        {
            WireController2D currentWire = currentWireObject.GetComponent<WireController2D>();
            WireController2D nextWire = nextWireObject.GetComponent<WireController2D>();
            if (currentWire != null && nextWire != null)
            {
                // detach the joint connencted body
                Transform p = currentWire.DetachEnd();
                // nextWire.AddSegment();
                // nextWire.AddEndPlayer(p);
                nextWire.AddSegmentAndPlayer(p);
                // make sure the player can move
                p.GetComponent<PlayerKinematicMovement>().freeze = false;

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
