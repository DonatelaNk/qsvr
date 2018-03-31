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
    }
    private Light lt;
    //get main camera
    private Camera mainCamera;
    private PostProcessingBehaviour ppBehaviour;
    private PostProcessingProfile ppProfile;

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
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void setSun(List<LightSet> timeOfDay)
    {

        foreach (LightSet set in timeOfDay)
        {
            //set position
            transform.position = set.sunPosition;
            //animate intensity
            lt.intensity = set.intensity;
            //set color
            lt.color = set.color;

            //set exposure
            var exposure = ppProfile.colorGrading.settings;
            exposure.basic.postExposure = set.exposure;
            ppProfile.colorGrading.settings = exposure;
        }
        
    }
}
