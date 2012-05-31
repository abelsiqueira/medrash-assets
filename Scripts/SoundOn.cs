using UnityEngine;
using System.Collections;

public class SoundOn : MonoBehaviour {
	
	public GameObject medrash;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (this.collider.bounds.Contains(medrash.transform.position))
		{
			Camera.mainCamera.GetComponent<Sound>().SoundOn();
			this.gameObject.active = false;
		}
	}
}
