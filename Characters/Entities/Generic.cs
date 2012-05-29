using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Generic : Entity {
	
	private int dyingDuration = 2, dyingTimer = 0;
	private int damageInstant = 10, damageTimer = 0;
	private int attackCooldown = 20, attackTimer = 0;
	private int idlePatrolChangeTime = 30, idlePatrolChangeTimer = 0;
	
	bool needToAttack = false;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateGeneric());
		
		life = 3;
		damage = 1;
		baseSpeed = 1.0f;
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
			case State.states.enPatrol:
				PatrolVerifications();
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
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void IdleVerifications () {
		return;
		idlePatrolChangeTimer++;
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (idlePatrolChangeTimer >= idlePatrolChangeTime) {
			fsm.ChangeState(Patrol.Instance());
			idlePatrolChangeTimer = 0;
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			receivedDamage = false;
		}

	}
	
	private void PatrolVerifications () {
		idlePatrolChangeTimer++;
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (idlePatrolChangeTimer >= idlePatrolChangeTime) {
			fsm.ChangeState(Idle.Instance());
			idlePatrolChangeTimer = 0;
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			receivedDamage = false;
		}
	}
	
	private void PursueVerifications () {
		float dist = DistanceToMainCharacter();
		if (dist < attackRadius) {
			needToAttack = true;
			fsm.ChangeState(Attack.Instance());
		} else if (dist > farRadius) {
			fsm.ChangeState(Patrol.Instance());
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
}
