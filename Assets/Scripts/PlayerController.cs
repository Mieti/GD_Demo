using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 moveInput;
    Rigidbody rb;
    public bool isMoving { get; private set; }
    private bool isRewinding = false;
    private bool isLengthening = false;
    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private WireController wc;

    [SerializeField] float maxTension = 100f;
    [SerializeField] float minTension = 150f;

    [SerializeField] float stuckThreshold = 0.005f;

    [SerializeField] float retreatDistance = 5f;
    private Vector2 previousPosition;

    [SerializeField] private Animator animator;

    // used for freezing the player movement, e.g. when attached to plug
    public bool freeze = false;
    private float interactionRadius = 1.0f;
    [SerializeField] private LayerMask interactableLayer;

    
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start() 
    {
        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (wc == null)
        {
            Debug.LogError("WireController (wc) is not assigned.");
            return;
        }

        if(!freeze){
            rb.velocity = new Vector2(moveInput.x * walkSpeed, moveInput.y * walkSpeed);
        }
        AddSegment();
        if (isLengthening)
        {
            wc.AddSegmentIncremental(transform.position);
        }
        if (isRewinding)
        {
            RewindRope();
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
        
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);
        
        if (moveInput.x == 1 || moveInput.x == -1)
        {
            animator.SetFloat("LastMoveHorizontal", moveInput.x);
        }
        else if (moveInput.x == 0)
        {
            animator.SetFloat("Horizontal", animator.GetFloat("LastMoveHorizontal"));
        }
    }
    public void OnRewind(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isRewinding = true;
        }
        else if (context.canceled)
        {
            isRewinding = false;
        }
    }
    public void OnLengthen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isLengthening = true;
        }
        else if (context.canceled)
        {
            isLengthening = false;
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Check for nearby interactable objects
            Collider[] interactableColliders = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);

            foreach (var collider in interactableColliders)
            {
                Plug plug = collider.GetComponent<Plug>();
                if (plug != null)
                {
                    plug.Interact(this);
                }
            }
        }

    }

    private void AddSegment(){

        Vector3 currentPos=transform.position;
        if (isMoving)
        {
           /* if (CheckIfStuck())
            {
                Debug.Log("Player is stuck due to the rope.");
                wc.AddSegmentIncremental(currentPos);
            }*/
            previousPosition = currentPos;
        }
    }
    /*
    private bool CheckIfStuck()
    {
        Vector2 currentPosition = rb.position;
        float distanceMoved = Vector2.Distance(currentPosition, previousPosition);
        float avgT = wc.RopeTension(10);
        
        // If the distance moved is less than the threshold, consider the player stuck
        // stuck due to the rope if it's tight
        return distanceMoved < stuckThreshold && avgT>maxTension;
    }*/
    private void RewindRope(){
        if (wc.RopeTension(10) < minTension)
        {
            wc.RemoveLastSegment();
        }
    }
    public void Retreat(){
        wc.RemoveSegmentsRadius(retreatDistance);
    }

    public void SetWireController(WireController wireController)
    {
        wc = wireController;
    }
}
