using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour {
    Camera m_main;
    bool outofbounds = false;
    bool fadeInTriggered = false;
    bool fadeOutTriggered = false;
    GameObject SceneManager;

    void Start()
    {
        m_main = Camera.main;
        SceneManager = GameObject.Find("SceneManager");
    }
    private void LateUpdate()
    {
        GetDistance(transform.position, m_main.transform.position);
    }

    //With percentage i.e. between 0 and 1
    public bool GetDistance(Vector3 me, Vector3 head)
    {
       Debug.Log(me.y - head.y);
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
    }
    private void FadeOutScene()
    {
        SceneManager.GetComponent<Blindfold>().fadeInBlindFold();
    }
    private void FadeInScene()
    {
        SceneManager.GetComponent<Blindfold>().fadeOutBlindFold();
    }
}
