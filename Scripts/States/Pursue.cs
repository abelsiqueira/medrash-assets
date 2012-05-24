using UnityEngine;
using System.Collections;

public class Pursue : State {
	
	private Transform transform;
	private Transform target;
	private Vector3 direction;
	
	private static Pursue instance = new Pursue();
	
	public static Pursue Instance() {
		return instance;
	}
	
	private Pursue () {
		state = states.enPursue;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(context.GetBaseSpeed());
	}
	
	public override void Execute (Entity context) {
		transform = context.transform;
		direction = context.GetMedrashPosition() - transform.position;
		direction.y = 0.0f;
		context.SetDirection(direction);
		context.SetRunAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
