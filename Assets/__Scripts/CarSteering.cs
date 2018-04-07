using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class CarSteering : MonoBehaviour {
    //used for moving the wheek a little to make it look alove 
    private float rotationAlternatorCounter;
    private bool rotateWheelRight = true;
    private bool rotateWheelLeft = false;
    private float rotationSpeed = 0.01f;
    private float rotationMax = 0.04f;
    private bool steer = false; //control steering wheel rotation

    private bool turnLeft = false;
    private bool turnRight = false;
    private bool reset = false;
    private Quaternion quats;

    // Use this for initialization
    void Start () {
    }

    void Update()
    {
        //Hit space to reposition the player in backseat
        if (Input.GetKeyDown(KeyCode.R))
        {
            TurnWheelRight();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            TurnWheelLeft();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ResetSteeringWheel();
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        //move steering wheel a little back and forth
        if (steer)
        {
            Quaternion rot = transform.rotation;
            float rand = Random.Range(0, rotationMax);
            if (rotateWheelLeft)
            {
                rot.z -= Time.deltaTime * rand;
                rotationAlternatorCounter -= rotationSpeed;
                if (rotationAlternatorCounter < 0)
                {
                    rotateWheelLeft = false;
                    rotateWheelRight = true;
                }
            }
            if (rotateWheelRight)
            {
                rot.z += Time.deltaTime * rand;
                rotationAlternatorCounter += rotationSpeed;
                if (rotationAlternatorCounter > 1)
                {
                    rotateWheelLeft = true;
                    rotateWheelRight = false;
                }
            }
            transform.rotation = rot;
        }

        if (turnLeft || turnRight || reset)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, quats, Time.deltaTime * 1);
        }
        
    }

    //Start steering
    public void StartSteering()
    {
        steer = true;
    }

    //Stop steering wheel
    public void StopSteering()
    {
        steer = false;
    }
    public void TurnWheelLeft()
    {
        quats = Quaternion.Euler(0, 0, 20f);
        StopSteering();
        turnLeft = true;
        turnRight = false;
        reset = false;
    }
    public void TurnWheelRight()
    {
        quats = Quaternion.Euler(0, 0, -20f);
        StopSteering();
        turnLeft = false;
        turnRight = true;
        reset = false;
    }
    public void ResetSteeringWheel()
    {
        quats = Quaternion.Euler(0, 0, 0);
        StartSteering();
        turnLeft = false;
        turnRight = false;
        reset = true;
    }


}
