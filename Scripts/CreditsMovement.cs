using UnityEngine;
using System.Collections;

public class CreditsMovement : MonoBehaviour {
	
	Vector3 startPosition;
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(-15.0f*Time.deltaTime, 0.0f, 0.0f);
		if (transform.position.x < -250.0f) {
			GameObject.Instantiate(gameObject, startPosition, transform.rotation);
			Destroy(this.gameObject);
		}
	}
}
