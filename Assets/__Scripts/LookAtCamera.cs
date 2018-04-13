using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    Transform target;
    Camera main;

    void Start()
    {
        main = Camera.main;
    }

    void Update()
    {
        // Rotate the credit every frame so it keeps looking at the camera
        transform.LookAt(main.transform);
    }
}
