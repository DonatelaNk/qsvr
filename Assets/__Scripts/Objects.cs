using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objects : MonoBehaviour {

    public GameObject ObjectsSets;
    private GameObject currentSet;
    

    // Use this for initialization
    void Start () {
       
        GetRandomObjectSet();
    }

    private void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            GetRandomObjectSet();
        }

        //Hit Escape to exit (build mode only)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyObjectSet();
        }
    }

    public void GetRandomObjectSet()
    {
        List<GameObject> sets = new List<GameObject>();
        foreach (Transform child in ObjectsSets.transform)
        {
            sets.Add(child.gameObject);
        }
        int currentRandIndex = Random.Range(0, sets.Count);
        currentSet = sets[currentRandIndex];
        currentSet.SetActive(true);
    }

    public void DestroyObjectSet()
    {
        Destroy(currentSet);
      
    }


    /* NOT USED // Object auto generation */

    /*public void GetRandomObjectSet()
    {
        GetRandIndexList();
        for (int i = 1; i < RandIndexes.Count; i++)
        {
            //Debug.Log(RandIndexes[i]);
            Rigidbody obj;
            //generate a unique position to prevent things instantiating over each other and causing a raucous
            Vector3 instancePosition = new Vector3(ObjectContainer.position.x, ObjectContainer.position.y + 0.06f, ObjectContainer.position.z);
            obj = Instantiate(InteractiveObjects[RandIndexes[i]], instancePosition, ObjectContainer.rotation) as Rigidbody;
            //Assign parent
            obj.transform.parent = ObjectContainer;
        }
    }*/


    /*public void GetRandIndexList()
    {
        while (RandIndexes.Count < MaxNumber)
        {
            for (int i = 1; i < InteractiveObjects.Length; i++)
            {
                //generate a random index
                int RandIndex = Random.Range(1, InteractiveObjects.Length);
                if (!RandIndexes.Contains(RandIndex))
                {
                    RandIndexes.Add(RandIndex);
                }
            }
        }

    }*/
}
