using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;

public class ModifyRigidBody : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void pickup()
    {
        //GetComponent<Rigidbody>().useGravity = false;
        //GetComponent<Rigidbody>().isKinematic = true;
    }
    public void dropoff()
    {
        //GetComponent<Rigidbody>().useGravity = true;
        //GetComponent<Rigidbody>().isKinematic = false;
    }
}
