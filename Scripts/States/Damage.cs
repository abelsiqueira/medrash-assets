using UnityEngine;
using System.Collections;

public class Damage : State {
	
	private static Damage instance = new Damage();
	private Vector3 direction;
	private Transform target;
	
	
	public static Damage Instance() {
		return instance;
	}
	
	private Damage () {
		state = states.enDamage;
	}
	
	public override void Enter (Entity context) {
		if (context.MainCharacterIsInDmgBox())
			context.DamageMedrash();
	}
	
	public override void Execute (Entity context) {
		context.SetAttackAnimation();
	}
	
	public override void Exit (Entity context) {
	}
}
