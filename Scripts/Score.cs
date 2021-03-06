using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	
	private int scoreValue = 0;
	private float top, left;
	
	public Texture2D number00;
	public Texture2D number01;
	public Texture2D number02;
	public Texture2D number03;
	public Texture2D number04;
	public Texture2D number05;
	public Texture2D number06;
	public Texture2D number07;
	public Texture2D number08;
	public Texture2D number09;
	
	// Use this for initialization
	void Start () {
		top = Screen.height - 70;
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
		string auxScoreValue = scoreValue.ToString();
		float leftAux = left + 150;
		
		for (int i = auxScoreValue.Length-1; i >=0 ; i--)
		{	
			char number = auxScoreValue[i];
			
			if(number == '0')
			{
				GUI.DrawTexture(new Rect(leftAux - number00.width, top, number00.width, number00.height), number00);
				leftAux = leftAux - number00.width;
			}
			if(number == '1')
			{
				GUI.DrawTexture(new Rect(leftAux - number01.width, top, number01.width, number01.height), number01);
				leftAux = leftAux - number01.width;
			}
			if(number == '2')
			{
				GUI.DrawTexture(new Rect(leftAux - number02.width, top, number02.width, number02.height), number02);
				leftAux = leftAux - number02.width;
			}
			if(number == '3')
			{
				GUI.DrawTexture(new Rect(leftAux - number03.width, top, number03.width, number03.height), number03);
				leftAux = leftAux - number03.width;
			}
			if(number == '4')
			{
				GUI.DrawTexture(new Rect(leftAux - number04.width, top, number04.width, number04.height), number04);
				leftAux = leftAux - number04.width;
			}
			if(number == '5')
			{
				GUI.DrawTexture(new Rect(leftAux - number05.width, top, number05.width, number05.height), number05);
				leftAux = leftAux - number05.width;
			}
			if(number == '6')
			{
				GUI.DrawTexture(new Rect(leftAux - number06.width, top, number06.width, number06.height), number06);
				leftAux = leftAux - number06.width;
			}
			if(number == '7')
			{
				GUI.DrawTexture(new Rect(leftAux - number07.width, top, number07.width, number07.height), number07);
				leftAux = leftAux - number07.width;
			}
			if(number == '8')
			{
				GUI.DrawTexture(new Rect(leftAux - number08.width, top, number08.width, number08.height), number08);
				leftAux = leftAux - number08.width;
			}
			if(number == '9')
			{
				GUI.DrawTexture(new Rect(leftAux - number09.width, top, number09.width, number09.height), number09);
				leftAux = leftAux - number09.width;
			}
		}
	}
}
