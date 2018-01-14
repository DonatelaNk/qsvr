﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RenderHeads;

public class SceneController : MonoBehaviour {

    //skybox for vanity card and opening title seqience
    public Material VanitySkybox;
    //Skybox for episode 1
    public Material StorySkybox;
    //time in seconds to wait for the vanite card animation to complete
    public float VanityCardDelay;
    //360 video gameobject reference
    public GameObject SphereVideo;
    //Fade blindfold
    public GameObject Blindfold;


    //Vanity card game object
    private GameObject Fancy;
    //Corutine to change skybox blend (fade to balck)
    IEnumerator changeSkyboxBlendCoroutine;


    //Setup a listener to be alerted when the title sequence has compeleted
    private UnityAction VanityCardAnimationeComplete;
    //Setup a listener to be alerted when the title sequence has compeleted
    private UnityAction OpenningSequenceComplete;


    void Awake()
    {
        //Liste for the title sequence completion event to be fired
        //once it's fired, start the scene
        OpenningSequenceComplete = new UnityAction(StartScene);
    }
    void OnEnable()
    {
        EventManager.StartListening("TitlesAreDone", OpenningSequenceComplete);
    }
    void OnDisable ()
    {
        EventManager.StopListening("TitlesAreDone", OpenningSequenceComplete);
    }
    //Function triggered by listener once the title sequence is compeleted
    void StartScene()
    {
        //Blindfold user while we're activating all the game objects
        //Blindfold.SetActive(true);
        //Set the story skybox;
        RenderSettings.skybox = StorySkybox;
        //enable the 360 video
        SphereVideo.SetActive(true);

    

    }


    // Use this for initialization
    void Start () {
        //set vanity nebula skybox
        RenderSettings.skybox = VanitySkybox;
        RenderSettings.skybox.SetFloat("_Blend", 0);

        //start the change skybox blend coroutine
        //Get the Fade Delay value from the Fade Object in/out script attached to the Fancy game object
        Fancy = GameObject.Find("Fancy");
        changeSkyboxBlendCoroutine = changeSkyboxBlend(); // create an IEnumerator object
        StartCoroutine(changeSkyboxBlendCoroutine);

        //Hide things till they are needed
        SphereVideo.SetActive(false);
        Blindfold.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator changeSkyboxBlend() 
    {
        yield return new WaitForSeconds(VanityCardDelay);
        for (float f = 0f; f <= 1; f += 0.01f)
        {
            //change skybox blend
            RenderSettings.skybox.SetFloat("_Blend", f);     
            if (f > 0.99)
            {
                //once done, stop this coroutine and destroy the fancy vanity card gameobject
                StopCoroutine(changeSkyboxBlendCoroutine);
                Destroy(Fancy);
            }
            yield return null;
        }
    }
}
