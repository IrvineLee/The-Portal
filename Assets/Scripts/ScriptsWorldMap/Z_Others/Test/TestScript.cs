using UnityEngine;
using System.Collections;

[System.Serializable]
public class Test
{
    public int x;
    public int y;
}

public class TestScript : MonoBehaviour 
{
	public Test[] haha = new Test[2];
	
	[RangeTest(0,5)]
	public int wuwu = 2;
	
	[RangeTest(0.5f,3.0f)]
	public float lala = 2.1f;
	
	[Tooltip("Test string value")]
    public float TestValue = 0.1f;
	
	[ContextMenu("Explode now!")]
	void DoSomething2 () 
	{
        Debug.Log ("Perform operation 2");
    }
	
    [ContextMenu ("Do Something")]
    void DoSomething () 
	{
        Debug.Log ("Perform operation");
    }
	
	void Update()
	{
	}
}