using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TitlesController : MonoBehaviour {
    public Transform Credit; 
    public GameObject TitleText; //Crew title
    public GameObject NameText; //Crew name
    public GameObject SubtitleText; //Crew subtitle

    private int CurrentCrewArrayPos = 0;
    private float delay = 0.5f;
    private float WaitTime = 3.5f;
    private float VanityCardDelay;
    private GameObject SceneManager;
    private bool SceneFinished = false;
    private GameObject QSVR_Title;
    private Transform[] spawnPoints;
    private Camera mainCamera;

    IEnumerator RollOpeningCreditsCoroutine;
    IEnumerator FadeInCoroutine;
    IEnumerator FadeOutCoroutine;

    string[,] titles;

    string[,] openingTitles = new string[,]
        {
            {"", "Hadley Boyd",""},
            {"", "Drew Moore",""},
            {"and", "Michael DeBartolo","as Sebastian"},
            {"sound mixer", "Laura Cunningham",""},
            {"360 video production & editing by", "Richard Hammer",""},
            {"depthkit operator", "Supreet Mahanti",""},
            {"director of photography", "Cory Allen",""},
            {"producer", "Kathleen Fox",""},
            {"associate producers", "Christopher E. Vroom\nDr. Yael Halaas",""},
            {"executive producer", "T-Mo Bauer",""},
            {"creative direction & unity development by", "Cyril Tsiboulski",""},
            {"written & directed by", "Illya Szilak",""},
            {"", "",""}, //Queerskins:\nA Love Story
        };

    string[,] closingCredits = new string[,]
        {
            
            {"TO DO:", "Closing Credits",""},
        };

    // Use this for initialization
    void Awake()
    {
        //empty out placeholders
        TitleText.GetComponent<Text>().text = "";
        NameText.GetComponent<Text>().text = "";
        SubtitleText.GetComponent<Text>().text = "";
        titles = openingTitles;

        //find the title png
        QSVR_Title = GameObject.Find("QSVR_Title");
        Color tmpColor = QSVR_Title.GetComponent<SpriteRenderer>().color;
        tmpColor.a = 0f;
        QSVR_Title.GetComponent<SpriteRenderer>().color = tmpColor;
    }
    void Start () {
        //Get the Fade Delay value from the Fade Object in/out script attached to teh Fancy game object
        SceneManager = GameObject.Find("SceneManager");
        VanityCardDelay = SceneManager.GetComponent<SceneController>().VanityCardDelay;
        //start title roll
        RollOpeningCreditsCoroutine = RollOpeningCredits(); // create an IEnumerator object
        StartCoroutine(RollOpeningCreditsCoroutine);
        mainCamera = Camera.main;
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
            //if this is the last array item and we're not rollwing closing credits show our title PNG
            if (!SceneFinished && CurrentCrewArrayPos == titles.GetUpperBound(0))
            {
                QSVR_Title.GetComponent<SpriteFader>().FadeInSprite();
            }
            string Title = titles[CurrentCrewArrayPos, 0];
            string Name = titles[CurrentCrewArrayPos, 1];
            string Subtitle = titles[CurrentCrewArrayPos, 2];
            //Fade in next crew name
            FadeInCoroutine = FadeIn(Title, Name, Subtitle); // Create the FadeIn IEnumerator object
            StartCoroutine(FadeInCoroutine);
            //also init the fadeout sequence
            FadeOutCoroutine = FadeOut(WaitTime + delay); // Create the FadeIn IEnumerator object
            StartCoroutine(FadeOutCoroutine);
            //incremenent titles position in array
            CurrentCrewArrayPos++;

        }
        else
        {
            if (!SceneFinished)
            {
                //Tell the scene manager, we're done with titles, only if this was
                //the openning credits roll
                EventManager.TriggerEvent("TitlesAreDone");
                Destroy(QSVR_Title);
            } else
            {
                //otherwise we are done done!
                //Show the replay button or something
                Application.Quit();
            }
           

            //Stop Coroutines
            StopCoroutine(FadeInCoroutine);
            StopCoroutine(FadeOutCoroutine);
            //Reset CurrentCrewArrayPos
            CurrentCrewArrayPos = 0;
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
            Credit.gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return null;
        }
    }

    IEnumerator FadeOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            Credit.gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return null;
        }
        NameSelector(); //pick the next name
    }

    IEnumerator RollOpeningCredits()
    {
        yield return new WaitForSeconds(VanityCardDelay+1);
        NameSelector();
    }

    public void RollClosingCredits()
    {
        titles = closingCredits;
        SceneFinished = true;
        NameSelector();
        SceneManager.GetComponent<SceneController>().MemoryDust.SetActive(true);
        // Call the Spawn function after a delay of 0 and then continue to call after the WaitTime amount of time.
       // InvokeRepeating("Spawn", 0, WaitTime);

       /* for (int i = 0; i <= titles.GetUpperBound(0); i++)
        {
            Transform CreditInstance;
            Vector3 center = mainCamera.GetComponent<Collider>().bounds.center;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(rotation_x, rotation_y, 0));
            CreditInstance = Instantiate(Credit, center, Quaternion.identity);
            //parent it
            CreditInstance.parent = transform;
            //populate it
            CreditInstance.Find("Title").GetComponent<Text>().text = titles[i, 0];
            CreditInstance.Find("Name").GetComponent<Text>().text = titles[i, 1];
            CreditInstance.Find("Subtitle").GetComponent<Text>().text = titles[i, 2];
        }*/

    }

    void Spawn(GameObject credit)
    {
        if (SceneFinished)
        {
            // ... exit the function.
            return;
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        // Create an instance of the credit at the randomly selected spawn point's position and rotation.
        Instantiate(credit, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
