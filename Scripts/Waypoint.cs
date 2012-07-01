using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	
	public Waypoint next;
	public GameObject runningEnemy;
	private MainCharacterController medrash;
	
	// Use this for initialization
	void Start () {
		runningEnemy = null;
		medrash = GameObject.FindGameObjectWithTag("Player").GetComponent<MainCharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public float GetDistance() {
		float distanceToNext = Mathf.Infinity;
		if (next)
			distanceToNext = (next.transform.position - transform.position).magnitude;
		if (runningEnemy) {
			float distanceToRunningEnemy = (runningEnemy.transform.position - transform.position).magnitude;
			if (distanceToNext < distanceToRunningEnemy)
				runningEnemy = null;
			else
				return distanceToRunningEnemy;
		}
		if (next)
			return distanceToNext + next.GetDistance();
		else {
			return 0.0f;
		}
	}
	
	public void OnTriggerEnter (Collider obj) {
		if (obj.tag == "Player") {
			medrash.SetClosestWaypoint(next);
		} else if (obj.name == "Fugitive") {
			runningEnemy = obj.gameObject;
		}
	}
}
