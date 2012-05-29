using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Alligator : Entity {
	
	private int dyingDuration = 2, dyingTimer = 0;
	private int damageInstant = 10, damageTimer = 0;
	private int attackCooldown = 20, attackTimer = 0;
	
	bool needToAttack = false;
	
	private float startPositionX;
	private float startPositionZ;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateGeneric());
		
		startPositionX = transform.position.x;
		startPositionZ = transform.position.z;
		returnPlace.x = startPositionX;
		returnPlace.z = startPositionZ;
		
		life = 3;
		damage = 20;
		baseSpeed = 2.0f;
		speed = baseSpeed;
		attackRadius = 1.0f;
		closeRadius = 3.0f;
		farRadius = 6.0f;
		canReceiveDamage = true;
		
		EntityStart();
	}
	
	public IEnumerator UpdateGeneric() {
		while (true) {
			switch(fsm.GetCurrentState()) {
			case State.states.enIdle:
				IdleVerifications();
				break;
			case State.states.enPursue:
				PursueVerifications();
				break;
			case State.states.enAttack:
				AttackVerifications();
				break;
			case State.states.enDying:
				DyingVerifications();
				break;
			case State.states.enReturnToHome:
				ReturnToHomeVerifications();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void IdleVerifications () {
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			receivedDamage = false;
		}

	}
	
	private void PursueVerifications () {
		float dist = DistanceToMainCharacter();
		
		float x = returnPlace.x - transform.position.x;
		float z = returnPlace.z - transform.position.z;
		float distToBase = Mathf.Sqrt(x*x + z*z);
		
		if (dist < attackRadius) {
			needToAttack = true;
			fsm.ChangeState(Attack.Instance());
		} else if (distToBase > farRadius) {
			fsm.ChangeState(ReturnToHome.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			receivedDamage = false;
		}
	}
	
	private void AttackVerifications () {
		damageTimer++;
		attackTimer++;
		if (needToAttack && (damageTimer >= damageInstant)) {
			fsm.ChangeState(Damage.Instance());
			fsm.RevertState();
			needToAttack = false;
		} else if (attackTimer >= attackCooldown) {
			attackTimer = 0;
			fsm.ChangeState(Pursue.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			damageTimer = 0;
			attackTimer = 0;
			fsm.ChangeState(Pursue.Instance());
			receivedDamage = false;
		}
	}
		
	private void DyingVerifications () {
		dyingTimer++;
		if (dyingTimer >= dyingDuration) {
			fsm.ChangeState(Idle.Instance());
		}
	}
	
	private void ReturnToHomeVerifications()
	{
		float x = returnPlace.x - transform.position.x;
		float z = returnPlace.z - transform.position.z;
		float distToBase = Mathf.Sqrt(x*x + z*z);
		float dist = DistanceToMainCharacter();
		
		if (distToBase < 1.0f) {
			fsm.ChangeState(Idle.Instance());
		} else if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		}
	}
}
