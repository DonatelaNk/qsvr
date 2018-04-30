using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //restart scene
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene("Episode.1");
        }
        //Hit Escape to exit (build mode only)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
