using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blindfold : MonoBehaviour {
    private static GameObject blindfold;

    void Start () {
        //  Get value from the SceneController
        blindfold = GameObject.Find("SceneManager").GetComponent<SceneController>().Blindfold;  
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
