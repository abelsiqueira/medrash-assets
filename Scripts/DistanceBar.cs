using UnityEngine;
using System.Collections;

public class DistanceBar : MonoBehaviour {
	
	private float distance = 50;
	private float top, left;
	private Texture2D mainBar;
	private Texture2D pivot;
	
	Rect box = new Rect((Screen.width / 2) - 200, Screen.height - (Screen.height - 20), 400, 10);
	
	// Use this for initialization
	void Start () {
		top = Screen.height - (Screen.height - 10);
		left = Screen.width / 2;
		
		mainBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		mainBar.SetPixel(0, 0, Color.green);
		
		pivot = new Texture2D(1, 1, TextureFormat.RGB24, false);
		pivot.SetPixel(0, 0, Color.blue);
		
		mainBar.Apply();
		pivot.Apply();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void set_distance(float distance)
	{
		this.distance = distance;
	}
	
	public float get_distance()
	{
		return distance;
	}
	
	void OnGUI()
	{
		GUI.BeginGroup(box);
        {
			GUI.DrawTexture(new Rect(0, 0, 400, 10), mainBar);
			GUI.DrawTexture(new Rect((distance*400/100)-5, 0, 10, 10), pivot); 
		}
		GUI.EndGroup();
	}
}
