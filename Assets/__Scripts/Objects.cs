using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objects : MonoBehaviour
{

    public GameObject ObjectsSets;
    public GameObject Box;
    private GameObject currentSet;

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
        //Destroy(currentSet);
        //make sure object is not being held before destroying it
        //int numCantDestroy = 0;
        foreach (Transform child in currentSet.transform)
        {
            if (!child.GetComponent<OVRGrabbable>().isGrabbed)
            {
                Destroy(child.gameObject);     
            }else
            {
                //unparent it and assign it to the Box
                child.parent = Box.transform;
            }
        }
        Destroy(currentSet);
    }

}
