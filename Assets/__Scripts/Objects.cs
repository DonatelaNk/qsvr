using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objects : MonoBehaviour
{

    public GameObject ObjectsSets;
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
        Destroy(currentSet);

    }

}
