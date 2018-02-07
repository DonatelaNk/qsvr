using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void enterMemorySpace()
    {
        //setupMemorySpaceCorutine = SetupMemorySpace(4.5f); // create an IEnumerator object
        //StartCoroutine(setupMemorySpaceCorutine);
        GetComponent<SceneController>().Sun.intensity = 11.0f;
        Debug.Log("Function hit");
    }

    public void hideSceneObjects()
    {
        GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Pause();
        
        GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Pause();

        GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Pause();
    }

    IEnumerator SetupMemorySpace(float wait)
    {

        yield return new WaitForSeconds(wait);
        hideSceneObjects();

    }
}
