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
    private Objects Objects;
    private Sun Sun;
    private CarSteering CarSteering;

    //Set up references for the 360 video
    MediaPlayer mediaplayer360;
    IMediaControl control360;

    // Use this for initialization
    void Start () {
        SoundManager = GetComponent<SoundManager>();
        SceneController = GetComponent<SceneController>();
        Sun = SceneController.Sun.GetComponent<Sun>();
        CarSteering = SceneController.SteeringWheel.GetComponent<CarSteering>();
        //video reference
        mediaplayer360 = SceneController.VideoPlayer.GetComponent<MediaPlayer>();
        control360 = mediaplayer360.Control;

        //Declare our triggers dictionary
        triggers = new Dictionary<string, Trigger>();

        /*
        Scene1
        00:00:00:00 / 0m0s

        Scene2
        00:04:16:22 / 4m16.990s

        Scene3 
        00:07:51:22 / 7m52.205s
        */


        //Triggers: 
        //1. Scene Number (Scene01, Memory01, Scene02, Memory02, Scene03)
        //2. Default trigger value, should be false meaning it has not yet been triggered
        //3. Time in seconds when the trigger should be fired
        //4. Function that should be called

        Trigger CarScene01Trigger = new Trigger("Scene01", false, 0.01f, SoundManager.StartCar01);
        triggers.Add("CarScene01Trigger", CarScene01Trigger);

        //Turn wheel to the right
        Trigger wheelTurnRight0 = new Trigger("Scene01", false, 0.5f, CarSteering.TurnWheelRight);
        triggers.Add("wheelTurnRight0", wheelTurnRight0);

        //Reset wheel
        Trigger wheelReset0 = new Trigger("Scene01", false, 5.0f, CarSteering.ResetSteeringWheel);
        triggers.Add("wheelReset0", wheelReset0);

        //Turn off dashboard lights
        //Trigger TurnOffDashLights = new Trigger("Scene01", false, 15.0f, SceneController.TurnOffDashLights);
        //triggers.Add("TurnOffDashLights", TurnOffDashLights);

        /************************ RADIO ************************************/
        Trigger Tune1 = new Trigger("Scene01", false, 62.297f, SceneController.StopTuneRadio);
        triggers.Add("Tune1", Tune1);
        Trigger Tune2 = new Trigger("Scene01", false, 65.3f, SceneController.TuneRadio);
        triggers.Add("Tune2", Tune2);
        Trigger Tune3 = new Trigger("Scene01", false, 67.231f, SceneController.StopTuneRadio);
        triggers.Add("Tune3", Tune3);
        Trigger Tune4 = new Trigger("Scene01", false, 74.00f, SceneController.TuneRadio);
        triggers.Add("Tune4", Tune4);
        Trigger Tune5 = new Trigger("Scene01", false, 79.6f, SceneController.StopTuneRadio);
        triggers.Add("Tune5", Tune5);
        Trigger Tune6 = new Trigger("Scene01", false, 102.2f, SceneController.TuneRadio);
        triggers.Add("Tune6", Tune6);
        Trigger Tune7 = new Trigger("Scene01", false, 103.6f, SceneController.StopTuneRadio);
        triggers.Add("Tune7", Tune7);
        Trigger Tune8 = new Trigger("Scene01", false, 119.83f, SceneController.TurnOffRadio);
        triggers.Add("Tune8", Tune8);
        /****************** //END Radio tuning ****************************/

        //Street light 1
        Trigger Streetlight1 = new Trigger("Scene01", false, 22.27f, Sun.TriggerStreetLights);
        triggers.Add("Streetlight1", Streetlight1);

        //Street light 2
        Trigger Streetlight2 = new Trigger("Scene01", false, 30.5f, Sun.TriggerStreetLights);
        triggers.Add("Streetlight2", Streetlight2);

        //Trigger preDawn Sun
        Trigger preDawnSunTrigger = new Trigger("Scene01", false, 47.13f, Sun.TriggerPreDawn);
        triggers.Add("preDawnSunTrigger", preDawnSunTrigger);

        //Trigger Radio
        Trigger radioTrigger = new Trigger("Scene01", false, 56.0f, SoundManager.StartRadio);
        triggers.Add("radioTrigger", radioTrigger);

        //Trigger Sunrise
        Trigger sunRiseTrigger = new Trigger("Scene01", false, 118.07f, Sun.TriggerSunrise);
        triggers.Add("sunRiseTrigger", sunRiseTrigger);

        //Trigger SunAdjust
        Trigger sunAdjustTrigger = new Trigger("Scene01", false, 158.0f, Sun.TriggerAdjustSun);
        triggers.Add("sunAdjustTrigger", sunAdjustTrigger);

        //Turn wheel to the left
        Trigger wheelTurnLeft1 = new Trigger("Scene01", false, 160.20f, CarSteering.TurnWheelLeft);
        triggers.Add("wheelTurnLeft1", wheelTurnLeft1);

        //Reset wheel
        Trigger wheelReset1 = new Trigger("Scene01", false, 174.18f, CarSteering.ResetSteeringWheel);
        triggers.Add("wheelReset1", wheelReset1);

        //Trigger EarlyDay sun
        Trigger earlyDayTrigger = new Trigger("Scene01", false, 202.29f, Sun.TriggerEarlyDay);
        triggers.Add("earlyDayTrigger", earlyDayTrigger);

        //Trigger Ed rolling down the window
        Trigger EdRollsWindowTrigger = new Trigger("Scene01", false, 229.5f, SoundManager.StartEdRollsWindow);
        triggers.Add("EdRollsWindowTrigger", EdRollsWindowTrigger);

        //Trigger Memory Space 1
        Trigger MemorySpace01Trigger = new Trigger("Memory01", false, 230.0f, EnterMemorySpaceOne);
        triggers.Add("MemorySpace01Trigger", MemorySpace01Trigger);

        //TriggerMidDay light
        Trigger MidDayLightTrigger = new Trigger("Scene02", false, 256.0f, Sun.TriggerMidDay);
        triggers.Add("MidDayLightTrigger", MidDayLightTrigger);

        //Trigger CarScene 2 Videos
        Trigger CarScene02VideoTrigger = new Trigger("Scene02", false, 255.00f, null); //258.04f
        triggers.Add("CarScene02VideoTrigger", CarScene02VideoTrigger);

        //Trigger CarScene 2 sounds
        Trigger CarScene02Trigger = new Trigger("Scene02", false, 256.8f, SoundManager.StartCar02);
        triggers.Add("CarScene02Trigger", CarScene02Trigger);

        //Turn wheel to the right
        Trigger wheelTurnRight1 = new Trigger("Scene02", false, 260.16f, CarSteering.TurnWheelRight);
        triggers.Add("wheelTurnRight1", wheelTurnRight1);

        //Reset wheel
        Trigger wheelReset2 = new Trigger("Scene02", false, 263.22f, CarSteering.ResetSteeringWheel);
        triggers.Add("wheelReset2", wheelReset2);

        //Trigger light over bridge
        Trigger bridgeLightTrigger = new Trigger("Scene02", false, 392.06f, Sun.TriggerBridgeDay);
        triggers.Add("bridgeLightTrigger", bridgeLightTrigger);


        //Trigger Memory Space 2
        Trigger MemorySpace02Trigger = new Trigger("Memory02", false, 440.11f, EnterMemorySpaceTwo);
        triggers.Add("MemorySpace02Trigger", MemorySpace02Trigger);

        //Trigger CarScene 3 Videos
        Trigger CarScene03VideoTrigger = new Trigger("Scene03", false, 470.0f, null);
        triggers.Add("CarScene03VideoTrigger", CarScene03VideoTrigger);

        //Trigger sunset light
        Trigger sunsetLightTrigger = new Trigger("Scene03", false, 471.0f, Sun.TriggerSunset);
        triggers.Add("sunsetLightTrigger", sunsetLightTrigger);

        //Trigger CarScene 3 sounds
        Trigger CarScene03Trigger = new Trigger("Scene03", false, 472.0f, SoundManager.StartCar03); //473.1
        triggers.Add("CarScene03Trigger", CarScene03Trigger);

        //Trigger cemetery light
        Trigger graveSiteLightTrigger = new Trigger("Scene03", false, 499.06f, Sun.TriggerGraveSiteLight);
        triggers.Add("graveSiteLightTrigger", graveSiteLightTrigger);

        //Turn wheel to the left
        Trigger wheelTurnLeft2 = new Trigger("Scene03", false, 521.03f, CarSteering.TurnWheelLeft);
        triggers.Add("wheelTurnLeft2", wheelTurnLeft2);

        //Reset wheel
        Trigger wheelReset3 = new Trigger("Scene03", false, 526.05f, CarSteering.ResetSteeringWheel);
        triggers.Add("wheelReset3", wheelReset3);
        
        //Stop steering wheel animation
        Trigger stopSteering = new Trigger("Scene03", false, 548f, CarSteering.StopSteering);
        triggers.Add("stopSteering", stopSteering);

        //Trigger finale light
        Trigger finaleLightTrigger = new Trigger("Scene03", false, 548.23f, Sun.TriggerFinaleLight);
        triggers.Add("finaleLightTrigger", finaleLightTrigger);

        //Trigger ShiftKey
        Trigger ShiftKeyTrigger = new Trigger("Scene03", false, 552.0f, SoundManager.StartShiftKey);
        triggers.Add("ShiftKeyTrigger", ShiftKeyTrigger);

        //Engage parking break
        Trigger engageParkingBreak = new Trigger("Scene03", false, 552.19f, SceneController.MoveGearShift);
        triggers.Add("engageParkingBreak", engageParkingBreak);

        //Trigger fly flyby
        Trigger FlybyTrigger = new Trigger("Scene03", false, 612.25f, SoundManager.StartFlyBi);
        triggers.Add("FlybyTrigger", FlybyTrigger);

        //Trigger fly hit
        Trigger FlyHitTrigger = new Trigger("Scene03", false, 612.25f, SoundManager.StartFlyHit);
        triggers.Add("FlyHitTrigger", FlyHitTrigger);

        //Trigger BigFinish
        Trigger BigFinish = new Trigger("Finale", false, 615.7f, Sun.TriggerBigFinish);
        triggers.Add("BigFinish", BigFinish);

        //Trigger finale
        Trigger FinaleTrigger = new Trigger("Finale", false, 638.0f, SceneController.StartFinale);
        triggers.Add("FinaleTrigger", FinaleTrigger);



        

        //Light cookies, grouped
        //Beginning of Scene 2 flickers
        Trigger setCookie0 = new Trigger("Scene02", false, 284.05f, Sun.SetCookie);
        triggers.Add("setCookie0", setCookie0);
        Trigger removeCookie0 = new Trigger("Scene02", false, 284.15f, Sun.RemoveCookie);
        triggers.Add("removeCookie0", removeCookie0);
        Trigger setCookie1 = new Trigger("Scene02", false, 284.19f, Sun.SetCookie);
        triggers.Add("setCookie1", setCookie1);
        Trigger removeCookie1 = new Trigger("Scene02", false, 286.07f, Sun.RemoveCookie);
        triggers.Add("removeCookie1", removeCookie1);
        Trigger setCookie2 = new Trigger("Scene02", false, 286.13f, Sun.SetCookie);
        triggers.Add("setCookie2", setCookie2);
        Trigger removeCookie2 = new Trigger("Scene02", false, 286.20f, Sun.RemoveCookie);
        triggers.Add("removeCookie2", removeCookie2);
        Trigger setCookie3 = new Trigger("Scene02", false, 289.14f, Sun.SetCookie);
        triggers.Add("setCookie3", setCookie3);
        Trigger removeCookie3 = new Trigger("Scene02", false, 291.16f, Sun.RemoveCookie);
        triggers.Add("removeCookie3", removeCookie3);
        Trigger setCookie4 = new Trigger("Scene02", false, 300.14f, Sun.FlashCookie);
        triggers.Add("setCookie4", setCookie4);
        Trigger removeCookie4 = new Trigger("Scene02", false, 302.16f, Sun.RemoveCookie);
        triggers.Add("removeCookie4", removeCookie4);
        Trigger setCookie5 = new Trigger("Scene02", false, 312.0f, Sun.FlashCookie);
        triggers.Add("setCookie5", setCookie5);
        Trigger removeCookie5 = new Trigger("Scene02", false, 315.16f, Sun.RemoveCookie);
        triggers.Add("removeCookie5", removeCookie5);
        //END




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

        if (SceneController.StartAt == SceneController.EnumeratedSkipPoints.FinalCredits)
        {
            foreach (string trigger in triggersList)
            {
                if (triggers[trigger].scene == "Scene01" ||
                    triggers[trigger].scene == "Memory01" ||
                    triggers[trigger].scene == "Scene02" ||
                    triggers[trigger].scene == "Memory02" ||
                    triggers[trigger].scene == "Scene03")
                {
                    triggers[trigger].triggerStatus = true;
                }
            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //get current video frame just once per udpate
        videoPosition = control360.GetCurrentTimeMs();
        //Debug.Log(videoPosition/1000);
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log(videoPosition / 1000);
        }

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
                    if(triggers[trigger].triggerFunction!=null)
                    {
                        triggers[trigger].triggerFunction();
                    }         
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


        
