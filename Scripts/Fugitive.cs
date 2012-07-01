using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (Animation))]
public class Fugitive : MonoBehaviour {
	
	public List<Waypoint> waypoints;
	private float speed = 7.0f;
	private CharacterController myController;
	
	// Use this for initialization
	void Start () {
		animation.Play();
		myController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		animation.Play();
		
		if (waypoints.Count == 0)
			return;
		
		Vector3 direction = waypoints[0].transform.position - transform.position;
		direction.y = 0;
		if (direction.magnitude < 2.0f) {
			waypoints.RemoveAt(0);
			return;
		}
		direction = direction.normalized;
		transform.forward = direction;
		direction = speed*direction;
		myController.SimpleMove(direction);
		
		
	}
}
