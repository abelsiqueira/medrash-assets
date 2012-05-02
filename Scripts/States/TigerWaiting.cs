using UnityEngine;
using System.Collections;

public class TigerWaiting : State {
	
	private Transform transform;
	private Transform target;
	private Vector3 direction, tangent;
	
	private TigerWaiting () {
		state = states.enTigerWaiting;
	}
	
	private static TigerWaiting instance = new TigerWaiting();
	
	public static TigerWaiting Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
	}
		
	public override void Execute (Entity context) {
		transform = context.transform;
		direction = context.GetMedrashPosition() - transform.position;
		direction.y = 0.0f;
		context.SetDirection(direction);
		context.SetWaitAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
