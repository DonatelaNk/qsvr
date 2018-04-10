using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Leap.Unity.Interaction;

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


    [Header("Page Audio")]
	public bool AutoplayRandMonologue = true;

	public AudioSource pageAudioSource;

	public List<PageTextureAudioType> pageTextureAudioList;

	[Header("Events")]
	public PageSetHandler onPageSet;

	[Serializable]
	public class PageSetHandler : UnityEvent<string> { }


	[Serializable]
	public class PageTextureAudioType
	{
		public Texture2D texture;
		public AudioClip audioClip;
	}



    private OVRGrabbable bookGrabbable;
    private InteractionBehaviour bookGrasped;
	private Vector3 pageTurnRightStartPositionLocal;
	private Vector3 pageTurnLeftStartPositionLocal;
	private float pageTurnDistance;
	private int currentPage;
	private float closeBookCurrentTime;
    private SoundManager SoundManager;


    private Coroutine setPageCoroutine;



	private void Start()
	{
		bookGrabbable = GetComponent<OVRGrabbable>(); //OVR
        bookGrasped = GetComponent<InteractionBehaviour>(); //LeapMotion
        SoundManager = GameObject.Find("SceneManager").GetComponent<SoundManager>();


        if (pageTurnRightHandle != null && pageTurnLeftHandle != null)
		{
			pageTurnRightStartPositionLocal = pageTurnRightHandle.transform.localPosition;
			pageTurnLeftStartPositionLocal = pageTurnLeftHandle.transform.localPosition;

			pageTurnDistance = Vector3.Distance(pageTurnRightHandle.transform.position, pageTurnLeftHandle.transform.position);
		}

		currentPage = (int)megaBookBuilder.page;

		closeBookCurrentTime = closeBookDelay;
		

		//RandomizeFirstPage();
	}

    void LateUpdate()
	{
		if (megaBookBuilder == null || pageTurnRightHandle == null || pageTurnLeftHandle == null)
			return;

		// Lock the axis of the grabbables to the book
		pageTurnRightHandle.transform.rotation = transform.rotation;
		pageTurnLeftHandle.transform.rotation = transform.rotation;

        // Set book colliders enabled based on the current page
        if (rightPageCollider)
			rightPageCollider.enabled = currentPage <= megaBookBuilder.NumPages;

		if (leftPageCollider)
			leftPageCollider.enabled = currentPage >= 0;

        if (currentPage >= 0)
        {
            leftPageCollider.enabled = true;
        }
        else
        {
            leftPageCollider.enabled = false;
        }
        

        // Reset to page 0 if the book isn't currently grabbed
        if (!bookGrabbable.isGrabbed && !bookGrasped.isGrasped)
		{
			/*if (pageTurnRightHandle.ovrGrabbable.isGrabbed || pageTurnLeftHandle.ovrGrabbable.isGrabbed ||
                pageTurnRightHandle.GetComponent<InteractionBehaviour>().isGrasped || 
                pageTurnLeftHandle.GetComponent<InteractionBehaviour>().isGrasped)
				closeBookCurrentTime = closeBookDelay;*/

			closeBookCurrentTime -= Time.deltaTime;

			if (closeBookWhenNotHeld && megaBookBuilder.page > -1 && closeBookCurrentTime <= 0)
			{
				if (setPageCoroutine != null)
					StopCoroutine(setPageCoroutine);

				setPageCoroutine = StartCoroutine(DoSetPage(-1, 0.25f));
                
            }
		}
		else
		{
			// Open the book to a randomized page (if closed)
			if (megaBookBuilder.page < 0)
			{
                int randomPage = 0;
                if (setPageCoroutine != null)
					StopCoroutine(setPageCoroutine);
                if(pageTextureAudioList.Count > 0)
                    randomPage = GetOddRandomPage(0, pageTextureAudioList.Count);
                
                setPageCoroutine = StartCoroutine(DoSetPage(randomPage, 0.5f));
                
            }

			closeBookCurrentTime = closeBookDelay;
		}
        
		// Page turning
		if (pageTurnRightHandle.ovrGrabbable.isGrabbed || pageTurnRightHandle.GetComponent<InteractionBehaviour>().isGrasped)
		{
			SetPageTurn(pageTurnRightHandle, pageTurnLeftHandle);
		}
		else if (pageTurnLeftHandle.ovrGrabbable.isGrabbed || pageTurnLeftHandle.GetComponent<InteractionBehaviour>().isGrasped)
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


    private void RandomizeFirstPage()
	{
        /*int randomPage = UnityEngine.Random.Range(0, megaBookBuilder.pages.Count);
		int randomSide = UnityEngine.Random.Range(0, 2);

		Texture2D firstPageTexture= megaBookBuilder.GetPageTexture(0, true);
		Texture2D randomPageTexture = megaBookBuilder.GetPageTexture(randomPage, Convert.ToBoolean(randomSide));

		megaBookBuilder.SetPageTexture(randomPageTexture, 0, true);
		megaBookBuilder.SetPageTexture(firstPageTexture, randomPage, Convert.ToBoolean(randomSide));*/
        
        int randomPage = UnityEngine.Random.Range(0, pageTextureAudioList.Count);
        //randomPage = randomPage * 2 + 1; //make sure it's always an odd number
        Debug.Log("Random page :" + randomPage);
        megaBookBuilder.page = randomPage;

    }

	private void UpdateHandleStates()
	{
		if (pageTurnRightHandle)
		{
			// Main hand
			if (pageTurnRightHandle.ovrGrabbable.isGrabbed || pageTurnRightHandle.GetComponent<InteractionBehaviour>().isGrasped)
			{
				pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.MainHand);
			}

			// Off hand
			else if (pageTurnLeftHandle.ovrGrabbable.isGrabbed || pageTurnLeftHandle.GetComponent<InteractionBehaviour>().isGrasped)
			{
				pageTurnRightHandle.SetHandleState(PageTurnHandle.HandleStates.OffHand);
			}

			// Off and Active
			else if (!pageTurnRightHandle.ovrGrabbable.isGrabbed && !pageTurnRightHandle.GetComponent<InteractionBehaviour>().isGrasped)
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
			if (pageTurnLeftHandle.ovrGrabbable.isGrabbed || pageTurnLeftHandle.GetComponent<InteractionBehaviour>().isGrasped)
			{
				pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.MainHand);
			}

			// Off hand
			else if (pageTurnRightHandle.ovrGrabbable.isGrabbed || pageTurnRightHandle.GetComponent<InteractionBehaviour>().isGrasped)
			{
				pageTurnLeftHandle.SetHandleState(PageTurnHandle.HandleStates.OffHand);
			}

			// Off and Active
			else if (!pageTurnLeftHandle.ovrGrabbable.isGrabbed && !pageTurnLeftHandle.GetComponent<InteractionBehaviour>().isGrasped)
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
				{
					foreach (string textureName in GetTextureNamesFromPageNumber(pageToSet))
						onPageSet.Invoke(textureName);
				}
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

		// play audio
		if (AutoplayRandMonologue && pageAudioSource != null)
		{
			PageTextureAudioType pageTextureAudioType = pageTextureAudioList.Find(p => p.texture == megaBookBuilder.GetPageTexture(currentPage, true));
            AutoplayRandMonologue = false;

            if (pageTextureAudioType != null && pageTextureAudioType.audioClip)
			{
				pageAudioSource.clip = pageTextureAudioType.audioClip;
                pageAudioSource.PlayDelayed(3);
			}
		}

		if (onPageSet != null)
		{
			foreach (string textureName in GetTextureNamesFromPageNumber((int)pageToSet))
				onPageSet.Invoke(textureName);
		}

		setPageCoroutine = null;
	}

	private List<string> GetTextureNamesFromPageNumber(int page)
	{
		List<string> textureNames = new List<string>();
		
		Texture2D leftPageTexture = page - 1 >= 0 && page - 1 < megaBookBuilder.pages.Count ? megaBookBuilder.GetPageTexture(page - 1, false) : null;
		Texture2D rightPageTexture = page >= 0 && page < megaBookBuilder.pages.Count ? megaBookBuilder.GetPageTexture(page, true) : null;

		string leftPageTextureName = leftPageTexture != null ? leftPageTexture.name : "";
		string rightPageTextureName = rightPageTexture != null ? rightPageTexture.name : "";

		if (leftPageTextureName != "")
			textureNames.Add(leftPageTextureName);

		if (rightPageTextureName != "")
			textureNames.Add(rightPageTextureName);

		return textureNames;
	}

    private int GetOddRandomPage(int min, int max)
    {
        int pageNumber;
        do
        {
            pageNumber = UnityEngine.Random.Range(min, max);
        } while (pageNumber % 2 == 0); //keep generating a rand number until we get an odd one
        return pageNumber;
    }

   
}