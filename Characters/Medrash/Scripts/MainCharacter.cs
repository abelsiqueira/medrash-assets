using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainCharacter : MonoBehaviour
{
	public Terrain terrain;
	public GameObject dmgBox;
	private bool hasTorch;
	private Scene scene;
	private PrimaryBar primaryBar;
	private SecondaryBar secondaryBar;
	private MainCharacterController characterController;
	
	private float lifeStatus = 100.0f;
    private float temperatureStatus = 100.0f;
    private float energyStatus = 100.0f;
	private float positionX;
	private float positionZ;
	
	public float energyLossValue = 0.2f;
	public float lifeLossValue = 0.1f;
	public float temperatureLossValue = 0.2f;
	public float torchTimerValue = 10.0f;
	private int delayAttackValue = 6;
	
	private List<Entity> listOfEnemies = new List<Entity>();
	
	public enum Scene
	{
		Scene01, 
		Scene02, 
		Scene03
	}
	
	void Start ()
	{
		scene = Scene.Scene01;	
		primaryBar = GetComponent<PrimaryBar>();
		secondaryBar = GetComponent<SecondaryBar>();
		characterController = GetComponent<MainCharacterController>();
		if (scene.Equals(Scene.Scene01))
		{
			secondaryBar.HasBar = true;
			secondaryBar.setopcao(2);
			StartCoroutine(CheckCharacterPhysicalCondition());	
		}
		else if (scene.Equals(Scene.Scene02))
		{
			GrabTorch();
			secondaryBar.HasBar = true;
			secondaryBar.setopcao(1);
			StartCoroutine(CheckCharacterBodyTemperature());	
		}
		else
		{
			secondaryBar.HasBar = false;
		}
		object[] obj = GameObject.FindObjectsOfType(typeof(Entity));
		foreach (object o in obj) {
			Entity e = (Entity) o;
			e.SetMainCharacter(this.gameObject);
			listOfEnemies.Add(e);
			e.enabled = true;
		}
	}
	
	void Update()
	{
		/*
		if (Input.GetButtonDown("Fire1"))
		{
			if (characterController.canAttack)
			{
				Attack();
			}
		}
		else if (Input.GetButtonDown("Fire2"))
		{
			//Interact();
		}
		else if (Input.GetButtonDown("Fire3"))
		{
			//Defend();
		}*/
	}
			
	
	// rotina para verificar a condição física de MainCharacter
	IEnumerator CheckCharacterPhysicalCondition()
	{
		float n = characterController.runSpeed - characterController.walkSpeed;
		n /= 6;
		int i = 0;
		while(true)
		{
			if(characterController.IsMoving())
			{
				DamageEnergyStatus(energyLossValue);
			}
			if (energyStatus > 50)
			{
				if (i != 0)
				{
					i = 0;
				}
			}
			if (energyStatus >= 20 && energyStatus < 50)
			{
				if ((((int)energyStatus) == 45) && (i == 0))
				{
					i++;
					characterController.runSpeed -= n;
				}
				else if ((((int)energyStatus) == 40) && (i == 1))
				{
					i++;
					characterController.runSpeed -= n;
				}
				else if ((((int)energyStatus) == 35) && (i == 2))
				{
					i++;
					characterController.runSpeed -= n;
				}
				else if ((((int)energyStatus) == 30) && (i == 3))
				{
					i++;
					characterController.runSpeed -= n;
				}
				else if ((((int)energyStatus) == 25) && (i == 4))
				{
					i++;
					characterController.runSpeed -= n;
				}
				else if ((((int)energyStatus) == 20) && (i == 5))
				{
					i++;
					characterController.runSpeed -= n;
				}
			}
			else if (energyStatus > 0 && energyStatus < 20)
			{
				characterController.canJump = false;
				characterController.canRun = false;
				characterController.runSpeed = characterController.walkSpeed;
			}
			else if (energyStatus == 0)
			{
				DamageLifeStatus(lifeLossValue);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	// rotina para verificar a temperatura corporal de MainCharacter
	IEnumerator CheckCharacterBodyTemperature()
	{
		while(true)
		{
			if (!hasTorch)
			{
				DamageTemperatureStatus(temperatureLossValue);
			}	
			if (temperatureStatus == 0)
			{
				DamageLifeStatus(lifeLossValue);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	// timer para o tempo da tocha
	IEnumerator TorchTimer()
	{
		int i = 0;
		float n = 100 - temperatureStatus;
		float j = n / 30;
		while (true)
		{
			if (i < torchTimerValue)
			{
				i++;
				IncreaseTemperatureStatus(j += n / 30);
				yield return new WaitForSeconds(1.0f);
			}
			else
			{
				hasTorch = false;
				temperatureStatus = 100.0f;
				break;
			}
		}
	}
	
	public void TryToAttack () {
		float minDist = 1e10f;
		Entity closestEntity = listOfEnemies[0];
		
		foreach (Entity entity in listOfEnemies) {
			if (!entity)
				continue;
			float dist = (entity.transform.position - transform.position).sqrMagnitude;
			if (dist < minDist) {
				minDist = dist;
				closestEntity = entity;
			}
		}
		
		Vector3 d = closestEntity.transform.position - transform.position;
		d.y = 0;
		characterController.SetDirection(d);
		
		StartCoroutine(DelayAttack(closestEntity));
	}
	
	// delay entre a execução da animação de ataque e do dano causado
	IEnumerator DelayAttack(Entity closestEntity)
	{
		int i = 0;
		while (true)
		{
			Debug.Log(i);
			if (i >= delayAttackValue) 
			{
				Attack(closestEntity);
				break;
			}
			else i++;
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	// infere dano sobre a vida de MainCharacter
	public void DamageLifeStatus(float x)
	{
		if (lifeStatus - x > 0)
		{
			lifeStatus -= x;
			primaryBar.setHealth(100 - lifeStatus);
		}
		else
		{
			lifeStatus = 0;
			primaryBar.setHealth(100);
			//characterController.KillCharacter();
			//PauseMenu menu = GetComponent<PauseMenu>();
			//menu.CallMenu();
		}
	}
	
	// infere dano sobre a energia de MainCharacter
	void DamageEnergyStatus(float x)
	{
		if (energyStatus - x > 0)
		{
			energyStatus -= x;
			secondaryBar.setStatus (100 - energyStatus);
		}
		else
		{
			energyStatus = 0;
			secondaryBar.setStatus(100);
		}
	}
	
	// infere dano sobre a temperatura de MainCharacter
	void DamageTemperatureStatus(float x)
	{
		if (temperatureStatus - x > 0)
		{
			temperatureStatus -= x;
			secondaryBar.setStatus (100 - temperatureStatus);
		}
		else
		{
			temperatureStatus = 0;
			secondaryBar.setStatus(100);
		}
	}
	
	// aumenta a vida de MainCharacter
	public void IncreaseLifeStatus(float x)
	{
		if (lifeStatus + x > 100)
		{
			lifeStatus = 100;
			primaryBar.setHealth(0);
		}
		else
		{
			lifeStatus += x;
			primaryBar.setHealth(100 - lifeStatus);
		}
	}
	
	// aumenta a energia de MainCharacter
	public void IncreaseEnergyStatus(float x)
	{
		if (energyStatus + x > 100)
		{
			energyStatus = 100;
			secondaryBar.setStatus(0);
		}
		else
		{
			energyStatus += x;
			secondaryBar.setStatus(100 - energyStatus);
		}	
	}
	
	// aumenta a temperatura de MainCharacter
	void IncreaseTemperatureStatus(float x)
	{
		if (temperatureStatus + x > 100)
		{
			temperatureStatus = 100;
			secondaryBar.setStatus(0);
		}
		else 
		{
			temperatureStatus += x;
			secondaryBar.setStatus(100 - temperatureStatus);
		}
	}
	
	// MainCharacter pega a tocha e o timer é disparado
	public void GrabTorch()
	{
		hasTorch = true;
		StartCoroutine(TorchTimer());
	}
	
	protected void Attack(Entity closestEntity)
	{
		Bounds bounds = closestEntity.GetComponent<CharacterController>().bounds;
		Bounds medBounds = dmgBox.collider.bounds;

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
	
	void Interact()
	{
		/*Debug.Log("didInteract");
		if (scene.Equals(Scene.Scene02))
		{
			GrabTorch();
		}*/
	}
	
	void Defend()
	{
		//Debug.Log("didDefend");
	}
}
