using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlesController : MonoBehaviour {
    public GameObject TitleText; //Crew title
    public GameObject NameText; //Crew name
    public GameObject SubtitleText; //Crew subtitle

    private int CurrentCrewArrayPos = 0;
    private float delay = 1.0f;
    private float WaitTime = 3.9f;
    private float VanityCardDelay;
    private GameObject SceneManager;

    IEnumerator RollOpeningCreditsCoroutine;
    IEnumerator FadeInCoroutine;
    IEnumerator FadeOutCoroutine;

    string[,] titles;

    string[,] openingTitles = new string[,]
        {
            {"", "Hadley Boyd",""},
            {"", "Drew Moore",""},
            {"and", "Michael DeBartolo","as Sebastian"},
            {"sound mixer", "Laura Laidlaw",""},
            {"360 video by", "Richard Hammer",""},
            {"depthkit operator", "Supreet Mahanti",""},
            {"director of photography", "Cory Allen",""},
            {"producer", "Kathleen Fox",""},
            {"creative & technical direction by", "Cyril Tsiboulski",""},
            {"written & directed by", "Illya Szilak",""},
            {"", "Queerskins:\nA Love Story",""},
        };

    string[,] closingCredits = new string[,]
        {
            
            {"", "The End",""},
        };

    // Use this for initialization
    void Awake()
    {
        //empty out placeholders
        TitleText.GetComponent<Text>().text = "";
        NameText.GetComponent<Text>().text = "";
        SubtitleText.GetComponent<Text>().text = "";
        titles = openingTitles;
    }
    void Start () {
        //Get the Fade Delay value from the Fade Object in/out script attached to teh Fancy game object
        SceneManager = GameObject.Find("SceneManager");
        VanityCardDelay = SceneManager.GetComponent<SceneController>().VanityCardDelay;
        //start title roll
        RollOpeningCreditsCoroutine = RollOpeningCredits(); // create an IEnumerator object
        StartCoroutine(RollOpeningCreditsCoroutine);
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    void NameSelector()
    {
        if (CurrentCrewArrayPos <= titles.GetUpperBound(0))
        {
            //for (int i = 0; i <= titles.GetUpperBound(0); i++)
            //{
            string Title = titles[CurrentCrewArrayPos, 0];
            string Name = titles[CurrentCrewArrayPos, 1];
            string Subtitle = titles[CurrentCrewArrayPos, 2];
            //incremenent titles position in array
            CurrentCrewArrayPos++;

            //Fade in next crew name
            FadeInCoroutine = FadeIn(Title, Name, Subtitle); // Create the FadeIn IEnumerator object
            StartCoroutine(FadeInCoroutine);

            //also init the fadeout sequence
            FadeOutCoroutine = FadeOut(WaitTime + delay); // Create the FadeIn IEnumerator object
            StartCoroutine(FadeOutCoroutine);

            //Debug.Log(titles.GetUpperBound(0));
        }
        else
        {
            //Tell the scene manager, we're done with titles
            EventManager.TriggerEvent("TitlesAreDone");

            //Stop Coroutines and Destroy the Titles Gameobject
            StopCoroutine(FadeInCoroutine);
            StopCoroutine(FadeOutCoroutine);
            //Destroy(this.gameObject); 
        }
    }

    IEnumerator FadeIn(string CrewTitle, string CrewName, string Subtitle)
    {
        yield return new WaitForSeconds(delay);
        TitleText.GetComponent<Text>().text = CrewTitle;
        NameText.GetComponent<Text>().text = CrewName;
        SubtitleText.GetComponent<Text>().text = Subtitle;
        for (float f = 0f; f <= 1; f += 0.01f)
        {
            GetComponent<CanvasGroup>().alpha = f;
            yield return null;
        }
    }

    IEnumerator FadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            GetComponent<CanvasGroup>().alpha = f;
            yield return null;
        }
        NameSelector(); //pick the next name
    }

    IEnumerator RollOpeningCredits()
    {
        yield return new WaitForSeconds(VanityCardDelay+1);
        NameSelector();
        StopCoroutine(RollOpeningCreditsCoroutine);
    }
}
