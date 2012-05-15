using UnityEngine;
using System.Collections;

public class TigerRunAround : State {
	
	private float clockwise = 1.0f;
	private Transform transform;
	private Transform target;
	private Vector3 direction, tangent;
	
	private TigerRunAround () {
		state = states.enTigerRunAround;
	}
	
	private static TigerRunAround instance = new TigerRunAround();
	
	public static TigerRunAround Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(8.0f);
	}
		
	public override void Execute (Entity context) {
		transform = context.transform;
		direction = context.GetMedrashPosition() - transform.position;
		direction.y = 0.0f;
		tangent.x = clockwise*direction.z;
		tangent.z = -clockwise*direction.x;
		tangent.y = 0.0f;
		direction = tangent + 0.05f*(direction.magnitude - context.GetFarRadius()) * direction;
		//if (Physics.Raycast(transform.position, direction, 0.1f))
		//	clockwise *= -1;
		context.SetDirection(direction);
		context.SetRunAnimation();
	}
	
	public override void Exit (Entity context) {
		
	}
}
