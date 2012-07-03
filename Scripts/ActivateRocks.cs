using UnityEngine;
using System.Collections;

public class ActivateRocks : MonoBehaviour {
	
	public GameObject []rocks;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			foreach (GameObject rock in rocks) {
				rock.SetActiveRecursively(true);
			}
		}
	}
}
