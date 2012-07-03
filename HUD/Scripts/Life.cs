using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {
	
	public Texture2D heart;
	public Texture2D x;
	
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
	
	private int life = 1;
	
	private int top;
	private int left;
	
	// Use this for initialization
	void Start () {
		top = Screen.height - 80;
		left = 10 + (heart.width) + 10 + x.width;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(10, Screen.height - 80, heart.width, heart.height), heart);
		GUI.DrawTexture(new Rect(heart.width + 10, Screen.height - 70, x.width - 20, x.height - 20), x);
		
		string auxLifeValue = life.ToString();
		float leftAux = left;
		
		for (int i = 0; i < auxLifeValue.Length; i++)
		{	
			char number = auxLifeValue[i];
			
			if(number == '0')
			{
				GUI.DrawTexture(new Rect(leftAux - number00.width, top, number00.width, number00.height), number00);
				leftAux = leftAux + number00.width;
			}
			if(number == '1')
			{
				GUI.DrawTexture(new Rect(leftAux - number01.width, top, number01.width, number01.height), number01);
				leftAux = leftAux + number01.width;
			}
			if(number == '2')
			{
				GUI.DrawTexture(new Rect(leftAux - number02.width, top, number02.width, number02.height), number02);
				leftAux = leftAux + number02.width;
			}
			if(number == '3')
			{
				GUI.DrawTexture(new Rect(leftAux - number03.width, top, number03.width, number03.height), number03);
				leftAux = leftAux + number03.width;
			}
			if(number == '4')
			{
				GUI.DrawTexture(new Rect(leftAux - number04.width, top, number04.width, number04.height), number04);
				leftAux = leftAux + number04.width;
			}
			if(number == '5')
			{
				GUI.DrawTexture(new Rect(leftAux - number05.width, top, number05.width, number05.height), number05);
				leftAux = leftAux + number05.width;
			}
			if(number == '6')
			{
				GUI.DrawTexture(new Rect(leftAux - number06.width, top, number06.width, number06.height), number06);
				leftAux = leftAux + number06.width;
			}
			if(number == '7')
			{
				GUI.DrawTexture(new Rect(leftAux - number07.width, top, number07.width, number07.height), number07);
				leftAux = leftAux + number07.width;
			}
			if(number == '8')
			{
				GUI.DrawTexture(new Rect(leftAux - number08.width, top, number08.width, number08.height), number08);
				leftAux = leftAux + number08.width;
			}
			if(number == '9')
			{
				GUI.DrawTexture(new Rect(leftAux - number09.width, top, number09.width, number09.height), number09);
				leftAux = leftAux + number09.width;
			}
		}
	}
}
