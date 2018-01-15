using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class SceneController : MonoBehaviour {

    //skybox for vanity card and opening title sequence
    public Material VanitySkybox;
    //Skybox for the car scene (uses one of Unitys HDRIs textures)
    public Material StorySkybox;
    
    //360 video gameobject reference
    public GameObject SphereVideo;
    //AVPRO video player
    public GameObject VideoPlayer;

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

    Color _color;
    bool videoLoaded=false;

    //Set up listeners
    private UnityAction OpenningSequenceComplete;
    private UnityAction SkipToSecond;


    void Awake()
    {
        //Listen for the title sequence completion event to be fired
        //once it's fired, start the scene
        OpenningSequenceComplete = new UnityAction(StartScene);
        //This listener waits for the assets to load before skipping to a particular point
        //in the experience (this is a debug feature, can be enbaled via the Skip To var found in the SceneController Script)
        if (SkipTo > 0)
        {
            SkipToSecond = new UnityAction(Skip);
        }
        
    }
    void OnEnable()
    {
        EventManager.StartListening("TitlesAreDone", OpenningSequenceComplete);
        if (SkipTo > 0)
        {
            EventManager.StartListening("SkipNow", SkipToSecond);
        }
            
    }
    void OnDisable ()
    {
        EventManager.StopListening("TitlesAreDone", OpenningSequenceComplete);
        if (SkipTo > 0)
        {
            EventManager.StopListening("SkipNow", SkipToSecond);
        }    
    }

    void Skip()
    {
        MediaPlayer mediaplayer = VideoPlayer.GetComponent<MediaPlayer>();
        IMediaControl control = mediaplayer.Control;
        control.Pause();
        control.SeekFast(SkipTo);
        control.Play();
    }
    //Function triggered by listener once the title sequence is compeleted
    void StartScene()
    {
        //Blindfold user while we're activating all the game objects
        Blindfold.SetActive(true);
        //Set the story skybox;
        RenderSettings.skybox = StorySkybox;
        //enable the 360 video
        SphereVideo.SetActive(true);
        
        //Activate car
        Car.SetActive(true);

        //TODO: Remove user blindfold more gracefully (fade it out)
        Blindfold.SetActive(false);
    }


    // Use this for initialization
    void Start () {
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

            /*****************/
            //Setup user bnlindfold 
            //store color value in variable
            _color = Blindfold.GetComponent<Renderer>().material.color;
            //set the alpha to 0
            _color.a = 0f;
            //assign color to out game object
            Blindfold.GetComponent<Renderer>().material.color = _color;
            Blindfold.SetActive(false);
            /*****************/

            Car.SetActive(false);
        } else
        {
            //If Skip Intro option checked, go straight to the cady scene
            Destroy(Fancy);
            Destroy(Titles);
            StartScene();
        }
       
    }
	
	// Update is called once per frame
	void Update () {
        if (!videoLoaded && SkipTo > 0)
        {
            MediaPlayer mediaplayer = VideoPlayer.GetComponent<MediaPlayer>();
            IMediaControl control = mediaplayer.Control;
            if (control.IsPlaying())
            {
                videoLoaded = true;
                //Tell listener, video is ready to be scrubbed
                EventManager.TriggerEvent("SkipNow");
            }
        } else
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
}
