using UnityEngine;
using System.Collections;

public class BarrierOff : MonoBehaviour {
	
	public GameObject Medrash;
	public GameObject MainCamera;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (this.collider.bounds.Contains(Medrash.transform.position))
		{	
			MainCamera.GetComponent<Sound>().FadeOn();
		}
	}
}
