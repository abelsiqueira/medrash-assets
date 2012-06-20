using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour {
	
	private int number_life = 3;
	private float top, left;
	private float distance;
	public Texture2D life;
	
	// Use this for initialization
	void Start () {
		top = Screen.height - (Screen.height - 10);
		left = 90;
		distance = left + 5;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void set_life(int number_life)
	{
		this.number_life = number_life;
	}
	
	public int get_life()
	{
		return number_life;
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(left, top, 95, 30), "");
		
		for(int i = 0; i < number_life; i++)
		{
			GUI.DrawTexture(new Rect(distance, top, 25, 25), life);
			distance = distance + 30;
		}
		
		distance = left + 5;
	}
}
