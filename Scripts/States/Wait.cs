using UnityEngine;
using System.Collections;


public class Wait : State {
	
	private Wait () {
		state = states.enWait;
	}
	
	private static Wait instance = new Wait();
	
	public static Wait Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
	}
		
	public override void Execute (Entity context) {
		context.SetWaitAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
