using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {
	
	public GameObject medrash;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F4))
			medrash.transform.position = this.transform.position;
	}
}
