using UnityEngine;
using System.Collections;

public class RockFall : MonoBehaviour {
	
	
	GameObject medrash;
	public float damgeDist;
	public GameObject RockDust;
	
	void Start()
	{
		medrash	= GameObject.FindGameObjectWithTag("Player");
		this.rigidbody.AddTorque(new Vector3(10, 1, 1));
	}
	
	void OnCollisionEnter(Collision c)
	{
		if (Vector3.Distance(this.transform.position, medrash.transform.position) < damgeDist)
		{
			medrash.GetComponent<MainCharacter>().DamageLifeStatus(10);
		}
		RockDust.transform.position = this.transform.position;
		GameObject.Instantiate(RockDust);
		GameObject.Destroy(gameObject);
	}
}
