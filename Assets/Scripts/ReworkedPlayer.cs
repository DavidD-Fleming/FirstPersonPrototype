using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReworkedPlayer : MonoBehaviour
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

    [Header("Jett Declarations")]
    // public declarations (JETT)
    public GameObject smokePrefab;
    public GameObject smokeStart;
    public GameObject smokeVision;
    public LayerMask smokeMask;
    public GameObject knifePrefab;
    public GameObject knivesLookAt;
    public GameObject[] knivesPositions;
    public float decreasedGravityPercent; // 0.9 (standard)
    public float updraftHeight; // 5 (standard)
    public float dashSpeed; // 25 (standard)
    public float dashDuration; // 0.2 (standard)
    public float knifeSpeed;
    // private declarations (JETT)
    GameObject[] knives;

    // Start is called before the first frame update
    void Start()
    {
        // get mouseSensitivity, gravity
        mouseSensitivity = GameSystem.ReturnMouseSensitivity();
        gravity = GameSystem.ReturnGravity();

        // set knives array to same size of knives position array
        knives = new GameObject[knivesPositions.Length];
    }

    // Update is called once per frame
    void Update()
    {
        // BASE PLAYER MOVEMENT
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

        // **************************************************JETT ABILITIES*********************************************
        // drift
        if (Input.GetButton("Jump") && velocity.y <= 0)
        {
            velocity.y -= gravity * decreasedGravityPercent * Time.deltaTime;
        }

        // updraft
        if (Input.GetKeyDown(KeyCode.Q))
        {
            velocity.y += Mathf.Sqrt(updraftHeight * -2f * gravity);
        }

        // tailwind
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Dash(traversalMovement));
        }

        // cloudburst
        if (Input.GetKeyDown(KeyCode.Mouse3))
        {
            // adds values to playerHead rotation to make it aimed at the middle (better solutions but this is a band-aid fix) then smokes out
            Quaternion smokeRotation = Quaternion.Euler(playerHead.transform.rotation.eulerAngles.x - 1.5f, playerHead.transform.rotation.eulerAngles.y - 1.3f, 0f);
            GameObject newSmoke = (GameObject)Instantiate(smokePrefab, smokeStart.transform.position, smokeRotation);

            // smoke is walk through-able
            Physics.IgnoreCollision(newSmoke.transform.GetComponent<Collider>(), GetComponent<Collider>());
        }
        // smoke interaction
        if (Physics.CheckSphere(ceilingCheck.position, 0.5f, smokeMask))
        {
            smokeVision.SetActive(true);
        } else
        {
            smokeVision.SetActive(false);
        }

        // ultimate
        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < knivesPositions.Length; i++)
            {
                // the better solution for knives angles
                Quaternion knifeRotation = Quaternion.LookRotation(knivesLookAt.transform.position - knivesPositions[i].transform.position) * Quaternion.Euler(90f, 0f, 0f);
                //Quaternion knifeRotation = Quaternion.Euler(playerHead.transform.rotation.eulerAngles.x + 90f, playerHead.transform.rotation.eulerAngles.y, 0f);

                // checks if there is a pre-existing knife, destroys object if there is. create knives
                if (knives[i] != null)
                {
                    Destroy(knives[i]);
                }
                knives[i] = (GameObject)Instantiate(knifePrefab, knivesPositions[i].transform.position, knifeRotation, transform);
                // set priority for left click
                Knife knifeScript = knives[i].GetComponent<Knife>();
                knifeScript.SetPriority(i);
                knifeScript.SetKnivesLookAt(knivesLookAt);
            }
        }
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^JETT ABILITIES^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

        // effects due to gravity (ALSO BASE PLAYER MOVEMENT)
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }

    // **************************************************JETT ABILITIES*********************************************
    private IEnumerator Dash(Vector3 dashingMovement)
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashDuration)
        {
            playerController.Move(dashingMovement * dashSpeed * Time.deltaTime);
            if (dashingMovement == new Vector3(0, 0, 0))
            {
                playerController.Move(playerBody.forward * dashSpeed * Time.deltaTime);
            }
            yield return null;
        }
    }
    // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^JETT ABILITIES^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
}
