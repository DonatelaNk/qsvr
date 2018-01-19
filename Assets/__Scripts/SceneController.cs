using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class SceneController : MonoBehaviour {

    /************************************************/
    /******* cheatsheet for skipping around *********/
    /* Enter these milliseconds as the `Skip To` variable value to skip around the story
    /* See the `Scene Controller` script attached to the `SceneManager` gameobject
    /* First Memory Space:  230000
     * Violent Outburst:    360000
     * End:                 120000
     * 

    /******* //END **********************************/

    //skybox for vanity card and opening title sequence
    public Material VanitySkybox;
    //Skybox for the car scene (uses one of Unitys HDRIs textures)
    public Material StorySkybox;
    //variable that tells our script to trigger the memory spaces
    bool triggerMemorySpace = true;
    int memorySpaceCounter = 0;

    //Skybox for the memory spaces
    public Material MemorySkybox;

    //360 video gameobject reference
    public GameObject SphereVideo;
    //AVPRO video player
    public GameObject VideoPlayer;

    //Depthkit videos
    public GameObject Red;
    public GameObject Blue;

    //Interactive Objects
    public GameObject InteractiveObjects;

    //User blindfold (A sphere around user's head with inside/visible shader)
    public GameObject Blindfold;

    //Cady
    public GameObject Car;

    //time in seconds to wait for the vanite card animation to complete
    public float VanityCardDelay;
    
    //Debug vars
    public bool SkipIntro = false;
    //seconds to skip to in the video and project timeline in milliseconds
    public float SkipTo = 0f;


    //Vanity card game object
    private GameObject Fancy;
    //openingTitles game object
    private GameObject Titles;
    
    //Corutine to change skybox blend (fade to black)
    IEnumerator changeSkyboxBlendCoroutine;

    //Set up reference for the 360 video
    MediaPlayer mediaplayer360;
    IMediaControl control360;
    bool video360Loaded = false;

    //Set up references for the 2 depthkit videos we have
    MediaPlayer mediaplayerRed;
    IMediaControl controlRed;
    bool videoRedLoaded = false;

    MediaPlayer mediaplayerBlue;
    IMediaControl controlBlue;
    bool videoBlueLoaded = false;


    //Are we done?
    bool finished = false;
    IEnumerator startWrapUpCoroutine;



    //Set up listeners
    private UnityAction OpenningSequenceComplete;
    private UnityAction Skip360;
    private UnityAction SkipRed;
    private UnityAction SkipBlue;
    private UnityAction MemorySpace;
    private UnityAction WrapItUp;


    void Awake()
    {
        //Listen for the title sequence completion event to be fired
        //once it's fired, start the scene
        OpenningSequenceComplete = new UnityAction(StartScene);

        //Listen for memory space triggers
        //once it's fired, enter memory space
        MemorySpace = new UnityAction(EnterMemorySpace);

        //This listener waits for the assets to load before skipping to a particular point
        //in the experience (this is a debug feature, can be enbaled via the Skip To var found in the SceneController Script)
        if (SkipTo > 0)
        {
            Skip360 = new UnityAction(Skip360Video);
            SkipRed = new UnityAction(SkipRedVideo);
            SkipBlue = new UnityAction(SkipBlueVideo);
        }

        //Listen for the finish trigger
        WrapItUp = new UnityAction(WrapUpScene);

    }


    void OnEnable()
    {
        EventManager.StartListening("TitlesAreDone", OpenningSequenceComplete);
        
        if (SkipTo > 0)
        {
            EventManager.StartListening("Skip360", Skip360);
            EventManager.StartListening("SkipRed", SkipRed);
            EventManager.StartListening("SkipBlue", SkipBlue);
        }
        EventManager.StartListening("EnterMemorySpace", MemorySpace);
        EventManager.StartListening("SceneIsDone", WrapItUp);
    }
    void OnDisable ()
    {
        EventManager.StopListening("TitlesAreDone", OpenningSequenceComplete);
        if (SkipTo > 0)
        {
            EventManager.StopListening("Skip360", Skip360);
            EventManager.StopListening("SkipRed", SkipRed);
            EventManager.StopListening("SkipBlue", SkipBlue);
        }
        EventManager.StopListening("EnterMemorySpace", MemorySpace);
        EventManager.StopListening("SceneIsDone", WrapItUp);
    }

    void Skip360Video() { Skip(control360); }
    void SkipRedVideo() { Skip(controlRed); }
    void SkipBlueVideo() { Skip(controlBlue); }
    void Skip(IMediaControl control)
    {
        //Skip to specific point in video
        control.Pause();
        control.SeekFast(SkipTo);
        control.Play();
    }

    



    // Use this for initialization
    void Start()
    {
        Fancy = GameObject.Find("Fancy");
        Titles = GameObject.Find("Titles");
        //See if we need to skip intro and/or to specific place
        if (!SkipIntro && SkipTo == 0f)
        {
            //set vanity nebula skybox
            RenderSettings.skybox = VanitySkybox;
            RenderSettings.skybox.SetFloat("_Blend", 0);

            //start the change skybox blend coroutine
            //Get the Fade Delay value from the Fade Object in/out script attached to the Fancy game object   
            changeSkyboxBlendCoroutine = changeSkyboxBlend(); // create an IEnumerator object
            StartCoroutine(changeSkyboxBlendCoroutine);

            //Hide things till they are needed
            SphereVideo.SetActive(false);
            Red.SetActive(false);
            Blue.SetActive(false);
            InteractiveObjects.SetActive(false);
            Titles.SetActive(false);

            Car.SetActive(false);
        }
        else
        {
            //If Skip Intro option checked, go straight to the cady scene
            Destroy(Fancy);
            Titles.SetActive(false);
            StartScene();
        }

    }



    //Function triggered by listener once the title sequence is compeleted (or if we skipped it)
    void StartScene()
    {
        //Blindfold user while we're activating all the game objects
        GetComponent<Blindfold>().setBlindFold();
        //Set the story skybox;
        RenderSettings.skybox = StorySkybox;
        //enable the 360 video
        SphereVideo.SetActive(true);
        
        //Activate car
        Car.SetActive(true);

        //Activate Actors
        Red.SetActive(true);
        Blue.SetActive(true);

        //Activate Interactive Objects
        InteractiveObjects.SetActive(true);

        //TODO: Remove user blindfold more gracefully (fade it out)
        GetComponent<Blindfold>().fadeOutBlindFold();
    }


    
	
	// Update is called once per frame
	void Update () {
       
        if (!video360Loaded)
        {
            //Keeping it in the loop since it takes a few seconds to init the player
            mediaplayer360 = VideoPlayer.GetComponent<MediaPlayer>();
            control360 = mediaplayer360.Control;
            if (control360.IsPlaying())
            {
                video360Loaded = true;
                //Tell listener, video is ready to be scrubbed
                if(SkipTo > 0) {
                    EventManager.TriggerEvent("Skip360");
                } 
            }
        } else
        {
            Debug.Log(control360.GetCurrentTimeMs());

            //Enter first Memory space 
            if (triggerMemorySpace && control360.GetCurrentTimeMs() > 229300 && control360.GetCurrentTimeMs() < 232400)
            {
                triggerMemorySpace = false;
                memorySpaceCounter = 1;
                EventManager.TriggerEvent("EnterMemorySpace");
            }

            //Wrap up the scene
            if (!finished && control360.GetCurrentTimeMs() > 630000)
            {
                
                startWrapUpCoroutine = startWrapUp(8.0f);
                StartCoroutine(startWrapUpCoroutine);
                this.gameObject.GetComponent<Blindfold>().fadeInBlindFold();
                finished = true;
            }
            
        }

        if (!videoRedLoaded)
        {
            //Keeping it in the loop since it takes a few seconds to init the player
            mediaplayerRed = Red.GetComponent<MediaPlayer>();
            controlRed = mediaplayerRed.Control;
            if (controlRed.IsPlaying())
            {
                videoRedLoaded = true;
                //Tell listener, video is ready to be scrubbed
                if(SkipTo > 0)
                {
                    EventManager.TriggerEvent("SkipRed");
                }
            }
        }
        else
        {
            
        }

        if (!videoBlueLoaded)
        {
            //Keeping it in the loop since it takes a few seconds to init the player
            mediaplayerBlue = Blue.GetComponent<MediaPlayer>();
            controlBlue = mediaplayerBlue.Control;
            if (controlBlue.IsPlaying())
            {
                videoBlueLoaded = true;
                //Tell listener, video is ready to be scrubbed
                if (SkipTo > 0)
                {
                    EventManager.TriggerEvent("SkipBlue");
                }
            }
        }
        else
        {

        }


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

    /********************************************/
    /* Things to do at the end fo the experience*/

    IEnumerator startWrapUp(float wait)
    {

        yield return new WaitForSeconds(wait);
        EventManager.TriggerEvent("SceneIsDone");
        StopCoroutine(startWrapUpCoroutine);
        this.gameObject.GetComponent<Blindfold>().fadeOutBlindFold();

    }
    void WrapUpScene()
    {
        Red.SetActive(false);
        Blue.SetActive(false);
        Car.SetActive(false);
        SphereVideo.SetActive(false);
        //Remove interactiveobjects
        Destroy(InteractiveObjects);
        //reset skybox
        RenderSettings.skybox = VanitySkybox;
        //show titles
        Titles.SetActive(true);
        //roll closing credits
        Titles.GetComponent<TitlesController>().RollClosingCredits();
    }


    //initiate memory space
    void EnterMemorySpace()
    {
        //set blindfold to white, and fade it in
        //Now fade it in
        //Color bColor = new Color(1, 1, 1, 1);
        //GetComponent<Blindfold>().initBlindFold(bColor);
        //FADE DOESN"T WORK HERE!!!????
        GetComponent<Blindfold>().fadeInBlindFold();

        GetComponent<MemorySpaces>().enterMemorySpace();
    }
}
