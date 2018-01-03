using UnityEngine;
using System.Collections;

public class RespawnerScript : MonoBehaviour 
{
	public GameObject mMasterCopy;
	GameObject mRespawningObject;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!mRespawningObject)
		{
			Debug.Log("no obj");
			GameObject temp = (GameObject)Instantiate(mMasterCopy,transform.position,Quaternion.identity);
			temp.SetActive(true);
			mRespawningObject = temp;
		}
	}
}
