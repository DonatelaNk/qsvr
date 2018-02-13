using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTurnHandle : MonoBehaviour 
{
	public HandleStates handleState
	{
		get;
		private set;
	}

	public enum HandleStates
	{
		Off,
		Active,
		MainHand,
		OffHand
	}

	public OVRGrabbable ovrGrabbable
	{
		get;
		private set;
	}

	private Animator animator;


	private void Awake()
	{
		ovrGrabbable = GetComponent<OVRGrabbable>();

		animator = GetComponent<Animator>();
	}


	public void SetHandleState(HandleStates state)
	{
		if (state == handleState)
			return;

		if (animator != null)
			animator.SetInteger("State", (int)state);
		
		handleState = state;
	}
}