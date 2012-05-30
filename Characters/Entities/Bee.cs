using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class Bee : Entity {
	
	private int damageInstant = 10, damageTimer = 0;
	private int attackCooldown = 4, attackTimer = 0;
	public GameObject colmeia;
	private GameObject colmeiaInstance;
	bool needToAttack = false;
	
	
	public void Awake () {
		fsm = new FSM(this);
	}
	
	void Start () {
		fsm.SetCurrentState (Idle.Instance());
		controller = GetComponent<CharacterController>();
		StartCoroutine(fsm.UpdateFSM());
		StartCoroutine(UpdateGeneric());
		returnPlace.x = transform.position.x;
		returnPlace.z = transform.position.z;
		GameObject colmeiaInstance = (GameObject)Instantiate(colmeia);
		colmeiaInstance.transform.position = this.transform.position;
			
		life = 1;
		damage = 1;
		baseSpeed = 5.0f;
		speed = baseSpeed;
		attackRadius = 2.5f;
		closeRadius = 7.0f;
		farRadius = 20.0f;
		canReceiveDamage = false;
		canBeAttacked = false;
		
		EntityStart();
		Physics.IgnoreCollision(this.gameObject.collider, medrash.collider);
	}
	
	void OnDisable()
	{
		GameObject.Destroy(colmeiaInstance);
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
		}
	}
	
	private void ReturnToHomeVerifications() {
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
