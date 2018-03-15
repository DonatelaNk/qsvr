﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float delay = 3.0f;
    //AudioSources
    public GameObject RadioSpeakers;
	public AudioSource RadioDial;
	public AudioClip RadioDialKnob;

    public AudioSource SebastianAudioSource;
    public AudioClip[] SebastianVoiceovers;

    public AudioSource PreludeAudioSource;
    public AudioClip PreludeFOA;

    public AudioSource CarAudioSource;
    public AudioClip CarScene01FOA;
    public AudioClip CarScene02FOA;
    public AudioClip CarScene03FOA;

	//Ed's Window
	public AudioSource EdWindow;
	public AudioClip EdRollsWindow;
	public AudioSource EdWindowWind;
	public AudioClip WindowWind;
	public AudioClip EdWindScene02;
	public AudioClip EdWindScene03;

	//Windows
	public AudioSource RearWindow;
	public AudioClip WindowsDownScene02SFX;
	public AudioClip WindowsDownScene03SFX;

    public AudioSource EdAudioSource;
    public AudioClip EdCarScene01DX;
    public AudioClip EdCarScene02DX;
    public AudioClip EdCarScene03DX;

	public AudioSource EdYellsAudioSource;
	public AudioClip EdCarScene02YellDX;

	public AudioSource EdPFXSource;
    public AudioClip EdCarScene01PFX;
    public AudioClip EdCarScene02PFX;
    public AudioClip EdCarScene03PFX;

    public AudioSource MhAudioSource;
    public AudioClip MhCarScene01DX;
    public AudioClip MhCarScene02DX;
    public AudioClip MhCarScene03DX;


	public AudioSource MhPFXSource;
    public AudioClip MhCarScene01PFX;
    public AudioClip MhCarScene02PFX;
    public AudioClip MhCarScene03PFX;

	public AudioSource Shifter;
	public AudioClip ShiftKey;

	public AudioSource Fly;
	public AudioClip FlyBi;
	public AudioSource FlyDash;
	public AudioClip FlyHit;
	public AudioSource FlyLand;
	public AudioClip FlyMono;

    public AudioSource MemorySpace;
    public AudioClip MemorySpaceEnter;
    public AudioClip MemorySpaceExit;
    public AudioClip MemorySpace01;
    public AudioClip MemorySpace02;




    IEnumerator playVoiceoverCoroutine;
    int currentVoiceOver = 0;

    // Use this for initialization
    void Awake()
    {
        //Attach the PFX audio to same Ed gameobject
        //EdPFXSource = EdAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //Attach the PFX audio to same MH gameobject
        //MhPFXSource = MhAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

    }

    //Prelude, Car Scene 1, Memory Space 1, Car Scene 2, Memory Space 2, and Car Scene 3
    public void StartPrelude()
    {
        Debug.Log("SoundManager: Starting Prelude");
        PreludeAudioSource.clip = PreludeFOA;
        PreludeAudioSource.Play();
    }
    public void StartCar01()
    {
        Debug.Log("SoundManager: Starting CarScene 1");
        //Start the car soundtrack
        initSceneSound(CarAudioSource, CarScene01FOA);

        //start ED Dialog 
        initSceneSound(EdAudioSource, EdCarScene01DX);
        //Add the sound effects clip for Ed (audio source added to same game objects, see onstart)
        initSceneSound(EdPFXSource, EdCarScene01PFX);

        //Add Mary Helen Dialogue
        initSceneSound(MhAudioSource, MhCarScene01DX);
        //Add MH sound effects clip (audio source added to same game objects, see onstart)
        initSceneSound(MhPFXSource, MhCarScene01PFX);

    }

    public void StartRadio()
    {
        //Radio Dial
		initSceneSound(RadioDial, RadioDialKnob);

		//Loop through all the children of the Radio game object and play
        //Array to hold all child obj
        foreach (Transform speaker in RadioSpeakers.transform)
        {
            speaker.GetComponent<AudioSource>().Play();
        }
    }

    public void StartEntryIntoMemorySpaceOne()
    {
       //test Memory transition sounds
		initSceneSound(MemorySpace, MemorySpaceEnter);
    }

	public void StartEdRollsWindow()
	{
		initSceneSound (EdWindow, EdRollsWindow);
		initSceneSound (EdWindowWind, WindowWind);
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
		initSceneSound(CarAudioSource, CarScene02FOA);

		//Start Wind SFX
		initSceneSound(EdWindowWind, EdWindScene02);
		initSceneSound(RearWindow, WindowsDownScene02SFX);

		//start ED Dialog 
		initSceneSound(EdAudioSource, EdCarScene02DX);
		//ED Yells Clip
		initSceneSound(EdYellsAudioSource, EdCarScene02YellDX);
		//Add the sound effects clip for Ed
		initSceneSound(EdPFXSource, EdCarScene02PFX);

		//Add Mary Helen Dialogue
		initSceneSound(MhAudioSource, MhCarScene02DX);
		//Add MH sound effects clip
		initSceneSound(MhPFXSource, MhCarScene02PFX);

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
		initSceneSound(CarAudioSource, CarScene03FOA);

		//Start Wind SFX
		initSceneSound(EdWindowWind, EdWindScene03);
		initSceneSound(RearWindow, WindowsDownScene03SFX);

		//start ED Dialog 
		initSceneSound(EdAudioSource, EdCarScene03DX);
		//Add the sound effects clip for Ed)
		initSceneSound(EdPFXSource, EdCarScene03PFX);

		//Add Mary Helen Dialogue
		initSceneSound(MhAudioSource, MhCarScene03DX);
		//Add MH sound effects clip
		initSceneSound(MhPFXSource, MhCarScene03PFX);

    }

	public void StartShiftKey()
	{
		initSceneSound(Shifter, ShiftKey);
	}

	public void StartFlyBi()
	{
		initSceneSound (Fly, FlyBi);
	}

	public void StartFlyHit()
	{
		initSceneSound (FlyLand, FlyMono);
		initSceneSound (FlyDash, FlyHit);
	}

    public void StartFinale()
    {

    }

    //this function takes the audioclip and audiosource and starts playing it
    void initSceneSound(AudioSource objAudioSource, AudioClip objAudioClip)
    {
        objAudioSource.clip = objAudioClip;
        objAudioSource.Play();
    }



    // Update is called once per frame
    void Update () {
        //Debug.Log("prelude state: " + PreludeAudioSource.clip.loadState);
    }

    public void initVoiceover()
    {
        //playVoiceoverCoroutine = playVoiceover(delay, SebastianVoiceovers[currentVoiceOver]);
        //StartCoroutine(playVoiceoverCoroutine);
    }

    IEnumerator playVoiceover(float delay, AudioSource voiceover)
    {
        yield return new WaitForSeconds(delay);
        //voiceover.Play();
        //StopCoroutine(playVoiceoverCoroutine);
    }
}
