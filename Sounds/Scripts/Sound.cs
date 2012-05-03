using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	// Use this for initialization
	public AudioClip MainSound;
	public AudioClip SecondarySound;
	public AudioClip DeadAudio;
	public float MainStartCut;
	public float MainEndCut;
	public float SecondaryStartCut;
	public float SecondaryEndCut;
	public int MainPercentual = 100;
	private float StartTime;
	private float StopTime;
	private float volume;
	private bool first = true;
	private bool fadeOut = false;
	public float fadeTime = 5.0f;
	public float fadeStart;
	private float fadeVolumeStart;
	
	private bool dead = false;
	
	void Start () {
		volume = audio.volume;	
	}
	
	public void isDead(bool d)
	{
		dead = d;	
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
		if (!Fade()) return;
		if (audio.isPlaying)
		{
			if (audio.time >= StopTime)
				audio.Stop();
		}
		if (!audio.isPlaying || audio.clip == DeadAudio)
		{
			if (Random.Range(0,100) < MainPercentual)
			{
				audio.clip = MainSound;
				StartTime = MainStartCut;
				StopTime = MainSound.length - MainEndCut;
			}
			else
			{
				audio.clip = SecondarySound;
				StartTime = SecondaryStartCut;
				StopTime = SecondarySound.length - SecondaryEndCut;
			}
			audio.time = StartTime;
			/*if (first)
			{
				first = false;
				audio.time =StopTime - 2;
			}*/
			audio.Play();
		}
		
	}
	
	private bool Fade()
	{
		if (fadeOut)
			if (audio.volume != 0.0f)
				FadeOut();
			else 
				return false;
		else if (audio.volume < volume)
			FadeIn();
		return true;
	}
	
	public void FadeOn()
	{
		if (fadeOut) return; 
		fadeVolumeStart = audio.volume;
		fadeStart = Time.time;
		fadeOut = true;
	}
	
	public void FadeOff()
	{
		if (!fadeOut) return;
		fadeStart = Time.time;
		fadeVolumeStart = audio.volume;
		fadeOut = false;
	}
	
	void FadeOut()
	{
		audio.volume = fadeVolumeStart - volume * (Time.time - fadeStart)/fadeTime;
		if (audio.volume < 0.0f)
			audio.volume = 0.0f;
	}
	
	void FadeIn()
	{
		audio.volume = fadeVolumeStart + volume * (Time.time - fadeStart)/fadeTime;
		if (audio.volume >= volume)
			audio.volume = volume;
	}
}
