using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	
	private int scoreValue = 0;
	private float top, left;
	
	// Use this for initialization
	void Start () {
		top = Screen.height - (Screen.height - 10);
		left = Screen.width - 160;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetScore(int scoreValue)
	{
		this.scoreValue = scoreValue;
	}
	
	public void AddToScore(int points) {
		this.scoreValue += points;
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(left-5, top, 150, 60), "");
		
		GUIStyle myStyle = new GUIStyle();
		myStyle.fontSize = 40;
		myStyle.fontStyle = FontStyle.Bold;
		myStyle.alignment = TextAnchor.MiddleRight;
		
		myStyle.normal.textColor = Color.white;
		
		GUI.TextField(new Rect(left, top, 140, 60),"" + scoreValue, myStyle);
		
		//Remover depois por favor
		float dist = GetComponent<MainCharacterController>().GetDistanceFromSora();
		GUI.TextField(new Rect(Screen.width/2, 10, 100, 100),"" + dist);
	}
}
