using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    float delay = 6.0f;
    AudioSource[] SebastianVoiceovers;
    IEnumerator playVoiceoverCoroutine;
    int currentVoiceOver = 0;

    // Use this for initialization
    void Start () {
        SebastianVoiceovers = GetComponent<SceneController>().SebastianVoiceovers;

    }
	
	// Update is called once per frame
	void Update () {
       
    }

    public void initVoiceover()
    {
        playVoiceoverCoroutine = playVoiceover(delay, SebastianVoiceovers[currentVoiceOver]);
        StartCoroutine(playVoiceoverCoroutine);
    }

    IEnumerator playVoiceover(float delay, AudioSource voiceover)
    {
        yield return new WaitForSeconds(delay);
        voiceover.Play();
        StopCoroutine(playVoiceoverCoroutine);
    }
}
