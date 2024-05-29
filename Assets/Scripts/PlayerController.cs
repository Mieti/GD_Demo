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
    public bool isRewinding;
    [SerializeField] private float walkSpeed = 5f;

    [SerializeField] private WireController wc;

    [SerializeField] float maxTension = 200f;
    [SerializeField] float minTension = 150f;

    [SerializeField] float stuckThreshold = 0.005f;
    private Vector2 previousPosition;

    private bool notPressed = true;

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
        rb.velocity = new Vector2(moveInput.x * walkSpeed, moveInput.y * walkSpeed);
        AddSegment();
        if (isRewinding){
            RewindRope();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
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

    private void AddSegment(){

        Vector3 currentPos=transform.position;
        if (isMoving)
        {
            if (CheckIfStuck())
            {
                Debug.Log("Player is stuck due to the rope.");
                wc.AddSegmentIncremental(currentPos);
            }
            previousPosition = currentPos;
        }
    }
    private bool CheckIfStuck()
    {
        Vector2 currentPosition = rb.position;
        float distanceMoved = Vector2.Distance(currentPosition, previousPosition);
        float avgT = wc.RopeTension(10);
        
        // If the distance moved is less than the threshold, consider the player stuck
        // stuck due to the rope if it's tight
        return distanceMoved < stuckThreshold && avgT>maxTension;
    }
    private void RewindRope(){
        if (wc.RopeTension(10) < minTension)
        {
            wc.RemoveLastSegment();
        }
    }
}
