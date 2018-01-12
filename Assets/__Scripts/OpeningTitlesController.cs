using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningTitlesController : MonoBehaviour {
    public GameObject TitleText; //Crew title
    public GameObject NameText; //Crew name
    public GameObject SubtitleText; //Crew subtitle

    private GameObject Fancy;

    private int CurrentCrewArrayPos = 0;
    private float delay = 1.0f;
    private float WaitTime = 3.9f;
    private float VanityCardDelay;

    IEnumerator RollOpeningCreditsCoroutine;

    string[,] crew = new string[,]
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


    // Use this for initialization
    void Awake()
    {
        //empty out placeholders
        TitleText.GetComponent<Text>().text = "";
        NameText.GetComponent<Text>().text = "";
        SubtitleText.GetComponent<Text>().text = "";
    }
    void Start () {
        //Get the Fade Delay value from the Fade Object in/out script attached to teh Fancy game object
        Fancy = GameObject.Find("Fancy");
        VanityCardDelay = Fancy.GetComponent<FadeObjectInOut>().fadeDelay;
        //start title roll
        RollOpeningCreditsCoroutine = RollOpeningCredits(); // create an IEnumerator object
        StartCoroutine(RollOpeningCreditsCoroutine);
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    void NameSelector()
    {
        if (CurrentCrewArrayPos <= crew.GetUpperBound(0))
        {
            //for (int i = 0; i <= crew.GetUpperBound(0); i++)
            //{
            string Title = crew[CurrentCrewArrayPos, 0];
            string Name = crew[CurrentCrewArrayPos, 1];
            string Subtitle = crew[CurrentCrewArrayPos, 2];
            //incremenent crew position in array
            CurrentCrewArrayPos++;

            //Fade in next crew name
            StartCoroutine(FadeIn(Title, Name, Subtitle));

            //also init the fadeout sequence 
            StartCoroutine(FadeOut(WaitTime + delay));

            //Debug.Log(crew.GetUpperBound(0));
        }
        else
        {
            //Destroy the Titles Gameobject
            Destroy(this.gameObject);
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
