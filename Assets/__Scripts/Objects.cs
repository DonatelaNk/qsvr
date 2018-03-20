using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objects : MonoBehaviour {

    public int MaxNumber = 6;
    public Transform ObjectContainer;
    public Rigidbody[] InteractiveObjects;
    public Rigidbody[] InteractiveObjectsSet1;
    public Rigidbody[] InteractiveObjectsSet2;
    public Rigidbody[] InteractiveObjectsSet3;
    private List<int> RandIndexes;
    
  
    // Use this for initialization
    void Start () {
        RandIndexes = new List<int>();
       // GetRandomObjectSet();
    }
	
    public void GetRandomObjectSet()
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
