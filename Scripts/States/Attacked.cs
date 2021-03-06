using UnityEngine;
using System.Collections;

public class Attacked : State {
	
	private Transform transform;
	private Transform target;
	private Vector3 direction, tangent;
	
	private Attacked () {
		state = states.enAttacked;
	}
	
	private static Attacked instance = new Attacked();
	
	public static Attacked Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
		if (context.blood)
			GameObject.Instantiate(context.blood, context.transform.position + Vector3.up*1.5f, context.transform.rotation);
	}
		
	public override void Execute (Entity context) {
		context.SetAttackedAnimation();
	}
	
	public override void Exit (Entity context) {
		
	}
}
