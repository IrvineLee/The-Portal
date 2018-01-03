using UnityEngine;
using System.Collections;

public class LineRenderScript : MonoBehaviour 
{
	public LineRenderer mLineRenderer;
	public GameObject mTarget;
	
	// Use this for initialization
	void Start () 
	{
		mLineRenderer.SetPosition(0,transform.position);
		mLineRenderer.SetPosition(1,mTarget.transform.position);
	}
	
	// Update is called once per frame
	void Update () 
	{
		mLineRenderer.SetPosition(0,transform.position);
		mLineRenderer.SetPosition(1,mTarget.transform.position);
	}
}
