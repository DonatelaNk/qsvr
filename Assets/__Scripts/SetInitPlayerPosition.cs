using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewtonVR;

public class SetInitPlayerPosition : MonoBehaviour {

    Vector3 carPosition;
    float x;
    float y;
    float z;
    GameObject Car;
    // Use this for initialization
    void Start () {
        //GameObject.Find("NVRPlayer").transform.position = new Vector3(0, -0.177f, 0.267f);
        Car = GameObject.Find("Car");
        //carPosition = Car.transform.position - NVRPlayer.Instance.Head.transform.position;
       // x = NVRPlayer.Instance.Head.transform.position.x + 0.09f;
        //y = NVRPlayer.Instance.Head.transform.position.y + 0.513f;
        //z = NVRPlayer.Instance.Head.transform.position.y + 0.427f;
       // Car.transform.position = new Vector3(x, y, z);
        //Debug.Log(carPosition);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
