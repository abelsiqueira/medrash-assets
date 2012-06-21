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
	public AnimationClip receiveAttackAnimation;
	public AnimationClip evadeLeftAnimation;
	public AnimationClip evadeRightAnimation;
	
	private Animation animation;
	
	private float evadeLeftAnimationSpeed = 1.0f;
	private float evadeRightAnimationSpeed = 1.0f;
	private float walkMaxAnimationSpeed = 0.75f;
	private float trotMaxAnimationSpeed = 1.0f;
	private float runMaxAnimationSpeed = 1.2f;
	private float landAnimationSpeed = 1.0f;
	private float attackAnimationSpeed = 1.4f;
	private float deathAnimationSpeed = 1.0f;
	private float receiveAttackAnimationSpeed = 0.75f;
	private float baseAttackDuration = 0.8f;
	
	enum CharacterState 
	{
		Idle,
		Walking,
		Trotting,
		Running,
		Attacking,
		Interacting,
		Dead,
		ReceivingAttack,
		EvadingLeft,
		EvadingRight,
	}

	private MainCharacter mainCharacter;
	private CharacterState characterState;

	private bool followEnemy = false;
	
	private float delayAttackValue;
	private float attackDuration;
	
	// The following variables were made private to tweak easier and
	// share the tweaking by git. These values were copied from the
	// inspector at date May 23rd, 23h13.
	private float walkSpeed = 5.0f;
	private float trotSpeed = 4.0f;
	private float runSpeed = 15.0f;
	private float inAirControlAcceleration = 10.0f;
	private float gravity = 20.0f;
	private float speedSmoothing = 10.0f;
	private float rotateSpeed = 500.0f;
	private float trotAfterSeconds = 3.0f;

	private float runVolume = 0.6f;
	private float walkVolume = 0.3f;
	private bool isOnWater = false;
	public bool canRun = true;
	public bool canAttack = true;
	public bool canMove = true;
	private bool canLightTorch = false;
	private bool falling = false;
	
	private float groundedTimeout = 0.25f;

	private float medrashAttackRadius = 7.0f;
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
	private bool evading = false;
	
	private float attackCooldownValue = 0.0f;
	private float fallingDamageMultiplier = 20f;
	private float fallingHeightThreshold = 12.0f;
	private float fallStartLevel;
	private float leapingHeight = 1.0f;
	
	private float stepTime = 0.0f;
	
	private PauseMenu pauseMenu;
	
	private MedrashSounds sounds;

	void Awake()
	{
		sounds = GetComponent<MedrashSounds>();
		pauseMenu = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PauseMenu>();
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
		if (!receiveAttackAnimation)
		{
			animation = null;
			Debug.Log("No receive attack animation found. Turning off animations.");
		}
		if (!evadeLeftAnimation)
		{
			animation = null;
			Debug.Log("No evade to left animation found. Turning off animations.");
		}
		if (!evadeRightAnimation)
		{
			animation = null;
			Debug.Log("No evade to right animation found. Turning off animations.");
		}
		
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
		StartCoroutine(StepSound());
	}
	
	IEnumerator StepSound()
	{
		float volume;
		while(true)
		{
			
			if (characterState == CharacterState.Running) {
				volume = runVolume;
				stepTime = 0.44f/animation[runAnimation.name].speed;
			}
			else {
				volume = walkVolume;
				stepTime = 0.5f/animation[walkAnimation.name].speed;
			}
			
			if (isMoving && IsGrounded())
				if (!isOnWater)sounds.PlayStepAudio(volume);
				else sounds.PlayWaterStepAudio(volume);
			
			yield return new WaitForSeconds(stepTime);
		}
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
		
		if (isOnWater)
			canRun = false;

		if (v < -0.2f) movingBack = true;
		else movingBack = false;
	
		bool wasMoving= isMoving;
		isMoving = Mathf.Abs (h) > 0.1f || Mathf.Abs (v) > 0.1f;
		
    	Vector3 targetDirection = h * right + v * forward;
	
		if (grounded && canMove)
		{
		
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
				moveDirection = targetDirection.normalized;
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
		else if (grounded && !canMove) 
		{
			moveSpeed = 0.0f;
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
		if (isControllable && mainCharacter.IsAlive())
		{
			if (IsGrounded ()) verticalSpeed = -10.0f; // Avoids (most) hopping
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
			if (d.magnitude < medrashAttackRadius) SetDirection(d);
			StartCoroutine(DelayAttack(closestEntity));
		}
	}
	
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
		if (closestEntity != null)
		{
			GameObject dmgBox = transform.Find("dmgBox").gameObject;
			Bounds bounds = closestEntity.GetComponent<CharacterController>().bounds;
			Bounds medBounds = dmgBox.collider.bounds;

			if (bounds.Intersects(medBounds)) {
				closestEntity.DamageLifeStatus(2);
			}
		}
		canMove = true;
	}
	
	void DidAttack()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.Attacking;
		animation[attackAnimation.name].wrapMode = WrapMode.Once;
		animation[attackAnimation.name].speed = attackAnimationSpeed;
		animation[attackAnimation.name].layer = 1;
		animation.CrossFade(attackAnimation.name);
		StartCoroutine(AttackCooldown());
	}
	
	public void ReceiveAttack()
	{
		canMove = false;
		Input.ResetInputAxes();
		characterState = CharacterState.ReceivingAttack;
		animation[receiveAttackAnimation.name].wrapMode = WrapMode.Clamp;
		animation[receiveAttackAnimation.name].speed = receiveAttackAnimationSpeed;
		animation[receiveAttackAnimation.name].layer = 1;
		animation.Play(receiveAttackAnimation.name);
		sounds.PlayReceiveDamageAudio(1.0f);
		canMove = true;
	}
	
	public void ForceDeath()
	{
		canMove = false;
		Input.ResetInputAxes();
		characterState = CharacterState.Dead;
		animation[deathAnimation.name].wrapMode = WrapMode.ClampForever;
		animation[deathAnimation.name].speed = deathAnimationSpeed;
		animation[deathAnimation.name].layer = 1;
		animation.Play(deathAnimation.name);
	}
	
	public void PutAlive()
	{
		canMove = true;
		characterState = CharacterState.Idle;
		animation[idleAnimation.name].layer = 1;
		animation.Play(idleAnimation.name);
		animation[idleAnimation.name].layer = 0;
		animation.Play(idleAnimation.name);
	}
	
	public void DidEvadeLeft()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.EvadingLeft;
		animation[evadeLeftAnimation.name].wrapMode = WrapMode.Once;
		animation[evadeLeftAnimation.name].speed = evadeLeftAnimationSpeed;
		animation[evadeLeftAnimation.name].layer = 1;
		animation.CrossFade(evadeLeftAnimation.name);
		StartCoroutine(Evade());
	}
	
	public void DidEvadeRight()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.EvadingRight;
		animation[evadeRightAnimation.name].wrapMode = WrapMode.Once;
		animation[evadeRightAnimation.name].speed = evadeRightAnimationSpeed;
		animation[evadeRightAnimation.name].layer = 1;
		animation.CrossFade(evadeRightAnimation.name);
		StartCoroutine(Evade());
	}
	
	IEnumerator Evade()
	{
		int i = 0;
		canMove = false;
		evading = true;
		
		Entity closestEntity = GetClosestEntity();	
		if (closestEntity)
		{
			Vector3 d = closestEntity.transform.position - transform.position;
			d.y = 0;
			if (d.magnitude < 5) SetDirection(d);
		}
		
		while (true)
		{
			if (i == 0) i++;
			else
			{
				evading = false;
				canMove = true;
				break;
			}
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	private bool fire2ButtonDown = false;
	
	void Update()
	{	
		if (!isControllable)
		{
			Input.ResetInputAxes();
		}
		
		if (mainCharacter.IsAlive() && !pauseMenu.IsPaused())
		{
			if (Input.GetButtonDown("Fire2")) fire2ButtonDown = true;
			if (Input.GetButtonUp("Fire2")) fire2ButtonDown = false;
			
			if (Input.GetButtonDown("Fire1"))
			{
				if (canAttack)
				{
					TryToAttack();
					DidAttack();
				}
			}
						
			if (Input.GetKeyDown(KeyCode.LeftArrow) && fire2ButtonDown) 
			{
				DidEvadeLeft();
				fire2ButtonDown = false;
			}
			
			if (Input.GetKeyDown(KeyCode.RightArrow) && fire2ButtonDown) 
			{
				DidEvadeRight();
				fire2ButtonDown = false;	
			}

		}
			
		UpdateSmoothedMovementDirection();
	
		ApplyGravity();
	
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
	
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		if(animation) 
		{
			if(controller.velocity.sqrMagnitude < 0.25f) 
			{
				animation.CrossFade(idleAnimation.name);
			}
			else 
			{
				if(characterState == CharacterState.Running)
				{
					animation[runAnimation.name].wrapMode = WrapMode.Loop;
					//animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					animation[runAnimation.name].speed = runSpeed/15.0f;
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
		// Must take in account the fact that he is always leaping
		bool isTouchingFloor = (collisionFlags & CollisionFlags.CollidedBelow) != 0;
		bool isLeaping = Physics.Raycast(transform.position, -transform.up, leapingHeight);
		
		return (isTouchingFloor | isLeaping);
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
	
	public Entity GetClosestEntity()
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
				if (!entity.CanBeAttacked())
					continue;
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
				float radius = GetComponent<CharacterController>().radius + closestEntity.GetComponent<CharacterController>().radius;
				d.y = 0;				
				if (d.magnitude < radius) closestEntity.transform.position = new Vector3(position.x + 8.0f, position.y + 4.0f, position.z);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public float GetWalkSpeed () {
		return walkSpeed;
	}
	
	public float GetRunSpeed () {
		return runSpeed;
	}
	
	public void SetRunSpeed (float rs) {
		runSpeed = rs;
	}
	
	public bool FollowEnemy() {
		return followEnemy;
	}
	
	public void StartFollowingEnemy() {
		followEnemy = true;
	}
	
	public void StopFollowingEnemy() {
		followEnemy = false;
	}
	
	public bool IsEvading()
	{
		return evading;
	}
	
	public void OnWater() {
		isOnWater = true;
	}
	
	public void OffWater() {
		isOnWater = false;
	}
}
	