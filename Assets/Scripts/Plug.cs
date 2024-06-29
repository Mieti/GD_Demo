using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Plug : MonoBehaviour
{
    public bool isConnected = false;

    private PlayerKinematicMovement player;

    // [SerializeField] private Component _light;

    private string _level;
    private string _side;


// this room has been solved (for this player)?
    public bool roomSolved = false;

    private Door doorL;
    private Door doorR;

    public bool isFakePlayer = false;
    
    private void Awake(){
        // tag ex. "Plug1L" -> _level="1", _side="L"
        _level = tag[4..^1];
        _side = tag[^1..];
        doorL = GameObject.FindGameObjectWithTag($"Door{_level}L").GetComponent<Door>();
        doorR = GameObject.FindGameObjectWithTag($"Door{_level}R").GetComponent<Door>();
        if (doorL == null || doorR == null)
        {
            Debug.LogError("One or both doors not found for level " + _level);
        }

    }

    private void Update()
    {
        // only for testing purpuses
        if(isFakePlayer)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log($"Faking the player {_side} at level {_level}");
                // _light.GetComponent<Renderer>().material.color = Color.green;
                doorL.PlayerCompletedRoom(_side);
                doorR.PlayerCompletedRoom(_side);
            }
        }
    }

    public void Interact(PlayerKinematicMovement p){
        isConnected = !isConnected;
        player = p;
        if(isConnected){
            Debug.Log("Connected to plug");
            p.freeze = true;
            bool correct = CheckCorrectPoles();

            if(correct){
                // _light.GetComponent<Renderer>().material.color = Color.green;
                doorL.PlayerCompletedRoom(_side);
                doorR.PlayerCompletedRoom(_side);
            }
            else{
                //_light.GetComponent<Renderer>().material.color = Color.red;
            }
            
        }
        else{
            Debug.Log("Disconnected from plug");
            p.freeze = false;
            //_light.GetComponent<Renderer>().material.color = Color.white;
            doorL.PlayerDetached(_side);
            doorR.PlayerDetached(_side);
        }

    }
    private bool CheckCorrectPoles()
    {
        string checkTagCorrect = gameObject.tag.Replace("Plug", "CorrectPole");
        string checkTagWrong = gameObject.tag.Replace("Plug", "WrongPole");
        // poles to connect
        GameObject[] correctPoleObjects = GameObject.FindGameObjectsWithTag(checkTagCorrect);
        // poles NOT to connect
        GameObject[] wrongPoleObjects = GameObject.FindGameObjectsWithTag(checkTagWrong);
        Debug.Log(correctPoleObjects.Length);
        // ne basta 1 NON connesso
        foreach (GameObject poleObject in correctPoleObjects)
        {
            PoleCollision pole = poleObject.GetComponent<PoleCollision>();
            if (pole != null && !pole.active)
            {
                Debug.Log("Wrong: A pole should be connected.");
                roomSolved = false;
                return false;
            }
        }
        // ne basta 1 connesso
        foreach (GameObject poleObject in wrongPoleObjects)
        {
            PoleCollision pole = poleObject.GetComponent<PoleCollision>();

            if (pole != null && pole.active)
            {
                Debug.Log("Wrong: A pole should not be connected");
                roomSolved = true;
                return false;
            }
        }

        Debug.Log("Right");
        roomSolved = true;
        return true;

    }

}