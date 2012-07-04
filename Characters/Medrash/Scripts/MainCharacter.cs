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
	
	private List<Entity> listOfEnemies = new List<Entity>();
	
	void Start ()
	{
		Time.timeScale = 1;
		primaryBar = GetComponent<PrimaryBar>();
		secondaryBar = GetComponent<SecondaryBar>();
		characterController = GetComponent<MainCharacterController>();
		secondaryBar.HasBar = true;
		secondaryBar.setopcao(2);
		StartCoroutine(CheckCharacterPhysicalCondition());	
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
				if (e.DistanceToMainCharacter() < 40.0f)
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
	
	public void DamageLifeStatus(float x)
	{
		if (lifeStatus - x > 0)
		{
			lifeStatus -= x;
			if (x != lifeLossValue) characterController.ReceiveAttack();
			primaryBar.setHealth(100 - lifeStatus);
		}
		else
		{
			if (lifeStatus != 0)
			{
				lifeStatus = 0;
				primaryBar.setHealth(100);
				characterController.ForceDeath();
				//Camera.mainCamera.GetComponent<PauseMenu>().CallMenu("Death");
			}			
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
	
	public float GetEnergyStatus()
	{
		return energyStatus;
	}
}
