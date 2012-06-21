using UnityEngine;
using System.Collections;

public class Wither : MonoBehaviour {
	
	public float witherTimer = 5.0f;
	private float startTime;
		
	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > startTime + witherTimer) {
			Destroy(this.gameObject);
		}
	}
}
