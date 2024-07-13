using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{

    [SerializeField] private SpriteRenderer leftLight;
    [SerializeField] private SpriteRenderer rightLight;
    [SerializeField] private Sprite lightOff;
    [SerializeField] private Sprite lightOn;

    [SerializeField] private Sprite close;
    [SerializeField] private Sprite open;

    [SerializeField] public AudioSource doorSound;

    private bool playerLCompleted = false;
    private bool playerRCompleted = false;
    public bool isAnimationCompleted = false;

    private int _level;
    private string _side;

    public GameManager gameManager;

    // public bool isFakePlayer = false;

    private NetworkVariable<bool> _playerLCompleted = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _playerRCompleted = new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _playerLCompleted.OnValueChanged += OnValueLChanged;
        _playerRCompleted.OnValueChanged += OnValueRChanged;
    }

    private void OnValueLChanged(bool previous, bool current)
    {
        Debug.Log("value changed L from " + OwnerClientId + " " + _playerLCompleted.Value + ";   " + _playerRCompleted.Value);
        LightOnLClientRpc(current);
        if (IsHost)
            CheckCompletion();
        else
            CheckCompletionClientRpc();
    }

    private void OnValueRChanged(bool previous, bool current)
    {
        Debug.Log("value changed R from " + OwnerClientId + " " + _playerLCompleted.Value + ";   " + _playerRCompleted.Value);
        LightOnRClientRpc(current);
        if (IsHost)
            CheckCompletion();
        else
            CheckCompletionClientRpc();
    }
   
    [Rpc(SendTo.Server)]
    private void OnPlayerLCompletedServerRpc(bool isCompleted)
    {
        _playerLCompleted.Value = isCompleted;
        leftLight.sprite = isCompleted ? lightOn : lightOff;
    }

    [Rpc(SendTo.Server)]
    private void OnPlayerRCompletedServerRpc(bool isCompleted)
    {
        _playerRCompleted.Value = isCompleted;
        rightLight.sprite = isCompleted ? lightOn : lightOff;
    }
    [Rpc(SendTo.NotServer)]
    private void LightOnLClientRpc(bool isCompleted)
    {
        leftLight.sprite = isCompleted ? lightOn : lightOff;
    }

    [Rpc(SendTo.NotServer)]
    private void LightOnRClientRpc(bool isCompleted)
    {
        rightLight.sprite = isCompleted ? lightOn : lightOff;
    }
    [Rpc(SendTo.Server)]
    private void DestroyWireR_ServerRpc()
    {
        GameObject wire = GameObject.FindGameObjectWithTag($"Player{_level}R");
        Destroy(wire);
    }
    public void Awake()
    {
        // tag ex. "Plug1L" -> _level="1", _side="L"
        _level = int.Parse(tag[4..^1]);
        _side = tag[^1..];

        if (doorSound == null)
        {
            //AudioSource[] audioSource = GetComponents<AudioSource>();
            //doorSound = audioSource[0];
            Debug.Log("No doorSound");

        }
        gameManager = GetComponentInParent<GameManager>();
        if (gameManager == null)
        {
            Debug.Log("No gameManager found");
        }

    }
    public void Update()
    {
        if (GetComponent<SpriteRenderer>().sprite == open && isAnimationCompleted)
        {
            GetComponent<SpriteRenderer>().sprite = close;
        }
    }

    // plugSide is "R" or "L"
    public void PlayerCompletedRoom(string plugSide)
    {
        if (plugSide.Contains('L'))
        {
            OnPlayerLCompletedServerRpc(true);
            //playerLCompleted = true;
            // leftLight.color = Color.green;
            //leftLight.sprite = lightOn;
        }
        else if (plugSide.Contains('R'))
        {
            OnPlayerRCompletedServerRpc(true);
            //playerRCompleted = true;
            // rightLight.color = Color.green;
            //rightLight.sprite = lightOn;
        }

    }
    public void PlayerDetached(string plugTag)
    {
        if (plugTag.Contains('L'))
        {
            OnPlayerLCompletedServerRpc(false);
            //playerLCompleted = false;
            // leftLight.color = Color.white;
            leftLight.sprite = lightOff;
        }
        else if (plugTag.Contains('R'))
        {
            OnPlayerRCompletedServerRpc(false);
            //playerRCompleted = false;
            // rightLight.color = Color.white;
            rightLight.sprite = lightOff;
        }

    }


    private void CheckCompletion()
    {
        //Debug.Log(_playerLCompleted.Value + ";   " + _playerRCompleted.Value + " ;clientId    " + OwnerClientId);
        if (_playerLCompleted.Value && _playerRCompleted.Value)
        {
            Debug.Log("sei arrivato ad aprire la porta " + OwnerClientId);
            OpenDoor();
        }
    }

    [Rpc(SendTo.NotServer)]
    private void CheckCompletionClientRpc()
    {
        //Debug.Log(_playerLCompleted.Value + ";   " + _playerRCompleted.Value + " ;clientId    " + OwnerClientId);
        if (_playerLCompleted.Value && _playerRCompleted.Value)
        {
            Debug.Log("sei arrivato ad aprire la porta " + OwnerClientId);
            OpenDoor2();
        }
    }

    private void OpenDoor()
    {
        // Implement door opening logic (e.g., animation or enabling/disabling objects)
        GetComponent<SpriteRenderer>().sprite = open;
        // Funziona anche per LastDoor
        gameManager.updateCount();
        MoveToNextRoom();
    }

    private void OpenDoor2()
    {
        // Implement door opening logic (e.g., animation or enabling/disabling objects)
        GetComponent<SpriteRenderer>().sprite = open;
        // Funziona anche per LastDoor
        print(gameObject.tag + "updating count");
        gameManager.updateCount();
        MoveToNextRoom2();
    }
    protected virtual void MoveToNextRoom()
    {
        /* if (isFakePlayer)
        {
            Plug currentP = GameObject.FindGameObjectWithTag($"Plug{_level}{_side}").GetComponent<Plug>();
            currentP.isFakePlayer = false;
            Plug nextP = GameObject.FindGameObjectWithTag($"Plug{_level + 1}{_side}").GetComponent<Plug>();
            Door nextD = GameObject.FindGameObjectWithTag($"Door{_level + 1}{_side}").GetComponent<Door>();
            nextP.isFakePlayer = true;
            nextD.isFakePlayer = true;
            return;
        } */
        GameObject currentWireObject = GameObject.FindGameObjectWithTag($"Player{_level}{_side}");
        GameObject nextWireObject = GameObject.FindGameObjectWithTag($"Player{_level + 1}{_side}");
        if (currentWireObject != null && nextWireObject != null)
        {
            Debug.Log("MOVETONEXTDOOR " + OwnerClientId + "      " + currentWireObject + "; " + nextWireObject);
            WireController2D currentWire = currentWireObject.GetComponent<WireController2D>();
            WireController2D nextWire = nextWireObject.GetComponent<WireController2D>();
            //Debug.Log("MOVETONEXTDOOR " + OwnerClientId + "      " + currentWire + "; " + nextWire);
            if (currentWire != null && nextWire != null)
            {
                // detach the joint connencted body
                Transform p = currentWire.DetachEnd();
                if (p != null)
                {
                // Move to door
                Vector3 wirePos = nextWire.GetStartingPoint();
                StartCoroutine(MovePlayerToDoor(p, transform.position, wirePos,
                () => {
                    // Stop animation
                    p.GetComponent<PlayerKinematicMovement>().DisableAnimation();
                    // attach player to next wire
                    nextWire.AddSegmentAndPlayer(p);
                    // make sure the player can move
                    p.GetComponent<PlayerKinematicMovement>().freeze = false;
                    // close door
                    GetComponent<SpriteRenderer>().sprite = close;
                    isAnimationCompleted = true;
                    // destroy the current wire
                    currentWire.CreateFixedWire();
                    Destroy(currentWireObject);
                }));

                doorSound.Play();

                /*
                // deprecated
                nextWire.activateUI();
                currentWire.disableUI();
                */
                gameManager.UpdateUI(nextWire);
                }
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
    protected virtual void MoveToNextRoom2()
    {
        /* if (isFakePlayer)
        {
            Plug currentP = GameObject.FindGameObjectWithTag($"Plug{_level}{_side}").GetComponent<Plug>();
            currentP.isFakePlayer = false;
            Plug nextP = GameObject.FindGameObjectWithTag($"Plug{_level + 1}{_side}").GetComponent<Plug>();
            Door nextD = GameObject.FindGameObjectWithTag($"Door{_level + 1}{_side}").GetComponent<Door>();
            nextP.isFakePlayer = true;
            nextD.isFakePlayer = true;
            return;
        } */
        GameObject currentWireObject = GameObject.FindGameObjectWithTag($"Player{_level}R");
        GameObject nextWireObject = GameObject.FindGameObjectWithTag($"Player{_level + 1}R");
        if (currentWireObject != null && nextWireObject != null)
        {
            // Debug.Log("MOVETONEXTDOOR " + OwnerClientId + "      " + currentWireObject + "; " + nextWireObject);
            WireController2D currentWire = currentWireObject.GetComponent<WireController2D>();
            WireController2D nextWire = nextWireObject.GetComponent<WireController2D>();
            //Debug.Log("MOVETONEXTDOOR " + OwnerClientId + "      " + currentWire + "; " + nextWire);
            if (currentWire != null && nextWire != null)
            {
                // detach the joint connencted body
                Transform p = currentWire.DetachEnd();
                if (p != null)
                {
                    // Move to door
                Vector3 wirePos = nextWire.GetStartingPoint();
                GameObject doorR = GameObject.FindGameObjectWithTag($"Door{_level}R");
                StartCoroutine(MovePlayerToDoor(p, doorR.transform.position, wirePos,
                () => {
                    // Stop animation
                    p.GetComponent<PlayerKinematicMovement>().DisableAnimation();
                    // attach player to next wire
                    nextWire.AddSegmentAndPlayer(p);
                    // make sure the player can move
                    p.GetComponent<PlayerKinematicMovement>().freeze = false;
                    // close door
                    GetComponent<SpriteRenderer>().sprite = close;
                    doorR.GetComponent<Door>().isAnimationCompleted = true;
                    // telling server to destroy the current wire?
                    currentWire.CreateFixedWire();
                    if(currentWireObject.GetComponent<NetworkObject>()!=null)
                    {
                        DestroyWireR_ServerRpc();
                    }
                    else
                    {
                        Destroy(currentWireObject);
                    }

                }));

                doorSound.Play();

                nextWire.activateUI();
                currentWire.disableUI();
                }
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
    private IEnumerator MovePlayerToDoor(Transform player,Vector3 doorPos, Vector3 wirePos, System.Action onComplete)
    {
        float epsilon = 0.05f;
        float moveSpeed = 3f;
        PlayerKinematicMovement playerMove = player.GetComponent<PlayerKinematicMovement>();


        // Move down first
        float direction = transform.position.x - player.position.x;
        player.GetComponent<PlayerKinematicMovement>().SetDirection(direction);
        playerMove.EnambleAnimation();
        float moveDown = transform.position.y - 1.5f;
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
        //Vector3 doorPosition = transform.position;
        while (Mathf.Abs(player.position.x - doorPos.x) > epsilon)
        {
            Vector3 newPosition = new Vector3(Mathf.Lerp(player.position.x, doorPos.x, moveSpeed * Time.deltaTime), player.position.y, player.position.z);
            player.position = newPosition;
            yield return null;
        }

        // Move on the y-axis next
        while (Mathf.Abs(player.position.y - doorPos.y) > epsilon)
        {
            Vector3 newPosition = new Vector3(player.position.x, Mathf.Lerp(player.position.y, doorPos.y, moveSpeed * Time.deltaTime), player.position.z);
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