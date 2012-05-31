using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {

	// Use this for initialization
	private Vector3 position;
	private Vector3 camPosition;
	public GameObject[] regions;
	private Quaternion rotation;
	public Texture image;
	private float startTime = -5.0f;
	
	void Start()
	{
		Save ();	
	}
	
	public void Save()
	{
		position = this.transform.position;
		//rotation = this.transform.rotation;
		camPosition = Camera.mainCamera.transform.position;
		startTime = Time.time;
		
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
		//medrash.transform.rotation = rotation;
	}
	
	void OnGUI()
	{
		if (Time.time - startTime >= 5.0)
				return;
		GUI.Label(new Rect(Screen.width - image.width, 0, image.width, image.height), image);
		
	}
}
