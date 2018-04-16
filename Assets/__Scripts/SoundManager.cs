using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float delay = 3.0f;
    //AudioSources
    public AudioSource PreludeAudioSource;

    [Header("Car Sounds")]
    public GameObject Radio;
    public AudioSource CarAudioSource;
    public AudioClip CarScene01FOA;
    public AudioClip CarScene02FOA;
    public AudioClip CarScene03FOA;
    public AudioSource ShifterAudioSource;

    //Windows
    [Header("Car Windows")]
    public AudioSource RearWindow;
    public AudioClip WindowsDownScene02SFX;
    public AudioClip WindowsDownScene03SFX;

    [Header("Fly Sounds")]
    public AudioSource FlyAudioSource;
    public AudioSource FlyDashAudioSource;
    public AudioSource FlyLandAudioSource;

   /* [Header("Sebastian Sounds")]
    public AudioSource SebastianAudioSource;
    public AudioClip[] SebastianVoiceovers; */

	[Header("DX Reverb")]
	public AudioSource DxReverbSource;
	public AudioClip CarScene01Verb;
	public AudioClip CarScene02Verb;
	public AudioClip CarScene03Verb;

    [Header("Ed Sounds")]
    //Ed's Window
    public AudioSource EdWindow;
	public AudioClip EdRollsWindow;
	public AudioSource EdWindowWind;
	public AudioClip WindowWind;
	public AudioClip EdWindScene02;
	public AudioClip EdWindScene03;

    [Header("Ed Dialogue")]
    public AudioSource EdAudioSource;
    public AudioClip EdCarScene01DX;
    public AudioClip EdCarScene02DX;
    public AudioClip EdCarScene03DX;

    [Header("Ed PFX")]
    public AudioSource EdYellsAudioSource;
	//public AudioClip EdCarScene02YellDX; //Assigned directly in scene
	public AudioSource EdPFXSource;
    public AudioClip EdCarScene01PFX;
    public AudioClip EdCarScene02PFX;
    public AudioClip EdCarScene03PFX;

    [Header("Mary-Helen Dialogue")]
    public AudioSource MhAudioSource;
    public AudioClip MhCarScene01DX;
    public AudioClip MhCarScene02DX;
    public AudioClip MhCarScene03DX;

    [Header("Mary-Helen PFX")]
    public AudioSource MhPFXSource;
    public AudioClip MhCarScene01PFX;
    public AudioClip MhCarScene02PFX;
    public AudioClip MhCarScene03PFX;

    private SceneController SceneController;


    /* [Header("Memory Spaces Sounds")]
     public AudioSource MemorySpace;
     public AudioClip MemorySpaceEnter;
     public AudioClip MemorySpaceExit;
     public AudioClip MemorySpace01;
     public AudioClip MemorySpace02; */




    /*IEnumerator playVoiceoverCoroutine;
    int currentVoiceOver = 0;*/

    // Use this for initialization
    void Awake()
    {
        //Attach the PFX audio to same Ed gameobject
        //EdPFXSource = EdAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //Attach the PFX audio to same MH gameobject
        //MhPFXSource = MhAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

    }
    void Start()
    {
        SceneController = GetComponent<SceneController>();
        SceneController.RadioStation.SetActive(false);
    }

    //Prelude, Car Scene 1, Memory Space 1, Car Scene 2, Memory Space 2, and Car Scene 3
    public void StartPrelude()
    {
        Debug.Log("SoundManager: Starting Prelude");
        PreludeAudioSource.Play();
    }
    
	public void StartCar01()
    {
        Debug.Log("SoundManager: Starting CarScene 1");
        //Start the car soundtrack
        InitSceneSound(CarAudioSource, CarScene01FOA);

        //start ED Dialog 
        InitSceneSound(EdAudioSource, EdCarScene01DX);
        //Add the sound effects clip for Ed (audio source added to same game objects, see onstart)
        InitSceneSound(EdPFXSource, EdCarScene01PFX);

        //Add Mary Helen Dialogue
        InitSceneSound(MhAudioSource, MhCarScene01DX);
        //Add MH sound effects clip (audio source added to same game objects, see onstart)
        InitSceneSound(MhPFXSource, MhCarScene01PFX);

		//start DX Reverb
		InitSceneSound(DxReverbSource, CarScene01Verb);
    }

    public void StartRadio()
    {
		//Loop through all the children of the Radio game object and play
        //Array to hold all child obj
        foreach (Transform speaker in Radio.transform)
        {
            speaker.GetComponent<AudioSource>().Play();
        }
        //turn on the station display
        SceneController.TuneRadio();
    }

    public void StartEntryIntoMemorySpaceOne()
    {
        //test Memory transition sounds
	    //InitSceneSound(MemorySpace, MemorySpaceEnter);
    }

	public void StartEdRollsWindow()
	{
		InitSceneSound (EdWindow, EdRollsWindow);
		InitSceneSound (EdWindowWind, WindowWind);
	}

    public void MemorySpaceOne()
    {
       
    }

    public void StartExitFromMemorySpaceOne()
    {

    }

    public void StartCar02()
    {

		Debug.Log("SoundManager: Starting CarScene 2");
		//Start the car 02 FOA
		InitSceneSound(CarAudioSource, CarScene02FOA);

		//Start Wind SFX
		//InitSceneSound(EdWindowWind, EdWindScene02);
		//InitSceneSound(RearWindow, WindowsDownScene02SFX);

		//start ED Dialog 
		InitSceneSound(EdAudioSource, EdCarScene02DX);
		//ED Yells Clip
		InitSceneSound(EdYellsAudioSource, null);
		//Add the sound effects clip for Ed
		InitSceneSound(EdPFXSource, EdCarScene02PFX);

		//Add Mary Helen Dialogue
		InitSceneSound(MhAudioSource, MhCarScene02DX);
		//Add MH sound effects clip
		InitSceneSound(MhPFXSource, MhCarScene02PFX);

		//start DX Reverb
		InitSceneSound(DxReverbSource, CarScene02Verb);

        //Fix dialogue audio timing issue
        //GetComponent<SceneController>().AudioSyncAdjust();
    }

    public void StartEntryIntoMemorySpaceTwo()
    {

    }

    public void MemorySpaceTwo()
    {

    }
    public void StartExitFromMemorySpaceTwo()
    {

    }

    public void StartCar03()
    {
		Debug.Log("SoundManager: Starting CarScene 3");
		//Start the car 03 FOA
		InitSceneSound(CarAudioSource, CarScene03FOA);

		//Start Wind SFX
		//InitSceneSound(EdWindowWind, EdWindScene03);
		//InitSceneSound(RearWindow, WindowsDownScene03SFX);

		//start ED Dialog 
		InitSceneSound(EdAudioSource, EdCarScene03DX);
		//Add the sound effects clip for Ed)
		InitSceneSound(EdPFXSource, EdCarScene03PFX);

		//Add Mary Helen Dialogue
		InitSceneSound(MhAudioSource, MhCarScene03DX);
		//Add MH sound effects clip
		InitSceneSound(MhPFXSource, MhCarScene03PFX);

		//start DX Reverb
		InitSceneSound(DxReverbSource, CarScene03Verb);
        
        //Fix dialogue audio timing issue
        //GetComponent<SceneController>().AudioSyncAdjust();

    }

	public void StartShiftKey()
	{
		InitSceneSound(ShifterAudioSource, null);
	}

	public void StartFlyBi()
	{
        InitSceneSound(FlyAudioSource, null);
	}

	public void StartFlyHit()
	{
		InitSceneSound (FlyLandAudioSource, null);
		InitSceneSound (FlyDashAudioSource, null);
	}

    public void StartFinale()
    {

    }

    //this function takes the audioclip and audiosource and starts playing it
    void InitSceneSound(AudioSource objAudioSource, AudioClip objAudioClip)
    {
        if (objAudioClip!=null)
        {
            objAudioSource.clip = objAudioClip;
        }    
        objAudioSource.Play();
    }



    // Update is called once per frame
    void Update () {
        //Debug.Log("prelude state: " + PreludeAudioSource.clip.loadState);
    }

    
}
