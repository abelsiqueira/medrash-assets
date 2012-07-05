using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (Animation))]
public class Fugitive : MonoBehaviour {
	
	public Waypoint waypoint;
	private float speed = 5.5f;
	private CharacterController myController;
	private Vector3 initialPosition;
	private Waypoint initialWaypoint;
	
	// Use this for initialization
	void Start () {
		animation.Play();
		myController = GetComponent<CharacterController>();
		initialPosition = transform.position;
		initialWaypoint = waypoint;
	}
	
	public void Reset () {
		transform.position = initialPosition;
		waypoint = initialWaypoint;
	}
	
	// Update is called once per frame
	void Update () {
		animation.Play();
		
		if (!waypoint) {
			Destroy(this.gameObject);
			return;
		}
		
		Vector3 direction = waypoint.transform.position - transform.position;
		direction.y = 0;
		if (direction.magnitude < 2.0f) {
			waypoint = waypoint.next;
			return;
		}
		direction = direction.normalized;
		transform.forward = direction;
		direction = speed*direction;
		myController.SimpleMove(direction);
		
		
	}
}
