using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Tiger : Entity {
	
	private int attackTime = 30, pursueTime = 30;
	private int tiredTime = 80, restTime = 50;
	private int waitingTime = 60, enoughwaitingTime = 80;
	private int specialattackingTime = 6, attackingTime = 8;
	private int damageTime = 2, dyingTime = 20;
	private int attackedTime = 7;
	
	private int countdownAttack = 0, countdownPursue = 0;
	private int countdownTired = 0, countdownRest = 0;
	private int countdownWaiting = 0, countdownEnoughWaiting = 0;
	private int countdownSpecialAttacking = 0, countdownAttacking = 0;
	private int countdownDamage = 0, countdownDying = 0;
	private int countdownAttacked = 0;
	
	private float criticalValue = 0;
	
	bool needToAttack = false;
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	//medrash.GetComponent<MainCharacter>().DamageLifeStatus(float);
	
	void Start () {
		fsm.SetCurrentState (TigerRunAround.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateTiger());
		
		life = 10;
		criticalValue = life/3.0f;
		damage = 15;
		baseSpeed = 15.0f;
		animation[runAnimation.name].speed = baseSpeed*0.08f;
		speed = baseSpeed;
		attackRadius = 4.0f;
		closeRadius = 5.0f;
		farRadius = 10.0f;
		float atkSpeed = 2.0f;
		animation[attackAnimation.name].speed = 2.0f;
		attackingTime = (int) (attackAnimation.length*10.0f/atkSpeed);
		
		EntityStart();
		scoreValue = 500;
	}
	
	public IEnumerator UpdateTiger() {
		while (true) {
			switch(fsm.GetCurrentState()) {
			case State.states.enTigerRunAround:
				RunAroundVerifications();
				break;
			case State.states.enPursue:
				PursueVerifications();
				break;
			case State.states.enAttack:
				AttackVerifications();
				break;
			case State.states.enTigerTired:
				TigerTiredVerifications();
				break;
			case State.states.enTigerSpecialAttack:
				SpecialAttackVerifications();
				break;
			case State.states.enTigerWaiting:
				WaitingVerifications();
				break;
			case State.states.enDying:
				DyingVerifications();
				break;
			case State.states.enAttacked:
				AttackedVerifications();
				break;
			}
			
			if(medrash.GetComponent<MainCharacter>().IsAlive() != true)
			{
				life = 9;
			}
			
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private void RunAroundVerifications () {
		countdownAttack++;
		countdownTired++;
		if (life <= criticalValue)
			countdownWaiting++;
		if (countdownAttack >= attackTime) {
			countdownAttack = 0;
			countdownPursue = 0;
			fsm.ChangeState(Pursue.Instance());
		} else if (countdownTired >= tiredTime) {
			countdownTired = 0;
			countdownRest = 0;
			fsm.ChangeState(TigerTired.Instance());
		} else if (countdownWaiting >= waitingTime) {
			countdownWaiting = 0;
			countdownEnoughWaiting = 0;
			fsm.ChangeState(TigerWaiting.Instance());
		}
	}
	
	private void PursueVerifications () {
		float dist = DistanceToMainCharacter();
		countdownPursue++;
		if (dist < attackRadius) {
			fsm.ChangeState(Attack.Instance());
			countdownAttacking = 0;
			countdownDamage = 0;
			needToAttack = true;
		} else if (countdownPursue >= pursueTime) {
			fsm.ChangeState(TigerRunAround.Instance());
		}
	}
	
	private void AttackVerifications () {
		countdownAttacking++;
		countdownDamage++;
		if (needToAttack && (countdownDamage >= damageTime)) {
			fsm.ChangeState(Damage.Instance());
			fsm.RevertState();
			needToAttack = false;
		} else if (countdownAttacking >= attackingTime) {
			fsm.ChangeState(TigerRunAround.Instance());
		}
	}
	
	private void TigerTiredVerifications () {
		canReceiveDamage = true;
		countdownRest++;
		if (countdownRest >= restTime) {
			fsm.ChangeState(TigerRunAround.Instance());
			canReceiveDamage = false;
			receivedDamage = false;
		} else if (receivedDamage) {
			if (life <= 0)
				fsm.ChangeState(Dying.Instance());
			else
				fsm.ChangeState(Attacked.Instance());
			canReceiveDamage = false;
			receivedDamage = false;
		}
	}
	
	private void WaitingVerifications () {
		countdownEnoughWaiting++;
		if (countdownEnoughWaiting >= enoughwaitingTime) {
			fsm.ChangeState(TigerRunAround.Instance());
		} else {
			float dist = DistanceToMainCharacter();
			if (dist < attackRadius) {
				countdownSpecialAttacking = 0;
				fsm.ChangeState(Damage.Instance());
				fsm.ChangeState(TigerSpecialAttack.Instance());
			}
		}
	}
	
	private void SpecialAttackVerifications () {
		countdownSpecialAttacking++;
		if (countdownSpecialAttacking >= specialattackingTime) {
			countdownSpecialAttacking = 0;
			fsm.ChangeState(TigerRunAround.Instance());
		}
	}
	
	private void DyingVerifications () {
		countdownDying++;
		if (countdownDying >= dyingTime) {
			fsm.ChangeState(Idle.Instance());
		}
	}
	
	private void AttackedVerifications () {
		countdownAttacked++;
		if (countdownAttacked > attackedTime) {
			fsm.ChangeState(TigerRunAround.Instance());
			countdownAttacked = 0;
		}
	}
}
