using UnityEngine;
using System.Collections;

public class ObjectDropScript : MonoBehaviour 
{
	public GameObject[] obstaclesArray;
	public float SpawnDuration= 2.0f;
	
	Bounds region;
	float SpawnTimer;
	
	void Start () 
	{
		region = renderer.bounds;
	}
	
	void Update () 
	{
		SpawnTimer += Time.deltaTime;
		if(SpawnTimer > SpawnDuration)
		{
			foreach(GameObject obs in obstaclesArray)
			{
				float randX = Random.Range(region.min.x, region.max.x);
				float randZ = Random.Range(region.min.z, region.max.z);
				
				Vector3 pos = new Vector3(randX, transform.position.y - 2.0f , randZ);
				GameObject.Instantiate(obs, pos, Quaternion.identity);
			}
			SpawnTimer = 0.0f;
		}
	}
}
