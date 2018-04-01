using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QSVR.Internals;

public class Blindfold : MonoBehaviour {

    //Fade options
    public float fadeTime = 0.25f;
    public Color fadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    public Material fadeMaterial = null;

    private ScreenFadeControl fadeControl;
    private List<ScreenFadeControl> fadeControls;

    void Awake()
    {
        initBlindFold();
    }


    void Start()
    {
    }
   

    public void initBlindFold()
    {
        fadeControls = new List<ScreenFadeControl>();
        foreach (Camera c in Camera.allCameras)
        {
            if (c.isActiveAndEnabled && c.targetTexture == null) // Is a camera visible to the player
            {
      
                fadeControl = c.gameObject.AddComponent<ScreenFadeControl>();
                fadeControl.fadeMaterial = fadeMaterial;
                fadeControls.Add(fadeControl);
            }
        }
        SetFadersEnabled(fadeControls, false);
    }

    public void fadeInBlindFold()
    {
        StartCoroutine(FadeIn(fadeControls));
    }

    public void fadeOutBlindFold()
    {
        StartCoroutine(FadeOut(fadeControls));
    }

    public void setBlindFold()
    {
        Color color = fadeColor;
        color.a = 1.0f;
        fadeMaterial.color = color;
        SetFadersEnabled(fadeControls, true);
    }

    //These are not called from outside scripts
    void SetFadersEnabled(IEnumerable<ScreenFadeControl> fadeControls, bool value)
    {
        foreach (ScreenFadeControl fadeControl in fadeControls)
            fadeControl.enabled = value;
    }
    public IEnumerator FadeIn(IEnumerable<ScreenFadeControl> fadeControls)
    {
       //Debug.Log("Doing fade In");
       // Derived from OVRScreenFade
       float elapsedTime = 0.0f;
       Color color = fadeColor;
       color.a = 0.0f;
       fadeMaterial.color = color;
       SetFadersEnabled(fadeControls, true);
       while (elapsedTime < fadeTime)
       {
           yield return new WaitForEndOfFrame();
           elapsedTime += Time.deltaTime;
           color.a = Mathf.Clamp01(elapsedTime / fadeTime);
           fadeMaterial.color = color;
       }

    }

    public IEnumerator FadeOut(IEnumerable<ScreenFadeControl> fadeControls)
    {
       //Debug.Log("Fading back out");
       float elapsedTime = 0.0f;
       Color color = fadeMaterial.color = fadeColor;
       while (elapsedTime < fadeTime)
       {
           yield return new WaitForEndOfFrame();
           elapsedTime += Time.deltaTime;
           color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
           fadeMaterial.color = color;
       }
       SetFadersEnabled(fadeControls, false);
    }

}
