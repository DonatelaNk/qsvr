using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RenderHeads.Media.AVProVideo;


public class SceneController : MonoBehaviour {

    /************************************************/
    /******* cheatsheet for skipping around *********/
    /* Enter these seconds as the `Skip To` variable value to skip around the story
    /* See the `Scene Controller` script attached to the `SceneManager` gameobject
    /* First Memory Space:  230
     * Violent Outburst:    350
     * Second Memory Space: 435
     * End:                 600
     * 

    /******* //END **********************************/

    //Direction light (sun)
    public Light Sun;
    //skybox for vanity card and opening title sequence
    public Material VanitySkybox;
    //Skybox for the car scene (uses one of Unitys HDRIs textures)
    public Material StorySkybox;
    //variable that tells our script to trigger the memory spaces
    bool triggerMemorySpace = true;
    int memorySpaceCounter = 0;

    //Skybox for the memory spaces
    //public Material MemorySkybox;

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

    //time in seconds to wait for the vanity card animation to complete
    public float VanityCardDelay;

    //Prelude time audio intro over the title sequence
    public float PreludeDelay;
    
    //Debug vars
    public bool SkipIntro = false;
    //Where to skip to in the video and project timeline in seconds
    public float SkipTo = 0f;

    public bool LeapMotion;
    public bool OculusTouch;
    public GameObject OVR;
    public GameObject LM;
    private float originalCamera_x = 0.151f;
    private float originalCamera_y = 0.715f;
    private float originalCamera_z = 0.286f;


    //Vanity card game object
    private GameObject Fancy;
    //Titles game object
    private GameObject Titles;
    

    //Set up references for the 360 video
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

    //AudioSources
    public AudioSource[] SebastianVoiceovers;
    public AudioSource NonDialogTrack;
    bool NonDialogTrackSkipped = false;

    //Are we done?
    bool finished = false;

    //Set up listeners
    private UnityAction OpenningSequenceComplete;
    private UnityAction RedVideoIsLoaded;
    private UnityAction BlueVideoIsLoaded;
    private UnityAction SphereVideoIsLoaded;

    private UnityAction EnterMemorySpaceOne;
    private UnityAction ExitMemorySpaceOne;
    private UnityAction EnterMemorySpaceTwo;
    private UnityAction ExitMemorySpaceTwo;

    //*************** Time manager ********************/
    float ProjectTime = 0; // updated if we skip, otherwose equal to ActualTime
    //*************** //END ***************************/


    void Awake()
    {
        //choose controller
        if (LeapMotion)
        {
            Destroy(OVR);
        }
        else
        {
            Destroy(LM);
        }
        //Listen for the title sequence completion event to be fired
        //once it's fired, start the scene
        OpenningSequenceComplete = new UnityAction(StartScene);
        //Videos take up to a second to actual load, so these triggers
        //are fired when the videos are actually loaded
        RedVideoIsLoaded  = new UnityAction(RedVideoIsLoadedAndReady);
        BlueVideoIsLoaded = new UnityAction(BlueVideoIsLoadedAndReady);
        SphereVideoIsLoaded = new UnityAction(SphereVideoIsLoadedAndReady);

        //Memory spaces triggers
        EnterMemorySpaceOne = new UnityAction(StartEntryIntoMemorySpaceOne);
        ExitMemorySpaceOne = new UnityAction(StartExitFromMemorySpaceOne);

        EnterMemorySpaceTwo = new UnityAction(StartEntryIntoMemorySpaceTwo);
        ExitMemorySpaceTwo = new UnityAction(StartExitFromMemorySpaceTwo);

    }


    void OnEnable()
    {
        EventManager.StartListening("TitlesAreDone", OpenningSequenceComplete);
        EventManager.StartListening("RedVideoIsLoaded", RedVideoIsLoaded);
        EventManager.StartListening("BlueVideoIsLoaded", BlueVideoIsLoaded);
        EventManager.StartListening("SphereVideoIsLoaded", SphereVideoIsLoaded);

        EventManager.StartListening("EnterMemorySpaceOne", EnterMemorySpaceOne);
        EventManager.StartListening("ExitMemorySpaceOne", ExitMemorySpaceOne);
        EventManager.StartListening("EnterMemorySpaceTwo", EnterMemorySpaceTwo);
        EventManager.StartListening("ExitMemorySpaceTwo", ExitMemorySpaceTwo);
    }
    void OnDisable ()
    {
        EventManager.StopListening("TitlesAreDone", OpenningSequenceComplete);
        EventManager.StopListening("RedVideoIsLoaded", RedVideoIsLoaded);
        EventManager.StopListening("BlueVideoIsLoaded", BlueVideoIsLoaded);
        EventManager.StopListening("SphereVideoIsLoaded", SphereVideoIsLoaded);

        EventManager.StopListening("EnterMemorySpaceOne", EnterMemorySpaceOne);
        EventManager.StopListening("ExitMemorySpaceOne", ExitMemorySpaceOne);
        EventManager.StopListening("EnterMemorySpaceTwo", EnterMemorySpaceTwo);
        EventManager.StopListening("ExitMemorySpaceTwo", ExitMemorySpaceTwo);
    }

    /*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*/
    // Use this for initialization
    void Start()
    {
       
        Fancy = GameObject.Find("Fancy");
        Titles = GameObject.Find("Titles");
       
        //See if we need to skip intro and/or go to specific place
        //if not, run the project from the beginning
        if (!SkipIntro && SkipTo == 0f)
        {
            //set vanity nebula skybox
            RenderSettings.skybox = VanitySkybox;
            RenderSettings.skybox.SetFloat("_Blend", 0);

            //start the change skybox blend coroutine 
            StartCoroutine(changeSkyboxBlend());

            //Hide our gameobejects till they are needed

            SphereVideo.SetActive(false);
            Red.SetActive(false);
            Blue.SetActive(false);
            InteractiveObjects.SetActive(false);
            Car.SetActive(false);
        }
        else
        {
            //Then see if we also need to skip ahead
            Debug.Log("SkipTo = " + SkipTo);
            if (SkipTo > 0)
            {
                ProjectTime = ProjectTime + SkipTo;
            }
            //If Skip Intro option checked, go straight to the cady scene
            Destroy(Fancy);
            Titles.SetActive(false);
            EventManager.TriggerEvent("TitlesAreDone");          
        }

    }


    // ╔═════ ∘◦ These functions are triggered by our listeners ◦∘ ══════╗
    // ╚═════ ∘◦ ❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉❉ ◦∘ ══════╝

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
        //recenter the headset
        resetHeadsetPosition();

        //Remove user blindfold more gracefully (fade it out)
        StartCoroutine(removeBlindfold());
    }

    //360 video is ready to play
    void SphereVideoIsLoadedAndReady()
    {
        control360.Play();
        if (SkipTo > 0) { Skip360Video(); }
        EventManager.StopListening("SphereVideoIsLoaded", SphereVideoIsLoadedAndReady);
    }

    //Red (Ed/Driver) video is ready to play
    void RedVideoIsLoadedAndReady()
    {
        controlRed.Play();
        if (SkipTo > 0) { SkipRedVideo(); }
        EventManager.StopListening("RedVideoIsLoaded", RedVideoIsLoadedAndReady);
    }

    //Blue (MH/Passenger) video is ready to play
    void BlueVideoIsLoadedAndReady()
    {
        controlBlue.Play();
        if (SkipTo > 0) { SkipBlueVideo(); }
        EventManager.StopListening("BlueVideoIsLoadedAndReady", BlueVideoIsLoadedAndReady);
    }

    void StartEntryIntoMemorySpaceOne()
    {
        Debug.Log("Now entering memory space 1");
        GetComponent<MemorySpaces>().enterMemorySpace(1);
        EventManager.StopListening("EnterMemorySpaceOne", StartEntryIntoMemorySpaceOne);
    }
    void StartExitFromMemorySpaceOne()
    {
        triggerMemorySpace = true; //reset this var for 2nd memory sapce entry
        EventManager.StopListening("ExitMemorySpaceOne", StartExitFromMemorySpaceOne);
    }

    void StartEntryIntoMemorySpaceTwo()
    {
        Debug.Log("Now entering memory space 2");
        GetComponent<MemorySpaces>().enterMemorySpace(2);
        EventManager.StopListening("EnterMemorySpaceTwo", StartEntryIntoMemorySpaceTwo);
    }
    void StartExitFromMemorySpaceTwo()
    {
        EventManager.StopListening("ExitMemorySpaceTwo", StartExitFromMemorySpaceTwo);
    }



    //This function is called when the scene is fully faded at the end
    IEnumerator startWrapUp(float wait)
    {
        yield return new WaitForSeconds(wait);
        WrapUpScene();
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
        RenderSettings.skybox.SetFloat("_Blend", 1.0f);
        //show titles
        Titles.SetActive(true);
        //roll closing credits
        Titles.GetComponent<TitlesController>().RollClosingCredits();
    }

    //--<<O>>--<<O>>--<<O>>--<<O>>--<<O>>--<<O>>--<<O>>--<<O>>--
    //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    // END LISTENERS / TRIGGERS







    // Update is called once per frame
    void Update () {
        //we're not skipping, project time is always equal to actual time,
        //if we are skipping, then ProjectTime is set to the Skip seconds above
        ProjectTime = ProjectTime + Time.deltaTime;
        //Debug.Log(ProjectTime);

        

        //Hit space to reposition the player in backseat
        if (Input.GetKeyDown("space"))
        {
            resetHeadsetPosition();
        }
        
        //is our 360 video running? if it is, check to see if it's loaded yet,
        //only do this once
        if (VideoPlayer.activeSelf && !video360Loaded)
        {
            //Keeping it in the loop since it takes a few seconds to init the player
            //Get player controls
            mediaplayer360 = VideoPlayer.GetComponent<MediaPlayer>();
            control360 = mediaplayer360.Control;
            //if the video is already playing, skip to the desired position
            if (control360.CanPlay())
            {
                video360Loaded = true;
                EventManager.TriggerEvent("SphereVideoIsLoaded");
            }
        }
       

        //Reat for the Red and Blue videos
        if (Red.activeSelf && !videoRedLoaded)
        {
            mediaplayerRed = Red.GetComponent<MediaPlayer>();
            controlRed = mediaplayerRed.Control;
            if (controlRed.CanPlay())
            {
                videoRedLoaded = true;
                EventManager.TriggerEvent("RedVideoIsLoaded");
            }
        }
        if (Blue.activeSelf && !videoBlueLoaded)
        {
            mediaplayerBlue = Blue.GetComponent<MediaPlayer>();
            controlBlue = mediaplayerBlue.Control;
            if (controlBlue.CanPlay())
            {
                videoBlueLoaded = true;
                EventManager.TriggerEvent("BlueVideoIsLoaded");
            }
        }


        //Check out background track
        if (NonDialogTrack.isPlaying && !NonDialogTrackSkipped)
        {
            NonDialogTrackSkipped = true;
            //Now check if we need to fast forward it
            if (SkipTo > 0 || SkipIntro) {
                skipAudio(SkipTo+PreludeDelay);
                //incremement projectime by the prelude that we skipped
                //so our times for various triggers remain correct
                ProjectTime = ProjectTime + PreludeDelay;
            }
        }

        if (video360Loaded)
        {
            Debug.Log(control360.GetCurrentTimeMs());
            //if we skipped past first memory space update counter
            if (SkipTo > 231) { memorySpaceCounter = 1; }
            //Enter memory space 1 
            if (triggerMemorySpace && 
                memorySpaceCounter == 0 && 
                control360.GetCurrentTimeMs() > 231600 && 
                SkipTo < 231)
            {
                triggerMemorySpace = false;
                memorySpaceCounter = 1;
                EventManager.TriggerEvent("EnterMemorySpaceOne");
            }

            //Enter memory space 2
            if (triggerMemorySpace &&
                memorySpaceCounter == 1 &&
                control360.GetCurrentTimeMs() > 440000 &&
                SkipTo < 440)
            {
                triggerMemorySpace = false;
                memorySpaceCounter = 2;
                EventManager.TriggerEvent("EnterMemorySpaceTwo");
            }
        }

        


        //If the finished flag is not yet set and we've reached 720 seconds in the experience
        //Wrap up the scene
        if (!finished && ProjectTime > 720)
        {
            StartCoroutine(startWrapUp(8.0f));
            this.gameObject.GetComponent<Blindfold>().fadeInBlindFold();
            finished = true;
        }
    }


    //********** FUNCTION TO SKIP AROUND TIMELINE************/
    //*******************************************************//

    void Skip360Video() { Skip(control360); }
    void SkipRedVideo() { Skip(controlRed); }
    void SkipBlueVideo() { Skip(controlBlue); }

    void Skip(IMediaControl control)
    {
        //Skip to specific point in video
        control.Pause();
        control.SeekFast(SkipTo*1000);
        control.Play();
    }
    void skipAudio(float s)
    {
        //s=second to start player from in the background track
        NonDialogTrack.time = s;
    }

    void resetHeadsetPosition()
    {
        if (OculusTouch)
        {
            OVRManager.display.RecenterPose();
        }
        else
        {
            LM.transform.position = new Vector3(originalCamera_x, originalCamera_y, originalCamera_z);
        }
    }

    //******************* COROUTINES *************************/
    //*******************************************************//
    //*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*//
    IEnumerator changeSkyboxBlend() 
    {
        yield return new WaitForSeconds(VanityCardDelay);
        for (float f = 0f; f <= 1; f += 0.01f)
        {
            //change skybox blend
            RenderSettings.skybox.SetFloat("_Blend", f);     
            if (f > 0.99)
            {
                //once done, destroy the fancy vanity card gameobject
                Destroy(Fancy);
            }
            yield return null;
        }
    }
    //Courtine to remove the user blindfold with a 2 second delay
    IEnumerator removeBlindfold()
    {
        yield return new WaitForSeconds(4);
        GetComponent<Blindfold>().fadeOutBlindFold();
    }

}
