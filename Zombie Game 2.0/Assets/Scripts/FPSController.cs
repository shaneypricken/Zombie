using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Image StaminaBar;
    public Image HealthBar;

    public float Stamina, MaxStamina, Health, MaxHealth;

    public Camera playerCamera;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 4f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float slideSpeed = 15f;  // Speed of sliding
    public float slideDuration = 1.5f;  // Duration of the slide
    private float slideTimer = 0f;  // Timer to track the slide duration
    private bool isSliding = false;  // Check if the player is currently sliding

<<<<<<< HEAD
    private Vector3 crouchScale = new Vector3(1, 0.65f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);
=======
    
>>>>>>> parent of 6c09d9e (crouch)

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    private bool wasRunning = false;
    private bool canRun = true; // New variable to track running availability
    private float staminaDepletionTime = 0f; // Time when stamina reached 0

    private bool canMantle = false; // To check if the player is close to a wall for mantling

    private float lastSpacePressTime = 0f; // For double space detection
    private float doubleTapTimeLimit = 0.3f; // Time window to detect double-tap
    private bool isMantling = false; // To check if the player is currently mantling

    public bool canMove = true;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        #region Handles Sliding

        // If Left Control key is held down, start sliding
        if (Input.GetKey(KeyCode.LeftControl) && !isSliding && characterController.isGrounded)
        {
            StartSliding();
        }

        // If sliding, update the slide timer and apply sliding
        if (isSliding)
        {
            slideTimer += Time.deltaTime;

            // Apply slide movement in the forward direction
            Vector3 slideDirection = transform.forward;  // Direction to slide in (forward)
            moveDirection = new Vector3(slideDirection.x * slideSpeed, moveDirection.y, slideDirection.z * slideSpeed);

            // Stop sliding after the duration has passed
            if (slideTimer >= slideDuration)
            {
                StopSliding();
            }
        }

        // Stop sliding when Left Control key is released
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopSliding();
        }

        #endregion

        #region Handles Movement

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Handle running logic
        bool isRunning = false;

        if (characterController.isGrounded)
        {
            if (canRun && Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
            {
                isRunning = true;
            }
        }

        // Calculate movement speed based on whether the player is running or walking
        else
        {
            if (wasRunning && canRun && Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
            {
                isRunning = true;
            }
        }

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping

        // Jumping logic
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        #region Handles Stamina

        if (isRunning)
        {
            Stamina -= 1;
            if (Stamina <= 0)
            {
                Stamina = 0;
                isRunning = false; // Stop running when stamina is 0
                canRun = false; // Prevent running
                staminaDepletionTime = Time.time; // Record time of stamina depletion
            }
        }
        else
        {
            Stamina += 1;
            if (Stamina > MaxStamina)
            {
                Stamina = MaxStamina;
            }

            // Allow running again after 1 second
            if (!canRun && Time.time - staminaDepletionTime >= 1f)
            {
                canRun = true;
            }
        }

        // Update stamina bar
        StaminaBar.fillAmount = Stamina / MaxStamina;

        #endregion

        #region Handles Health
        HealthBar.fillAmount = Health / MaxHealth;
        #endregion

<<<<<<< HEAD

        #region Mantle Detection

        // Detect double-tap of spacebar to trigger mantle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastSpacePressTime <= doubleTapTimeLimit && canMantle)
            {
                StartMantling();
            }
            lastSpacePressTime = Time.time;
        }

        #endregion
=======
        
>>>>>>> parent of 6c09d9e (crouch)
    }

    #region Mantle Functions

    void StartMantling()
    {
        if (!isMantling)
        {
            isMantling = true;
            // Apply an upward force to simulate the player climbing
            moveDirection.y = jumpPower * 2;  // Mantling with a higher jump power
            StartCoroutine(MantleCooldown()); // Wait for the cooldown to finish mantling
        }
    }

    IEnumerator MantleCooldown()
    {
        // Disable movement temporarily while mantling
        canMove = false;
        yield return new WaitForSeconds(0.5f); // Mantle action duration
        canMove = true;
        isMantling = false;
    }

    #endregion

    #region Wall Detection

    // Raycast to detect if the player is near a wall
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            canMantle = true;
        }
    }

    void OnControllerColliderExit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Wall"))
        {
            canMantle = false;
        }
    }

    #endregion

    #region Sliding Functions

    // Start the sliding
    void StartSliding()
    {
        isSliding = true;
        slideTimer = 0f;  // Reset the slide timer
    }

    // Stop the sliding
    void StopSliding()
    {
        isSliding = false;
        moveDirection = Vector3.zero;  // Stop sliding by setting velocity to zero
    }

    #endregion
}
