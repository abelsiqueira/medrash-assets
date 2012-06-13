using UnityEngine;
using System.Collections;

public class EnemyRoute : MonoBehaviour 
{

	public GameObject[] waypoints;
	private float totalDistance = 0;
	private float stepThreshold = 4.0f;
	
	void Start () 
	{
		int i = 0;
		foreach (GameObject waypoint in waypoints) i++;
		for (int j = 0; j < i - 1; j++) totalDistance += (waypoints[j + 1].transform.position.magnitude - waypoints[j].transform.position.magnitude) * -1;
		StartCourse();
	}
	
	IEnumerator Travel()
	{
		float currentDistance = totalDistance;
		while(true)
		{
			if(currentDistance > 0)
			{
				currentDistance -= stepThreshold;
				Debug.Log(currentDistance);
			}
			else
			{
				Debug.Log("O inimigo escapou");
				break;
			}
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	public void StartCourse()
	{
		StartCoroutine(Travel());
	}
	
}
