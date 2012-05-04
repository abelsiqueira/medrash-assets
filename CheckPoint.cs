using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	// Use this for initialization
	private Vector3 position;
	private Vector3 camPosition;
	public GameObject[] regions;
	
	void Start()
	{
		Save ();	
	}
	
	public void Save()
	{
		position = this.transform.position;
		camPosition = Camera.mainCamera.transform.position;
		
	}
	
	public void Load()
	{
		for (int i = 0; i < regions.Length; i++)
			regions[i].GetComponent<RegionBox>().Restart();
		
		GameObject medrash = GameObject.FindGameObjectWithTag("Player");
		medrash.GetComponent<MainCharacter>().IncreaseLifeStatus(1000);
		medrash.GetComponent<MainCharacter>().IncreaseEnergyStatus(1000);
		medrash.transform.position = position;
		Camera.mainCamera.transform.position = camPosition;
		
		
	}
}
