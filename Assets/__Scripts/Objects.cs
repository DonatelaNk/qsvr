using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objects : MonoBehaviour {

    public int MaxNumber = 6;
    public Rigidbody[] InteractiveObjects;
    private List<int> RandIndexes;
  
    // Use this for initialization
    void Start () {
        RandIndexes = new List<int>();
        GetRandomObjectSet();
    }
	
    public void GetRandomObjectSet()
    {
        GetRandIndexList();
        for (int i = 1; i < RandIndexes.Count; i++)
        {
            //Debug.Log(RandIndexes[i]);
            //polaroidInstance = Instantiate(polaroid, instancePosition, m_parent.rotation) as Rigidbody;

        }
    }
    
    public void GetRandIndexList()
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
        
    }
}
