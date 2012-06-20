using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour
{

	//public ParticleSystem dieExplosion;
	private GameObject medrash;	
	public float minDist = 2.0f;
	public float lifeValue = 100.0f;
	public float energyValue = 100.0f;
	private int foodDuration = 200, foodTimer = 0;
	
	void Start () {
		medrash = GameObject.FindGameObjectWithTag("Player");
		StartCoroutine(FoodUpdate());
	}
	
	void Update () {
		transform.RotateAround(transform.up, 5.0f*Time.deltaTime);
	}
	
	IEnumerator FoodUpdate () {
		float dist;
		
		while(true) {
			dist = DistanceToMainCharacter();
			foodTimer++;
			if (dist < minDist)
			{
				medrash.GetComponent<MainCharacter>().IncreaseLifeStatus(lifeValue);
				medrash.GetComponent<MainCharacter>().IncreaseEnergyStatus(lifeValue);
				Dispose();
				break;
			} else if ((foodTimer >= foodDuration) || (medrash.GetComponent<MainCharacter>().IsAlive() != true)) {
				Dispose();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	private float DistanceToMainCharacter()
	{
		Vector2 dist = new Vector2(medrash.transform.position.x - transform.position.x, medrash.transform.position.z - transform.position.z);
		return dist.magnitude;
	}
	
	public void setValues(float life, float energy)
	{
		lifeValue = life;
		energyValue = energy;
	}
	
	private void Dispose()
	{
		//Instantiate(dieExplosion, transform.position, transform.rotation);
		Destroy(this.gameObject);
	}
	
} 