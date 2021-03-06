using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class MainCharacterController : MonoBehaviour 
{

	public AnimationClip idle1Animation;
	public AnimationClip idle2Animation;
	public AnimationClip idle3Animation;
	public AnimationClip walkAnimation;
	public AnimationClip walkSlowAnimation;
	public AnimationClip runAnimation;
	public AnimationClip death1Animation;
	public AnimationClip death2Animation;
	public AnimationClip receiveAttackAnimation;
	public AnimationClip evadeLeftAnimation;
	public AnimationClip evadeBackAnimation;
	public AnimationClip evadeRightAnimation;
	public AnimationClip attack1Animation;
	public AnimationClip attack12Animation;
	public AnimationClip attack123Animation;
	public AnimationClip danceAnimation;
	public AnimationClip fightStanceAnimation;
	public AnimationClip runFastAnimation;
	
	private Animation animation;
	
	private float evadeLeftAnimationSpeed = 1.5f;
	private float evadeRightAnimationSpeed = 1.5f;
	private float evadeBackAnimationSpeed = 1.0f;
	private float walkAnimationSpeed = 1.0f;
	private float walkSlowAnimationSpeed = 1.0f;
	private float runAnimationSpeed = 1.0f;
	private float runFastAnimationSpeed = 1.0f;
	private float landAnimationSpeed = 1.0f;
	private float attack1AnimationSpeed = 1.5f;
	private float attack12AnimationSpeed = 1.5f;
	private float attack123AnimationSpeed = 1.5f;
	private float death1AnimationSpeed = 1.0f;
	private float death2AnimationSpeed = 1.0f;
	private float receiveAttackAnimationSpeed = 1.0f;
	private float baseAttackDuration = 0.7f;
	private float danceAnimationSpeed = 1.0f;
	private float idle1AnimationSpeed = 1.0f;
	private float idle2AnimationSpeed = 1.0f;
	private float idle3AnimationSpeed = 1.0f;
	
	private Waypoint closestWaypoint;
	private GameObject runningEnemy;
	
	enum CharacterState 
	{
		Idle,
		Walking,
		Running,
		Attacking,
		Interacting,
		Dead,
		ReceivingAttack,
		EvadingLeft,
		EvadingRight,
		EvadingBack,
		Dancing
	}

	private MainCharacter mainCharacter;
	private CharacterState characterState;

	private bool followEnemy = false;
	
	private float delayAttackValue;
	private float attackDuration;
	
	// The following variables were made private to tweak easier and
	// share the tweaking by git. These values were copied from the
	// inspector at date May 23rd, 23h13.
	private float walkSpeed = 4.0f;
	private float runSpeed = 7.0f;
	private float inAirControlAcceleration = 10.0f;
	private float gravity = 20.0f;
	private float speedSmoothing = 10.0f;
	private float rotateSpeed = 500.0f;

	private float runVolume = 0.6f;
	private float walkVolume = 0.3f;
	private bool isOnWater = false;
	public bool canRun = true;
	public bool canAttack = true;
	public bool canMove = true;
	private bool falling = false;
	
	private float groundedTimeout = 0.25f;

	private float medrashAttackRadius = 7.0f;
	private float lockCameraTimer = 0.0f;

	private Vector3 moveDirection = Vector3.zero;
	private float verticalSpeed = 0.0f;

	private float moveSpeed = 0.0f;

	private CollisionFlags collisionFlags; 

	private bool movingBack = false;
	private bool isMoving = false;
	private float walkTimeStart = 0.0f;
	private Vector3 inAirVelocity= Vector3.zero;

	private float lastGroundedTime = 0.0f;

	private bool isControllable = true;
	private bool isDamnIdle = false;
	private bool idleCounterPaused = false;
	
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
		runningEnemy = GameObject.FindGameObjectWithTag("Fugitive");
		pauseMenu = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PauseMenu>();
		moveDirection = transform.TransformDirection(Vector3.forward);
		mainCharacter = GetComponent<MainCharacter>();
		animation = GetComponent<Animation>();
		
		if(!animation) Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
	
		if(!idle1Animation) 
		{
			animation = null;
			Debug.Log("No idle 1 animation found. Turning off animations.");
		}
		if(!idle2Animation) 
		{
			animation = null;
			Debug.Log("No idle 2 animation found. Turning off animations.");
		}
		if(!idle3Animation) 
		{
			animation = null;
			Debug.Log("No idle 3 animation found. Turning off animations.");
		}
		if(!walkAnimation) 
		{
			animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(!walkSlowAnimation) 
		{
			animation = null;
			Debug.Log("No walk slow animation found. Turning off animations.");
		}
		if(!runAnimation) 
		{
			animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!attack1Animation)
		{
			animation = null;
			Debug.Log("No attack animation found. Turning off animations.");
		}
		if (!attack12Animation)
		{
			animation = null;
			Debug.Log("No attack12 animation found. Turning off animations.");
		}
		if (!attack123Animation)
		{
			animation = null;
			Debug.Log("No attack123 animation found. Turning off animations.");
		}
		if (!death1Animation)
		{
			animation = null;
			Debug.Log("No death 1 animation found. Turning off animations.");
		}
		if (!death2Animation)
		{
			animation = null;
			Debug.Log("No death 2 animation found. Turning off animations.");
		}
		if (!receiveAttackAnimation)
		{
			animation = null;
			Debug.Log("No receive attack animation found. Turning off animations.");
		}
		if (!evadeLeftAnimation)
		{
			animation = null;
			Debug.Log("No evade left animation found. Turning off animations.");
		}
		if (!evadeRightAnimation)
		{
			animation = null;
			Debug.Log("No evade right animation found. Turning off animations.");
		}
		if (!evadeBackAnimation)
		{
			animation = null;
			Debug.Log("No evade back animation found. Turning off animations");
		}
		if (!danceAnimation)
		{
			animation = null;
			Debug.Log("No dance animation found. Turning off animations");
		}
		if (!fightStanceAnimation)
		{
			animation = null;
			Debug.Log("No fight stance animation found. Turning off animations.");
		}
		if (!runFastAnimation)
		{
			animation = null;
			Debug.Log("No run fast animation found. Turning off animations.");
		}
		
		attackDuration = baseAttackDuration/attack1AnimationSpeed;
		delayAttackValue = attackDuration;
		attackCooldownValue = delayAttackValue*1.5f;	
		
		StartCoroutine(FixPositionRelativeToEntities());
		StartCoroutine(ComboVerification());
		StartCoroutine(ChangeIdleActivity());
	}
	
	void Start () {
		fallStartLevel = 0;
		StartCoroutine(StepSound());
	}
	
	IEnumerator ChangeIdleActivity()
	{
		int n = 0;
		while(true)
		{
			if (!IsMoving() && idleCounterPaused == false)
			{
				n++;
				if (n == 30) 
				{
					n = 0;
					PlayRandomIdleAnimation();
				}
			}
			else n = 0;
			yield return new WaitForSeconds(0.5f);
		}
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
	
	IEnumerator Evade(String direction)
	{
		Entity closestEntity = GetClosestEntity();	
		if (closestEntity)
		{
			Vector3 d = closestEntity.transform.position - transform.position;
			d.y = 0;
			if (d.magnitude < medrashAttackRadius) SetDirection(d);
		}
		CharacterController controller = GetComponent<CharacterController>();
		Vector3 movement = new Vector3();
		if (direction.Equals("right") || direction.Equals("left"))
		{
			movement = this.transform.right;
			if (direction.Equals("left"))
			{
				movement.x = (-1) * movement.x;
				movement.z = (-1) * movement.z;
			}	
		}
		else if (direction.Equals("back"))
		{
			movement = this.transform.forward;
			movement.x = (-1) * movement.x;
			movement.z = (-1) * movement.z;
		}
		float n = 0;
		while (true)
		{
			if (n == 12) 
			{
				canMove = true;
				break;
			}
			else 
			{	
				n++;
				controller.SimpleMove(movement * 5f);
			}
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	void DidAttack1()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.Attacking;
		animation[attack1Animation.name].wrapMode = WrapMode.Once;
		animation[attack1Animation.name].speed = attack1AnimationSpeed;
		animation[attack1Animation.name].layer = 1;
		animation.CrossFade(attack1Animation.name);
		StartCoroutine(AttackCooldown());
	}
	
	void DidAttack12()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.Attacking;
		animation[attack12Animation.name].wrapMode = WrapMode.Once;
		animation[attack12Animation.name].speed = attack12AnimationSpeed;
		animation[attack12Animation.name].layer = 1;
		animation.CrossFade(attack12Animation.name);
		StartCoroutine(AttackCooldown());
	}
	
	void DidAttack123()
	{
		Input.ResetInputAxes();
		characterState = CharacterState.Attacking;
		animation[attack123Animation.name].wrapMode = WrapMode.Once;
		animation[attack123Animation.name].speed = attack123AnimationSpeed;
		animation[attack123Animation.name].layer = 1;
		animation.CrossFade(attack123Animation.name);
		StartCoroutine(AttackCooldown());
	}
	
	void DidDance()
	{
		Input.ResetInputAxes();
		sounds.PlayChaChaChaAudio(1.0f);
		canMove = false;
		characterState = CharacterState.Dancing;
		animation[danceAnimation.name].wrapMode = WrapMode.Once;
		animation[danceAnimation.name].speed = danceAnimationSpeed;
		animation[danceAnimation.name].layer = 1;
		animation.CrossFade(danceAnimation.name);
		StartCoroutine(UnlockMovement(55.0f));
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
		StartCoroutine(UnlockMovement(receiveAttackAnimationSpeed));
	}
	
	IEnumerator UnlockMovement(float time)
	{
		int i = 0;
		while (true)
		{
			if (i != 0)
			{
				canMove = true;
				break;
			}
			else i++;
			yield return new WaitForSeconds(time);
		}
	}
	
	IEnumerator DelayIdleCounter(float time)
	{
		int i = 0;
		idleCounterPaused = true;
		while (true)
		{
			if (i != 0)
			{
				idleCounterPaused = false;
				break;
			}
			else i++;
			yield return new WaitForSeconds(time);
		}
	}
	
	public void ForceDeath()
	{
		canMove = false;
		Input.ResetInputAxes();
		characterState = CharacterState.Dead;
		PlayRandomDeathAnimation();
	}
	
	private void PlayRandomDeathAnimation()
	{
		System.Random random = new System.Random();
		int i = random.Next(0, 2);
		if (i == 0)
		{
			animation[death1Animation.name].wrapMode = WrapMode.ClampForever;
			animation[death1Animation.name].speed = death1AnimationSpeed;
			animation[death1Animation.name].layer = 1;
			animation.Play(death1Animation.name);
		}
		else
		{
			animation[death2Animation.name].wrapMode = WrapMode.ClampForever;
			animation[death2Animation.name].speed = death2AnimationSpeed;
			animation[death2Animation.name].layer = 1;
			animation.Play(death2Animation.name);
		}
	}
	
	private void PlayRandomIdleAnimation()
	{
		isDamnIdle = true;
		System.Random random = new System.Random();
		int i = random.Next(0, 2);
		if (i == 0)
		{
			animation[idle2Animation.name].wrapMode = WrapMode.Once;
			animation[idle2Animation.name].speed = idle2AnimationSpeed;
			animation[idle2Animation.name].layer = 1;
			animation.Play(idle2Animation.name);
			StartCoroutine(DelayIdleCounter(idle2Animation.length));
		}
		else
		{
			animation[idle3Animation.name].wrapMode = WrapMode.Once;
			animation[idle3Animation.name].speed = idle3AnimationSpeed;
			animation[idle3Animation.name].layer = 1;
			animation.Play(idle3Animation.name);
			StartCoroutine(DelayIdleCounter(idle3Animation.length));
		}
	}
	
	public void ForceIdle()
	{
		canMove = true;
		characterState = CharacterState.Idle;
		animation[idle1Animation.name].layer = 1;
		animation.Play(idle1Animation.name);
		animation[idle1Animation.name].layer = 0;
		animation.Play(idle1Animation.name);
	}
	
	public void DidEvadeLeft()
	{
		Input.ResetInputAxes();
		canMove = false;
		characterState = CharacterState.EvadingLeft;
		animation[evadeLeftAnimation.name].wrapMode = WrapMode.Once;
		animation[evadeLeftAnimation.name].speed = evadeLeftAnimationSpeed;
		animation[evadeLeftAnimation.name].layer = 1;
		animation.CrossFade(evadeLeftAnimation.name);
		StartCoroutine(Evade("left"));
	}
	
	public void DidEvadeRight()
	{
		Input.ResetInputAxes();
		canMove = false;
		characterState = CharacterState.EvadingRight;
		animation[evadeRightAnimation.name].wrapMode = WrapMode.Once;
		animation[evadeRightAnimation.name].speed = evadeRightAnimationSpeed;
		animation[evadeRightAnimation.name].layer = 1;
		animation.CrossFade(evadeRightAnimation.name);
		StartCoroutine(Evade("right"));
	}
	
	public void DidEvadeBack()
	{
		Input.ResetInputAxes();
		canMove = false;
		characterState = CharacterState.EvadingBack;
		animation[evadeBackAnimation.name].wrapMode = WrapMode.Once;
		animation[evadeBackAnimation.name].speed = evadeBackAnimationSpeed;
		animation[evadeBackAnimation.name].layer = 1;
		animation.CrossFade(evadeBackAnimation.name);
		StartCoroutine(Evade("back"));
	}
	
	IEnumerator ComboVerification()
	{
		while(true)
		{
			if (n != 0)
			{
				animation.Play(fightStanceAnimation.name);
				if (n == 1) 
				{
					n = 0;
					TryToAttack();
					DidAttack1();
				}
				else if(n == 2) 
				{
					n = 0;
					TryToAttack();
					DidAttack12();
				}
				else if(n == 3) 
				{
					n = 0;
					TryToAttack();
					DidAttack123();
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
		
	}
	
	private bool fire2ButtonDown = false;
	private bool leftCtrlKeyDown = false;
	
	int n = 0;
	
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
			if (Input.GetKeyDown(KeyCode.LeftControl)) leftCtrlKeyDown = true;
			if (Input.GetKeyUp(KeyCode.LeftControl)) leftCtrlKeyDown = false;
			
			if (Input.GetButtonDown("Fire1"))
			{
				if (canAttack)
				{
					if (n >= 0 && n < 3)
					n++;
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
			
			if (Input.GetKeyDown(KeyCode.DownArrow) && fire2ButtonDown)
			{
				DidEvadeBack();
				fire2ButtonDown = false;
			}
			
			if (Input.GetKeyDown(KeyCode.Space) && leftCtrlKeyDown)
			{
				DidDance();
				leftCtrlKeyDown = false;
			}

		}
			
		if (IsMoving() && isDamnIdle)
		{
			ForceIdle();
			isDamnIdle = false;
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
				Entity closestEntity = GetClosestEntity();	
				if (closestEntity)
				{	
					Vector3 d = closestEntity.transform.position - transform.position;
					d.y = 0;
					if (d.magnitude < 15) animation.CrossFade(fightStanceAnimation.name);
					else animation.CrossFade(idle1Animation.name);	
				}
			}
			else 
			{
				if (isOnWater)
				{
					animation[walkSlowAnimation.name].wrapMode = WrapMode.Loop;
					animation[walkSlowAnimation.name].speed = walkSlowAnimationSpeed;
					animation.CrossFade(walkSlowAnimation.name);	
				}
				else
				{
					if(characterState == CharacterState.Running)
					{	
						float energyStatus = mainCharacter.GetEnergyStatus();
						if (energyStatus <= 100.0f && energyStatus > 50.0f)
						{	
							animation[runFastAnimation.name].wrapMode = WrapMode.Loop;
							animation[runFastAnimation.name].speed = runFastAnimationSpeed;
							animation.CrossFade(runFastAnimation.name);	
						}
						else if (energyStatus <= 50.0f && energyStatus > 20.0f)
						{
							animation[runAnimation.name].wrapMode = WrapMode.Loop;
							animation[runAnimation.name].speed = runAnimationSpeed;
							animation.CrossFade(runAnimation.name);	
						}
					}
					else 
					{
						animation[walkAnimation.name].wrapMode = WrapMode.Loop;
						animation[walkAnimation.name].speed = walkAnimationSpeed;
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

	public void OnWater() {
		isOnWater = true;
	}
	
	public void OffWater() {
		isOnWater = false;
	}
	
	public void SetClosestWaypoint (Waypoint wp) {
		closestWaypoint = wp;	
	}
	
	public float GetDistanceFromSora () {
		if (closestWaypoint) {
			if (!closestWaypoint.runningEnemy)
				return (runningEnemy.transform.position - transform.position).magnitude;
			return closestWaypoint.GetDistance() + (closestWaypoint.transform.position - transform.position).magnitude;
		} else
			return (runningEnemy.transform.position - transform.position).magnitude;
	}
			
}
