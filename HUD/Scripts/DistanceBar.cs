using UnityEngine;
using System.Collections;

public class DistanceBar : MonoBehaviour {
	
	private float distance = 50;
	private float top, left;
	private Texture2D mainBar, dangerBar;
	private Texture2D pivot;
	
	public Texture2D medrash;
	public Texture2D sora;
	
	Rect box = new Rect(Screen.width - 500, Screen.height - (Screen.height - 10), 480, 60);
	
	// Use this for initialization
	void Start () {
		top = 20;
		left = 0;
		
		mainBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		mainBar.SetPixel(0, 0, Color.green);
		
		dangerBar = new Texture2D(1, 1, TextureFormat.RGB24, false);
		dangerBar.SetPixel(0, 0, Color.red);
		
		pivot = new Texture2D(1, 1, TextureFormat.RGB24, false);
		pivot.SetPixel(0, 0, Color.blue);
		
		mainBar.Apply();
		dangerBar.Apply();
		pivot.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		distance = GetComponent<MainCharacterController>().GetDistanceFromSora();
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
		Debug.Log(distance);
		GUI.BeginGroup(box);
        {
			GUI.DrawTexture(new Rect(left, top, 370, 10), mainBar);
			GUI.DrawTexture(new Rect(left + 300, top, 70, 10), dangerBar);
			GUI.DrawTexture(new Rect(left + distance*3, 0, 60, 60), sora);
			GUI.DrawTexture(new Rect(left, 0, 60, 60), medrash);
		}
		GUI.EndGroup();
		
		
	}
}
