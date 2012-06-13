using UnityEngine;
using System.Collections;

public class Points : MonoBehaviour {
	
	private int point_value = 255;
	private float top, left;
	
	// Use this for initialization
	void Start () {
		top = Screen.height - (Screen.height - 10);
		left = Screen.width - 160;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void set_point(int point_value)
	{
		this.point_value = point_value;
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(left-5, top, 150, 45), "");
		
		GUIStyle myStyle = new GUIStyle();
		myStyle.fontSize = 40;
		myStyle.fontStyle = FontStyle.Bold;
		
		myStyle.normal.textColor = Color.white;
		
		GUI.TextField(new Rect(left, top, 150, 60),"" + point_value, myStyle);
	}
}
