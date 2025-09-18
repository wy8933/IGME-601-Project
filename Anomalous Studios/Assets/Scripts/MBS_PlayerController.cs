using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MBS_PlayerController : MonoBehaviour
{
    // Follow Camera
    public Camera playerCamera;
    // Camera Yaw
    public float lookPitchSpeed = 1.0f;
    public float lookPitchLimit = 60.0f; 
    private float rotationX = 0.0f;

    // Camera Lean Left/Right Variables
    private bool canLean = true;
    public Transform leanPivot;
    private float currentLean;   // Actual value of the lean
    private float targetLean;    // Will change as player pressed Q or E to lean
    public float leanAngle;     // Set the targetLean depending on leanAngle
    public float leanSmoothing; // Used to smooth the currentLean to targetLean
    private float leanVelocity;

    private bool isLeaningLeft;
    private bool isLeaningRight;

    // Movement Variables
    private CharacterController characterController;
    private bool canMove = true;
    static public float WALK_SPEED = 3.0f;
    static public float RUN_SPEED = 6.0f;
    private float walkSpeed = WALK_SPEED;
    private float runSpeed = RUN_SPEED;
    private Vector3 moveDirection = Vector3.zero;

    // Sprint Variables
    private bool canSprint = true;
    public float stamina = 100.0f;
    private float staminaDepletionFactor = 20.0f;
    private float staminaRegenFactor = 5.0f;

    // Jump Variables
    public float jumpForce = 3.0f;
    public float gravity = 9.8f;

    // Crouch Variables
    public float defaultHeight = 2.0f;
    public float crouchHeight = 1.0f;
    public float crouchSpeed = 2.0f;

    // Item Hotbar Variables
    public int selectedItemIndex = -1;
    private string[] itemHotbar = new string[4];

    // Journal Variables
    private bool inJournal = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Populate hotbar with temporary placeholder items
        itemHotbar[0] = "FlashLight";
        itemHotbar[1] = "Rusty Key";
        itemHotbar[2] = "Batteries";
        itemHotbar[3] = "Taco Cat";
    }

    // Update is called once per frame
    void Update()
    {
        Locomotion(Time.deltaTime);

        Interact();
        Use();
        SwitchItems();
        ToggleJournal();
        ScanInteractables();
    }

    void Locomotion(float dt)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && canSprint && leanPivot.localRotation == Quaternion.Euler(Vector3.zero) && !inJournal;
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        CheckSprint(isRunning, dt);
        Jump(movementDirectionY);
        ApplyGravityForce(dt);
        Crouch(dt);
        Move(dt);
        LeanLeftRight();
    }

    void CheckSprint(bool isRunning, float dt)
    {
        if (isRunning)
        {
            canLean = false;
            stamina -= staminaDepletionFactor * dt;

            if (stamina < 0)
            {
                canSprint = false;
            }
        }
        else
        {
            canLean = true;
            stamina += staminaRegenFactor * dt;

            if (stamina > 10.0f)
            {
                canSprint = true;
            }
        }

        // Clamp stamina between [0, 100]
        stamina = Mathf.Clamp(stamina, 0, 100);

        // Debug Logs
        Debug.Log("Stamina: " + stamina);
        Debug.Log("isRunning: " + isRunning);
    }

    void Move(float dt)
    {
        if(!inJournal)
        {
            characterController.Move(moveDirection * dt);

            // Rotate Camera on X and Y axes according to mouse movement to simulate orientation
            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookPitchSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookPitchLimit, lookPitchLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookPitchSpeed, 0);
            }
        }
    }

    void Crouch(float dt)
    {
        if (!inJournal)
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C) && canMove)
            {
                //float crouchTime = 0;
                //crouchTime += Time.deltaTime;
                //characterController.height = Mathf.Lerp(defaultHeight, crouchHeight, crouchTime);
                characterController.height = crouchHeight;
                walkSpeed = crouchSpeed;
                runSpeed = crouchSpeed;
            }
            else
            {
                //float crouchTime = 0;
                //crouchTime += Time.deltaTime;
                //characterController.height = Mathf.Lerp(crouchHeight, defaultHeight, crouchTime);
                characterController.height = defaultHeight;
                walkSpeed = WALK_SPEED;
                runSpeed = RUN_SPEED;
            }
        }
        
    }

    void Jump(float movementDirectionY)
    {
        if (!inJournal)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpForce;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }
        }
    }

    void ApplyGravityForce(float dt)
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * dt;
        }
    }

    void LeanLeftRight()
    {
        if (!inJournal)
        {
            if (canMove && canLean)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    if(!isLeaningLeft)
                    {
                        // Lean Left
                        targetLean = leanAngle;
                    }
                    else
                    {
                        // Reset leaning
                        targetLean = 0;
                    }

                    isLeaningLeft = !isLeaningLeft;
                    canSprint = false;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    if (!isLeaningRight)
                    {
                        // Lean Right
                        targetLean = -leanAngle;
                    }
                    else
                    {
                        // Reset leaning
                        targetLean = 0;
                    }

                    isLeaningRight = !isLeaningRight;
                    canSprint = false;
                }
                else
                {
                    // Reset leaning
                    targetLean = 0;

                    if (stamina > 10.0f)
                    {
                        canSprint = true;
                    }
                }

                currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothing);

                leanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
            }
        }
    }

    void Interact()
    {
        if (!inJournal)
        {
            if (Input.GetKeyDown(KeyCode.F) && IInteractable.Target != null)
            {
                IInteractable.Target.Interact();
            }
        }
    }

    void Use()
    {
        if (!inJournal)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(selectedItemIndex != -1 && itemHotbar[selectedItemIndex] != null)
                {
                    Debug.Log("Use Currently Selected {" + selectedItemIndex + "} item: " + itemHotbar[selectedItemIndex]);
                    itemHotbar[selectedItemIndex] = null;
                }
                else
                {
                    Debug.Log("No Item Selected!");
                }
            }
        }
    }

    void SwitchItems()
    {
        if (!inJournal)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedItemIndex = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedItemIndex = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedItemIndex = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedItemIndex = 3;
            }
        }
    }

    void ToggleJournal()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inJournal = !inJournal;

            if (inJournal)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    /// <summary>
    /// Finds objects within range that can be interacted with and highlights the item
    /// </summary>
    private void ScanInteractables()
    {
        // Scans the area for ANY colliders, not just interactables - allows walls to occlude items
        float interactRange = 5.0f;

        // Looks for an object, makes sure its an interactable, and that it is usable
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, 
            out RaycastHit hit, interactRange) && 
            hit.collider.TryGetComponent<IInteractable>(out IInteractable obj) &&
            obj.CanInteract)
        {
            IInteractable.Target = obj;
            IInteractable.Target.Highlight();
        }
        else
        {
            IInteractable.Target = null;
        }
    }
}
