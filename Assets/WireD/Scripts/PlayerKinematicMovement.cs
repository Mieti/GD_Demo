using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerKinematicMovement : MonoBehaviour
{
    //[SerializeField]
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    Vector2 movementVector = Vector2.zero;
    public bool IsMoving { get; private set; }
    private bool IsRewinding = false;
    private bool IsLengthening = false;

    //[SerializeField]
    //private Animator animator;
    //[SerializeField]
    //private SpriteRenderer spriteRenderer;

    //[SerializeField]
    //bool isGrounded = false;

    //bool isGroundedCheckStop = false;

    //public Sounds playerSopunds;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private WireController2D wc;
    [SerializeField]
    float maxTension = 1.5f;
    [SerializeField]
    float minTension = 1f;
    //[SerializeField]
    //float stuckThreshold = 0.005f;
    [SerializeField]
    float retreatDistance = 5f;

    // to freeze the player movement, e.g. when attached to a plug
    public bool freeze = false;
    private float interactionRadius = 1.0f;
    [SerializeField] private LayerMask interactableLayer;

    //[SerializeField]
    //private Animator animator;
    //[SerializeField]
    private SpriteRenderer spriteRenderer;

    //[SerializeField]
    //bool isGrounded = false;

    //bool isGroundedCheckStop = false;
    //public Sounds playerSopunds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //playerSounds = GetComponentInChildren<Sounds>();
        rb.isKinematic = true;
        wc = GetComponentInParent<WireController2D>();
    }

    void Start()
    {
        
    }

    /* private void Update()
    {

        {
            movementVector.x = Input.GetAxis("Horizontal");
            movementVector.y = Input.GetAxis("Vertical");
            IsMoving = movementVector != Vector2.zero;
            OnRewind();
            OnLengthen();
        }
    } */

    private void HandleMovementDirectionSpriteFlip()
    {
        if (movementVector.x > 0)
            spriteRenderer.flipX = false;
        else if (movementVector.x < 0)
            spriteRenderer.flipX = true;
    }

    private void FixedUpdate()
    {
        if (wc == null)
        {
            wc = GetComponentInParent<WireController2D>();
        }
        
        if(!freeze)
        {
            if(IsRewinding)
            {
                RewindRope();
            }
            else if(rb.isKinematic)
            {
                if (IsMoving)
                {
                    Vector2 newPosition = rb.position + (movementVector * speed * Time.fixedDeltaTime);
                    if (!IsColliding(newPosition))
                    {
                        rb.MovePosition(newPosition);
                        AddSegment();
                    }  
                }  
            }
            else
            {
                // dynamic body -> I can use velocity
                rb.velocity = movementVector * speed;
            }

        }
    }

    private bool IsColliding(Vector2 newPosition)
    {
        Vector2 colliderCenter = newPosition + playerCollider.offset;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(colliderCenter, playerCollider.size, 0);
        foreach (var hitCollider in hitColliders)
        {
            //ignores itself, any trigger collider and the wire segments
            if (hitCollider != playerCollider  && !hitCollider.isTrigger && hitCollider.gameObject.layer != LayerMask.NameToLayer("Wire"))
            {
                return true;
            }
        }
        return false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        movementVector = context.ReadValue<Vector2>();
        IsMoving = movementVector != Vector2.zero;
        
    }
    public void OnRewind(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsRewinding = true;
        }
        else if (context.canceled)
        {
            IsRewinding = false;
        }
    }
    public void OnLengthen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsLengthening = true;
        }
        else if (context.canceled)
        {
            IsLengthening = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            int layerMask = LayerMask.GetMask("Interactable");
            // Check for nearby interactable objects
            Collider2D[] interactableColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, layerMask);
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

    private void AddSegment()
    {
        /* if (IsMoving)
        {
            if (wc.IsMaxLen()){
                if (rb.isKinematic)
                {
                    wc.ChangeJoints();
                    rb.isKinematic = false;
                    rb.mass = wc.RopeMass();
                }
            }
            else if (wc.RopeDistance())
            {
                wc.AddSegmentIncremental();
            }
        } */
        if (wc.RopeDistance(0.1f))
        {
            wc.AddSegmentIncremental();
        }
        // if max len is reached -> player to dynamic
        if (wc.IsMaxLen()){
            if (rb.isKinematic)
            {
                wc.ChangeJoints();
                rb.isKinematic = false;
                rb.mass = wc.RopeMass();
            }
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
    private void RewindRope()
    {
        if (rb.isKinematic)
        {
            if (!wc.RopeDistance(0))
            {
                wc.RemoveLastSegment();
                // if (!rb.isKinematic && !wc.IsMaxLen()){
                //     wc.ResetJoints();
                //     rb.isKinematic = true;
                // }
            }
        }
        else
        {
            // is dynamic, so: remove 1 segment + change back to dynamic
            wc.RemoveLastSegment();
            wc.ResetJoints();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }
        
    }
    public void Retreat()
    {
        wc.RemoveSegmentsRadius(retreatDistance);
    }

    public void SetWireController(WireController2D wireController)
    {
        wc = wireController;
    }


}
