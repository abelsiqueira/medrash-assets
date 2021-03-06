using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	private int numItem = 4;
	private enum item{
		continuar,
		autores,
		creditos,
		sair
	};
	private string[] itemString = {"Novo jogo", "Autores", "Creditos", "Sair"};

	private item selItem;
	public Texture background;
	public Font font;
	public Color Selected;
	public Color UnSelected;
	public int optionSize;
	private GUIStyle Style;

	void Start()
	{
		Style = new GUIStyle();
		Style.font = font;
		Style.alignment = TextAnchor.MiddleCenter;
		if (Selected.a == 0.0) Selected = new Color(255f, 241f, 0.0f, 1.000f);
		if (UnSelected.a == 0.0) UnSelected = new Color(0.500f, 0.643f, 0.500f, 1.000f);
		selItem = item.continuar;
		Screen.showCursor = false;
		if (optionSize <= 0)
			optionSize = Screen.height/(numItem+5);
		Style.fontSize = (int)(optionSize*0.7);


	}

	void OnGUI()
	{

		Color tmp = GUI.color;
		GUI.color = Color.black;

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

		//GUI.Label(new Rect(0, 0, backWidth, backHeight), background);
		int posIni = Screen.height/2 - numItem*optionSize/2 + (int)(backHeight*0.05); 



		for (int i = 0; i < numItem; i++)
		{
			if ((int)selItem == i)
				Style.normal.textColor = Selected;
			else 
				Style.normal.textColor = UnSelected;
			GUI.Label(new Rect(Screen.width / 2 - 70, ((Screen.height / 2)-200) + optionSize*i, 500, 200), itemString[i], Style);
		}




	}


	void LateUpdate()
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
					Application.LoadLevel(2);
					break;
				case item.autores:
					Application.LoadLevel(4);
					break;
				case item.creditos:
					Application.LoadLevel(5);
					break;
				case item.sair:
					Application.Quit();
					break;
			}
		}

	}
}