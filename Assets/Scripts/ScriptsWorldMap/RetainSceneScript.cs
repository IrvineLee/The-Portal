using UnityEngine;
using System.Collections;

public class RetainSceneScript : MonoBehaviour 
{
	void Start () 
	{
		DontDestroyOnLoad(transform.gameObject);
	}
}
