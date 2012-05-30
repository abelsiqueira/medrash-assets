using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Gardain : MonoBehaviour 
{
	
	public AnimationClip stanceAnimation;
	public AnimationClip lookUpAnimation;
	public AnimationClip pointTheWayAnimation;
	public AnimationClip sayNoAnimation;
	public AnimationClip putHandInHeadAnimation;
	
	private Animation animation;
	
	private float stanceAnimationSpeed = 1.0f;
	private float lookUpAnimationSpeed = 1.0f;
	private float pointTheWayAnimationSpeed = 1.0f;
	private float sayNoAnimationSpeed = 1.0f;
	private float putHandInHeadAnimationSpeed = 1.0f;
	
	private GameObject medrash;
	private float minDist = 5.0f;
	AnimationClip[] animationArray;
	private bool canPlayAnimations;
	
	void Awake() 
	{				
		animation = GetComponent<Animation>();
		
		if(!animation) Debug.Log("Gardain doesn't have animations.");
	
		if(!stanceAnimation) 
		{
			animation = null;
			Debug.Log("No Stance animation found. Turning off animations.");
		}
		if(!lookUpAnimation) 
		{
			animation = null;
			Debug.Log("No LookUp animation found. Turning off animations.");
		}
		if(!pointTheWayAnimation) 
		{
			animation = null;
			Debug.Log("No PointTheWay animation found. Turning off animations.");
		}
		if (!sayNoAnimation)
		{
			animation = null;
			Debug.Log("No SayNo animation found. Turning off animations.");
		}
		if (!putHandInHeadAnimation)
		{
			animation = null;
			Debug.Log("No PutHandInHead animation found. Turning off animations.");
		}
		
		medrash = GameObject.FindGameObjectWithTag("Player");
		StartCoroutine(VerifyDistanceToMainCharacter());
		
		animationArray = new AnimationClip[4];
		animationArray[0] = lookUpAnimation;
		animationArray[1] = pointTheWayAnimation;
		animationArray[2] = sayNoAnimation;
		animationArray[3] = putHandInHeadAnimation;
	}
	
	void PlayRandomAnimation()
	{
		if (canPlayAnimations)
		{
			System.Random random = new System.Random();
			int i = random.Next(animationArray.Length);
			AnimationClip currentAnimation = animationArray[i];
			animation[currentAnimation.name].wrapMode = WrapMode.Clamp;
			animation[currentAnimation.name].layer = 1;
			animation.Play(currentAnimation.name);
			canPlayAnimations = false;
		}
	}
	
	void PlayStanceAnimation()
	{
		animation.Play(stanceAnimation.name);	
	}
	
	IEnumerator VerifyDistanceToMainCharacter()
	{
		float dist;
		while(true) 
		{
			dist = DistanceToMainCharacter();
			if (dist < minDist)
			{
				PlayRandomAnimation();
			} 
			else
			{
				PlayStanceAnimation();
				canPlayAnimations = true;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private float DistanceToMainCharacter()
	{
		Vector3 dist = medrash.transform.position - transform.position;
		return dist.magnitude;
	}
}
