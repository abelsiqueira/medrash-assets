using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Bear : Entity {
	
	private int dyingDuration = 4, dyingTimer = 0;
	private int damageInstant = 7, damageTimer = 0;
	private int attackCooldown = 10, attackTimer = 0;
	private int idlePatrolChangeTime = 30, idlePatrolChangeTimer = 0;
	private int defenseDuration = 30, defenseTimer = 0;
	private int countdownAttacked = 0, attackedTime = 7;
	
	private float probabilityToDefend = 0.0f;
	
	bool needToAttack = false;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
    EntityStart();
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateBear());
		
		life = 9;
		damage = 15;
		baseSpeed = 9.0f;
		speed = baseSpeed;
		attackRadius = 5.0f;
		closeRadius = 20.0f;
		farRadius = 40.0f;
		canReceiveDamage = true;
	}
	
	public IEnumerator UpdateBear() {
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
			case State.states.enDefense:
				DefenseVerifications();
				break;
			case State.states.enAttacked:
				AttackedVerifications();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void IdleVerifications () {
		canReceiveDamage = true;
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
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}

	}
	
	private void PatrolVerifications () {
		canReceiveDamage = true;
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
			else
				fsm.ChangeState(Attacked.Instance());
			receivedDamage = false;
		}
	}
	
	private void PursueVerifications () {
		canReceiveDamage = true;
		float dist = DistanceToMainCharacter();		
		if (dist < attackRadius) {
			float r = Random.value;
			if (r < probabilityToDefend) {
				fsm.ChangeState(Defense.Instance());
			} else {
				needToAttack = true;
				fsm.ChangeState(Attack.Instance());
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
	
	private void AttackVerifications () {
		canReceiveDamage = true;
		damageTimer++;
		attackTimer++;
		if (needToAttack && (damageTimer >= damageInstant)) {
			fsm.ChangeState(Damage.Instance());
			fsm.RevertState();
			needToAttack = false;
			damageTimer = 0;
		} else if (attackTimer >= attackCooldown) {
			attackTimer = 0;
			damageTimer = 0;
			fsm.ChangeState(Pursue.Instance());
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else {
				fsm.ChangeState(Attacked.Instance());
				fsm.RevertState();
			}
			damageTimer = 0;
			attackTimer = 0;
			receivedDamage = false;
		}
	}
		
	private void DyingVerifications () {
		dyingTimer++;
		if (dyingTimer >= dyingDuration) {
			fsm.ChangeState(Idle.Instance());
		}
	}
	
	private void DefenseVerifications () {
		canReceiveDamage = false;
		defenseTimer++;
		if (defenseTimer >= defenseDuration) {
			defenseTimer = 0;
			fsm.ChangeState(Pursue.Instance());
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
