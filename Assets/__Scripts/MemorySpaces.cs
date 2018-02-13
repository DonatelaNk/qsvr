using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;

    //Intantiate the photograph prefabs used in Memory Space 1
    public Rigidbody polaroid;
    public Transform m_parent;
    public Material[] PictureSet1;
    public Material[] PictureSet2;
    public Material[] PictureSet3;
    public Rigidbody Diary;  

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

            //Populate the box with a random set of potographs
            //Instantiate a polaroid prefab and assign materials from our random array set ( 1of 3)
            //TODO: Pick a random set
            Material[] randomSet = PictureSet1;
            foreach (Material photo in randomSet)
            {
                Rigidbody polaroidInstance;
                polaroidInstance = Instantiate(polaroid, m_parent.position, m_parent.rotation) as Rigidbody;
                //parent it
                polaroidInstance.transform.parent = m_parent;
                //assign our photo from the array to the front of polaroid which is the first child in prefab
                polaroidInstance.transform.GetChild(0).GetComponent<Renderer>().material = photo;
            }
        }

        //Second memory space logic
        if (memberSpaceNumber == 2)
        {
            //Instantiate the diary
            Rigidbody diaryInstance;
            diaryInstance = Instantiate(Diary, m_parent.position, m_parent.rotation) as Rigidbody;
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
        //destroy photographs from memory space 1
        foreach (Transform child in m_parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        EventManager.TriggerEvent(triggerLabel);
    }
}
