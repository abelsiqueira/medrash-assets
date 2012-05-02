using UnityEngine;
using System.Collections;


public class Idle : State {
	
	private Idle () {
		state = states.enIdle;
	}
	
	private static Idle instance = new Idle();
	
	public static Idle Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
	}
		
	public override void Execute (Entity context) {
		context.SetIdleAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
