using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class Water : MonoBehaviour {
	
	private GameObject medrash;
	// Use this for initialization
	void Start () {
		medrash = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Bounds bounds = GetComponent<BoxCollider>().bounds;
		Bounds medBounds = medrash.GetComponent<CharacterController>().bounds;
		
		if (bounds.Intersects(medBounds)) {
			medrash.GetComponent<MainCharacterController>().OnWater();
		} else
			medrash.GetComponent<MainCharacterController>().OffWater();
	}
}
