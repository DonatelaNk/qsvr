using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class TriggerDictionary : MonoBehaviour {

    public Dictionary<string, Trigger> triggers;
    private List<string> triggersList;
    private float videoPosition;
    private SoundManager SoundManager;
    private SceneController SceneController;

    // Use this for initialization
    void Start () {
        SoundManager = GetComponent<SoundManager>();
        SceneController = GetComponent<SceneController>();
        //Declare our triggers dictionary
        triggers = new Dictionary<string, Trigger>();

        //Triggers: 
        //1. Scene Number (Scene01, Memory01, Scene02, Memory02, Scene03)
        //2. Default trigger value, should be false meaning it has not yet been triggered
        //3. Time in seconds when the trigger should be fired
        //4. Function that should be called

        Trigger CarScene01Trigger = new Trigger("Scene01", false, 0.01f, SoundManager.StartCar01);
        triggers.Add("CarScene01Trigger", CarScene01Trigger);

        //Trigger Radio
        Trigger radioTrigger = new Trigger("Scene01", false, 56.0f, SoundManager.StartRadio);
        triggers.Add("radioTrigger", radioTrigger);

        //Trigger Ed rolling down the window
        Trigger EdRollsWindowTrigger = new Trigger("Scene01", false, 229.5f, SoundManager.StartEdRollsWindow);
        triggers.Add("EdRollsWindowTrigger", EdRollsWindowTrigger);

        //Trigger Memory Space 1
        Trigger MemorySpace01Trigger = new Trigger("Memory01", false, 227.0f, EnterMemorySpaceOne);
        triggers.Add("MemorySpace01Trigger", MemorySpace01Trigger);

        //Trigger CarScene 2 sounds
        Trigger CarScene02Trigger = new Trigger("Scene02", false, 258.0f, SoundManager.StartCar02);
        triggers.Add("CarScene02Trigger", CarScene02Trigger);

        //Trigger Memory Space 2
        Trigger MemorySpace02Trigger = new Trigger("Memory02", false, 437.0f, EnterMemorySpaceTwo);
        triggers.Add("MemorySpace02Trigger", MemorySpace02Trigger);

        //Trigger CarScene 3 sounds
        Trigger CarScene03Trigger = new Trigger("Scene03", false, 473.0f, SoundManager.StartCar03);
        triggers.Add("CarScene03Trigger", CarScene03Trigger);

        //Trigger ShiftKey
        Trigger ShiftKeyTrigger = new Trigger("Scene03", false, 552.0f, SoundManager.StartShiftKey);
        triggers.Add("ShiftKeyTrigger", ShiftKeyTrigger);

        //Trigger fly flyby
        Trigger FlybyTrigger = new Trigger("Scene03", false, 612.25f, SoundManager.StartFlyBi);
        triggers.Add("FlybyTrigger", FlybyTrigger);

        //Trigger fly hit
        Trigger FlyHitTrigger = new Trigger("Scene03", false, 612.25f, SoundManager.StartFlyHit);
        triggers.Add("FlyHitTrigger", FlyHitTrigger);

        //Trigger finale
        Trigger FinaletTrigger = new Trigger("Scene03", false, 638.0f, SoundManager.StartFinale);
        triggers.Add("FinaletTrigger", FinaletTrigger);


        // ╔═════ ∘◦ END Trigger values  ◦∘ ══════╗
        // ╚═════ ∘◦ ❉❉❉❉❉❉❉❉❉❉❉  ◦∘ ══════╝

        // Store dictionary keys in a List
        triggersList = new List<string>(triggers.Keys);


        //Depending on if we skipped using the StartAt dropdown, set some triggers to true, so we dont trigger these sounds
        if (SceneController.StartAt == SceneController.EnumeratedSkipPoints.FirstMemorySpace)
        {
            foreach (string trigger in triggersList)
            {
                if (triggers[trigger].scene == "Scene01")
                {
                    triggers[trigger].triggerStatus = true;
                }
            }
        }
        if (SceneController.StartAt == SceneController.EnumeratedSkipPoints.CarTwo)
        {
            foreach (string trigger in triggersList)
            {
                if (triggers[trigger].scene == "Scene01" || 
                    triggers[trigger].scene == "Memory01")
                {
                    triggers[trigger].triggerStatus = true;
                }
            }
        }
        if (SceneController.StartAt == SceneController.EnumeratedSkipPoints.SecondMemorySpace)
        {
            foreach (string trigger in triggersList)
            {
                if (triggers[trigger].scene == "Scene01" || 
                    triggers[trigger].scene == "Memory01" ||
                    triggers[trigger].scene == "Scene02")
                {
                    triggers[trigger].triggerStatus = true;
                }
            }
        }
        if (SceneController.StartAt == SceneController.EnumeratedSkipPoints.CarThree)
        {
            foreach (string trigger in triggersList)
            {
                if (triggers[trigger].scene == "Scene01" ||
                    triggers[trigger].scene == "Memory01" ||
                    triggers[trigger].scene == "Scene02" ||
                    triggers[trigger].scene == "Memory02")
                {
                    triggers[trigger].triggerStatus = true;
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        //get current video frame just once per udpate
        videoPosition = SceneController.VideoPlayer.GetComponent<MediaPlayer>().Control.GetCurrentTimeMs();
        Debug.Log(videoPosition);
        
        //loop through triggers list to see if anything needs to be triggered
        foreach (string trigger in triggersList)
        {
            //if this trigger is false, see if we should trigger it now
            if (!triggers[trigger].triggerStatus)
            {
                if (videoPosition >= triggers[trigger].triggerTime * 1000)
                {
                    //update this trigger's bool value, set it to true indicating that it has been triggered
                    triggers[trigger].triggerStatus = true;
                    triggers[trigger].triggerFunction();
                    Debug.Log("Triggered:" + trigger);
                }
            }
        }
    }

    void EnterMemorySpaceOne()
    {
        EventManager.TriggerEvent("EnterMemorySpaceOne");
    }
    void EnterMemorySpaceTwo()
    {
        EventManager.TriggerEvent("EnterMemorySpaceTwo");
    }
}


        
