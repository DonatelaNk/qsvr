using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class Sun : MonoBehaviour {
    public List<LightSet> night;
    public List<LightSet> preDawn;
    public List<LightSet> sunrise;
    public List<LightSet> earlyDay;
    public List<LightSet> midDay;
    public List<LightSet> bridgeDay;
    public List<LightSet> sunset;
    public List<LightSet> graveSite;
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
    private float moveSpeed = 1.0f;
    private float intensityChangeSpeed = 5.0f;
    private float colorChangeSpeed = 5.0f;
    private float exposureChangeSpeed = 5.0f;

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
            lt.intensity = set.intensity;
            //set color
            lt.color = set.color;
            //set exposure
            var exposure = ppProfile.colorGrading.settings;
            exposure.basic.postExposure = set.exposure;
            ppProfile.colorGrading.settings = exposure;
        }
    }

    //called by our triggers (found in TriggerDictionary)
    public void TriggerPreDawn() { SetSun(preDawn); }
    public void TriggerSunrise() { SetSun(sunrise); }
    public void TriggerEarlyDay() { SetSun(earlyDay); }
    public void TriggerMidDay() { SetSun(midDay); }
    public void TriggerBridgeDay() { SetSun(bridgeDay); }
    public void TriggerSunset() { SetSun(sunset); }
    public void TriggerGraveSiteLight() { SetSun(graveSite); }

    void SetSun(List<LightSet> timeOfDay)
    {

        foreach (LightSet set in timeOfDay)
        {
            //set position and rotation
            StartCoroutine(AnimateSunPositionAndRotation(set.sunPosition, set.rotationX, set.rotationY));
            //animate intensity
            StartCoroutine(AnimateSunIntensity(set.intensity));
            //set color
            StartCoroutine(AnimateSunLightColor(set.color));
            //set exposure
            StartCoroutine(AnimateExposure(set.exposure));
        }   
    }


    //******************* COROUTINES *************************/
    //*******************************************************//
    //*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*^*//

    IEnumerator AnimateSunPositionAndRotation(Vector3 targetPosition, float rotation_x, float rotation_y)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = lt.transform.position;
        Quaternion startingRotation = lt.transform.rotation; // have a startingRotation as well
        Quaternion targetRotation = Quaternion.Euler(new Vector3(rotation_x, rotation_y, 0));

        while (elapsedTime < moveSpeed)
        {
            //move
            lt.transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / moveSpeed));
            //rotate
            lt.transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AnimateSunIntensity(float targetIntensity)
    {
        float elapsedTime = 0;
        //get current intensity
        float currentIntensity = lt.intensity;
        while (elapsedTime < intensityChangeSpeed)
        {
            lt.intensity = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / intensityChangeSpeed);
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
