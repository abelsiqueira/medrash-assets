using UnityEngine;
using System.Collections;

public class Patrol : State {
	
	private Transform transform;
	private Transform target;
	private Vector3 direction;
	
	private float rotateAngle;

	private static Patrol instance = new Patrol();
	
	public static Patrol Instance() {
		return instance;
	}
	
	private Patrol () {
		state = states.enPatrol;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(context.GetBaseSpeed());
		float r = Random.value;
		rotateAngle = 120.0f*(2*r - 1)*Mathf.PI/180.0f;
		if (r < 0.4)
			context.RotateBy(rotateAngle);
		else if (r < 0.8)
			context.RotateBy(-rotateAngle);
		direction = context.transform.forward;
		direction.y = 0.0f;
		if (Physics.Raycast(context.transform.position, direction, 1.0f));
			direction = -direction;
		context.SetDirection(direction);
	}
	
	public override void Execute (Entity context) {
		
	}
	
	public override void Exit (Entity context) {
	}
}
