using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PageTurnController : MonoBehaviour 
{
	[Header("Settings")]
	[Tooltip("The duration of a full page turn when a page is released.")]
	public float pageTurnDuration = 0.5f;

	[Space(10)]
	public bool closeBookWhenNotHeld = true;
	public float closeBookDelay = 3.0f;

	[Header("Book Script")]
	public MegaBookBuilder megaBookBuilder;

	[Header("Page Turn Handles")]
	public PageTurnHandle pageTurnRightHandle;
	public PageTurnHandle pageTurnLeftHandle;

	[Header("Colliders")]
	public Collider rightPageCollider;
	public Collider leftPageCollider;

    public Collider pageTurnRightHandleCollider;
    public Collider pageTurnLeftHandleCollider;

    [Header("Events")]
	public PageSetHandler onPageSet;

	[Serializable]
	public class PageSetHandler : UnityEvent<int> { }


	private OVRGrabbable bookGrabbable;
	private Vector3 pageTurnRightStartPositionLocal;
	private Vector3 pageTurnLeftStartPositionLocal;
	private float pageTurnDistance;
	private int currentPage;
	private float closeBookCurrentTime;

	private Coroutine setPageCoroutine;


	private void Start()
	{
		bookGrabbable = GetComponent<OVRGrabbable>();

		if (pageTurnRightHandle != null && pageTurnLeftHandle != null)
		{
			pageTurnRightStartPositionLocal = pageTurnRightHandle.transform.localPosition;
			pageTurnLeftStartPositionLocal = pageTurnLeftHandle.transform.localPosition;

			pageTurnDistance = Vector3.Distance(pageTurnRightHandle.transform.position, pageTurnLeftHandle.transform.position);
		}

		currentPage = (int)megaBookBuilder.page;

		closeBookCurrentTime = closeBookDelay;
	}

	private void LateUpdate()
	{
		if (megaBookBuilder == null || pageTurnRightHandle == null || pageTurnLeftHandle == null)
			return;

        // Lock the axis of the grabbables to the book
        pageTurnRightHandle.transform.rotation = transform.rotation;
        pageTurnLeftHandle.transform.rotation = transform.rotation;

        // Set book colliders enabled based on the current page
        //if (rightPageCollider)
        //rightPageCollider.enabled = currentPage <= megaBookBuilder.NumPages;

        //if (leftPageCollider)
        //leftPageCollider.enabled = currentPage >= 0;


        if (currentPage >= 0)
        {
            leftPageCollider.enabled = true;
        } else
        {
            leftPageCollider.enabled = false;
        }


		// Reset to page 0 if the book isn't currently grabbed
		if (!bookGrabbable.isGrabbed)
		{
			closeBookCurrentTime -= Time.deltaTime;

			if (closeBookWhenNotHeld && megaBookBuilder.page > -1 && closeBookCurrentTime <= 0)
			{
				if (setPageCoroutine != null)
					StopCoroutine(setPageCoroutine);

				setPageCoroutine = StartCoroutine(DoSetPage(-1, 0.25f));
                

            }
           
        }
		else
			closeBookCurrentTime = closeBookDelay;


        if (bookGrabbable.isGrabbed)
        {
            

            if (pageTurnRightHandleCollider.enabled == false)
            {
                pageTurnRightHandleCollider.enabled = true;
                pageTurnLeftHandleCollider.enabled = true;
            }
        } else
        {
            if (pageTurnRightHandleCollider.enabled == true)
            {
                pageTurnRightHandleCollider.enabled = false;
                pageTurnLeftHandleCollider.enabled = false;
            }
        }

        // Page turning
        if (pageTurnRightHandle.ovrGrabbable.isGrabbed)
		{
			SetPageTurn(pageTurnRightHandle, pageTurnLeftHandle);
		}
		else if (pageTurnLeftHandle.ovrGrabbable.isGrabbed)
		{
			SetPageTurn(pageTurnLeftHandle, pageTurnRightHandle);
		}
		else
		{
			// Send the grabbables back to their original positions
			pageTurnRightHandle.transform.position = Vector3.MoveTowards(pageTurnRightHandle.transform.position, transform.TransformPoint(pageTurnRightStartPositionLocal), Time.deltaTime * 2.5f);
			pageTurnLeftHandle.transform.position = Vector3.MoveTowards(pageTurnLeftHandle.transform.position, transform.TransformPoint(pageTurnLeftStartPositionLocal), Time.deltaTime * 2.5f);

			// Set the page
			if (Mathf.Approximately(megaBookBuilder.page, currentPage) == false && setPageCoroutine == null)
				setPageCoroutine = StartCoroutine(DoSetPage());
		}

		// Handle states
		UpdateHandleStates();
	}


	private void UpdateHandleStates()
	{
		if (pageTurnRightHandle)
		{
			// Main hand
			if (pageTurnRightHandle.ovrGrabbable.isGrabbed)
			{
				pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.MainHand);
			}

			// Off hand
			else if (pageTurnLeftHandle.ovrGrabbable.isGrabbed)
			{
				pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.OffHand);
			}

			// Off and Active
			else if (!pageTurnRightHandle.ovrGrabbable.isGrabbed)
			{
				if (megaBookBuilder.page >= megaBookBuilder.NumPages)
					pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.Off);
				else
					pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.Active);
			}
		}

		if (pageTurnLeftHandle)
		{
			// Main hand
			if (pageTurnLeftHandle.ovrGrabbable.isGrabbed)
			{
				pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.MainHand);
			}

			// Off hand
			else if (pageTurnRightHandle.ovrGrabbable.isGrabbed)
			{
				pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.OffHand);
			}

			// Off and Active
			else if (!pageTurnLeftHandle.ovrGrabbable.isGrabbed)
			{
				if (megaBookBuilder.page < 0)
					pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.Off);
				else
					pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.Active);
			}
		}
	}

	private void SetPageTurn(PageTurnHandle pageTurnMainHand, PageTurnHandle pageTurnOffHand)
	{
		// Stop the set page coroutine if it's active
		if (setPageCoroutine != null)
		{
			StopCoroutine(setPageCoroutine);

			setPageCoroutine = null;
		}

		// If the page turn main hand grabbable is on the inside of the page turn off hand grabbable, lerp the page value on the book builder script based on the main hand grabbable's position. 
		// Otherwise set the page value to the next page.
		float grabbableDistance = pageTurnOffHand.transform.InverseTransformPoint(pageTurnMainHand.transform.position).x;

		bool isGrabbableInside = pageTurnMainHand == pageTurnRightHandle ?
			grabbableDistance > 0 :
			grabbableDistance < 0;

		int pageToSet = pageTurnMainHand == pageTurnRightHandle ? currentPage + 1 : currentPage - 1;
		
		if (isGrabbableInside)
		{
			float currentPageTurnDistance = Mathf.Abs(grabbableDistance);
			float pageValueToSet = Mathf.Lerp(pageToSet, currentPage, currentPageTurnDistance / pageTurnDistance);
			
			megaBookBuilder.page = pageValueToSet;
		}
		else
		{
			if (Mathf.Approximately(pageToSet, megaBookBuilder.page) == false)
			{
				megaBookBuilder.page = pageToSet;

				if (onPageSet != null)
					onPageSet.Invoke(pageToSet);
			}
		}
	}

	private IEnumerator DoSetPage(float? page = null, float? pageTurnDuration = null)
	{
		float pageToSet = page.HasValue ? page.Value : Mathf.Clamp(Mathf.Round(megaBookBuilder.page), -1, megaBookBuilder.NumPages);
		float pageTurnDurationCurrent = pageTurnDuration.HasValue ? pageTurnDuration.Value : this.pageTurnDuration;
		int pageTurnDirection = megaBookBuilder.page < pageToSet ? 1 : -1;
		int otherPage = currentPage - pageTurnDirection;
		
		while (Mathf.Approximately(megaBookBuilder.page, pageToSet) == false)
		{
			megaBookBuilder.page += Time.deltaTime / pageTurnDurationCurrent * pageTurnDirection;

			megaBookBuilder.page = Mathf.Clamp(megaBookBuilder.page, 
				pageToSet < otherPage ? pageToSet : otherPage, 
				pageToSet > otherPage ? pageToSet : otherPage);
			
			yield return null;
		}

		megaBookBuilder.page = currentPage = (int)pageToSet;

		if (onPageSet != null)
			onPageSet.Invoke((int)pageToSet);

		setPageCoroutine = null;
	}
}