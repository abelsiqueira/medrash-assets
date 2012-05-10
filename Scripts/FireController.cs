using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
	private GameObject medrash;	
	private MainCharacterController controller;
	public float minDist = 3.0f;
	
	void Start ()
	{
		medrash = GameObject.FindGameObjectWithTag("Player");
		controller = medrash.GetComponent<MainCharacterController>();
		StartCoroutine(FireUpdate());
	}
	
	IEnumerator FireUpdate ()
	{
		float dist;
	
		while(true) 
		{
			dist = DistanceToMainCharacter();
			if (dist < minDist)
			{
				controller.CanLightTorch(true);
			} 
			else 
			{
				controller.CanLightTorch(false);
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