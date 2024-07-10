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

    private AudioSource doorSound;

    private bool playerLCompleted = false;
    private bool playerRCompleted = false;
    
    private int _level;
    private string _side;

    public bool isFakePlayer = false;


    private void Awake(){
        // tag ex. "Plug1L" -> _level="1", _side="L"
        _level = int.Parse(tag[4..^1]);
        _side = tag[^1..];

        if (doorSound == null )
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            doorSound = audioSource;
            
        }

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
                // Move to door
                Vector3 wirePos = nextWire.GetStartingPoint();
                StartCoroutine( MovePlayerToDoor(p, wirePos,
                () => {
                    // Stop animation
                    p.GetComponent<PlayerKinematicMovement>().DisableAnimation();
                    // attach player to next wire
                    nextWire.AddSegmentAndPlayer(p);
                    // make sure the player can move
                    p.GetComponent<PlayerKinematicMovement>().freeze = false;
                    // close door
                    GetComponent<SpriteRenderer>().sprite = close; 
                    // destroy the current wire
                    currentWire.CreateFixedWire();
                    Destroy(currentWireObject);
                
                }));
                doorSound.Play();
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
    private IEnumerator MovePlayerToDoor(Transform player, Vector3 wirePos, System.Action onComplete)
    {
        float epsilon = 0.05f;
        float moveSpeed = 3f;
        PlayerKinematicMovement playerMove = player.GetComponent<PlayerKinematicMovement>();

        
        // Move down first
        float direction = transform.position.x - player.position.x;
        player.GetComponent<PlayerKinematicMovement>().SetDirection(direction);
        playerMove.EnambleAnimation();
        float moveDown = transform.position.y-1.5f;
        if (moveDown < player.position.y)
        {
            while (Mathf.Abs(player.position.y - moveDown) > epsilon)
            {
                Vector3 newPosition = new Vector3(player.position.x, Mathf.Lerp(player.position.y, moveDown, moveSpeed * Time.deltaTime), player.position.z);
                player.position = newPosition;
                yield return null;
            }
        }
        
        // Move on the x-axis first
        Vector3 doorPosition = transform.position;
        while (Mathf.Abs(player.position.x - doorPosition.x) > epsilon)
        {
            Vector3 newPosition = new Vector3(Mathf.Lerp(player.position.x, doorPosition.x, moveSpeed * Time.deltaTime), player.position.y, player.position.z);
            player.position = newPosition;
            yield return null;
        }
        
        // Move on the y-axis next
        while (Mathf.Abs(player.position.y - doorPosition.y) > epsilon)
        {
            Vector3 newPosition = new Vector3(player.position.x, Mathf.Lerp(player.position.y, doorPosition.y, moveSpeed * Time.deltaTime), player.position.z);
            player.position = newPosition;
            yield return null;
        }
        // REACHED THE DOOR

        // to the wire
        direction = wirePos.x - player.position.x;
        player.GetComponent<PlayerKinematicMovement>().SetDirection(direction);
        while (Mathf.Abs(player.position.y - wirePos.y) > epsilon)
        {
            Vector3 newPosition = new Vector3(player.position.x, Mathf.Lerp(player.position.y, wirePos.y, moveSpeed * Time.deltaTime), player.position.z);
            player.position = newPosition;
            yield return null;
        }

        while (Mathf.Abs(player.position.x - wirePos.x) > epsilon)
        {
            Vector3 newPosition = new Vector3(Mathf.Lerp(player.position.x, wirePos.x, moveSpeed * Time.deltaTime), player.position.y, player.position.z);
            player.position = newPosition;
            yield return null;
        }
        
        // Ensure final position is exactly the target position
        player.position = wirePos;
        playerMove.DisableAnimation();

        // Invoke the callback if provided
        onComplete?.Invoke();
    }
    
}
