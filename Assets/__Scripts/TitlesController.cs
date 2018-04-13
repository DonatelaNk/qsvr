using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TitlesController : MonoBehaviour {
    public Transform Credit;
    public Font ClosingCreditsFont;
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
    //private Transform[] spawnPoints;
    //private Camera mainCamera;

    //float scaling = 5;
    //Vector3[] pts;


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
            {"TODO: ", "Closing Credits","..."},
            /*{"Mary-Helen", "Hadley Boyd",""},
            {"Ed", "Drew Moore",""},
            {"Sebastian", "Michael DeBartolo",""},
            {"written & directed by", "Illya Szilak",""},
            {"creative direction & unity development by", "Cyril Tsiboulski",""},
            {"executive producer", "T-Mo Bauer",""},
            {"associate producers", "Christopher E. Vroom\nDr. Yael Halaas",""},
            {"producer", "Kathleen Fox",""},
            {"director of photography", "Cory Allen",""},
            {"choreographer", "Dawn Saito",""},
            {"depthkit operator", "Supreet Mahanti",""},
            {"assistant Camera", "Tony Bartalini",""},
            {"sound mixer", "Laura Cunningham",""},
            {"360 video production & editing by", "Richard Hammer",""}, 
            
            {"depthkit post production", "Jillian Morrow",""},
            {"depthkit consultants", "Kyle Kukshtel\nAlexander Porter",""},
            {"3D artist", "Pat Goodwin",""},
            {"unity lighting design", "Pat Goodwin\nDale Henry",""},
            {"interactive development", "Chronosapien Interactive",""},
            {"color & finish", "theColourSpace",""},
            {"colorist & finishing artist", "Juan Salvo",""},
            {"VFX compositor", "Uroš Perišić",""},
            
            {"Post production sound services provided by Skywalker Sound, a Lucasfilm Ltd. Company, Marin County, California", "",""},
            {"sound designer", "Jeremy Bowker",""},
            {"sound lead", "Kevin Bolen",""},
            {"dialog editors", "Elizabeth Marston\nDanielle Dupre",""},
            {"VR audio intern", "Jonathan Do",""},
            {"set designers", "Nate Frieswyk\nD. Schuyler Burks\n(The Dad Shop)",""},
            {"haptics design", "Allen Yee",""},
            {"graphic design", "Willy Wong",""},

            {"special thanks", "Zeina Abi Assy, Opeyemi Olukemi, Tom Krueger, Alexander Porter and Kyle Kukshtel, Ebony Peay Ramirez (Oculus Launchpad), Heather Lee MacFarlane and Jacqueline Bosnjak (Q Department), Oscar Raby and Katy Morrison (VRTOV Studio), Alex Colgan and Angela Bermudo (LeapMotion), all of our generous Kickstarter supporters, the people of Missouri.",""},*/

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
        //mainCamera = Camera.main;
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
       // NameText.GetComponent<Text>().font = ClosingCreditsFont;
        SceneFinished = true;
        NameSelector();


        //SceneManager.GetComponent<SceneController>().MemoryDust.SetActive(true);
        /*pts = PointsOnSphere(titles.GetUpperBound(0));
        int i = 0;
        List<GameObject> uspheres = new List<GameObject>();
        foreach (Vector3 value in pts)
        {

            Transform CreditInstance;
            CreditInstance = Instantiate(Credit, value * scaling, Quaternion.identity);
            //parent it
            CreditInstance.transform.parent = transform;
            //CreditInstance.transform.position = targetPosition;
            //populate it
            CreditInstance.Find("CreditCanvas").gameObject.transform.Find("Title").GetComponent<Text>().text = titles[i, 0];
            CreditInstance.Find("CreditCanvas").gameObject.transform.Find("Name").GetComponent<Text>().text = titles[i, 1];
            CreditInstance.Find("CreditCanvas").gameObject.transform.Find("Subtitle").GetComponent<Text>().text = titles[i, 2];
            i++;
        }*/

    }
    /*Vector3[] PointsOnSphere(int n)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }*/
    /*void Spawn(GameObject credit)
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
    }*/
}
