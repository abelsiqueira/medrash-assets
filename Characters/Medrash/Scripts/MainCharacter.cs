using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainCharacter : MonoBehaviour
{
	public Terrain terrain;
	public GameObject dmgBox;
	private Score myScore;
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
	
	private float energyLossValue = 0.1f;
	private float lifeLossValue = 0.1f;
	private float temperatureLossValue = 0.2f;
	private float torchTimerValue = 10.0f;
	
	private List<Entity> listOfEnemies = new List<Entity>();
	
	public enum Scene
	{
		Scene01, 
		Scene02, 
		Scene03
	}
	
	void Start ()
	{
		Time.timeScale = 1;
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
		}
		myScore = GetComponent<Score>();
		myScore.SetScore(0);
		StartCoroutine(ActivateEnemies());
	}
	
	public void addEnemy(Entity e)
	{
		e.SetMainCharacter(this.gameObject);
		listOfEnemies.Add(e);
		e.enabled = true;
	}
	
	IEnumerator ActivateEnemies ()
	{
		while(true)
		{
			foreach (Entity e in listOfEnemies) {
				if (!e)
					continue;
				if (e.DistanceToMainCharacter() < 100.0f)
					e.enabled = true;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	IEnumerator CheckCharacterPhysicalCondition()
	{
		float originalSpeed = characterController.GetRunSpeed();
		float n = characterController.GetRunSpeed() - characterController.GetWalkSpeed();
		float walkSpeed, runSpeed;
		n /= 6;
		int i = 0;
		while(true)
		{
			walkSpeed = characterController.GetWalkSpeed();
			runSpeed  = characterController.GetRunSpeed();
			
			if(characterController.IsMoving())
			{
				DamageEnergyStatus(energyLossValue);
			}
			if (energyStatus > 50)
			{
				characterController.canRun = true;
				characterController.SetRunSpeed(originalSpeed);
				if (i != 0)
				{
					i = 0;
				
				}
			}
			if (energyStatus >= 20 && energyStatus < 50)
			{
				characterController.canRun = true;
				if ((((int)energyStatus) == 45) && (i == 0))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
				else if ((((int)energyStatus) == 40) && (i == 1))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
				else if ((((int)energyStatus) == 35) && (i == 2))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
				else if ((((int)energyStatus) == 30) && (i == 3))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
				else if ((((int)energyStatus) == 25) && (i == 4))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
				else if ((((int)energyStatus) == 20) && (i == 5))
				{
					i++;
					characterController.SetRunSpeed(runSpeed - n);
				}
			}
			else if (energyStatus > 0 && energyStatus < 20)
			{
				characterController.canRun = false;
				characterController.SetRunSpeed(walkSpeed);
			}
			else if (energyStatus <= 0)
			{
				DamageLifeStatus(lifeLossValue);
			}
			yield return new WaitForSeconds(0.1f);
		}
	}

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
	
	public void DamageLifeStatus(float x)
	{
		if (lifeStatus - x > 0)
		{
			if (!characterController.IsEvading())
			{
				lifeStatus -= x;
				if (x != lifeLossValue) characterController.ReceiveAttack();
				primaryBar.setHealth(100 - lifeStatus);
			}
		}
		else
		{
			lifeStatus = 0;
			primaryBar.setHealth(100);
			characterController.ForceDeath();
			Camera.mainCamera.GetComponent<PauseMenu>().CallMenu("Death");
		}
	}
	
	void DamageEnergyStatus(float x)
	{
		if (!secondaryBar.HasBar) return;
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
	
	public void GrabTorch()
	{
		hasTorch = true;
		StartCoroutine(TorchTimer());
	}
	
	public bool IsAlive()
	{
		if (lifeStatus > 0) return true;
		else return false;
	}
	
	
	public void hasSecondaryBar(bool has)
	{
		secondaryBar.HasBar = has;
	}
	
	public List<Entity> GetListOfEnemies () {
		return listOfEnemies;
	}
	
	public void AddToScore (int points) {
		myScore.AddToScore (points);
	}
}
