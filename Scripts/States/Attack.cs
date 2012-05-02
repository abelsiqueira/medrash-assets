using UnityEngine;
using System.Collections;

public class Attack : State {
	
	private static Attack instance = new Attack();
	private Vector3 direction;
	private Transform target;
	
	public static Attack Instance() {
		return instance;
	}
	
	private Attack () {
		state = states.enAttack;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
	}
	
	public override void Execute (Entity context) {
		context.SetAttackAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
