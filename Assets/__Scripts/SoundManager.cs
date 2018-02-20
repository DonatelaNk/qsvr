using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float delay = 3.0f;
    //AudioSources
    public AudioSource SebastianAudioSource;
    public AudioClip[] SebastianVoiceovers;

    public AudioSource PreludeAudioSource;
    public AudioClip PreludeFOA;

    public AudioSource CarAudioSource;
    public AudioClip CarScene01FOA;
    public AudioClip CarScene02FOA;
    public AudioClip CarScene03FOA;

    public AudioSource EdAudioSource;
    public AudioClip EdCarScene01DX;
    public AudioClip EdCarScene02DX;
    public AudioClip EdCarScene03DX;

    private AudioSource EdPFXSource;
    public AudioClip EdCarScene01PFX;
    public AudioClip EdCarScene02PFX;
    public AudioClip EdCarScene03PFX;

    public AudioSource MhAudioSource;
    public AudioClip MhCarScene01DX;
    public AudioClip MhCarScene02DX;
    public AudioClip MhCarScene03DX;


    private AudioSource MhPFXSource;
    public AudioClip MhCarScene01PFX;
    public AudioClip MhCarScene02PFX;
    public AudioClip MhCarScene03PFX;

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
        EdPFXSource = EdAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        //Attach the PFX audio to same MH gameobject
        MhPFXSource = MhAudioSource.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;

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
        CarAudioSource.clip = CarScene01FOA;
        CarAudioSource.Play();
        //start ED and MH Dialog
        EdAudioSource.clip = EdCarScene01DX;
        EdAudioSource.Play();
        //Add the sound effects clip (audio source added to same game objects, see onstart)
        EdPFXSource.clip = EdCarScene01PFX;

        //TODO: Add Mary Helen
        //MhAudioSource.clip = MhCarScene01DX;
        //MhAudioSource.Play();
    }
    public void StartEntryIntoMemorySpaceOne()
    {
       
    }

    public void MemorySpaceOne()
    {
       
    }
    public void StartExitFromMemorySpaceOne()
    {

    }

    public void StartCar02()
    {

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

    }

    public void StartFinale()
    {

    }



    // Update is called once per frame
    void Update () {
       
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
