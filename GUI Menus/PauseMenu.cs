using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
	
	private int numItem = 3;
	private enum item{
		continuar,
		voltar,
		sair
	};
	private string[] itemString = {"continuar o jogo", "voltar ao menu inicial", "sair do jogo"};
	
	private item selItem;
	
	private bool active;
	private bool death = false;
	public Texture background;
	public Font font;
	public Color Selected;
	public Color UnSelected;
	public int optionSize;
	private GUIStyle Style;
	private float CallTime;
	
	void Start()
	{
		Style = new GUIStyle();
		Style.font = font;
		Style.alignment = TextAnchor.MiddleCenter;
		if (Selected.a == 0.0) Selected = new Color(0.388f, 0.078f, 0.063f, 1.000f);
		if (UnSelected.a == 0.0) UnSelected = new Color(0.459f, 0.365f, 0.255f, 1.000f);
		active = false;	
		selItem = item.continuar;
		Screen.showCursor = false;
		if (optionSize <= 0)
			optionSize = Screen.height/(numItem+5);
		Style.fontSize = (int)(optionSize*0.7);
		
			
	}
	
	void PauseGame()
	{
		Time.timeScale = 0;
		active = true;
	}
	
	void UnPauseGame()
	{
		if (death)
		{
			Camera.mainCamera.GetComponent<Sound>().isDead(false);
			GameObject.FindGameObjectWithTag("Player").GetComponent<CheckPoint>().Load();
			GameObject.FindGameObjectWithTag("Player").GetComponent<MainCharacterController>().ForceIdle();
		}
		Time.timeScale = 1;
		active = false;
	}
	
	public void CallMenu(string why)
	{
		if (why == "Death")
		{
			death = true;
			if (CallTime == 0.0f)
				CallTime = Time.time + 1.0f;
			Camera.mainCamera.GetComponent<Sound>().isDead(true);
		}
		
	}
	
	void OnGUI()
	{
		if (Time.time >= CallTime && CallTime != 0.0f)
		{
			PauseGame();
			CallTime = 0.0f;
		}
		if (active)
		{
			Color tmp = GUI.color;
			GUI.color = Color.black;
			GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
			GUI.color = tmp;
			int backHeight = Screen.height;
			int backWidth;
			if (backHeight > background.height)
			{
				backHeight = background.height;
				backWidth = background.width;
			}
			else
			{
				double factor = ((double)backHeight)/((double)background.height);
				backWidth = (int)(factor*(double)background.height);
			}
			
			GUI.Label(new Rect(Screen.width/2 - backWidth/2, Screen.height/2 - backHeight/2, backWidth, backHeight), background);
			int posIni = Screen.height/2 - numItem*optionSize/2 + (int)(backHeight*0.05); 
			

			
			for (int i = 0; i < numItem; i++)
			{
				if ((int)selItem == i)
					Style.normal.textColor = Selected;
				else 
					Style.normal.textColor = UnSelected;
				GUI.Label(new Rect(Screen.width/2 - backWidth/2, posIni + optionSize*i, backWidth, optionSize), itemString[i], Style);
			}

			
			
		}
	}
	
	
	void LateUpdate()
	{
		if (active)
		{
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				selItem = (item)(((int)selItem + 1) % numItem);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (selItem != 0)
					selItem -= 1;
				else
					selItem = (item)numItem-1;
			}
			if (Input.GetKeyDown(KeyCode.Return))
			{
				switch (selItem)
				{
					case item.continuar:
						UnPauseGame();
						break;
					case item.voltar:
						Application.LoadLevel("MainMenu");
						break;
					case item.sair:
						Application.Quit();
						break;
				}
				
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (!death)
					UnPauseGame();	
			}
		}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame();	
		}
	}
	
	public bool IsPaused()
	{
		return active;
	}
}
