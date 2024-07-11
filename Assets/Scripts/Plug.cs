using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Plug : NetworkBehaviour
{
    public bool isConnected = false;

    private PlayerKinematicMovement player;
    private AudioSource plugSound;
    private AudioSource roomCompleteSound;
    private AudioSource roomFailSound;


    [SerializeField] private SpriteRenderer _light;
    [SerializeField] private Sprite lightOff;
    [SerializeField] private Sprite lightCorrect;
    [SerializeField] private Sprite lightWrong;

    private string _level;
    private string _side;


// this room has been solved (for this player)?
    public bool roomSolved = false;

    private Door doorL;
    private Door doorR;

    public bool isFakePlayer = false;
    
    private void Awake() {
        // tag ex. "Plug1L" -> _level="1", _side="L"
        _level = tag[4..^1];
        _side = tag[^1..];
        doorL = GameObject.FindGameObjectWithTag($"Door{_level}L").GetComponent<Door>();
        doorR = GameObject.FindGameObjectWithTag($"Door{_level}R").GetComponent<Door>();
        if (doorL == null || doorR == null)
        {
            Debug.LogError("One or both doors not found for level " + _level);
        }

        // Ensure both audio sources are assigned
        if (plugSound == null || roomCompleteSound == null || roomFailSound == null)
        {
            AudioSource[] audioSources = GetComponents<AudioSource>();
            if (audioSources.Length >= 2)
            {
                plugSound = audioSources[0];
                roomCompleteSound = audioSources[1];
                roomFailSound = audioSources[2];
            }
            else
            {
                Debug.LogError("Not enough AudioSource components found on " + gameObject.name);
            }
        }

    }

    private void Update()
    {
        
        // only for testing purpuses
        if (isFakePlayer)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log($"Faking the player {_side} at level {_level}");
                // _light.GetComponent<Renderer>().material.color = Color.green;
                doorL.PlayerCompletedRoom(_side);
                doorR.PlayerCompletedRoom(_side);
            }
        }
        if(!isConnected && _light.sprite != lightOff)
        {
            _light.sprite = lightOff;
        }
    }

    public void Interact(PlayerKinematicMovement p){
        isConnected = !isConnected;
        player = p;
        WireController2D w = GameObject.FindGameObjectWithTag($"Player{_level}{_side}").GetComponent<WireController2D>();

        plugSound.Play();
        if (isConnected){
            p.freeze = true;
            bool correct = CheckCorrectPoles();
            w.AddPlug(new Vector2(transform.position.x, transform.position.y-0.657f));
            if (correct) {
                _light.sprite = lightCorrect;
                doorL.PlayerCompletedRoom(_side);
                doorR.PlayerCompletedRoom(_side);
            }
            else{
                _light.sprite = lightWrong;
            }
            
        }
        else{
            w.RemovePlug();
            p.freeze = false;
            _light.sprite = lightOff;
            doorL.PlayerDetached(_side);
            doorR.PlayerDetached(_side);
        }

    }
    private bool CheckCorrectPoles()
    {
        /* string checkTagCorrect = gameObject.tag.Replace("Plug", "CorrectPole");
        string checkTagWrong = gameObject.tag.Replace("Plug", "WrongPole");
        // poles to connect
        GameObject[] correctPoleObjects = GameObject.FindGameObjectsWithTag(checkTagCorrect);
        // poles NOT to connect
        GameObject[] wrongPoleObjects = GameObject.FindGameObjectsWithTag(checkTagWrong); */
        List<GameObject> correctPoleObjects = FindInParentWithTag("Correct Pole");
        List<GameObject> wrongPoleObjects = FindInParentWithTag("Wrong Pole");

        Debug.Log("Found correct: "+ correctPoleObjects.Count);
        Debug.Log("Found wrong: "+ wrongPoleObjects.Count);
        // ne basta 1 NON connesso
        foreach (GameObject poleObject in correctPoleObjects)
        {
            PoleCollision pole = poleObject.GetComponent<PoleCollision>();
            if (pole != null && !pole.active)
            {
                roomFailSound.Play();
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
                roomFailSound.Play();
                Debug.Log("Wrong: A pole should not be connected");
                roomSolved = true;
                return false;
            }
        }

        roomCompleteSound.Play();

        roomSolved = true;
        return true;

    }

    private List<GameObject> FindInParentWithTag(string tag)
    {
        Transform parent = transform.parent;
        List<GameObject> taggedObjects = new List<GameObject>();
        foreach (Transform child in parent)
        {
            // Check if the child has the specified tag
            if (child.CompareTag(tag))
            {
                taggedObjects.Add(child.gameObject);
            }
            // can ba added recursion if check in grandchildren
        }
        return taggedObjects;
    }

}