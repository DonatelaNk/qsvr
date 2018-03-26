using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour {

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Out of bounds");
    }
}
