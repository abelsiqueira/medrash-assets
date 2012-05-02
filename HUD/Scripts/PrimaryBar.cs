using UnityEngine;
using System.Collections;

public class PrimaryBar : MonoBehaviour {
	
	Rect box = new Rect(10, 53, 25, 300);
 
    private Texture2D background;
    private Texture2D foreground;
 	private Texture2D teste;
	
    public float health;
    public int maxHealth = 100;
	
	public Texture2D heart;
	
	// Use this for initialization
	void Start () {
		background = new Texture2D(1, 1, TextureFormat.RGB24, false);
        foreground = new Texture2D(1, 1, TextureFormat.RGB24, false);
			
        background.SetPixel(0, 0, Color.green);
        foreground.SetPixel(0, 0, Color.white);
 		
		health = 0;
		
        background.Apply();
        foreground.Apply();
	}
	
	// Update is called once per frame
	void Update () {
		//Altera cor conforme a vida
		if (health <= 50 ){
			background.SetPixel(0, 0, Color.green);
			background.Apply();
		}
		
		if ((health > 50) && (health <= 80)){
			background.SetPixel(0, 0, Color.yellow);
			background.Apply();
		}
		
		if (health > 80){
			background.SetPixel(0, 0, Color.red);
			background.Apply();
		}	
	}
	
	public void setHealth(float value)
	{
		if ((value >= 0) && (value <= 100)){
			health = value;
		}
	}
	
	public float getHealth()
	{
		return health;
	}
	
	
	void OnGUI()
	{
		//Draw heart
		
		GUI.Box (new Rect(7, 10, 31, 350),"");
		//Draw life bar
		GUI.BeginGroup(box);
        {
            GUI.DrawTexture(new Rect(0, 0, box.width, box.height), background, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(0, 0, box.width, box.height*health/maxHealth), foreground, ScaleMode.StretchToFill);
        }
        GUI.EndGroup();
		GUI.DrawTexture(new Rect(10,17, 25, 25), heart);
	}
}
