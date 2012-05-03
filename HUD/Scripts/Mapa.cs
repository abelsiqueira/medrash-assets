using UnityEngine;
using System.Collections;

public class Mapa : MonoBehaviour {
	
	public Texture2D terreno01;
	
	private float positionX;
	private float positionZ;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		positionX = transform.position.x;
		positionZ = transform.position.z;
	}
	
	public Terrain terrain;
	
	void OnGUI() {
		Vector3 dimensions = terrain.terrainData.size;
		float W = dimensions.x;
		float H = dimensions.z;
		float minimapWidth = Screen.height*0.33f;
		float scale = minimapWidth/W;
		float minimapHeight = H*scale;
		float minimapPositionX = Screen.width - minimapWidth - 10;
		float minimapPositionZ = Screen.height - minimapHeight - 10;
		
		
		GUI.DrawTexture(new Rect(minimapPositionX,minimapPositionZ, minimapWidth, minimapHeight), terreno01);
		GUI.color = Color.red;
		GUI.Box (new Rect(positionX*scale + minimapPositionX, minimapHeight-positionZ*scale + minimapPositionZ, 10, 10), "");
	}
}
