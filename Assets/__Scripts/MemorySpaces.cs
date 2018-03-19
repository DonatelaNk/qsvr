using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using Leap.Unity.Interaction;

public class MemorySpaces : MonoBehaviour {

    //Corutine setup memory space
    IEnumerator setupMemorySpaceCorutine;
    IEnumerator addFogCoroutine;
    IEnumerator removeFogCoroutine;

    public float fogDensity = 0.3f;

    //Intantiate the photograph prefabs used in Memory Space 1
    public Rigidbody polaroid;
    public Transform m_parent;
    public Material[] PictureSet1;
    public Material[] PictureSet2;
    public Material[] PictureSet3;
    public Rigidbody Diary;
    public float MemoryMaxTime;

    //max time to wait for the user being inactive before existing the memory space
    private float MaxUserIdleTime = 5.0f; //5 seconds
    private float CountDownUserIdleTime;

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
    }

    void Update()
    {
       /* if (MemorySpaceActive)
        {
            LoopActorVideos();
            WatchUserIdleTime();
        } */
    }
    /*
    Logic for each memory space goes into this function
    */
    public void MemorySpaceIsReady(float memorySpaceNumber)
    {
        
        //First memory space logic
        if (memorySpaceNumber == 1)
        {
            PauseVideos();
            //TODO: This is temp
            StartCoroutine(ExitMemorySpace(MemoryMaxTime, "ExitMemorySpaceOne"));
            CounterMemoryMaxTime = MemoryMaxTime;
            CounterMaxActorLoop = MaxActorLoop;
            MemorySpaceActive = true;

            //Trigger Memory space 1 audio
            GetComponent<SoundManager>().MemorySpaceOne();

            //Position the MemorySpace parent in the box (in case the box was moved by user)
            Box = GameObject.Find("Box");
            m_parent.position = new Vector3(Box.transform.position.x, Box.transform.position.y, Box.transform.position.z);


            //Populate the box with a random set of potographs
            //Instantiate a polaroid prefab and assign materials from our random array set ( 1of 3)
            //TODO: Pick a random set
            Material[] randomSet = PictureSet1;
            foreach (Material photo in randomSet)
            {
                Rigidbody polaroidInstance;
                //generate a unique position to prevent things instantiating over each other and causing a raucous
                /*float x = Random.Range(-5f, 5f);
                float y = Random.Range(-4f, 4f);
                Vector3 spawnLocation = new Vector3(x, y, 1f);

                //Detect Overlap
                Collider[] hitColliders = Physics.OverlapSphere(spawnLocation, 1.33f);

                if (hitColliders.Length == 0)
                {
                }   */
                Vector3 instancePosition = new Vector3(m_parent.position.x, m_parent.position.y + 0.06f, m_parent.position.z);
                polaroidInstance = Instantiate(polaroid, instancePosition, m_parent.rotation) as Rigidbody;
                //parent it
                polaroidInstance.transform.parent = m_parent;
                //assign our photo from the array to the front of polaroid which is the first child in prefab
                polaroidInstance.transform.GetChild(0).GetComponent<Renderer>().material = photo;
                //if using Leapmotion, add interaction manager
                if (GetComponent<SceneController>().LeapMotion)
                {
                    LMInteractionBehaviour = polaroidInstance.gameObject.AddComponent<InteractionBehaviour>();
                    //LMInteractionBehaviour.allowMultiGrasp = true;
       
                }

                //add a script to prevent very thin colliders (like paper and photos) from going though each other
                polaroidInstance.gameObject.AddComponent<DontGoThroughThings>();

            }
        }

        //Second memory space logic
        if (memorySpaceNumber == 2)
        {
            PauseVideos();
            StartCoroutine(ExitMemorySpace(MemoryMaxTime, "ExitMemorySpaceTwo"));
            CounterMemoryMaxTime = MemoryMaxTime;
            MemorySpaceActive = true;
            //Trigger Memory space 2 audio
            GetComponent<SoundManager>().MemorySpaceTwo();

            //Instantiate the diary
            Rigidbody diaryInstance;
            diaryInstance = Instantiate(Diary, m_parent.position, m_parent.rotation) as Rigidbody;
            //make a child of the memory space object
            diaryInstance.transform.parent = m_parent;
            //if using Leapmotion, add interaction manager
            if (GetComponent<SceneController>().LeapMotion)
            {
                diaryInstance.gameObject.AddComponent<InteractionBehaviour>();
                diaryInstance.transform.GetChild(5).gameObject.AddComponent<InteractionBehaviour>();
                diaryInstance.transform.GetChild(6).gameObject.AddComponent<InteractionBehaviour>();
            }
               
            GetComponent<SoundManager>().InitVoiceover();


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
        //GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Pause();
        //GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Pause();
    }
    public void ResumeVideos()
    {
        GetComponent<SceneController>().VideoPlayer.GetComponent<MediaPlayer>().Control.Play();
        //if exiting first memory space
        if(GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.GetCurrentTimeMs() < 400*1000)
        {
            GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Seek(GetComponent<TriggerDictionary>().triggers["CarScene02Trigger"].triggerTime * 1000);
            GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Seek(GetComponent<TriggerDictionary>().triggers["CarScene02Trigger"].triggerTime * 1000);
        } else
        {
            //we're exiting out of the 2nd memory space
            GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Seek(GetComponent<TriggerDictionary>().triggers["CarScene03Trigger"].triggerTime * 1000);
            GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Seek(GetComponent<TriggerDictionary>().triggers["CarScene03Trigger"].triggerTime * 1000);
        }
        //GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control.Play();
        //GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control.Play();
    }
    public void LoopActorVideos()
    {
        CounterMaxActorLoop -= Time.deltaTime;
        //Debug.Log(CounterMaxActorLoop);
        if (CounterMaxActorLoop <= 0)
        {
            //Get actor current pos and rewind it xx seconds
            IMediaControl controlRed = GetComponent<SceneController>().Red.GetComponent<MediaPlayer>().Control;
            IMediaControl controlBlue = GetComponent<SceneController>().Blue.GetComponent<MediaPlayer>().Control;
            float resetPoint = GetComponent<TriggerDictionary>().triggers["CarScene02Trigger"].triggerTime * 1000 - (MaxActorLoop * 1000)-1000;
            controlRed.Seek(resetPoint);
            controlBlue.Seek(resetPoint);
            //reset the loop
            CounterMaxActorLoop = MaxActorLoop;
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
        StartCoroutine(addFogCoroutine);
    }

    public void WatchUserIdleTime()
    {
        CountDownUserIdleTime -= Time.deltaTime;
        if (CountDownUserIdleTime <=0)
        {
            //break the memory space corutine anex and;
            //Debug.Log("break the memory space corutine and exit");
        }
    }
    public void ResetUserIdleTime()
    {
        CountDownUserIdleTime = MaxUserIdleTime;
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
        yield return new WaitForSeconds(wait);
        //remove momory dust
        GetComponent<SceneController>().MemoryDust.SetActive(false);
        removeFogCoroutine = EndFog();
        StartCoroutine(removeFogCoroutine);
        ResumeVideos();
        MemorySpaceActive = false;
        //destroy objects available in memory space
        foreach (Transform child in m_parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        EventManager.TriggerEvent(triggerLabel);    
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
