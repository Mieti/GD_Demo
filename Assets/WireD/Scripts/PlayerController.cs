using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class PlayerKinematicMovement : MonoBehaviour
{
    //[SerializeField]
    private Rigidbody2D rb;

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
        //rb.velocity = new Vector2(movementVector.x * speed, movementVector.y * speed);
        //if (flag)
        //{
        //    float[] tensions = wc.MaxRopeTension();
        //    Debug.Log("Max Tension X: " + tensions[0].ToString() + " Max Tension Y: " + tensions[1].ToString());
        //}
        //float[] tension = wc.MaxRopeTension();
        //if (wc.RopeDistance(false)) // || tension[0] > maxTension || tension[1] > maxTension)
        //{
        //    if (rb.isKinematic)
        //    {
        //        //Debug.Log("FALSE: ropeDistance");
        //        //if (tension[0] > maxTension)
        //        //{
        //        //    Debug.Log("FALSE: tensionX, " + tension[0]);
        //        //}
        //        //if (tension[1] > maxTension)
        //        //{
        //        //    Debug.Log("FALSE: tensionY, " + tension[0]);
        //        //}
        //        //rb.isKinematic = false;
        //        //movementVector = Vector2.zero;
        //        //wc.AddSegmentIncremental(transform.position);
        //    }
        //}
        //else
        //{
        //    //if (!rb.isKinematic && movementVector != Vector2.zero)
        //    if (!rb.isKinematic)
        //    {
        //        Debug.Log("IsKinematic true");
        //        rb.isKinematic = true;
        //    }
        //}
        if(IsRewinding)
        {
            if(!wc.RopeDistance())
            {
                RewindRope();
            }
        }
        else if(!freeze)
        {

            rb.MovePosition(rb.position + (movementVector * speed * Time.fixedDeltaTime));
            AddSegment();
        }
        //if (IsLengthening)
        //{
        //    Debug.Log("IsLeghtening");
        //    wc.AddSegmentIncremental();
        //}
        //if (IsRewinding)
        //{
        //    RewindRope();
        //}
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

    private void AddSegment()
    {

        
        if (IsMoving)
        {
            if (wc.RopeDistance())
            {
                Vector3 currentPos = transform.position;
                //Debug.Log("currentPos: " + currentPos);
                //Debug.Log("TransformPoint: " + transform.TransformPoint(currentPos));
                //Debug.Log("Player is stuck due to the rope.");
                wc.AddSegmentIncremental();
            }
            //wc.AddSegmentIncremental(currentPos);
            //previousPosition = currentPos;
            //IsMoving = false;
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
        //if (wc.RopeTension(10) < minTension)
        //{
            wc.RemoveLastSegment();
        //}
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
