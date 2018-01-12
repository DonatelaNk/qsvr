using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads;

public class Scene_manager : MonoBehaviour {

    public Material VanitySkybox;
    public Material StorySkybox;

    private float VanityCardDelay;
    private GameObject Fancy;
    IEnumerator changeSkyboxBlendCoroutine;

    // Use this for initialization
    void Start () {
        //set vanity nebula skybox
        RenderSettings.skybox = VanitySkybox;
        RenderSettings.skybox.SetFloat("_Blend", 0);

        //start the change skybox blend coroutine
        //Get the Fade Delay value from the Fade Object in/out script attached to teh Fancy game object
        Fancy = GameObject.Find("Fancy");
        VanityCardDelay = Fancy.GetComponent<FadeObjectInOut>().fadeDelay;
        changeSkyboxBlendCoroutine = changeSkyboxBlend(); // create an IEnumerator object
        StartCoroutine(changeSkyboxBlendCoroutine);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator changeSkyboxBlend() 
    {
        yield return new WaitForSeconds(VanityCardDelay);
        //yield return new WaitForSeconds(initialDelay);
        for (float f = 0f; f <= 1; f += 0.01f)
        {
            //change skybox blend
            RenderSettings.skybox.SetFloat("_Blend", f);
            
            if (f > 0.99)
            {
                StopCoroutine(changeSkyboxBlendCoroutine);
                Destroy(Fancy);
            }
            yield return null;
        }
        //once done, stop this coroutine and destroy the fancy vanity card gameobject
        
    }
}
