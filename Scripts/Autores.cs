using UnityEngine;
using System.Collections;

public class Autores : MonoBehaviour {
	
	public Texture2D autores;
	
	// Use this for initialization
	void Start () {
		StartCoroutine(Delay(4.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private enum Take {Take01};
	private Take take;
	
	void OnGUI()
	{
		switch(take)
		{
			case Take.Take01:
			GUI.DrawTexture(new Rect((Screen.width / 2)-512, (Screen.height/2) - 384, 1024, 768), autores);
			break;
		}
	}
	
	IEnumerator Delay(float time)
	{
		int i = 0;
		while (true)
		{
			if (i == 1) Application.LoadLevel(1);
			else if(i == 0) take = Take.Take01;
			i++;
			yield return new WaitForSeconds(time);
		}
	}
}
