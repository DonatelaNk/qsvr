using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using Leap.Unity.Interaction;


public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;
    IEnumerator exitMemorySpaceCoroutine;
    IEnumerator addFogCoroutine;
    IEnumerator removeFogCoroutine;

    [Header("Settings")]

    public float fogDensity = 0.3f;
    public float MemoryMaxTime;
    public Transform m_parent;
    //max time to wait for the user being inactive before existing the memory space
    public float MaxUserIdleTime = 15.0f; //15 seconds
    private float CountDownUserIdleTime;
    private string currentMemorySpaceTrigger;
    private string currentVideoResetTrigger;

    private Sun Sun;

    [Header("Memory Space 1")]
    //Intantiate the photograph prefabs used in Memory Space 1
    public Rigidbody polaroid;
   
    /*public Material[] PictureSet1;
    public Material[] PictureSet2;
    public Material[] PictureSet3;*/

    public List<PictureSetPair> PictureSet1;
    public List<PictureSetPair> PictureSet2;
    public List<PictureSetPair> PictureSet3;
    [Serializable]
    public class PictureSetPair
    {
        public Texture2D texture;
        public AudioClip audioClip;
    }
    private List<PictureSetPair> randomSet;


    [Header("Memory Space 2")]
    public GameObject Diary;

    private GameObject Box;

    private float CounterMemoryMaxTime;
    private float MaxActorLoop = 19.0f;
    private float CounterMaxActorLoop;

    private bool MemorySpaceActive = false;

    //leapmotion interaction manager must be added to instantiated game objects at runtime
    private InteractionBehaviour LMInteractionBehaviour;

    void Awake()
    {
        
    }

    void Start()
    {
        CountDownUserIdleTime = MaxUserIdleTime;
        Diary.SetActive(false);
        Sun = GameObject.Find("SUN_Animated").GetComponent<Sun>();
    }

    void Update()
    {
        if (MemorySpaceActive)
        {
            LoopActorVideos();
            WatchUserIdleTime();

            //keep an eye on objects in memory space, if they are being interacted with
            //reset the countdown clock
            foreach (Transform child in m_parent.transform)
            {
                if (GetComponent<SceneController>().LeapMotion)
                {
                    if (child.gameObject.GetComponent<InteractionBehaviour>().isGrasped)
                    {
                        CountDownUserIdleTime = MaxUserIdleTime;
                    }
                }
                else
                {
                    if (child.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
                    {
                        CountDownUserIdleTime = MaxUserIdleTime;
                    }
                }
            }

        } 
    }
    /*
    Logic for each memory space goes into this function
    */
    public void MemorySpaceIsReady(float memorySpaceNumber)
    {
        
        //First memory space logic
        if (memorySpaceNumber == 1)
        {
            CounterMemoryMaxTime = MemoryMaxTime;
            CountDownUserIdleTime = MaxUserIdleTime;
            CounterMaxActorLoop = MaxActorLoop;
            PauseVideos();
            currentMemorySpaceTrigger = "ExitMemorySpaceOne";
            currentVideoResetTrigger = "CarScene02VideoTrigger";
            exitMemorySpaceCoroutine = ExitMemorySpace(MemoryMaxTime, currentMemorySpaceTrigger);
            StartCoroutine(exitMemorySpaceCoroutine);

            
            MemorySpaceActive = true;

            //Trigger Memory space 1 audio
            GetComponent<SoundManager>().MemorySpaceOne();

            //Position the MemorySpace parent in the box (in case the box was moved by user)
            Box = GameObject.Find("Box");
            m_parent.position = new Vector3(Box.transform.position.x, Box.transform.position.y, Box.transform.position.z);


            //Populate the box with a random set of potographs
            //Instantiate a polaroid prefab and assign materials from our random array set ( 1of 3)
            int randSetNum = UnityEngine.Random.Range(1, 3);
            switch (randSetNum)
            {
                case 1:
                    randomSet = PictureSet1;
                    break;
                case 2:
                    randomSet = PictureSet2;
                    break;
                case 3:
                    randomSet = PictureSet3;
                    break;
            }
            
            foreach (PictureSetPair photo in randomSet)
            {
                Rigidbody polaroidInstance;
                Vector3 instancePosition = new Vector3(m_parent.position.x, m_parent.position.y + 0.00974f, m_parent.position.z);
                polaroidInstance = Instantiate(polaroid, instancePosition, m_parent.rotation) as Rigidbody;
                //parent it
                polaroidInstance.transform.parent = m_parent;
                //assign our photo from the array to the front of polaroid which is the first child in prefab
                polaroidInstance.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = photo.texture;

                if (photo.audioClip!=null)
                {
                    //Attach audio if exists
                    polaroidInstance.GetComponent<AudioSource>().clip = photo.audioClip;
                    polaroidInstance.GetComponent<AudioSource>().Play();
                }

                //if using Leapmotion, add interaction manager
                if (GetComponent<SceneController>().LeapMotion)
                {
                    LMInteractionBehaviour = polaroidInstance.gameObject.AddComponent<InteractionBehaviour>();
                    //LMInteractionBehaviour.allowMultiGrasp = true;
       
                }

                //add a script to prevent very thin colliders (like paper and photos) from going though each other
                //polaroidInstance.gameObject.AddComponent<DontGoThroughThings>();

            }
        }

        //Second memory space logic
        if (memorySpaceNumber == 2)
        {
            CounterMemoryMaxTime = MemoryMaxTime;
            CountDownUserIdleTime = MaxUserIdleTime;
            CounterMaxActorLoop = MaxActorLoop;
            PauseVideos();
            currentMemorySpaceTrigger = "ExitMemorySpaceTwo";
            currentVideoResetTrigger = "CarScene03VideoTrigger";

            exitMemorySpaceCoroutine = ExitMemorySpace(MemoryMaxTime, currentMemorySpaceTrigger);
            StartCoroutine(exitMemorySpaceCoroutine);

            MemorySpaceActive = true;
            //Trigger Memory space 2 audio
            GetComponent<SoundManager>().MemorySpaceTwo();
            Diary.SetActive(true);
            //make it a child of the memory room parent
            Diary.transform.parent = m_parent;
            //GetComponent<SoundManager>().InitVoiceover();


        }
    }
    //~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~^~

    public void EnterMemorySpace(float memberSpaceNumber)
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
    public void ResumeVideos()
    {
        //if exiting first memory space
        float seekTime = GetComponent<TriggerDictionary>().triggers[currentVideoResetTrigger].triggerTime * 1000;
        Debug.Log("seekTime: " + seekTime);
        GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Seek(seekTime);
        GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Seek(seekTime);
        GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Seek(seekTime);
        //GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Play();
        GetComponent<SceneController>().ResetSync();
    }
    public void LoopActorVideos()
    {
        /*CounterMaxActorLoop -= Time.deltaTime;
        //Debug.Log(CounterMaxActorLoop);
        if (CounterMaxActorLoop <= 0)
        {
            //Get actor current pos and rewind it xx seconds
            IMediaControl controlRed = GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control;
            IMediaControl controlBlue = GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control;
            float resetPoint = GetComponent<TriggerDictionary>().triggers[currentVideoResetTrigger].triggerTime * 1000 - (MaxActorLoop * 1000)-3000;
            controlRed.Seek(resetPoint);
            controlBlue.Seek(resetPoint);
            //reset the loop
            CounterMaxActorLoop = MaxActorLoop;
        }*/
    }
    public void WatchUserIdleTime()
    {
        CountDownUserIdleTime -= Time.deltaTime;
        if (CountDownUserIdleTime <= 0)
        {
            //break the memory space corutine;
            Debug.Log("break the memory space corutine and exit");
            ExitMemorySpaceNow();
        }
    }

    public void SetMemorySpaceMood()
    {
        //Reveal memory dust
        GetComponent<SceneController>().MemoryDust.SetActive(true);
        //Start fod
        RenderSettings.fog = true;
        RenderSettings.fogDensity = 0.0f;
       /* RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10;
        RenderSettings.fogEndDistance = 20;*/
        addFogCoroutine = StartFog();
        Sun.TriggerMemorySpaceLight();
        StartCoroutine(addFogCoroutine);
    }

   
    public void ResetUserIdleTime()
    {
        CountDownUserIdleTime = MaxUserIdleTime;
    }

    private void ExitMemorySpaceNow()
    {
        MemorySpaceActive = false;
        StopCoroutine(exitMemorySpaceCoroutine);
        //remove momory dust
        GetComponent<SceneController>().MemoryDust.SetActive(false);
        removeFogCoroutine = EndFog();
        StartCoroutine(removeFogCoroutine);
        ResumeVideos();  
        //destroy objects available in memory space
        foreach (Transform child in m_parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        EventManager.TriggerEvent(currentMemorySpaceTrigger);
    }

    IEnumerator SetupMemorySpace(float wait, float memberSpaceNumber)
    {
        yield return new WaitForSeconds(wait);
        SetMemorySpaceMood();
        PauseVideos();
        MemorySpaceIsReady(memberSpaceNumber);
        StopCoroutine(setupMemorySpaceCorutine);
    }

    IEnumerator ExitMemorySpace(float wait, string triggerLabel)
    {
        while (true)
        {
            yield return new WaitForSeconds(wait);
            Debug.Log("ExitMemorySpace coroutine finished.");
            ExitMemorySpaceNow();
        }

            
    }

    IEnumerator StartFog()
    {
       
        for (float f = 0f; f <= fogDensity; f += 0.001f)
        {
            //change skybox blend
            RenderSettings.fogDensity = f;
            yield return null;
        }
        StopCoroutine(addFogCoroutine);
    }
    IEnumerator EndFog()
    {

        for (float f = fogDensity; f >= 0; f -= 0.001f)
        {
            //change skybox blend
            RenderSettings.fogDensity = f;
            yield return null;
        }
        StopCoroutine(removeFogCoroutine);
    }
}
