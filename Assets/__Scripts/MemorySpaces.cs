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
        RenderSettings.skybox = GetComponent<SceneController>().MemorySkybox;
        setupMemorySpaceCorutine = SetupMemorySpace(4.5f); // create an IEnumerator object
        StartCoroutine(setupMemorySpaceCorutine);
    }

    public void hideSceneObjects()
    {
        GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Pause();
        GetComponent<SceneController>().SphereVideo.SetActive(false);

        GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Pause();
        GetComponent<SceneController>().Red.SetActive(false);

        GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Pause();
        GetComponent<SceneController>().Blue.SetActive(false);

        GetComponent<SceneController>().Car.SetActive(false);
    }

    IEnumerator SetupMemorySpace(float wait)
    {

        yield return new WaitForSeconds(wait);
        hideSceneObjects();
        GetComponent<Blindfold>().fadeOutBlindFold();

    }
}
