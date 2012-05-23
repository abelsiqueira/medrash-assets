using UnityEngine;
using System.Collections;

public class FlameController : MonoBehaviour
{
	private GameObject medrash;	
	private MainCharacterController controller;
	public float minDist = 1.0f;
	
	void Start ()
	{
		medrash = GameObject.FindGameObjectWithTag("Player");
		controller = medrash.GetComponent<MainCharacterController>();
		StartCoroutine(FlameUpdate());
	}
	
	IEnumerator FlameUpdate ()
	{
		float dist;
	
		while(true) 
		{
			dist = DistanceToMainCharacter();
			if (dist < minDist)
			{
				medrash.GetComponent<MainCharacter>().DamageLifeStatus(1);
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