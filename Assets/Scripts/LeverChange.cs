using UnityEngine;
using System.Collections;
using Unity.Netcode;
using Newtonsoft.Json.Linq;

public class LeverController : NetworkBehaviour
{
    //private SpriteRenderer spriteRenderer;
    private Vector3 newScale = new Vector3(-1, 1, 1);
    private Vector3 oldScale = new Vector3(1, 1, 1);
    public GameObject lever;
    public GameObject toHidePole;
    public GameObject toShowPole;
    private int colliderCount = 0;
    private bool active = false;
    private bool hasBeenActivated = false;
    private NetworkVariable<bool> _leverActivated = new NetworkVariable<bool>(false);

    [SerializeField] private AudioSource soundLeverActivation;
    [SerializeField] private AudioSource soundLeverDeactivation;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _leverActivated.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(bool previous, bool current)
    {
        //Debug.Log("value changed L from " + OwnerClientId + " " + _playerLCompleted.Value + ";   " + _playerRCompleted.Value);
        if (IsHost)
        {
            toShowPole.SetActive(current);
            if (toHidePole != null)
            {
                toHidePole.SetActive(!current);
            }
        }
        ActivateLeverClientRpc(current);
    }

    [Rpc(SendTo.Server)]
    private void OnLeverActivatedServerRpc(bool value)
    {
        _leverActivated.Value = value;
    }

    [Rpc(SendTo.NotServer)]
    private void ActivateLeverClientRpc(bool value)
    {
        toShowPole.SetActive(value);
        if (toHidePole != null)
        {
            toHidePole.SetActive(!value);
        }

    }
    private void Start()
    {

    }

    private void Update()
    {
        if (active && colliderCount <= 0)
        {
            colliderCount = 0;
            active = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        colliderCount++;
        if (colliderCount > 0)
        {
            if (!hasBeenActivated)
            {
                ActivateLever();
                if (!soundLeverActivation.isPlaying)
                {
                    soundLeverActivation.Play();
                }
                hasBeenActivated = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        

        active = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        

        colliderCount--;
        if (colliderCount <= 0)
        {
            DeactivateLever();
            if (!soundLeverDeactivation.isPlaying)
            {
                soundLeverDeactivation.Play();
            }
            hasBeenActivated = false;
        }
    }

    private void ActivateLever()
    {
        OnLeverActivatedServerRpc(true);
        active = true;
        //spriteRenderer.flipX = true;
        lever.transform.localScale = newScale;
        Debug.Log("Lever activaqtion");
        if (toShowPole != null)
        {
            toShowPole.SetActive(true);
        }

        if (toHidePole != null)
        {
            toHidePole.SetActive(false);
        }


    }

    private void DeactivateLever()
    {
        OnLeverActivatedServerRpc(false);
        active = false;
        //spriteRenderer.flipX = false;
        lever.transform.localScale = oldScale;
        if (toShowPole != null)
        {
            toShowPole.SetActive(false);
        }

        if (toHidePole != null)
        {
            toHidePole.SetActive(true);
        }



    }

}
