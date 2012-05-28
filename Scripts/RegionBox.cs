using UnityEngine;
using System.Collections;

public class RegionBox : MonoBehaviour {

	// Use this for initialization
	
	
	private bool Active = false;
	public GameObject medrash;
	public GameObject[] enemy;
	private Vector3[] position;
	private GameObject[] copy;
	private int length;
	private bool first = false;
	private bool canUpdate = false;
	private bool saved = true;
	
	void Start () {
		length = enemy.Length;
		position = new Vector3[length];
		for (int i = 0; i < length; i++)
			position[i] = enemy[i].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.collider.bounds.Contains(medrash.transform.position))
			Activate();
		else
			DeActivate(true);
	}
	
	void Activate()
	{
		if (Active) return;
		Active = true;
		copy = new GameObject[length];
		for (int i = 0; i < length; i++)
		{
			copy[i] = (GameObject)Instantiate(enemy[i]);
			copy[i].transform.position = position[i];
			medrash.GetComponent<MainCharacter>().addEnemy(copy[i].GetComponent<Entity>());
		}
		
	}

	void DeActivate(bool save)
	{
		if (!Active) return;
		Active = false;
		if (save && saved)
		{
			saved = false;
			medrash.GetComponent<CheckPoint>().Save();
		}
		for (int i = 0; i < length; i++)
			if (copy[i] != null)
				GameObject.Destroy(copy[i]);

	}
	
	public void Restart()
	{
		DeActivate(false);
	}
}

