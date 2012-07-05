using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Coyote : Entity {
	
	private int dyingDuration = 10, dyingTimer = 0;
	private int damageInstant = 7, damageTimer = 0;
	private int attackCooldown = 15, attackTimer = 0;
	private int countdownAttack = 0, attackDuration = 8;
	private int idlePatrolChangeTime = 30, idlePatrolChangeTimer = 0;
	private int countdownAttacked = 0, attackedTime = 7;
	
	private float probabilityToDefend = 0.0f;
	
	bool needToAttack = false;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		
		float aux = 1.3f;
		animation[attackAnimation.name].speed = aux;
		damageInstant = (int) (damageInstant/aux);
		attackDuration = (int) (attackDuration/aux);
		attackCooldown = (int) (attackCooldown/aux);
		
		life = 2;
		damage = 15;
		baseSpeed = 9.0f;
		speed = baseSpeed;
		attackRadius = 3.0f;
		closeRadius = 10.0f;
		farRadius = 15.0f;
		canReceiveDamage = true;
		
		EntityStart();
		scoreValue = 50;
		
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateCoyote());
	}
	
	public IEnumerator UpdateCoyote() {
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
			case State.states.enAttacked:
				AttackedVerifications();
				break;
			case State.states.enWait:
				WaitVerifications();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void IdleVerifications () {
		canReceiveDamage = true;
		idlePatrolChangeTimer++;
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (idlePatrolChangeTimer >= idlePatrolChangeTime) {
			fsm.ChangeState(Patrol.Instance());
			idlePatrolChangeTimer = 0;
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}

	}
	
	private void PatrolVerifications () {
		canReceiveDamage = true;
		idlePatrolChangeTimer++;
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (idlePatrolChangeTimer >= idlePatrolChangeTime) {
			fsm.ChangeState(Idle.Instance());
			idlePatrolChangeTimer = 0;
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}
	}
	
	private void PursueVerifications () {
		canReceiveDamage = true;
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();		
		if (dist < attackRadius) {
			if (countdownAttack == 0) {
				needToAttack = true;
				countdownAttack = attackCooldown;
				fsm.ChangeState(Attack.Instance());
			} else {
				fsm.ChangeState(Wait.Instance());
			}
		} else if (dist > farRadius) {
			fsm.ChangeState(Patrol.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}
	}
	
	private void WaitVerifications () {
		canReceiveDamage = true;
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();
		if (countdownAttack == 0) {
			needToAttack = true;
			countdownAttack = attackCooldown;
			fsm.ChangeState(Attack.Instance());
		} else if (dist > attackRadius) {
			fsm.ChangeState(Pursue.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}
	}
	
	private void AttackVerifications () {
		canReceiveDamage = true;
		damageTimer++;
		attackTimer++;
		countdownAttack--;
		if (needToAttack && (damageTimer >= damageInstant)) {
			fsm.ChangeState(Damage.Instance());
			fsm.RevertState();
			needToAttack = false;
			damageTimer = 0;
		} else if (attackTimer >= attackDuration) {
			attackTimer = 0;
			damageTimer = 0;
			fsm.ChangeState(Wait.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else {
				fsm.ChangeState(Attacked.Instance());
				//fsm.RevertState();
			}
			damageTimer = 0;
			attackTimer = 0;
			receivedDamage = false;
		}
	}
		
	private void DyingVerifications () {
		canBeAttacked = false;
		dyingTimer++;
		if (dyingTimer >= dyingDuration) {
			fsm.ChangeState(Idle.Instance());
		}
	}
		
	private void AttackedVerifications () {
		countdownAttacked++;
		if (countdownAttacked > attackedTime) {
			fsm.ChangeState(Pursue.Instance());
			countdownAttacked = 0;
		}
	}
}
