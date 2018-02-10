using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;
    
    
    /*
    Logic for each memory space goes into this function
    */
    public void MemorySpaceIsReady(float memberSpaceNumber)
    {
        
        //First memory space logic
        if (memberSpaceNumber == 1)
        {
            //PauseVideos();
            //TODO: This is temp
            StartCoroutine(ExitMemorySpace(25.5f, "ExitMemorySpaceOne"));
        }

        //Second memory space logic
        if (memberSpaceNumber == 2)
        {
            GetComponent<SoundManager>().initVoiceover();
            //TODO: This is temp
            StartCoroutine(ExitMemorySpace(25.5f, "ExitMemorySpaceTwo"));
        }
    }
    //~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~




    public void enterMemorySpace(float memberSpaceNumber)
    {
        setupMemorySpaceCorutine = SetupMemorySpace(4.5f, memberSpaceNumber); // create an IEnumerator object
        StartCoroutine(setupMemorySpaceCorutine);
    }

    public void PauseVideos()
    {
        GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Pause();   
        GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Pause();
        GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Pause();
    }

    IEnumerator SetupMemorySpace(float wait, float memberSpaceNumber)
    {
        yield return new WaitForSeconds(wait);
        MemorySpaceIsReady(memberSpaceNumber);
        StopCoroutine(setupMemorySpaceCorutine);
    }

    IEnumerator ExitMemorySpace(float wait, string triggerLabel)
    {
        yield return new WaitForSeconds(wait);
        EventManager.TriggerEvent(triggerLabel);
    }
}
