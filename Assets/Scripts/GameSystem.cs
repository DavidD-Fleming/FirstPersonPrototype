using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    static float gravity = -22f;
    static float mouseSensitivity = 300f;

    // Start is called before the first frame update
    void Start()
    {
        // locks mouse into first person perspective
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static float ReturnMouseSensitivity()
    {
        return mouseSensitivity;
    }

    public static float ReturnGravity()
    {
        return gravity;
    }
}
