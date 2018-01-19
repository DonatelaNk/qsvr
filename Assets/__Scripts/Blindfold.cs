using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blindfold : MonoBehaviour {

    private static GameObject blindfold;
    Color _color;
    Renderer _renderer;

    //Corutine reference
    IEnumerator FadeCoroutine;

    bool triggerFadeInBlindFold = false;
    bool triggerFadeOutBlindFold = false;

    private void Awake()
    {
        //  Get value from the SceneController
        blindfold = GetComponent<SceneController>().Blindfold;
        _renderer = blindfold.GetComponent<Renderer>(); // do this in awake, it has an impact on performances in Update
        _color = _renderer.material.color;
        //set blindfold to black and initialize it
        Color color = new Color(0, 0, 0, 0);
        initBlindFold(color);
    }

    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
    }
   

    // Define an enumerator to perform our fading.
    // Pass it the opacity to fade to (0 = transparent, 1 = opaque),
    // and the number of seconds to fade over.
    IEnumerator FadeTo(float targetOpacity, float duration)
    {

        // Cache the current color of the material, and its initiql opacity.
        float startOpacity = _color.a;

        // Track how many seconds we've been fading.
        float t = 0;

        while (t < duration)
        {
            // Step the fade forward one frame.
            t += Time.deltaTime;
            // Turn the time into an interpolation factor between 0 and 1.
            float blend = Mathf.Clamp01(t / duration);

            // Blend to the corresponding opacity between start & target.
            _color.a = Mathf.Lerp(startOpacity, targetOpacity, blend);

            // Apply the resulting color to the material.
            _renderer.material.color = _color;

            // Wait one frame, and repeat.
            yield return null;
        }
        if (t > duration)
        {
            //kill the coroutine
            StopCoroutine(FadeCoroutine);
        }
    }


    public void initBlindFold(Color myColor)
    {
        /*****************/
        _renderer.material.color = myColor;
        /*****************/

    }

    public void setBlindFold()
    {
        _color.a = 1.0f;
        _renderer.material.color = _color;
        /*****************/

    }

    public void removeBlindFold()
    {
        _color.a = 0f;
        _renderer.material.color = _color;
        /*****************/

    }

    public void fadeInBlindFold()
    {
        // Start a coroutine to fade the material to zero alpha over x seconds.
        // Caching the reference to the coroutine lets us stop it mid-way if needed.
        FadeCoroutine = FadeTo(1f, 4f); // create an IEnumerator object
        StartCoroutine(FadeCoroutine);
    }

    public void fadeOutBlindFold()
    {
        FadeCoroutine = FadeTo(0f, 4f); // create an IEnumerator object
        StartCoroutine(FadeCoroutine);
    }

}
