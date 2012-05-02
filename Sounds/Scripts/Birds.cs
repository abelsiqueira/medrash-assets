using UnityEngine;
using System.Collections;

public class Birds : MonoBehaviour {
	
	public AudioClip[] birds;
	private float time;
	private float interval;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying && Time.time > time + interval)
		{
			time = Time.time;
			interval = Random.Range(0.0f, 5.0f);
			int n = Random.Range(0,birds.Length-1);
			audio.clip = birds[n];
			interval += birds[n].length;
			audio.Play();
		}
	}
}
