using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Alligator : Entity {
	
	private int dyingDuration = 2, dyingTimer = 0;
	private int damageInstant = 10, damageTimer = 0;
	private int attackCooldown = 20, attackTimer = 0;
	private int countdownAttack = 0, attackDuration = 6;
	private int countdownAttacked = 0, attackedTime = 3;
	
	bool needToAttack = false;
	
	private float startPositionX;
	private float startPositionZ;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		
		
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		
		startPositionX = transform.position.x;
		startPositionZ = transform.position.z;
		returnPlace.x = startPositionX;
		returnPlace.z = startPositionZ;
		
		life = 10;
		maxLife = life;
		damage = 20;
		baseSpeed = 2.0f;
		speed = baseSpeed;
		attackRadius = 5.0f;
		closeRadius = 10.0f;
		farRadius = 30.0f;
		canReceiveDamage = true;
		
		EntityStart();
		
		scoreValue = 100;
		
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateGeneric());
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
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();
		if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
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
		float x = returnPlace.x - transform.position.x;
		float z = returnPlace.z - transform.position.z;
		float distToBase = Mathf.Sqrt(x*x + z*z);
		
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
		} else if (dist > farRadius || distToBase > farRadius) {
			fsm.ChangeState(ReturnToHome.Instance());
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
	
	private void ReturnToHomeVerifications()
	{
		float x = returnPlace.x - transform.position.x;
		float z = returnPlace.z - transform.position.z;
		float distToBase = Mathf.Sqrt(x*x + z*z);
		float dist = DistanceToMainCharacter();
		
		if (distToBase < attackRadius) {
			fsm.ChangeState(Idle.Instance());
		} else if (dist < closeRadius) {
			fsm.ChangeState(Pursue.Instance());
		}
	}
	
	private void WaitVerifications () {
		canReceiveDamage = true;
		if (countdownAttack > 0)
			countdownAttack--;
		float dist = DistanceToMainCharacter();
		if (countdownAttack < 0) {
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
	
	private void AttackedVerifications () {
		countdownAttacked++;
		if (countdownAttacked > attackedTime) {
			fsm.RevertState();
			//fsm.ChangeState(Pursue.Instance());
			countdownAttacked = 0;
		}
	}
}
