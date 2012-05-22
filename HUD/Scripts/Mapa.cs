using UnityEngine;
using System.Collections;

public class Mapa : MonoBehaviour {
	
	public  Texture2D terreno01;
	public  Texture2D baseTriangle;
	private Texture2D triangle;
	
	private float positionX;
	private float positionZ;
	private float angulo;
	
	private Vector3 direction;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float directionX, directionZ;
		directionX = transform.forward.x;
		directionZ = transform.forward.z;
		positionX = transform.position.x;
		positionZ = transform.position.z;
		
		angulo = 180.0f*Mathf.Atan2(directionZ, directionX)/Mathf.PI;
		
		triangle = rotateTexture(baseTriangle, angulo); 
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
		
		float triangleSize = 20;
		float trianglePositionX = positionX*scale + minimapPositionX - triangleSize/2;
		float trianglePositionZ = minimapHeight - positionZ*scale + minimapPositionZ - triangleSize/2;
		
		GUI.DrawTexture(new Rect(minimapPositionX,minimapPositionZ, minimapWidth, minimapHeight), terreno01);
		//GUI.color = Color.red;
		GUI.DrawTexture(new Rect(trianglePositionX, trianglePositionZ, triangleSize, triangleSize), triangle);
		//GUI.Box (new Rect(positionX*scale + minimapPositionX, minimapHeight-positionZ*scale + minimapPositionZ, 10, 10), "");
	}
	
	Texture2D rotateTexture(Texture2D tex, float angle) {
        Texture2D rotImage = new Texture2D(tex.width, tex.height);
        int  x,y;
        float x1, y1, x2,y2;
        int w = tex.width;
        int h = tex.height;
        float x0 = rot_x (angle, -w/2.0f, -h/2.0f) + w/2.0f;
        float y0 = rot_y (angle, -w/2.0f, -h/2.0f) + h/2.0f;
        float dx_x = rot_x (angle, 1.0f, 0.0f);
        float dx_y = rot_y (angle, 1.0f, 0.0f);
        float dy_x = rot_x (angle, 0.0f, 1.0f);
        float dy_y = rot_y (angle, 0.0f, 1.0f);

        x1 = x0;
        y1 = y0;
		
        for (x = 0; x < tex.width; x++)
		{ 
			x2 = x1;
            y2 = y1;
			
            for ( y = 0; y < tex.height; y++)
			{ 
            	x2 += dx_x;//rot_x(angle, x1, y1); 
            	y2 += dx_y;//rot_y(angle, x1, y1); 

	            rotImage.SetPixel ( (int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex,x2, y2)); 
            }
            x1 += dy_x;
            y1 += dy_y;
        }
		
        rotImage.Apply();
		
        return rotImage; 
    }

    private Color getPixel(Texture2D tex, float x, float y) {
        Color pix;
        int x1 = (int) Mathf.Floor(x);
        int y1 = (int) Mathf.Floor(y);
		
        if(x1 > tex.width || x1 < 0 || y1 > tex.height || y1 < 0)
		{
            pix = Color.clear;

        }
		else
		{
            pix = tex.GetPixel(x1,y1);
        }

        return pix;
    }

    private float rot_x (float angle, float x, float y) {
        float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
        float sin = Mathf.Sin(angle/180.0f*Mathf.PI);

        return (x * cos + y * (-sin));
    }

    private float rot_y (float angle, float x, float y) {
        float cos = Mathf.Cos(angle/180.0f*Mathf.PI);
        float sin = Mathf.Sin(angle/180.0f*Mathf.PI);

        return (x * sin + y * cos);
    }
}
