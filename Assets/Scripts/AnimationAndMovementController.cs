using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // Variables to store player input values
    Vector2 currentMovementInput;
    Vector3 moveDirection;
    bool isMovementPressed;

    // Movement related 
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float rotationFactorPerSecond = 5f;

    // Other
    float groundedGravity = -0.05f;
    float fallGravity = -5f;



    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Assign input handling callbacks
        playerInput.CharacterControls.Run.started += onMovementInput;
        playerInput.CharacterControls.Run.canceled += onMovementInput;
        playerInput.CharacterControls.Run.performed += onMovementInput;
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        // x and z make up the ground plane axes
        moveDirection.x = currentMovementInput.x;
        moveDirection.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    // Update is called once per frame
    void Update()
    {
        handleGravity();
        handleMovement();
        handleRotation();
        handleAnimation();
    }

    void handleAnimation()
    {
        if (isMovementPressed) {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void handleGravity()
    {
        // Gravity needs to be accounted for manually
        if (characterController.isGrounded) {
            moveDirection.y = groundedGravity;   
        }
        else
        {
            moveDirection.y = fallGravity;
        }
    }

    void handleMovement()
    {
        characterController.Move(moveDirection * runSpeed * Time.deltaTime);
    }

    void handleRotation()
    {
        // The change in posiiton our character should point to
        Vector3 positionToLookAt = new Vector3(moveDirection.x, 0f, moveDirection.z);
        // The current rotation of our character
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            // Creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerSecond * Time.deltaTime);
        }

        
    }

    void  OnEnable()
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
