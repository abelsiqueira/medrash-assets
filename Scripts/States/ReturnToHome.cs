using UnityEngine;
using System.Collections;

public class ReturnToHome : State {

	private static ReturnToHome instance = new ReturnToHome();
	
	private Vector3 direction;
	
	public static ReturnToHome Instance() {
		return instance;
	}
	
	private ReturnToHome () {
		state = states.enReturnToHome;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(context.GetBaseSpeed());
	}
	
	public override void Execute (Entity context) {
		direction = context.ReturnPlace() - context.transform.position;
		direction.y = 0;
		context.SetDirection(direction);
		context.SetWalkAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
