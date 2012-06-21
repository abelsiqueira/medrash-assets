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
	protected int scoreValue;
	
	protected CharacterController controller;
	protected Vector3 direction;
	protected Vector3 returnPlace;
	protected float attackRadius = 3.0f, closeRadius = 7.0f, farRadius = 15.0f;
	protected FSM fsm;
	protected bool canReceiveDamage = false, receivedDamage = false;
	protected bool canBeAttacked = true;
	
	protected float life, maxLife, damage, speed, baseSpeed;
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public AnimationClip runAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip waitAnimation;
	public AnimationClip dyingAnimation;
	public AnimationClip tigerSpecialAttackAnimation;
	public AnimationClip attackedAnimation;
	
	protected AnimationClip currentAnimation;
	
	private Vector3 position;
	private Vector3 lifeBarPos;
	private Vector3 screenPos;
	
	private Texture2D background;
    private Texture2D foreground;
	
	public void SetMainCharacter(GameObject obj) {
		medrash = obj;
	}
	
	protected void EntityStart () {
		scoreValue = 10;
		controller = GetComponent<CharacterController>();
		medrash = GameObject.FindGameObjectWithTag("Player");
		Transform dBox = transform.Find("dmgBox");
		if (dBox)
			dmgBox = dBox.gameObject;
		if (reward != null && (lifeValue*energyValue > 0.0f))
			reward.GetComponent<FoodController>().setValues(lifeValue, energyValue);
		
		background = new Texture2D(1, 1, TextureFormat.RGB24, false);
        foreground = new Texture2D(1, 1, TextureFormat.RGB24, false);
		
		maxLife = life;
		
        background.SetPixel(0, 0, Color.white);
        foreground.SetPixel(0, 0, Color.green);
		
		background.Apply();
        foreground.Apply();
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
	
	public void AddScoreToMedrash () {
		medrash.GetComponent<MainCharacter>().AddToScore(scoreValue);
	}
	
	public int GetScoreValue () {
		return scoreValue;
	}
	
	public void DamageLifeStatus (float dmg) {
		if (canReceiveDamage) {
			life -= dmg;
			canReceiveDamage = false;
			Debug.Log(name + " damaged. Life: " + life);
			receivedDamage = true;
		}
	}
	
	public void SetIdleAnimation () {
		currentAnimation = idleAnimation;
		animation.wrapMode = WrapMode.Loop;
	}
	
	public void SetRunAnimation () {
		currentAnimation = runAnimation;
		animation.wrapMode = WrapMode.Loop;
	}
	
	public void SetWalkAnimation () {
		currentAnimation = walkAnimation;
		animation.wrapMode = WrapMode.Loop;
	}
	
	public void SetAttackAnimation () {
		currentAnimation = attackAnimation;
	}
	
	public void SetWaitAnimation () {
		currentAnimation = waitAnimation;
		animation.wrapMode = WrapMode.Loop;
	}
	
	public void SetAttackedAnimation () {
		currentAnimation = attackedAnimation;
		animation.wrapMode = WrapMode.Once;
	}
	
	public void SetTigerSpecialAttackAnimation () {
		currentAnimation = tigerSpecialAttackAnimation;
		animation.wrapMode = WrapMode.Once;
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
		if (medrash == null)
			medrash = GameObject.FindGameObjectWithTag("Player");
		return medrash.transform.position;
	}
	
	public bool CanBeAttacked() {
		return canBeAttacked;
	}
	
	public void MakeInvunerable() {
		canBeAttacked = false;
		canReceiveDamage = false;
	}
	
	void OnGUI(){
		if (!canBeAttacked)
			return;
		position = this.transform.position;
		float dist = DistanceToMainCharacter();
		if (dist < 20){
			float scaling = 2*(2 - dist/10);
			float barHeight = 2.5f*scaling;
			float barWidth = 15.0f*scaling;
			lifeBarPos = position;
			screenPos = Camera.main.WorldToScreenPoint(lifeBarPos);
			if (screenPos.z < 0)
				return;
			GUI.DrawTexture(new Rect(screenPos.x, screenPos.z + 100, barWidth, barHeight), background, ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(screenPos.x, screenPos.z + 100, barWidth*(life/maxLife), barHeight), foreground, ScaleMode.StretchToFill);
		}
	}
}
