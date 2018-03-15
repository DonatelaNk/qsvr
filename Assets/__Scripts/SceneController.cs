using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using System.Linq;
using Leap.Unity;


public class SceneController : MonoBehaviour {

    //Sound trigger times
    //MOVED to TriggerDictionary.cs
    //Sound trigger times
    float SkipTo = 0;
    /*private float Car01StartTime = 0f;
    private float RadioStartTime = 56.0f; //based on RED clip
                                          //Ed rolls the window down
    private float EdRollsWindowTime = 229.5f;
    private float Memory01StartTime = 227.0f; //Based on the 360 video
    private float Car02StartTime = 258.0f; //Based on RED clip
    private float Memory02StartTime = 437.0f; //Bassed on 360 clip
    private float Car03StartTime = 473.0f; // Based RED Clip
                                           //Shift into park and turn off engine
    private float ShiftKeyStartTime = 552.0f;
    //Fly and Hit
    private float FlyBiStartTime = 612.25f;
    private float FlyHitStartTime = 612.25f;
    private float FinaleStartTime = 638.0f; // Based RED Clip */

    //Direction light (sun)
    public Light Sun;
    //skybox for vanity card and opening title sequence
    public Material VanitySkybox;
    //Skybox for the car scene (uses one of Unitys HDRIs textures)
    public Material StorySkybox;
    //variable that tells our script to trigger the memory spaces
    bool triggerMemorySpace = true;
    public GameObject MemoryDust;
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

    //Cady
    public GameObject Car;

    //time in seconds to wait for the vanity card onscreen
    public float VanityCardDelay;

    // Set our skipping options
    public enum EnumeratedSkipPoints
    {
		Prelude, CarOne, FirstMemorySpace, CarTwo, SecondMemorySpace, CarThree
    }
    public EnumeratedSkipPoints StartAt;

    public bool LeapMotion;
    public bool OculusTouch;
    public GameObject OVR;
    public GameObject LM;
    private float originalCamera_x;
    private float originalCamera_y;
    private float originalCamera_z;


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

    //var to make sure all three videos and scene 1 sounds start in sync
    //it hold everything from starting until all videos and sounds are ready to play
    bool sync = false;

    //CarScene 2 and 3 trigger bools
    // MOVED TO TriggersDictionary.cs
    /* private bool CarScene03Triggered = false;

     // MOVED TO TriggersDictionary.cs
     //Shift Keys Fly Hit
     private bool ShiftKeyTriggered = false;
     private bool FlyBiTriggered = false;
     private bool FlyHitTriggered = false; */


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
        //hide memory dust
        MemoryDust.SetActive(false);
        //choose controller
        if (LeapMotion)
        {
            //Destroy(OVR);
            OVR.SetActive(false);
            //remember where the LMRig is positioned
            originalCamera_x = LM.transform.GetChild(0).transform.position.x;
            originalCamera_y = LM.transform.GetChild(0).transform.position.y;
            originalCamera_z = LM.transform.GetChild(0).transform.position.z;
            Debug.Log("originalCamera_x: " + originalCamera_x);
        }
        else
        {
            //Destroy(LM);
            LM.SetActive(false);
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
        if (StartAt == EnumeratedSkipPoints.Prelude)
        {
            //set vanity nebula skybox
            RenderSettings.skybox = VanitySkybox;
            RenderSettings.skybox.SetFloat("_Blend", 0);

            //start the change skybox blend coroutine 
            StartCoroutine(ChangeSkyboxBlend());

            //Hide our gameobjects till they are needed

            SphereVideo.SetActive(false);
            Red.SetActive(false);
            Blue.SetActive(false);
            InteractiveObjects.SetActive(false);
            Car.SetActive(false);

            //Trigger the prelude sound track
            GetComponent<SoundManager>().StartPrelude();

        }
        else
        {   
            //If Skiping, go straight to selected scene
            Destroy(Fancy);
            Titles.SetActive(false);
            //TitlesAreDone event triggers the StartScene function
            EventManager.TriggerEvent("TitlesAreDone");          
        }

        //Cancel out some stuff based on where we skipped to
        //Loop thru triggers and set to true the ones we need to skip over
        //See where we're skiping
        /*if (StartAt == EnumeratedSkipPoints.FirstMemorySpace)
        {
            radioTriggered = true;
        }
        else if (StartAt == EnumeratedSkipPoints.CarTwo)
        {
            radioTriggered = true;
            memorySpaceCounter = 1;
        }
        else if (StartAt == EnumeratedSkipPoints.SecondMemorySpace)
        {
            radioTriggered = true;
            CarScene02Triggered = true;
            memorySpaceCounter = 1;
        }
        else if (StartAt == EnumeratedSkipPoints.CarThree)
        {
            radioTriggered = true;
            CarScene02Triggered = true;
            memorySpaceCounter = 1;
            triggerMemorySpace = false;
        }*/


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
        ResetHeadsetPosition();

        //Start Audio
        //CueAudio();

        //Remove user blindfold gracefully (fade it out)
        StartCoroutine(RemoveBlindfold());
    }

    //360 video is ready to play
    void SphereVideoIsLoadedAndReady()
    {
        Cue360Video();
        EventManager.StopListening("SphereVideoIsLoaded", SphereVideoIsLoadedAndReady);
    }

    //Red (Ed/Driver) video is ready to play
    void RedVideoIsLoadedAndReady()
    {
        CueRedVideo();
        EventManager.StopListening("RedVideoIsLoaded", RedVideoIsLoadedAndReady);
    }

    //Blue (MH/Passenger) video is ready to play
    void BlueVideoIsLoadedAndReady()
    {
        CueBlueVideo();
        EventManager.StopListening("BlueVideoIsLoadedAndReady", BlueVideoIsLoadedAndReady);
    }

    void StartEntryIntoMemorySpaceOne()
    {
        Debug.Log("SceneController: Now entering memory space 1");
        GetComponent<MemorySpaces>().EnterMemorySpace(1);
        GetComponent<SoundManager>().StartEntryIntoMemorySpaceOne();
        EventManager.StopListening("EnterMemorySpaceOne", StartEntryIntoMemorySpaceOne);
    }
    void StartExitFromMemorySpaceOne()
    {
        triggerMemorySpace = true; //reset this var for 2nd memory sapce entry
        //trigger any sounds for the exit
        GetComponent<SoundManager>().StartExitFromMemorySpaceOne();
        EventManager.StopListening("ExitMemorySpaceOne", StartExitFromMemorySpaceOne);
    }

    void StartEntryIntoMemorySpaceTwo()
    {
        Debug.Log("Now entering memory space 2");
        GetComponent<MemorySpaces>().EnterMemorySpace(2);
        GetComponent<SoundManager>().StartEntryIntoMemorySpaceTwo();
        EventManager.StopListening("EnterMemorySpaceTwo", StartEntryIntoMemorySpaceTwo);
    }
    void StartExitFromMemorySpaceTwo()
    {
        EventManager.StopListening("ExitMemorySpaceTwo", StartExitFromMemorySpaceTwo);
    }



    //This function is called when the scene is fully faded at the end
    IEnumerator StartWrapUp(float wait)
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
       
        //Hit space to reposition the player in backseat
        if (Input.GetKeyDown("space"))
        {
            ResetHeadsetPosition();
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
                //if (StartAt == EnumeratedSkipPoints.FirstMemorySpace ||
                //    StartAt == EnumeratedSkipPoints.SecondMemorySpace)
                //{
                    EventManager.TriggerEvent("SphereVideoIsLoaded");
                //}

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
                //if (StartAt == EnumeratedSkipPoints.FirstMemorySpace ||
                //    StartAt == EnumeratedSkipPoints.SecondMemorySpace)
                //{
                    EventManager.TriggerEvent("RedVideoIsLoaded");
                //}
            }
        }
        if (Blue.activeSelf && !videoBlueLoaded)
        {
            mediaplayerBlue = Blue.GetComponent<MediaPlayer>();
            controlBlue = mediaplayerBlue.Control;
            if (controlBlue.CanPlay())
            {
                videoBlueLoaded = true;
                //if (StartAt == EnumeratedSkipPoints.FirstMemorySpace ||
                //    StartAt == EnumeratedSkipPoints.SecondMemorySpace)
                //{
                    EventManager.TriggerEvent("BlueVideoIsLoaded");
                //}
            }
        }

        //make sure that the ED and MH dialogue for scene 01 have both loaded
        //we get out of sync here if they are not ready but videos are
        //Debug.Log("MhAudioSource state: " + GetComponent<SoundManager>().MhAudioSource.clip.loadState);
        
        /*if (!sync && Blue.activeSelf && Red.activeSelf && VideoPlayer.activeSelf
            )
        {
            if (
            GetComponent<SoundManager>().MhAudioSource.clip != null &&
            GetComponent<SoundManager>().EdAudioSource.clip !=null &&
            GetComponent<SoundManager>().EdPFXSource.clip != null &&
            GetComponent<SoundManager>().MhPFXSource.clip != null &&
            video360Loaded && videoBlueLoaded && videoRedLoaded)
            {
                sync = true;
                EventManager.TriggerEvent("BlueVideoIsLoaded");
                EventManager.TriggerEvent("RedVideoIsLoaded");
                EventManager.TriggerEvent("SphereVideoIsLoaded");
            }
        }*/
        
        /*
        if (video360Loaded && videoBlueLoaded && videoRedLoaded)
        {

		

            //Enter memory space 1 
            if (triggerMemorySpace && 
                memorySpaceCounter == 0 && 
                control360.GetCurrentTimeMs() > Memory01StartTime*1000 )
            {
                triggerMemorySpace = false;
                memorySpaceCounter = 1;
                EventManager.TriggerEvent("EnterMemorySpaceOne");
            }

            //Enter memory space 2
            if (triggerMemorySpace &&
                memorySpaceCounter == 1 &&
                control360.GetCurrentTimeMs() > Memory02StartTime*1000)
            {
                triggerMemorySpace = false;
                memorySpaceCounter = 2;
                EventManager.TriggerEvent("EnterMemorySpaceTwo");
            }


			//Trigger Shift Key
			if (!ShiftKeyTriggered && controlRed.GetCurrentTimeMs() > ShiftKeyStartTime * 1000)
				{
					ShiftKeyTriggered = true;
					GetComponent<SoundManager>().StartShiftKey();
				}

			//Trigger Fly and Hit
			if (!FlyBiTriggered && controlRed.GetCurrentTimeMs () > FlyBiStartTime * 1000) 
			{
				FlyBiTriggered = true;
				GetComponent<SoundManager> ().StartFlyBi();
			}

			if (!FlyHitTriggered && controlRed.GetCurrentTimeMs () > FlyHitStartTime * 1000) 
			{
				FlyHitTriggered = true;
				GetComponent<SoundManager> ().StartFlyHit();
			}

        } 




        //If the finished flag is not yet set and we've reached FinaleStartTime seconds in the experience
        //Wrap up the scene
        if (!finished && control360.GetCurrentTimeMs() > FinaleStartTime*1000)
        {
            finished = true;
            startFinale();
        }*/
    }
    public void StartFinale()
    {
        StartCoroutine(StartWrapUp(8.0f));
        this.gameObject.GetComponent<Blindfold>().fadeInBlindFold();
        //start any finale music/sound
        GetComponent<SoundManager>().StartFinale();
    }

    //********** FUNCTIONS TO SKIP AROUND TIMELINE************/
    //*******************************************************//
    void Cue360Video() { CueVideo(control360); }
    void CueRedVideo() { CueVideo(controlRed); }
    void CueBlueVideo() { CueVideo(controlBlue); }

    void CueVideo(IMediaControl control)
    {
        //Skip to specific point in video
        SkipTo = EvalDestination();
        //control.Pause();
        if (SkipTo > 0) { control.SeekFast(SkipTo); }    
        control.Play();
    }


    /*void CueAudio() //this function is only called once from StartScene
    {
        if (StartAt == EnumeratedSkipPoints.FirstMemorySpace)
        {
            GetComponent<SoundManager>().StartEntryIntoMemorySpaceOne();
        } else if (StartAt == EnumeratedSkipPoints.CarTwo)
        {
            GetComponent<SoundManager>().StartCar02();
        } else if (StartAt == EnumeratedSkipPoints.SecondMemorySpace) {
            GetComponent<SoundManager>().StartEntryIntoMemorySpaceTwo();
        } else if (StartAt == EnumeratedSkipPoints.CarThree) {
            GetComponent<SoundManager>().StartCar03();
        } else
        {
            //otherwise, we're either skipping to Car 1 or this needs to be
            //triggered following the prelude
            GetComponent<SoundManager>().StartCar01();
        }
    }*/

    private float EvalDestination()
    {
        //See where we're skiping
        if (StartAt == EnumeratedSkipPoints.FirstMemorySpace)
        {
            SkipTo = GetComponent<TriggerDictionary>().triggers["MemorySpace01Trigger"].triggerTime * 1000;
        }
        else if (StartAt == EnumeratedSkipPoints.CarTwo)
        {
            SkipTo = GetComponent<TriggerDictionary>().triggers["CarScene02Trigger"].triggerTime * 1000;
        }
        else if (StartAt == EnumeratedSkipPoints.SecondMemorySpace)
        {
            SkipTo = GetComponent<TriggerDictionary>().triggers["MemorySpace02Trigger"].triggerTime * 1000;
        }
        else if (StartAt == EnumeratedSkipPoints.CarThree)
        {
            SkipTo = GetComponent<TriggerDictionary>().triggers["CarScene03Trigger"].triggerTime * 1000;
        }
        return SkipTo;
    }


    void ResetHeadsetPosition()
    {
        if (OculusTouch){
            OVRManager.display.RecenterPose();
        }else{
            //LM
            

        }
    }

    //******************* COROUTINES *************************/
    //*******************************************************//
    //*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*//
    IEnumerator ChangeSkyboxBlend() 
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
    IEnumerator RemoveBlindfold()
    {
        yield return new WaitForSeconds(4);
        GetComponent<Blindfold>().fadeOutBlindFold();
    }

}
