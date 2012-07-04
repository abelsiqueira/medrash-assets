using UnityEngine;
using System.Collections;

public class IntroPlay : MonoBehaviour {

	// Use this for initialization
	void Start () {
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying) {
			Application.LoadLevel("As Cronicas de Medrash");
		}
	}
}
