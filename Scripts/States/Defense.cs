using UnityEngine;
using System.Collections;

public class Defense : State {
	
	private static Defense instance = new Defense();
	private Vector3 direction;
	private Transform target;
	
	public static Defense Instance() {
		return instance;
	}
	
	private Defense () {
		state = states.enDefense;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
	}
	
	public override void Execute (Entity context) {
	}
	
	public override void Exit (Entity context) {
	}
}
