using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class SetInitPlayerPosition : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
       NVRPlayer.Instance.transform.position = new Vector3(0, 0.34f, -0.19f);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
