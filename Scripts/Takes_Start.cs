using UnityEngine;
using System.Collections;

public class Takes_Start : MonoBehaviour {

	public Texture2D take01;
	public Texture2D take02;
	public Texture2D take03;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Delay(4.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private enum Take {Take01, Take02, Take03, Take04};
	private Take take;
	
	void OnGUI()
	{
		switch(take)
		{
			case Take.Take01:
			GUI.DrawTexture(new Rect((Screen.width / 2)-512, (Screen.height/2) - 384, 1024, 768), take01);
			break;
			case Take.Take02:
			GUI.DrawTexture(new Rect((Screen.width / 2)-512, (Screen.height/2) - 384, 1024, 768), take02);
			break;
			case Take.Take03:
			GUI.DrawTexture(new Rect((Screen.width / 2)-512, (Screen.height/2) - 384, 1024, 768), take03);
			break;
			case Take.Take04:
			Application.LoadLevel(1);
			break;
			
		}
	}
	
	IEnumerator Delay(float time)
	{
		int i = 0;
		while (true)
		{
			if (i == 4) break;
			else if(i == 0) take = Take.Take01;
			else if(i == 1) take = Take.Take02;
			else if(i == 2) take = Take.Take03;
			else if(i == 3) take = Take.Take04;
			i++;
			yield return new WaitForSeconds(time);
		}
	}
}
