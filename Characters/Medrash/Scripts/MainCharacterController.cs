using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class MainCharacterController : MonoBehaviour 
{

	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip deathAnimation;
	//public AnimationClip defenseAnimation;
	//public AnimationClip interactAnimation;
	
	private Animation animation;
	
	private float walkMaxAnimationSpeed = 0.75f;
	private float trotMaxAnimationSpeed = 1.0f;
	private float runMaxAnimationSpeed = 1.2f;
	private float landAnimationSpeed = 1.0f;
	private float attackAnimationSpeed = 1.4f;
	private float deathAnimationSpeed = 1.0f;
	//public float defenseAnimationSpeed = 1.0f;
	//public float interactAnimationSpeed = 1.0f;	
	private float baseAttackDuration = 0.8f;

	enum CharacterState 
	{
		Idle,
		Walking,
		Trotting,
		Running,
		Attacking,
		Defending,
		Interacting,
		Dead
	}

	private MainCharacter mainCharacter;
	private CharacterState characterState;

	public float walkSpeed = 4.0f;
	private float delayAttackValue;
	private float attackDuration;
	
	public float trotSpeed = 5.0f;
	
	public float runSpeed = 8.0f;

	public float inAirControlAcceleration = 3.0f;

	public float gravity = 20.0f;
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 500.0f;
	public float trotAfterSeconds = 3.0f;

	public bool canRun = true;
	public bool canAttack = true;
	public bool canMove = true;
	private bool canLightTorch = false;
	private bool falling = false;
	
	private float groundedTimeout = 0.25f;

	private float lockCameraTimer = 0.0f;

	private Vector3 moveDirection = Vector3.zero;
	private float verticalSpeed = 0.0f;

	private float moveSpeed = 0.0f;

	private CollisionFlags collisionFlags; 

	private bool movingBack= false;
	private bool isMoving= false;
	private float walkTimeStart = 0.0f;
	private Vector3 inAirVelocity= Vector3.zero;

	private float lastGroundedTime = 0.0f;

	private bool isControllable = true;
	
	private float attackCooldownValue = 0.0f;
	private float fallingDamageMultiplier = 20f;
	private float fallingHeightThreshold = 12.0f;
	private float fallStartLevel;

	void Awake()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		mainCharacter = GetComponent<MainCharacter>();
		animation = GetComponent<Animation>();
		
		if(!animation) Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
		if(!idleAnimation) 
		{
			animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(!walkAnimation) 
		{
			animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(!runAnimation) 
		{
			animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!attackAnimation)
		{
			animation = null;
			Debug.Log("No attack animation found. Turning off animations.");
		}
		if (!deathAnimation)
		{
			animation = null;
			Debug.Log("No death animation found. Turning off animations.");
		}
		/*if (!defenseAnimation)
		{
			animation = null;
			Debug.Log("No defend animation found. Turning off animations.");
		}*/
		
		attackDuration = baseAttackDuration/attackAnimationSpeed;
		delayAttackValue = attackDuration;
		attackCooldownValue = delayAttackValue*1.3f;
		runSpeed = runSpeed*runMaxAnimationSpeed;
		trotSpeed = trotSpeed*trotMaxAnimationSpeed;
		walkSpeed = walkSpeed*walkMaxAnimationSpeed;

		
		StartCoroutine(FixPositionRelativeToEntities());
	}
	
	void Start () {
		fallStartLevel = 0;
	}

	void UpdateSmoothedMovementDirection()
	{
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();
	
		Vector3 forward= cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
	
		Vector3 right= new Vector3(forward.z, 0, -forward.x);
		
		float v = Input.GetAxisRaw("Vertical");
	    float h = Input.GetAxisRaw("Horizontal");

		if (v < -0.2f) movingBack = true;
		else movingBack = false;
	
		bool wasMoving= isMoving;
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;
		
    	Vector3 targetDirection = h * right + v * forward;
	
		if (grounded)
		{
			if (!canMove)
				moveSpeed = 0.0f;
		
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving) lockCameraTimer = 0.0f;
			
			if (falling) {
				falling = false;
				float fallLenght = fallStartLevel - transform.position.y;
				if (fallLenght > fallingHeightThreshold) {
					float fallDamage = fallLenght/fallingHeightThreshold;
					fallDamage *= fallDamage;
					fallDamage *= fallingDamageMultiplier;
			
					mainCharacter.DamageLifeStatus(fallDamage);
				}
				fallStartLevel = transform.position.y;
			}
			if (targetDirection != Vector3.zero)
			{
				if (moveSpeed < walkSpeed * 0.9f && grounded)
				{
					moveDirection = targetDirection.normalized;
				}
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					moveDirection = moveDirection.normalized;
				}
			}
		
	        float curSmooth = speedSmoothing * Time.deltaTime;
			float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			characterState = CharacterState.Idle;
			
			/*if (Input.GetKey (KeyCode.LeftShift) | Input.GetKey (KeyCode.RightShift))
			{
				targetSpeed *= runSpeed;
				characterState = CharacterState.Running;
			}
			else if (Time.time - trotAfterSeconds > walkTimeStart)
			{
				targetSpeed *= trotSpeed;
				characterState = CharacterState.Trotting;
			}
			else
			{
				targetSpeed *= walkSpeed;
				characterState = CharacterState.Walking;
			}*/
			
			if (canRun)
			{
				targetSpeed *= runSpeed;
				characterState = CharacterState.Running;
			}
			else if (Time.time - trotAfterSeconds > walkTimeStart)
			{
				targetSpeed *= trotSpeed;
				characterState = CharacterState.Trotting;
			}
			else
			{
				targetSpeed *= walkSpeed;
				characterState = CharacterState.Walking;
			}
		
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
	
			if (moveSpeed < walkSpeed * 0.3f) walkTimeStart = Time.time;
		} 
		else
		{
			if (!falling) {
				falling = true;
				fallStartLevel = transform.position.y;
			}
			if (isMoving) inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}

	void ApplyGravity()
	{
		if (isControllable)
		{
			if (IsGrounded ()) verticalSpeed = 0.0f;
			else verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	
	public void TryToAttack () {
		Entity closestEntity = GetClosestEntity();	
		if (closestEntity)
		{
			canMove = false;
			Vector3 d = closestEntity.transform.position - transform.position;
			d.y = 0;
			if (d.magnitude < 5) SetDirection(d);
			StartCoroutine(DelayAttack(closestEntity));
		}
	}
	
	// delay entre a execução da animação de ataque e do dano causado
	IEnumerator DelayAttack(Entity closestEntity)
	{
		int i = 0;
		while (true)
		{
			if (i > 0) 
			{
				Attack(closestEntity);
				break;
			}
			i++;
			yield return new WaitForSeconds(delayAttackValue);
		}
	}
	
	void DidAttack()
	{
		characterState = CharacterState.Attacking;
		StartCoroutine(AttackCooldown());
	}
	
	IEnumerator AttackCooldown()
	{
		int i = 0;
		canAttack = false;
		while (true)
		{
			if (i > 0) 
			{
				canAttack = true;
				break;			
			}
			else i++;
			yield return new WaitForSeconds(attackCooldownValue);
		}
	}
	
	protected void Attack(Entity closestEntity)
	{
		GameObject dmgBox = transform.Find("dmgBox").gameObject;
		Bounds bounds = closestEntity.GetComponent<CharacterController>().bounds;
		Bounds medBounds = dmgBox.collider.bounds;
		canMove = true;

		if (bounds.Intersects(medBounds)) {
			closestEntity.DamageLifeStatus(3);
		}
		
		
		/*foreach (Entity entity in listOfEnemies) {
			if (!entity) {
				//listOfEnemies.Remove(entity);
				continue;
			}
			bounds = entity.GetComponent<CharacterController>().bounds;
			if (bounds.Intersects(medBounds)) {
				StartCoroutine(DelayAttack(entity));
			}
		}*/
	}
	
	void DidInteract()
	{
		characterState = CharacterState.Interacting;
		mainCharacter.GrabTorch();
	}
	
	/*void DidDefend()
	{
		characterState = CharacterState.Defending;
	}*/
	
	public void KillCharacter()
	{
		characterState = CharacterState.Dead;
	}
	
	void Update()
	{	
		if (!isControllable)
		{
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown("Fire1"))
		{
			if (canAttack)
			{
				if (IsMoving())
				{
					Input.ResetInputAxes();
				}
				TryToAttack();
				DidAttack();
			}
		}
		
		if (Input.GetButtonDown("Fire2"))
		{
			if (canLightTorch)
			{
				if (IsMoving())
				{
					Input.ResetInputAxes();
				}
				DidInteract();
			}
		}
			
		/*if (Input.GetButtonDown("Fire3"))
		{
			if (IsMoving())
			{
				Input.ResetInputAxes();
			}
			DidDefend();
		}*/

		UpdateSmoothedMovementDirection();
	
		ApplyGravity();
	
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
	
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		if(animation) 
		{
			
			if (characterState == CharacterState.Attacking)
			{
				animation[attackAnimation.name].wrapMode = WrapMode.Once;
				animation[attackAnimation.name].speed = attackAnimationSpeed;
				animation[attackAnimation.name].layer = 1;
				animation.CrossFade(attackAnimation.name);
			}
			/*else if (characterState == CharacterState.Defending)
			{
				animation[defenseAnimation.name].wrapMode = WrapMode.Once;
				animation[defenseAnimation.name].speed = defenseAnimationSpeed;
				animation[defenseAnimation.name].layer = 1;
				animation.CrossFade(defenseAnimation.name);
			}*/
			/*else if (characterState == CharacterState.Interacting)
			{
				animation[interactAnimation.name].wrapMode = WrapMode.Once;
				animation[interactAnimation.name].speed = interactAnimationSpeed;
				animation[interactAnimation.name].layer = 1;
				animation.CrossFade(interactAnimation.name);
			}*/
			else if (characterState == CharacterState.Dead)
			{
				animation[deathAnimation.name].wrapMode = WrapMode.Once;
				animation[deathAnimation.name].speed = deathAnimationSpeed;
				animation[deathAnimation.name].layer = 1;
				animation.CrossFade(deathAnimation.name);
			}
			else 
			{
				if(controller.velocity.sqrMagnitude < 0.1f) 
				{
					animation.CrossFade(idleAnimation.name);
				}
				else 
				{
					if(characterState == CharacterState.Running)
					{
						animation[runAnimation.name].wrapMode = WrapMode.Loop;
						animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
						animation.CrossFade(runAnimation.name);	
					}
					else if(characterState == CharacterState.Trotting) 
					{
						animation[walkAnimation.name].wrapMode = WrapMode.Loop;
						animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, trotMaxAnimationSpeed);
						animation.CrossFade(walkAnimation.name);	
					}
					else if(characterState == CharacterState.Walking) 
					{
						animation[walkAnimation.name].wrapMode = WrapMode.Loop;
						animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						animation.CrossFade(walkAnimation.name);	
					}
				}
			}
		}
		if (IsGrounded())
		{
			transform.rotation = Quaternion.LookRotation(moveDirection);		
		}	
		else
		{
			Vector3 xzMove= movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(xzMove);
		}	
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.moveDirection.y > 0.01f) return;
	}

	float GetSpeed()
	{
		return moveSpeed;
	}

	bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}

	Vector3 GetDirection()	
	{
		return moveDirection;
	}
	
	public void SetDirection(Vector3 d) {
		moveDirection = d.normalized;
	}	

	public bool IsMovingBackwards()
	{
		return movingBack;
	}

	public float GetLockCameraTimer()
	{
		return lockCameraTimer;
	}

	public bool IsMoving()
	{
	 	return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}

	bool  IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	void  Reset()
	{
		gameObject.tag = "Player";
	}
	
	public void CanLightTorch(bool cond)
	{
		canLightTorch = cond;
	}
	
	Entity GetClosestEntity()
	{
		List<Entity> listOfEnemies;
		Entity closestEntity;
		float minDist = 1e10f;
		listOfEnemies = mainCharacter.GetListOfEnemies();
		if (listOfEnemies.Count != 0)
		{
			closestEntity = listOfEnemies[0];
			foreach (Entity entity in listOfEnemies) 
			{
				if (!entity) continue;
				float dist = (entity.transform.position - transform.position).sqrMagnitude;
				if (dist < minDist)
				{
					minDist = dist;
					closestEntity = entity;
				}
			}
			return closestEntity;
		}
		else
		{
			return null;
		}
	}

	IEnumerator FixPositionRelativeToEntities()
	{
		Entity closestEntity;
		while(true)
		{			
			closestEntity = GetClosestEntity();
			if (closestEntity)
			{
				Vector3 d = closestEntity.transform.position - transform.position;
				Vector3 position = closestEntity.transform.position;	
				d.y = 0;				
				if (d.magnitude < 1.5f) closestEntity.transform.position = new Vector3(position.x + 8.0f, position.y + 4.0f, position.z);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
}