/* class Entity
 *
 * This is an abstract class for all enemies.
 *
 * Variables:
 *  gameObject medrash - points to the Player;
 *  gameObject dmgBox  - points to the box where the player can cause damage;
 *  gameObject reward  - points to the food the entity will drop on death;
 *  gameObject dieExplosion - points to the explosion that will appear on death.
 *
 *  Vector3 direction - the direction the entity will move and face.
 *  Vector3 returnPlace - the origin place for the entity.
 *  float attackRadius, closeRadius, farRadius - Variables for the logic for the
 *      the change of state.
 *    attackRadius < closeRadius < farRadius
 *  bool canReceiveDamage - indicate that the player can receive damage in the current state.
 */
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (Animation))]
public abstract class Entity : MonoBehaviour {

	protected GameObject medrash;
	protected GameObject dmgBox;
	public GameObject reward;
	public float energyValue = 0.0f;
	public float lifeValue = 0.0f;
	public GameObject dieExplosion;
	public GameObject Prefab;
	
	protected CharacterController controller;
	protected Vector3 direction;
	protected Vector3 returnPlace;
	protected float attackRadius = 3.0f, closeRadius = 7.0f, farRadius = 15.0f;
	protected FSM fsm;
	protected bool canReceiveDamage = false, receivedDamage = false;
	
	protected float life, damage, speed, baseSpeed;
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip waitAnimation;
	public AnimationClip dyingAnimation;
	public AnimationClip tigerSpecialAttackAnimation;
	
	protected AnimationClip currentAnimation;
	
	public void SetMainCharacter(GameObject obj) {
		medrash = obj;
	}
	
	protected void EntityStart () {
		medrash = GameObject.FindGameObjectWithTag("Player");
		dmgBox = transform.Find("dmgBox").gameObject;
		if (reward != null)
			reward.GetComponent<FoodController>().setValues(lifeValue, energyValue);
	}
	
	public void Update () {
		direction.y = 0;
		if (animation && currentAnimation)
			animation.CrossFade(currentAnimation.name);
		if (direction != Vector3.zero) {
			direction.Normalize();
			transform.forward = direction;
		}
		
		controller.SimpleMove(speed*direction);
	}
	
	public void RotateBy (float theta) {//Around y-axis
		Vector3 d = transform.forward;
		float x = d.x, z = d.z;
		float c = Mathf.Cos(theta), s = Mathf.Sin(theta);
		d.x = x*c + z*s;
		d.z = -x*s + z*c;
		transform.forward = d;
	}
	
	public CharacterController GetController () {
		return controller;
	}
	
	public void SetDirection (Vector3 v) {
		direction = v;
	}
	
	public void DamageLifeStatus (float dmg) {
		if (canReceiveDamage) {
			life -= dmg;
			canReceiveDamage = false;
			Debug.Log(name + " damaged. Life: " + life);
		}
		receivedDamage = true;
	}
	
	public void SetIdleAnimation () {
		currentAnimation = idleAnimation;
	}
	
	public void SetRunAnimation () {
		currentAnimation = runAnimation;
	}
	
	public void SetWalkAnimation () {
		currentAnimation = walkAnimation;
	}
	
	public void SetAttackAnimation () {
		currentAnimation = attackAnimation;
	}
	
	public void SetWaitAnimation () {
		currentAnimation = waitAnimation;
	}
	
	public void SetTigerSpecialAttackAnimation () {
		currentAnimation = tigerSpecialAttackAnimation;
	}
	
	public void SetDyingAnimation () {
		currentAnimation = dyingAnimation;
		animation.wrapMode = WrapMode.ClampForever;
	}
	
	public void SetSpeed(float sp) {
		speed = sp;
	}
	
	public float GetAttackRadius () {
		return attackRadius;
	}
	
	public float GetCloseRadius () {
		return closeRadius;
	}
	
	public float GetFarRadius () {
		return farRadius;
	}
	
	public float GetDamage () {
		return damage;
	}
	
	public float GetSpeed () {
		return speed;
	}
	
	public float GetBaseSpeed () {
		return baseSpeed;
	}
	
	public Vector3 ReturnPlace()
	{
		return returnPlace;
	}
	
	public bool MainCharacterIsInDmgBox () {
		if (!dmgBox)
			return false;
		Bounds bounds = dmgBox.collider.bounds;
		Bounds medBounds = medrash.GetComponent<CharacterController>().bounds;
		
		return bounds.Intersects(medBounds);
	}

	public float DistanceToMainCharacter () {
		Vector3 d = medrash.transform.position - transform.position;
		return d.magnitude;
	}
	
	public void DamageMedrash () {
		medrash.GetComponent<MainCharacter>().DamageLifeStatus(damage);
	}
	
	public Vector3 GetMedrashPosition () {
		return medrash.transform.position;
	}
}
