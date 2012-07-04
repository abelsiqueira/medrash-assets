using UnityEngine;
using System.Collections;

public class DieWhenTigerDies : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject tiger = GameObject.FindGameObjectWithTag("Tiger");
		if (!tiger)
			Destroy(this.gameObject);
	}
}
