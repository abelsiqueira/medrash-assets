using UnityEngine;
using System.Collections;

public class FallingObjectCreator : MonoBehaviour {
	
	
	public GameObject rock;
	GameObject medrash;
	GameObject rockInstance;
	// Use this for initialization
	void Start () {
		medrash = GameObject.FindGameObjectWithTag("Player");
		rockInstance = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.collider.bounds.Contains(medrash.transform.position))
		{
			if (rockInstance == null)
			{
				Vector3 pos = this.transform.position;
				pos.y += 10;
				rock.transform.position = pos;
				rockInstance = (GameObject)GameObject.Instantiate(rock);
			}
		}
	}
}
