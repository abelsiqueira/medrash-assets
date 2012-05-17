using UnityEngine;
using System.Collections;


public class TigerTired : State {
	
	private Transform transform;
	private Transform target;
	private Vector3 direction, tangent;
	
	private TigerTired () {
		state = states.enTigerTired;
	}
	
	private static TigerTired instance = new TigerTired();
	
	public static TigerTired Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(2.0f);
	}
		
	public override void Execute (Entity context) {
		context.SetWalkAnimation();
	}
	
	public override void Exit (Entity context) {
		
	}
}
