using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerKinematicMovement : NetworkBehaviour
{
    //[SerializeField]
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider;

    Vector2 movementVector = Vector2.zero;
    public bool IsMoving { get; private set; }
    private bool IsRewinding = false;

    private bool isFacingRight = true;

    [SerializeField] private Animator animator;

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private WireController2D wc;
    [SerializeField]
    float retreatDistance = 5f;

    // to freeze the player movement, e.g. when attached to a plug
    public bool freeze = false;
    private float interactionRadius = 1.5f;
    private int interactableLayer;
    
    [SerializeField] private AudioSource audioFootstep;
    [SerializeField] private AudioSource audioRewindRope;

    private InGameUI ui;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        //playerSounds = GetComponentInChildren<Sounds>();
        rb.isKinematic = true;
        wc = GetComponentInParent<WireController2D>();

        if (animator == null){
            animator = GetComponent<Animator>();
        }
        interactableLayer = LayerMask.GetMask("Interactable");

    }

    void Start()
    {
        ui = GameObject.Find("Ingame UI").GetComponent<InGameUI>();
        ui.UpdateWC(wc);
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

    private void Flip()
    {
        /* if (movementVector.x > 0)
            spriteRenderer.flipX = false;
        else if (movementVector.x < 0)
            spriteRenderer.flipX = true; */
        if((movementVector.x>0 && !isFacingRight) ||(movementVector.x<0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
        
    }

    private void FixedUpdate()
    {
        if (wc == null)
        {
            wc = GetComponentInParent<WireController2D>();
        }
        
        if(!freeze)
        {
            Flip();
            if(IsRewinding)
            {
                animator.SetFloat("Horizontal", 0);
                animator.SetFloat("Speed", 0);
                RewindRope();
                playLoopAudioSource(audioRewindRope);
                
            }
            else
            {
                Move();
                stopAudioSource(audioRewindRope);
            }

        }
    }
    private void Move()
    {
        Flip();
        animator.SetFloat("Horizontal", Mathf.Abs(movementVector.x));
        animator.SetFloat("Speed", Mathf.Abs(movementVector.sqrMagnitude * speed));
        if(rb.isKinematic)
            {
                if (IsMoving)
                {
                    Vector2 newPosition = rb.position + (movementVector * speed * Time.fixedDeltaTime);
                    if (!IsColliding(newPosition))
                    {
                        rb.MovePosition(newPosition);
                        AddSegment();
                    }

                    playLoopAudioSource(audioFootstep);
                }
                else
                {
                    stopAudioSource(audioFootstep);
                }
            }
            else
            {
                // dynamic body -> I can use velocity
                rb.velocity = movementVector * speed;
                if(IsMoving)
                {
                    playLoopAudioSource(audioFootstep);
                }
                else
                {
                    stopAudioSource(audioFootstep);
                }
            }
    }

    private bool IsColliding(Vector2 newPosition)
    {
        var offset = playerCollider.offset;
        if (!isFacingRight)
        {
            offset.x = -offset.x;
        }
        var res = isFacingRight? 1: -1;
        Vector2 colliderCenter = newPosition + offset;
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
        /* if (context.performed)
        {
            IsLengthening = true;
        }
        else if (context.canceled)
        {
            IsLengthening = false;
        } */
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Check for nearby interactable objects
            Collider2D[] interactableColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
            foreach (var collider in interactableColliders)
            {
                Plug plug = collider.GetComponent<Plug>();
                if (plug != null)
                {
                    PutKinematic();
                    animator.SetFloat("Horizontal", 0);
                    animator.SetFloat("Speed", 0);
                    stopAudioSource(audioFootstep);
                    plug.Interact(this);
                }
            }
        }

    }

    private void AddSegment()
    {
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
    private void RewindRope()
    {
        if (rb.isKinematic)
        {
            if (!wc.RopeDistance(0.01f))
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
            // is dynamic, so: remove 1 segment + change back to Kinematic
            wc.RemoveLastSegment();
            PutKinematic();
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
    public void SetDirection(float xDirection)
    {
        if ((xDirection<0 && isFacingRight) || (xDirection>0 && !isFacingRight)){
            isFacingRight = !isFacingRight;
            Vector3 scale  = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    public void EnambleAnimation()
    {
        animator.SetFloat("Horizontal", 1);
        animator.SetFloat("Speed", speed);
        playLoopAudioSource(audioFootstep);
    }
    public void DisableAnimation()
    {
        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Speed", 0);
        stopAudioSource(audioFootstep);
    }

    private void playLoopAudioSource(AudioSource audio)
    {
        if (!audio.isPlaying)
            audio.Play();
    }

    private void stopAudioSource(AudioSource audio)
    {
        if (audio.isPlaying)
            audio.Stop();
    }

    private void PutKinematic()
    {
        wc.ResetJoints();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }

}
