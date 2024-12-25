using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Image StaminaBar;
    public Image HealthBar;

    public float Stamina, MaxStamina, Health, MaxHealth;

    public Camera playerCamera;
    public float walkSpeed = 9999f;
    public float runSpeed = 10f;
    public float jumpPower = 4f;
    public float gravity = 10f;

    public TextMeshProUGUI displayText;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;


    private Vector3 crouchScale = new Vector3(1, 0.65f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);


    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    private bool wasRunning = false;
    private bool canRun = true; // New variable to track running availability
    private float staminaDepletionTime = 0f; // Time when stamina reached 0

    public bool canMove = true;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        displayText.gameObject.SetActive(false);
    }

    void Update()
    {
        
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

        // Stamina bar colors
        // Blue
        if (Stamina > MaxStamina / 2){
            StaminaBar.color = new Color(0.337f, 0.643f, 0.819f);
        }
        // Yellow
        else if (Stamina <= MaxStamina / 2 && Stamina > 362) {
            StaminaBar.color = new Color(0.820f, 0.808f, 0.468f);
        }
        // Red
        else{
            StaminaBar.color = new Color(0.603f, 0.320f, 0.293f);
        }

        // Update stamina bar
        StaminaBar.fillAmount = Stamina / MaxStamina;

        #endregion

        #region Handles Health


        //hi jonny
        HealthBar.fillAmount = Health / MaxHealth;

        if (Health == 0){
            displayText.gameObject.SetActive(true);
        }
        #endregion

       
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the Player
        if (other.gameObject.tag == "enemy")
        {
            Health -= 1;
            // Add logic here (e.g., deal damage, trigger an event, etc.)
        }
    }

 
}
