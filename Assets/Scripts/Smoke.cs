using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Smoke : MonoBehaviour
{
    // public declarations
    public float speed;
    public float expandedWidth;
    public float smokeDuration;
    public float decreasedSmokeGravityPercent;
    public LayerMask groundMask;

    // private variables
    float mouseSensitivity;
    float gravity;
    float downVelocity = 0f;
    float xInitialRotation;
    float yInitialRotation;
    float xCurrentRotation;
    float yCurrentRotation;
    bool smokesNotDown = true;
    bool notReleased = true;

    // Start is called before the first frame update
    void Start()
    {
        // get mouse sensitivity
        mouseSensitivity = GameSystem.ReturnMouseSensitivity();
        gravity = GameSystem.ReturnGravity();

        // initializes rotations
        xInitialRotation = transform.localRotation.eulerAngles.x;
        yInitialRotation = transform.localRotation.eulerAngles.y;
        xCurrentRotation = xInitialRotation;
        yCurrentRotation = yInitialRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // expand once smoke collides with terrain and starts self destruction, when player walks into smoke, activate smoke vision
        if (smokesNotDown && Physics.CheckSphere(transform.position, transform.localScale.x / 2, groundMask))
        {
            transform.localScale = new Vector3(expandedWidth, expandedWidth, expandedWidth);
            smokesNotDown = false;
            StartCoroutine(SelfDestruct());
        }

        // movement when not expanded
        if (smokesNotDown)
        {
            // checks when mouse3 is released
            if (Input.GetKeyUp(KeyCode.Mouse3))
            {
                notReleased = false;
            }

            // control smoke movement with mouse when mouse3 is not released. lets gravity do its thing when released
            if (Input.GetKey(KeyCode.Mouse3) && notReleased)
            {
                // takes mouse input
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                // and directs smoke
                xCurrentRotation -= mouseY;
                xCurrentRotation = Mathf.Clamp(xCurrentRotation, xInitialRotation - 90f, xInitialRotation + 90f); // prevents smoke from going backwards
                yCurrentRotation += mouseX;
                yCurrentRotation = Mathf.Clamp(yCurrentRotation, yInitialRotation - 90f, yInitialRotation + 90f); // prevents smoke from going backwards
                transform.localRotation = Quaternion.Euler(xCurrentRotation, yCurrentRotation, 0f);
            } else
            {
                downVelocity += gravity * decreasedSmokeGravityPercent * Time.deltaTime;
            }

            // smoke moves forward and down
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.Translate(Vector3.up * downVelocity * Time.deltaTime);
        }
    }

    IEnumerator SelfDestruct()
    {
        // wait smokeDuration seconds before destroying object
        yield return new WaitForSeconds(smokeDuration);
        Destroy(gameObject);
    }
}
