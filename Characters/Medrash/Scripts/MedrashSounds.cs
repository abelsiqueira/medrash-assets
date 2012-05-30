using UnityEngine;
using System.Collections;

public class MedrashSounds : MonoBehaviour {
	
	public AudioClip attackAudio;
	public AudioClip receiveDamageAudio;
	public AudioClip stepAudio;
	private float volume;
	private bool canPlayReceiveDamageAudio = true;

	void Start () {
		volume = audio.volume;
	}
	
	IEnumerator ReceiveDamageAudioCooldown()
	{
		int i = 0;
		canPlayReceiveDamageAudio = false;
		while (true)
		{
			if (i != 0) 
			{
				canPlayReceiveDamageAudio = true;	
				break;
			}
			else i++;
			yield return new WaitForSeconds(0.5f);
		}
	}
	
	public void PlayStepAudio(float volume)
	{
		this.volume = volume;
		audio.volume = volume;		
		audio.clip = stepAudio;
		audio.Play();
	}
	
	public void PlayAttackAudio(float volume)
	{
		this.volume = volume;
		audio.volume = volume;
		audio.clip = attackAudio;
		audio.Play();
	}
	
	public void PlayReceiveDamageAudio(float volume)
	{
		if (canPlayReceiveDamageAudio)
		{
			Debug.Log("passei");
			this.volume = volume;
			audio.volume = volume;
			audio.clip = receiveDamageAudio;
			audio.Play();
			StartCoroutine(ReceiveDamageAudioCooldown());
		}
	}

}
