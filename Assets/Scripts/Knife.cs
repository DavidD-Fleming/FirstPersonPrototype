using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    // public declarations
    public float speed;
    public LayerMask groundMask;

    // private declarations
    float mouseSensitivity;
    GameObject knivesLookingObject;
    bool knifeGo = false;
    int currentPriority = 0;
    int priority;

    // Start is called before the first frame update
    void Start()
    {
        // get mouse sensitivity
        mouseSensitivity = GameSystem.ReturnMouseSensitivity();
    }

    // Update is called once per frame
    void Update()
    {
        // left click (each knife is given a priority number, when priority matches their number -> they go
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentPriority == priority)
            {
                // rotates knife to pointed direction and release
                transform.parent = null;
                transform.localRotation = Quaternion.LookRotation(knivesLookingObject.transform.position - transform.position) * Quaternion.Euler(90f, 0f, 0f);
                knifeGo = true;
            }
            currentPriority++;
        }

        // right click
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // rotates knife to pointed direction and release
            transform.parent = null;
            transform.localRotation = Quaternion.LookRotation(knivesLookingObject.transform.position - transform.position) * Quaternion.Euler(90f, 0f, 0f);
            knifeGo = true;
        }

        // knife moves when thrown
        if (knifeGo)
        {
            // destroy knife once it hits terrain
            if (Physics.CheckSphere(transform.position, transform.localScale.x / 2, groundMask))
            {
                Destroy(gameObject);
            }
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    public void SetPriority(int givenPriority)
    {
        priority = givenPriority;
    }

    public void SetKnivesLookAt(GameObject knivesLookAt)
    {
        knivesLookingObject = knivesLookAt; 
    }
}
