using UnityEngine;
using System.Collections;

public class EnterFinalBoss : MonoBehaviour {

	private GameObject Medrash;
	// Use this for initialization
	void Start () {
		Medrash = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (this.collider.bounds.Contains(Medrash.transform.position))
		{	
			Medrash.GetComponent<MainCharacter>().IncreaseEnergyStatus(1000);
			Medrash.GetComponent<MainCharacter>().hasSecondaryBar(false);
			//this.gameObject.SetActiveRecursively(true);
			this.enabled = false;
		}
	}
}
