using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationComplete : MonoBehaviour {
    private GameObject a;
    private GameObject production;
    Color _color;

    private void Awake()
    {
        //find the game object
        a = GameObject.Find("a");
        //store color value in variable
        _color = a.GetComponent<Renderer>().material.color;
        //set the alpha to 0
        _color.a = 0f;
        //assign color to out game object
        a.GetComponent<Renderer>().material.color = _color;
        a.SetActive(false);

        //Repeat for the production tagline
        production = GameObject.Find("production");
        _color = production.GetComponent<Renderer>().material.color;
        _color.a = 0f;
        production.GetComponent<Renderer>().material.color = _color;
        production.SetActive(false);
    }
    // Use this for initialization
    void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {
      
    }

    public void ShowMoreText()
    {
        a.SetActive(true);
        production.SetActive(true);
    }

}
