using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // !!!manipulate slope offset to influence going up stairs

    // public gameobjects
    public float mouseSensitivity;
    public Transform playerBody;
    public CharacterController playerController;
    public Transform groundCheck;
    public Transform ceilingCheck;

    // movement variables
    public float speed;
    public float gravity = -9.81f;
    public float jumpHeight;
    public float groundDistance = 0.4f; // for gravity interaction
    public LayerMask groundMask; // for gravity interaction
    bool isGrounded; // for gravity interaction
    bool isCeiling;
    float xRotation = 0f;
    Vector3 velocity;


    // Start is called before the first frame update
    void Start()
    {
        // locks mouse into first person perspective
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // simply checks if the player is grounded and resets velocity
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // arbitrary negative constant to interact better with gravity
        }

        // checks if the player's head is touching the ceiling and resets velocity
        isCeiling = Physics.CheckSphere(ceilingCheck.position, groundDistance, groundMask);
        if (isCeiling && velocity.y > 0)
        {
            velocity.y = 0;
        }

        // takes mouse input and converts to first person camera movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        // prevents player from looking backwards
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // takes arrowkeys/wasd as input and converts to player traversal movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 traversalMovement = transform.right * x + transform.forward * z;
        playerController.Move(traversalMovement * speed * Time.deltaTime);

        // takes spacebar and converts to player jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // effects due to gravity
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }
}
