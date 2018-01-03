using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
	
	public GameObject target;
	
	/*private Plane plane = new Plane(Vector3.up, Vector3.zero);
    private Vector3 v3Center = new Vector3(0.5f,0.5f,0.0f);*/
	
	// Use this for initialization
	void Start () 
	{
		gameObject.SetActive(false);
		//target.transform.position = Camera.main.WorldToScreenPoint(new Vector3(Screen.width/2, Screen.height/2, Camera.main.nearClipPlane) );
		//Vector3 screenPos  = Camera.main.WorldToScreenPoint (target.transform.position);
		//Debug.Log(screenPos.x);
		
		/*Ray ray = Camera.main.ViewportPointToRay(v3Center);
		Debug.Log (ray);
		float fDist;
		if (plane.Raycast(ray, out fDist))
		{
			Debug.Log (fDist);
			Vector3 v3Hit = ray.GetPoint (fDist);
			Vector3 v3Delta = target.transform.position - v3Hit;
			Camera.main.transform.Translate (v3Delta);
		}*/
		
		/*foreach (Transform child in transform)
		{
			//child.GetComponent<HexScript>().SaveScene ();
		}*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

/*public class PlayerPrefsX
{
	public static void SetBool(string name, bool booleanValue) 
	{
		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
	}
 
	public static bool GetBool(string name)  
	{
	    return PlayerPrefs.GetInt(name) == 1 ? true : false;
	}
 
	public static bool GetBool(string name, bool defaultValue)
	{
	    if(PlayerPrefs.HasKey(name)) 
		{
	        return GetBool(name);
	    }
 
	    return defaultValue;
	}
}*/