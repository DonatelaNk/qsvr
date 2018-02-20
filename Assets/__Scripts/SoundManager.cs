using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float delay = 3.0f;
    //AudioSources
    public AudioSource[] SebastianVoiceovers;
    public AudioSource Prelude;
    public AudioSource Car01;
    public AudioSource MemorySpace01;
    public AudioSource Car02;
    public AudioSource MemorySpace02;
    public AudioSource Car03;


    IEnumerator playVoiceoverCoroutine;
    int currentVoiceOver = 0;

    //Prelude, Car Scene 1, Memory Space 1, Car Scene 2, Memory Space 2, and Car Scene 3
    public void StartPrelude()
    {
        Debug.Log("Start Prelude");
    }
    public void StartCar01()
    {
        Debug.Log("Start Car 01");
    }


    // Use this for initialization
    void Start () {
        
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
