using UnityEngine;
using System.Collections;

public class Flee : State {
	
	private Transform transform;
	private Vector3 direction;
	
	private static Flee instance = new Flee();
	
	public static Flee Instance() {
		return instance;
	}
	
	private Flee () {
		state = states.enFlee;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(context.GetBaseSpeed());
		context.SetRunAnimation();
	}
	
	public override void Execute (Entity context) {
		transform = context.transform;
		direction = transform.position - context.GetMedrashPosition();
		direction.y = 0.0f;
		context.SetDirection(direction);
	}
	
	public override void Exit (Entity context) {
	}
}
