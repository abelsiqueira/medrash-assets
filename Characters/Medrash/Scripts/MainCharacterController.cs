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
	public AnimationClip jumpPoseAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip deathAnimation;
	//public AnimationClip defenseAnimation;
	//public AnimationClip interactAnimation;
	
	private Animation animation;
	
	public float walkMaxAnimationSpeed = 0.75f;
	public float trotMaxAnimationSpeed = 1.0f;
	public float runMaxAnimationSpeed = 1.0f;
	public float jumpAnimationSpeed = 1.15f;
	public float landAnimationSpeed = 1.0f;
	public float attackAnimationSpeed = 1.0f;
	public float deathAnimationSpeed = 1.0f;
	//public float defenseAnimationSpeed = 1.0f;
	//public float interactAnimationSpeed = 1.0f;

	enum CharacterState 
	{
		Idle,
		Walking,
		Trotting,
		Running,
		Jumping,
		Attacking,
		Defending,
		Interacting,
		Dead
	}

	private MainCharacter mainCharacter;
	private CharacterState characterState;

	public float walkSpeed = 3.0f;
	
	public float trotSpeed = 4.0f;
	
	public float runSpeed = 9.0f;

	public float inAirControlAcceleration = 3.0f;

	public float jumpHeight = 0.5f;

	public float gravity = 20.0f;
	public float speedSmoothing = 10.0f;
	public float rotateSpeed = 500.0f;
	public float trotAfterSeconds = 3.0f;

	public bool canJump = true;
	public bool canRun = true;
	public bool canAttack = true;
	
	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;

	private float lockCameraTimer = 0.0f;

	private Vector3 moveDirection = Vector3.zero;
	private float verticalSpeed = 0.0f;

	private float moveSpeed = 0.0f;

	private CollisionFlags collisionFlags; 

	private bool jumping= false;
	private bool jumpingReachedApex= false;

	private bool movingBack= false;
	private bool isMoving= false;
	private float walkTimeStart = 0.0f;
	private float lastJumpButtonTime = -10.0f;
	private float lastJumpTime = -1.0f;

	private float lastJumpStartHeight = 0.0f;

	private Vector3 inAirVelocity= Vector3.zero;

	private float lastGroundedTime = 0.0f;

	private bool isControllable = true;
	
	public float attackCooldownValue = 2.0f;

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
		if(!jumpPoseAnimation && canJump) 
		{
			animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
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
		
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving) lockCameraTimer = 0.0f;

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
			
			if (Input.GetButtonDown("Fire1"))
			{
				if (canAttack)
				{
					if (IsMoving())
					{
						Input.ResetInputAxes();
					}
					mainCharacter.TryToAttack();
					DidAttack();
				}
			}
			
			/*if (Input.GetButtonDown("Fire2"))
			{
				if (IsMoving())
				{
					Input.ResetInputAxes();
				}
				DidInteract();
			}
			
			if (Input.GetButtonDown("Fire3"))
			{
				if (IsMoving())
				{
					Input.ResetInputAxes();
				}
				DidDefend();
			}*/
		
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
	
			if (moveSpeed < walkSpeed * 0.3f) walkTimeStart = Time.time;
		}
		else
		{
			if (jumping) lockCameraTimer = 0.0f;
		
			if (isMoving) inAirVelocity += targetDirection.normalized * Time.deltaTime * inAirControlAcceleration;
		}
	}
	
	void ApplyJumping()
	{
		if (lastJumpTime + jumpRepeatTime > Time.time) return;
	
		if (IsGrounded()) 
		{	
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) 
			{
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}


	void ApplyGravity()
	{
		if (isControllable)
		{
			bool jumpButton= Input.GetButton("Jump");
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			if (IsGrounded ()) verticalSpeed = 0.0f;
			else verticalSpeed -= gravity * Time.deltaTime;
		}
	}

	float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}

	void DidJump()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		
		characterState = CharacterState.Jumping;
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
			if (i == attackCooldownValue) 
			{
				canAttack = true;
				break;			
			}
			else i++;
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	/*void DidInteract()
	{
		characterState = CharacterState.Interacting;
	}
	
	void DidDefend()
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

		if (Input.GetButtonDown("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		
		UpdateSmoothedMovementDirection();
	
		ApplyGravity();

		ApplyJumping();
	
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
	
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		if(animation) 
		{
			if(characterState == CharacterState.Jumping) 
			{
				if(!jumpingReachedApex) 
				{
					animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					animation.CrossFade(jumpPoseAnimation.name);
				}
				else 
				{
					animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					animation.CrossFade(jumpPoseAnimation.name);				
				}
			}
			else if (characterState == CharacterState.Attacking)
			{
				animation[attackAnimation.name].wrapMode = WrapMode.Once;
				animation[attackAnimation.name].speed = attackAnimationSpeed;
				animation[attackAnimation.name].layer = 1;
				animation.CrossFade(attackAnimation.name);
			}
			else if (characterState == CharacterState.Dead)
			{
				animation[deathAnimation.name].wrapMode = WrapMode.Once;
				animation[deathAnimation.name].speed = deathAnimationSpeed;
				animation[deathAnimation.name].layer = 1;
				animation.CrossFade(deathAnimation.name);
			}
			/*else if (characterState == CharacterState.Defending)
			{
				animation[defenseAnimation.name].wrapMode = WrapMode.Once;
				animation[defenseAnimation.name].speed = defenseAnimationSpeed;
				animation[defenseAnimation.name].layer = 1;
				animation.CrossFade(defenseAnimation.name);
			}*/
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
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
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

	public bool IsJumping()
	{
		return jumping;
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

	bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}

	bool  IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	void  Reset()
	{
		gameObject.tag = "Player";
	}

}
