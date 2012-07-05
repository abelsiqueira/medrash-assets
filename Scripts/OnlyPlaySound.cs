using UnityEngine;
using System.Collections;

public class OnlyPlaySound : MonoBehaviour {

	// Use this for initialization
	void Start () {
		audio.Play();
		audio.loop = true;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
