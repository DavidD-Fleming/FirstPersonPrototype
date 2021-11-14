using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // game objects
    [Header("Game Objects")]
    public Transform playerEntire;
    public Transform playerHead;
    public Transform playerBody;
    public CharacterController playerController;
    public Transform groundCheck;
    public Transform ceilingCheck;

    // public variables
    [Header("Base Variables")]
    public float speed;
    public float jumpHeight;
    public float terrainDistance;
    public LayerMask terrainMask;

    // private variables
    float mouseSensitivity;
    float gravity;
    float xRotation = 0f;
    bool isGrounded;
    bool isCeiling;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        // get mouseSensitivity, gravity
        mouseSensitivity = GameSystem.ReturnMouseSensitivity();
        gravity = GameSystem.ReturnGravity();
    }

    // Update is called once per frame
    void Update()
    {
        // simply checks if the player is grounded and resets velocity
        isGrounded = Physics.CheckSphere(groundCheck.position, terrainDistance, terrainMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // arbitrary negative constant to interact better with gravity
        }

        // checks if the player's head is touching the ceiling and resets velocity
        isCeiling = Physics.CheckSphere(ceilingCheck.position, terrainDistance, terrainMask);
        if (isCeiling && velocity.y > 0)
        {
            velocity.y = 0;
        }

        // takes mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        // and makes player look around
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // prevents looking backwards
        playerHead.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerEntire.Rotate(Vector3.up * mouseX);

        // takes arrowkeys/wasd as input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // and moves player around
        Vector3 traversalMovement = playerBody.right * x + playerBody.forward * z;
        playerController.Move(traversalMovement * speed * Time.deltaTime);

        // takes spacebar and converts to player jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // effects due to gravity
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }
}
