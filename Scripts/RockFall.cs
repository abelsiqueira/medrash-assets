using UnityEngine;
using System.Collections;

public class RockFall : MonoBehaviour {
	
	
	GameObject medrash;
	public float damgeDist;
	public GameObject RockDust;
	
	void Start()
	{
		medrash	= GameObject.FindGameObjectWithTag("Player");
	}
	
	void OnCollisionEnter(Collision c)
	{
		if (Vector3.Distance(this.transform.position, medrash.transform.position) < damgeDist)
		{
			
		}
		RockDust.transform.position = this.transform.position;
		GameObject.Instantiate(RockDust);
		GameObject.Destroy(gameObject);
	}
}
