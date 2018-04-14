using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap.Unity.Interaction;

public class PosterUnrollController : MonoBehaviour
{
    [Header("Settings")]
    public bool closePosterWhenNotHeld = true;
    public float closePosterDelay = 3.0f;

    [Header("Poster Mega Bend")]
    public MegaBend megaBend;

    [Header("Page Turn Handles")]
    public PageTurnHandle posterUnrollHandle;
    public GameObject posterUnrollAnimation;

    [Header("Poster Textures")]
    public List<Texture2D> posterTextures;


    private OVRGrabbable posterGrabbable;
    private OVRGrabbable posterUnrollHandleGrabbable;
    private InteractionBehaviour posterGraspable;
    private InteractionBehaviour posterUnrollHandleGraspable;

    private Vector3 posterUnrollHandleStartPositionLocal;
    private float posterUnrollDistance;
    private float closePosterCurrentTime;

    private Coroutine setPosterRolledCoroutine;

    private const float posterGizmoPositionMin = -0.04f;
    private const float posterGizmoPositionMax = -0.432f;


    private void Start()
    {
        posterGrabbable = GetComponent<OVRGrabbable>();
        posterGraspable = GetComponent<InteractionBehaviour>();

        posterUnrollHandleGrabbable = posterUnrollAnimation.GetComponent<OVRGrabbable>();
        posterUnrollHandleGraspable = posterUnrollAnimation.GetComponent<InteractionBehaviour>();
        


        if (posterUnrollHandle != null)
        {
            posterUnrollHandleStartPositionLocal = posterUnrollHandle.transform.localPosition;

            posterUnrollDistance = Mathf.Abs(posterGizmoPositionMax - posterGizmoPositionMin);
        }

        closePosterCurrentTime = closePosterDelay;


        RandomizePosterTexture();
    }

    private void LateUpdate()
    {
        if (megaBend == null || posterUnrollHandle == null)
            return;

        // Lock the axis of the grabbable to the poster's axis
        posterUnrollHandle.transform.rotation = transform.rotation;


        //Show hand animation only of the poster is being held
        /*if (posterGrabbable.isGrabbed || posterGraspable.isGrasped)
        {
           posterUnrollAnimation.SetActive(true);
        }else
        {
            posterUnrollAnimation.SetActive(false);
        }*/
		// Reset the poster to closed if the poster isn't currently grabbed
		if ((!posterGrabbable.isGrabbed && !posterGraspable.isGrasped) || 
            (!posterUnrollHandleGrabbable.isGrabbed &&
            !posterUnrollHandleGraspable.isGrasped))
		{
			closePosterCurrentTime -= Time.deltaTime;
			
			if (closePosterWhenNotHeld && megaBend.gizmoPos.x < posterGizmoPositionMin && closePosterCurrentTime <= 0 && setPosterRolledCoroutine == null)
			{
				setPosterRolledCoroutine = StartCoroutine(DoSetPosterRolled(0.5f));
			}

			// Send the handle back to its original position
			posterUnrollHandle.transform.position = Vector3.MoveTowards(posterUnrollHandle.transform.position, transform.TransformPoint(posterUnrollHandleStartPositionLocal), Time.deltaTime * 2.5f);
		}
		else
		{
			closePosterCurrentTime = closePosterDelay;
		}

		// Poster unrolling
		if (posterUnrollHandle.ovrGrabbable.isGrabbed || posterUnrollHandleGraspable.isGrasped)
		{
			SetPosterUnroll(posterUnrollHandle);
		}

		// Handle states
		UpdateHandleStates();
	}


	private void RandomizePosterTexture()
	{
		if (posterTextures.Count > 0)
			megaBend.GetComponent<Renderer>().material.SetTexture("_MainTex", posterTextures[UnityEngine.Random.Range(0, posterTextures.Count)]);
	}

	private void UpdateHandleStates()
	{
		if (posterUnrollHandle)
		{
			// Main hand
			if (posterUnrollHandleGrabbable.isGrabbed ||
               posterUnrollHandleGraspable.isGrasped)
			{
				posterUnrollHandle.SetHandleState(PageTurnHandle.HandleStates.MainHand);
			}
			// Active
			else if (!posterUnrollHandleGrabbable.isGrabbed &&
                     !posterUnrollHandleGraspable.isGrasped)
			{
				posterUnrollHandle.SetHandleState(PageTurnHandle.HandleStates.Active);
			}
		}
	}

	private void SetPosterUnroll(PageTurnHandle pageTurnHandle)
	{
		// Stop the poster rolled coroutine if it's active
		if (setPosterRolledCoroutine != null)
		{
			StopCoroutine(setPosterRolledCoroutine);

			setPosterRolledCoroutine = null;
		}

		// If the poster unroll handle is on the inside of (above) the poster grabbable, lerp the mega bend offset value based on the poster unroll handle's position. 
		// Otherwise --
		float grabbableDistance = transform.InverseTransformPoint(pageTurnHandle.transform.position).y - posterUnrollHandleStartPositionLocal.y;

		bool isGrabbableInside = grabbableDistance > 0;
		//Debug.Log("grabbable distance: " + grabbableDistance + ", is grabbable inside: " + isGrabbableInside + ", @ " + Time.time);
		if (isGrabbableInside)
		{
			float currentHandleDistance = Mathf.Abs(grabbableDistance);
			float positionValueToSet = Mathf.Lerp(posterGizmoPositionMin, posterGizmoPositionMax, currentHandleDistance / posterUnrollDistance);
			
			megaBend.gizmoPos = new Vector3(positionValueToSet, megaBend.gizmoPos.y, megaBend.gizmoPos.z);
		}
		else
		{
			
		}
	}

	private IEnumerator DoSetPosterRolled(float rollDuration)
	{
		while (megaBend.gizmoPos.x < posterGizmoPositionMin)
		{
			megaBend.gizmoPos += new Vector3(Time.deltaTime / rollDuration, 0, 0);
			
			yield return null;
		}

		megaBend.gizmoPos = new Vector3(posterGizmoPositionMin, 0, 0);


		setPosterRolledCoroutine = null;
	}
}