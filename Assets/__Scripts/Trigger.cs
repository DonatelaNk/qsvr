using UnityEngine;
using System.Collections.Generic;
using System;

public class Trigger {

    public string scene;
    public bool triggerStatus;
    public float triggerTime;
    public System.Action triggerFunction;

    public Trigger(string newScene, bool newTriggerStatus, float newTriggerTime, System.Action newTriggerFunction)
    {
        scene = newScene;
        triggerStatus = newTriggerStatus;
        triggerTime = newTriggerTime;
        triggerFunction = newTriggerFunction;
    }

}
