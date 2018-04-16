using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Sun : MonoBehaviour {
    public Texture lightCookie;
    public List<LightSet> night;
    public List<LightSet> preDawn;
    public List<LightSet> sunrise;
    public List<LightSet> earlyDay;
    public List<LightSet> midDay;
    public List<LightSet> bridgeDay;
    public List<LightSet> sunset;
    public List<LightSet> graveSite;
    public List<LightSet> finaleLight;
    public List<LightSet> bigFinish;
    public List<LightSet> memorySpace;
    [Serializable]
    public class LightSet
    {
        public Color color;
        public float intensity;
        public float exposure;
        public Vector3 sunPosition;
        public float rotationX;
        public float rotationY;
    }
    private Light lt;
    //get main camera
    private Camera mainCamera;
    private PostProcessingBehaviour ppBehaviour;
    private PostProcessingProfile ppProfile;

    //change speed variables
    private float intensityChangeSpeed = 5.0f;
    private float colorChangeSpeed = 5.0f;
    private float exposureChangeSpeed = 5.0f;
    private float moveSpeed = 7.0f;
    private float resetSunIntensity;
    IEnumerator AnimateSunIntensityCoroutine;
    private bool sunFlicker = false;
    private bool lightPoleAnimation = false;
    private bool increaseSunIntensity = false;
    private bool decreaseSunIntensity = false;


    // Use this for initialization
    void Start () {
        lt = GetComponent<Light>();
        mainCamera = Camera.main;
        ppBehaviour = mainCamera.GetComponent<PostProcessingBehaviour>();

        if (ppBehaviour.profile == null)
        {
            enabled = false;
            return;
        }

        ppProfile = Instantiate(ppBehaviour.profile);
        ppBehaviour.profile = ppProfile;

        //reset sun
        foreach (LightSet set in night)
        {
            //set position
            lt.transform.position = set.sunPosition;
            //set rotation
            lt.transform.eulerAngles = new Vector3(set.rotationX, set.rotationY, 0);
            //set intensity
            lt.intensity = 0; // set.intensity;
            //set color
            lt.color = set.color;
            //set exposure
            var exposure = ppProfile.colorGrading.settings;
            exposure.basic.postExposure = set.exposure;
            ppProfile.colorGrading.settings = exposure;
        }
    }

    void FixedUpdate()
    {
        if (sunFlicker)
        {
            if (lt.intensity > 0.4f)
            {
                float randFlickerSpeed = UnityEngine.Random.Range(0.01f, 0.2f);
                lt.intensity = Mathf.Lerp(lt.intensity, 0.3f, Time.deltaTime / randFlickerSpeed);
            } else {
                lt.intensity = resetSunIntensity;
            }
        }
        if (lightPoleAnimation)
        {
            if (lt.intensity < 1.4f)
            {
                lt.intensity = Mathf.Lerp(lt.intensity, 1.5f, Time.deltaTime / .06f);
            } else
            {
                lightPoleAnimation = false;
                SetSun(night);
            }
        }
    }

    //called by our triggers (found in TriggerDictionary)
    public void TriggerPreDawn() { SetSun(preDawn); }
    public void TriggerSunrise() { SetSun(sunrise); }
    public void TriggerEarlyDay() { SetSun(earlyDay);}
    public void TriggerMidDay() { SetSun(midDay); }
    public void TriggerBridgeDay() { SetSun(bridgeDay); }
    public void TriggerSunset() { SetSun(sunset); }
    public void TriggerGraveSiteLight() { SetSun(graveSite); }
    public void TriggerMemorySpaceLight() { SetSun(memorySpace); }
    public void TriggerBigFinish() { SetSun(bigFinish); }
    public void TriggerFinaleLight() { SetSun(finaleLight); }

    public void TriggerAdjustSun()
    {
        Vector3 t = new Vector3(-4.65f, 2.57f, 6.0f);
        float rx = 169.528f;
        float ry = -52.689f;
        StartCoroutine(AnimateSunPositionAndRotation(t, rx, ry, 25.0f));
    }

    public void TriggerStreetLights()
    {
        Vector3 streetLightTargetPosition = new Vector3(3.53f, 1.49f, 0);
        Vector3 streetLightTargetRotation = new Vector3(169.528f, -267.109f, 20.32899f);

        //place it here:
        lt.transform.position = streetLightTargetPosition;
        //set rotation
        lt.transform.eulerAngles = streetLightTargetRotation;
        //set intensity
        lt.intensity = 0;
        //set color
        lt.color = new Color32(255, 185, 79, 255);
        
        lightPoleAnimation = true;
       
    }

    void MoveSun()
    {

    }

    public void SetCookie()
    {
        lt.cookie = lightCookie;
        StopCoroutine(AnimateSunIntensityCoroutine);
        AnimateSunIntensityCoroutine = AnimateSunIntensity(0.4f, 0.3f);
        StartCoroutine(AnimateSunIntensityCoroutine);
    }

    
    public void FlashCookie()
    {
        lt.cookie = lightCookie;
        sunFlicker = true;
    }

    public void RemoveCookie()
    {
        lt.cookie = null;
        sunFlicker = false;
        StopCoroutine(AnimateSunIntensityCoroutine);
        AnimateSunIntensityCoroutine = AnimateSunIntensity(resetSunIntensity, 0.3f);
        StartCoroutine(AnimateSunIntensityCoroutine);
    }

    void SetSun(List<LightSet> timeOfDay)
    {

        foreach (LightSet set in timeOfDay)
        {
            //set position and rotation
            if (timeOfDay == finaleLight)
            {
                //also animate sun setting
                moveSpeed = 60.0f;
                intensityChangeSpeed = 60.0f;
                colorChangeSpeed = 60.0f;
                exposureChangeSpeed = 60.0f;
                lt.intensity = 0.8f;
            }
            if (timeOfDay == night)
                intensityChangeSpeed = 0.5f;

            StartCoroutine(AnimateSunPositionAndRotation(set.sunPosition, set.rotationX, set.rotationY, moveSpeed));
            //animate intensity
            if (AnimateSunIntensityCoroutine!=null)
                StopCoroutine(AnimateSunIntensityCoroutine);
            AnimateSunIntensityCoroutine = AnimateSunIntensity(set.intensity, intensityChangeSpeed);
            StartCoroutine(AnimateSunIntensityCoroutine);
            //set color
            StartCoroutine(AnimateSunLightColor(set.color));
            //set exposure
            StartCoroutine(AnimateExposure(set.exposure));
            
        }   
    }


    //******************* COROUTINES *************************/
    //*******************************************************//
    //*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*//

    IEnumerator AnimateSunPositionAndRotation(Vector3 targetPosition, float rotation_x, float rotation_y, float speed)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = lt.transform.position;
        Quaternion startingRotation = lt.transform.rotation; // have a startingRotation as well
        Quaternion targetRotation = Quaternion.Euler(new Vector3(rotation_x, rotation_y, 0));

        while (elapsedTime < speed)
        {
            //move
            lt.transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / speed));
            //rotate
            lt.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedTime / speed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AnimateSunIntensity(float targetIntensity, float speed)
    {
        float elapsedTime = 0;
        //get current intensity
        float currentIntensity = lt.intensity;
        //remember the current intesnity so we can go back to it once we're passed the trees
        resetSunIntensity = 1.24f;
      
        while (elapsedTime < intensityChangeSpeed && true)
        {
            lt.intensity = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AnimateSunLightColor(Color targetColor)
    {
        float elapsedTime = 0;
        //get current color
        Color currentColor = lt.color;
        while (elapsedTime < colorChangeSpeed)
        {
            lt.color = Color.Lerp(currentColor, targetColor, elapsedTime / colorChangeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AnimateExposure(float targetExposure)
    {
        float elapsedTime = 0;
        //get current exposure
        float currentExposure = ppProfile.colorGrading.settings.basic.postExposure;
        //get exposure settings
        var exposure = ppProfile.colorGrading.settings;
        while (elapsedTime < exposureChangeSpeed)
        {
            exposure.basic.postExposure = Mathf.Lerp(currentExposure, targetExposure, elapsedTime / exposureChangeSpeed);
            ppProfile.colorGrading.settings = exposure;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
