using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    // Reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // Gravity
    float groundedGravity = -1f;
    float regularGravity = -9.8f;  // this value is overwritten. Determined by jump params
    float gravityFallMultiplier = 2.0f;

    // current movement, considering all 3 dimensions
    Vector3 currentMovement;

    // Plane (i.e non-jump) movement - variables to store player input values
    Vector2 currentPlaneMovementInput;
    Vector2 planeMovementDirection;
    bool isPlaneMovementPressed;
    bool isRunKeyHeld;

    // Plane movement related 
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float rotationFactorPerSecond = 5f;

    // Jump movement - variables to store player input values
    bool isJumpKeyHeld = false;

    // Jump movement related
    [SerializeField] float maxJumpHeight = 10.0f;
    [SerializeField] float maxJumpTime = 3f;
    float initialJumpVelocity;
    float previousYVelocity;
    bool isJumping = false;
    bool isJumpAnimating = false;

    // Performance variables - store optimized setter/getter parameter IDs
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;

    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");


        // Assign input handling callbacks
        playerInput.CharacterControls.Move.started += OnPlaneMovementInput;
        playerInput.CharacterControls.Move.canceled += OnPlaneMovementInput;
        playerInput.CharacterControls.Move.performed += OnPlaneMovementInput;

        playerInput.CharacterControls.Run.started += OnRunKeyInput;
        playerInput.CharacterControls.Run.canceled += OnRunKeyInput;

        playerInput.CharacterControls.Jump.started += OnJumpInput;
        playerInput.CharacterControls.Jump.canceled += OnJumpInput;

        // Jump setup
        SetupJumpVariables();
        Debug.Log("Initial jump velocity:" + initialJumpVelocity);
    }

    public void Start()
    {
        // Debug.Log("regular gravity:" + regularGravity);
        // We call the character controller's move at the very start with the groundedGravity
        // y velocity, otherwise the player is not considered grounded!
        currentMovement.x = 0f;
        currentMovement.y = groundedGravity;
        currentMovement.z = 0f;
        Move();
    }

    void OnPlaneMovementInput(InputAction.CallbackContext context)
    {
        currentPlaneMovementInput = context.ReadValue<Vector2>();
        planeMovementDirection.x = currentPlaneMovementInput.x;
        planeMovementDirection.y = currentPlaneMovementInput.y;
        isPlaneMovementPressed = currentPlaneMovementInput.x != 0 || currentPlaneMovementInput.y != 0;
    }

    void OnRunKeyInput(InputAction.CallbackContext context)
    {
        isRunKeyHeld = context.ReadValueAsButton();
    }

    void OnJumpInput(InputAction.CallbackContext context)
    {
        isJumpKeyHeld = context.ReadValueAsButton();
        if (isJumpKeyHeld)
        {
            Debug.Log("Space bar pressed.");
        }
        else
        {
            Debug.Log("Space bar released.");
        } 
    } 

    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        regularGravity = -2 * maxJumpHeight / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = 2 * maxJumpHeight / timeToApex;
    }

    // Update is called once per frame
    void Update()
    {
        // Question - do we move faster when walking diagonally?
        // Debug.Log("currentMovement: " + currentMovement);
        HandlePlaneMovement();
        HandleRotation();
        handleAnimation();

        // HandleJump must be called after handle gravity or we stay at ground as velocity 
        // set back to groundedGravity

        // ** If Move() is called after HandleGravity(), there is a bug where sometimes when you're
        //    on the ground and press spacebar, no jump occurs. This is since isGrounded is considered false. 
        //    REASON - the animations cause the player to move independent of the Move() call, and this causes
        //             the isGrounded property to toggle. If Move() is called and handleGravity() and HandleJump()
        //             occur the next frame, this toggling may lead to isGrounded being false and not being able to jump.
        //             The toggling can still occur in the setup below, but it seems to be so fast / unlikely that if
        //             never causes an issue.
        //             BUT because of this unreliability we should implement our own, more reliable isGrounded approach.
        //             *TODO* - Implement your own isGrounded approach, which should ideally be more reliable!
        //             Refer to this video of checking via Boxcast: https://www.youtube.com/watch?v=jxCVHBMdTWo
        Move();

        HandleGravity();
        HandleJump();
    }

    void handleAnimation()
    {
        if (isPlaneMovementPressed && isRunKeyHeld) {
            // For run animation to be triggered, the isWalking conditional must first be true
            // since we transition from walkin to running
            animator.SetBool(isWalkingHash, true);
            animator.SetBool(isRunningHash, true);
            //animator.SetBool("isWalking", false);
        }
        else if (isPlaneMovementPressed && !isRunKeyHeld)
        {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isWalkingHash, true);
        }
        else
        {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isWalkingHash, false);
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpKeyHeld;
        float appliedGravity = isFalling ? (regularGravity * gravityFallMultiplier) : regularGravity;

        // Gravity needs to be accounted for manually
        if (characterController.isGrounded) {
            // Set jump animation to stop here. Conditional check gives slight performance boost
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }

            currentMovement.y = groundedGravity;

            // This line doesn't matter but done for consistency
            previousYVelocity = groundedGravity;
        }
        else
        {
            // Debug.Log("Handling non gravity velocity");
            // 'Euler Integration' approach
            // Issue - trajectories are inconsistent depending on the framerate the game is running 
            // currentMovement.y += fallGravity * Time.deltaTime;

            // 'Velocity Verlet Integration' approach
            // -Frame rate independent!
            // TODO - better understand the differences between these two approaches
            // float previousYVelocity = currentMovement.y;
            float newYVelocity = previousYVelocity + (appliedGravity * Time.deltaTime);
            // optional - clamp y velocity so fall speed from large height doesn't increase past some point
            // float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) / 2, -20.0f);

            // Velocity applied to change character's position is the average of the previous and new Y velocity
            float nextYVelocity = (previousYVelocity + newYVelocity) / 2;
            currentMovement.y = nextYVelocity;
            
            // Previous y velocity is set to the new Y velocity, not the averaged one!
            previousYVelocity = newYVelocity;

        }
    }

    void HandlePlaneMovement()
    {
        // Plane (ground) movement is along the x and z axes
        float moveSpeed = isRunKeyHeld ? runSpeed : walkSpeed;
        currentMovement.x = planeMovementDirection.x * moveSpeed;
        currentMovement.z = planeMovementDirection.y * moveSpeed;
    }

    void HandleRotation()
    {
        // The change in posiiton our character should point to
        Vector3 positionToLookAt = new Vector3(planeMovementDirection.x, 0f, planeMovementDirection.y);
        // The current rotation of our character
        Quaternion currentRotation = transform.rotation;
        if (isPlaneMovementPressed)
        {
            // Creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerSecond * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        //Debug.Log("playerMovement.y: " + currentMovement.y );
        //if (isJumpKeyHeld)
        //{
        //    Debug.Log("isGrounded: " + characterController.isGrounded);
        //}
        //if (!isJumping && isJumpKeyHeld && !characterController.isGrounded)
        //{
        //    Debug.Log("Pressed jump but not on ground.");
        //}
        // Debug.Log("isJumpKeyHeld, isJumping, isGrounded: " + isJumpKeyHeld + isJumping + characterController.isGrounded);
        //Debug.Log("isJumpKeyHeld, isJumping, isGrounded, currentMovementY: " + isJumpKeyHeld + isJumping + characterController.isGrounded + " " +
        //        currentMovement.y);

        if (!isJumping && characterController.isGrounded && isJumpKeyHeld) {
            // Set jump animator here
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;

            // 'Euler Integration' approach - initial jump
            // currentMovement.y = initialJumpVelocity;

            // 'Velocity Verlet Integration' approach - treat initial y velocity as 0
            currentMovement.y = initialJumpVelocity / 2;

            // New previous YVelocity
            previousYVelocity = initialJumpVelocity;

        }
        // Only allow space bar held to lead to one jump
        else if (isJumping && characterController.isGrounded && !isJumpKeyHeld)
        {
            isJumping = false;
        }

        // Allow continouous jumps if spacebar is held
        //else if (isJumping && characterController.isGrounded)
        //{
        //    isJumping = false;
        //}
    }

    void Move()
    {
        // Debug.Log("isGrounded: " + characterController.isGrounded);
        // Debug.Log("current movement.y: " + currentMovement.y);
        characterController.Move(currentMovement * Time.deltaTime);
    }

    void OnEnable()
    {
        // Enable the character controls action map
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // Disable the character controls action map
        playerInput.CharacterControls.Disable();
    }
}
