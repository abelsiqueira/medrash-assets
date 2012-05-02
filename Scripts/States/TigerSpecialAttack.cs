using UnityEngine;
using System.Collections;

public class TigerSpecialAttack : State {
	
	private static TigerSpecialAttack instance = new TigerSpecialAttack();
	private Transform transform;
	private Vector3 direction;
	private Transform target;
	
	public static TigerSpecialAttack Instance() {
		return instance;
	}
	
	private TigerSpecialAttack () {
		state = states.enTigerSpecialAttack;
	}
	
	public override void Enter (Entity context) {
		transform = context.transform;
		direction = context.GetMedrashPosition() - transform.position;
		direction.y = 0.0f;
		context.SetSpeed(15.0f);
		context.SetDirection(direction);
	}
	
	public override void Execute (Entity context) {
		context.SetTigerSpecialAttackAnimation();
	}
	
	public override void Exit (Entity context) {
		
	}
}
