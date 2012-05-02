// DO NOT USE. OLD
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Wolf : Entity {
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateWolf());
		
		life = 3;
		damage = 1;
		speed = 7.0f;
		attackRadius = 1.0f;
		closeRadius = 5.0f;
		farRadius = 10.0f;
	}
	
	public IEnumerator UpdateWolf() {
		while (true) {
			switch(fsm.GetCurrentState()) {
			case State.states.enIdle:
				IdleVerifications();
				break;
			case State.states.enFlee:
				FleeVerifications();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void IdleVerifications () {
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius)
			fsm.ChangeState(Flee.Instance());
	}
	
	private void FleeVerifications () {
		float dist = DistanceToMainCharacter();
		if (dist > farRadius)
			fsm.ChangeState(Idle.Instance());
	}
}
