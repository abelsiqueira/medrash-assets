using UnityEngine;
using System.Collections;

public class Dying : State {
	
	private static Dying instance = new Dying();
	private Vector3 direction;
	private Transform target;
	
	
	public static Dying Instance() {
		return instance;
	}
	
	private Dying () {
		state = states.enDying;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(0.0f);
		context.MakeInvunerable();
		context.SetDyingAnimation();
		if (context.blood)
			GameObject.Instantiate(context.blood, context.transform.position + Vector3.up*1.5f, context.transform.rotation);
	}
	
	public override void Execute (Entity context) {
			
	}
	
	public override void Exit (Entity context) {
		if (context.dieExplosion)
			GameObject.Instantiate(context.dieExplosion, context.transform.position, context.transform.rotation);
		if (context.reward)
			GameObject.Instantiate(context.reward, context.transform.position, context.transform.rotation);
		context.AddScoreToMedrash();
		GameObject.Destroy(context.gameObject);
	}
}
