using UnityEngine;
using System.Collections;

public class TigerRunAround : State {
	
	private float clockwise = 1.0f;
	private Transform transform;
	private Transform target;
	private Vector3 direction, tangent;
	
	private TigerRunAround () {
		state = states.enTigerRunAround;
	}
	
	private static TigerRunAround instance = new TigerRunAround();
	
	public static TigerRunAround Instance() {
		return instance;
	}
	
	public override void Enter (Entity context) {
		context.SetSpeed(context.GetBaseSpeed());
	}
		
	public override void Execute (Entity context) {
		CharacterController controller = context.GetComponent<CharacterController>();
		transform = controller.transform;
		//transform = context.transform;
		direction = context.GetMedrashPosition() - transform.position;
		direction.y = 0.0f;
		
		tangent.x = clockwise*direction.z;
		tangent.z = -clockwise*direction.x;
		tangent.y = 0.0f;
		
		if (context.DistanceToMainCharacter() > context.GetCloseRadius())
			direction = tangent + 0.01f*(direction.magnitude - context.GetFarRadius()) * direction;
		else
			direction = tangent + 0.1f*(direction.magnitude - context.GetFarRadius()) * direction;
		
		RaycastHit hitInfo;
		if (Physics.Raycast(transform.position + controller.center, direction, out hitInfo, 3.0f)) {
			clockwise *= -1;
			direction *= -1;
		}
		context.SetDirection(direction);
		context.SetRunAnimation();
	}
	
	public override void Exit (Entity context) {
		
	}
}
