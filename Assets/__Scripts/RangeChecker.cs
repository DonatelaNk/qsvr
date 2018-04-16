using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour {
    Camera m_main;
    bool outofbounds = false;
    bool fadeInTriggered = false;
    bool fadeOutTriggered = false;
    private GameObject SceneManager;
    private Blindfold Blindfold;
    private Collider cameraCollider;
    private Collider thisCollider;

    void Start()
    {
        m_main = Camera.main;
        cameraCollider = m_main.GetComponent<Collider>();
        thisCollider = GetComponent<Collider>();
        SceneManager = GameObject.Find("SceneManager");
        Blindfold = SceneManager.GetComponent<Blindfold>();
    }
    private void LateUpdate()
    {
        //GetDistance(transform.position, m_main.transform.position);
        if (thisCollider.bounds.Intersects(cameraCollider.bounds))
        {
            //print("Camera within bounds");
            if (!fadeInTriggered)
            {
                FadeInScene();
                fadeInTriggered = true;
                fadeOutTriggered = false;
            }
        } else
        {
            if (!fadeOutTriggered)
            {
                FadeOutScene();
                fadeOutTriggered = true;
                fadeInTriggered = false;
            }
                
        }
    }

    //With percentage i.e. between 0 and 1
    /*public bool GetDistance(Vector3 me, Vector3 head)
    {
       //Debug.Log(me.y - head.y);
       if ((me.y-head.y) < -0.3f)
        {
            outofbounds = true;
            if (!fadeOutTriggered)
            {
                fadeOutTriggered = true;
                FadeOutScene();
            }
        } else
        {
            outofbounds = false;
        }
        return outofbounds;
    }*/
    private void FadeOutScene()
    {
        Blindfold.FadeInBlindFold(0.2f);
    }
    private void FadeInScene()
    {
        Blindfold.FadeOutBlindFold(0.2f);
    }
}
