using UnityEngine;
using System.Collections;

public class SecondaryBar : MonoBehaviour {

	Rect box = new Rect(45, 53, 25, 300);
 
    private Texture2D background;
    private Texture2D foreground;
 
	private bool hasBar = true;
	
    public float status;
    public int maxStatus = 100;
	
	public Texture2D thermometer;
	public Texture2D lightning;
	
	public int opcaoX = 2;
	
	public void setopcao(int valor)
	{
		opcaoX = valor;
	}
	
	public int getopcao()
	{
		return opcaoX;
	}
	
	public bool HasBar
    {
        get { return hasBar; }
        set { hasBar = value; }
    }
	
	// Use this for initialization
	void Start () {
		if (hasBar)
		{
			background = new Texture2D(1, 1, TextureFormat.RGB24, false);
	        foreground = new Texture2D(1, 1, TextureFormat.RGB24, false);
 
        	background.SetPixel(0, 0, Color.green);
        	foreground.SetPixel(0, 0, Color.white);
 		
			status = 0;
		
        	background.Apply();
        	foreground.Apply();
			StartCoroutine(DrawBar());
		}
	}
	
	// Update is called once per frame
	IEnumerator DrawBar () {
		while(true) {
			//Altera cor conforme o estado
			if (status <= 50 ){
				background.SetPixel(0, 0, Color.green);
				background.Apply();
			}
				
			if ((status > 50) && (status <= 80)){
				background.SetPixel(0, 0, Color.yellow);
				background.Apply();
			}
			
			if (status > 80){
				background.SetPixel(0, 0, Color.red);
				background.Apply();
			}			
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public void setStatus(float value)
	{
		if ((value >= 0) && (value <= 100)){
			status = value;
		}
	}
	
	float getStatus()
	{
		return status;
	}
	
	void OnGUI()
	{
		if (hasBar)
		{
			
			
			GUI.Box (new Rect(42, 10, 31, 350),"");
			
			GUI.BeginGroup(box);
       		{
				GUI.Box (new Rect(42, 10, 31, 350),"");
            	GUI.DrawTexture(new Rect(0, 0, box.width, box.height), background, ScaleMode.StretchToFill);
            	GUI.DrawTexture(new Rect(0, 0, box.width, box.height*status/maxStatus), foreground, ScaleMode.StretchToFill);
        	}
        	GUI.EndGroup();
			if (opcaoX == 1)
			{
				GUI.DrawTexture(new Rect(45,17, 25, 25), thermometer);
			}
			else
			if (opcaoX == 2)
			{
				
				GUI.DrawTexture(new Rect(45,17, 25, 25), lightning);
			}
		}
	}
}
