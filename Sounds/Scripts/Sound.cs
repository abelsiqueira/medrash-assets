using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	// Use this for initialization
	public AudioClip MainSound;
	public AudioClip FinalBossSound;
	public AudioClip DeadAudio;
	public AudioClip Wind;
	public float MainStartCut;
	public float MainEndCut;
	public float SecondaryStartCut;
	public float SecondaryEndCut;
	public int MainPercentual = 100;
	private float StartTime;
	private float StopTime;
	private float volume;
	private bool first = true;
	private bool fade = false;
	private bool finalBoss = false;
	public float fadeDistance;
	private Vector3 fadeReference;	
	private bool dead = false;
	
	void Start () {
		volume = audio.volume;	
	}
	
	public void isDead(bool d)
	{
		dead = d;	
	}
	
	public void enterFinalBoss()
	{
		finalBoss = true;
		audio.volume = volume;
	}
	
	// Update is called once per frame
	void Update () {
		if (dead)
		{
			audio.volume = volume;
			if (!audio.isPlaying || audio.clip != DeadAudio)
			{
				audio.clip= DeadAudio;
				audio.Play();
			}
			return;
		}
		if (finalBoss)
		{
			audio.volume = volume;
			if (!audio.isPlaying || audio.clip != FinalBossSound)
			{
				audio.clip= FinalBossSound;
				audio.Play();
			}
			return;
		}
		
		Fade();
		if (audio.isPlaying)
		{
			if (audio.time >= StopTime)
				audio.Stop();
		}
		if (!audio.isPlaying || audio.clip != MainSound)
		{
				audio.clip = MainSound;
				StartTime = MainStartCut;
				StopTime = MainSound.length - MainEndCut;
				audio.time = StartTime;
				audio.Play();
		}
		
	}
	
	private void Fade()
	{
		if (fade)
		{
			float v = volume*((fadeDistance - Vector3.Distance(this.transform.position, 
				fadeReference))/fadeDistance);
			Debug.Log(v);
			if (v < 0.0f)
				audio.volume = 0.0f;
			else
				audio.volume = v;
		}
	}
	
	public void FadeOn()
	{
		Debug.Log("Fade ON");
		fade = true;
		fadeReference = this.transform.position;
	}
	
	public void FadeOff()
	{
		fade = false;
		audio.volume = volume;
	}
	
}
