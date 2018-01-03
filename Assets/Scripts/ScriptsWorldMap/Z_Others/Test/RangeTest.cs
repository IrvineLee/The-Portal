using UnityEngine;
using System.Collections;

public class RangeTest : PropertyAttribute 
{
	public int minInt, maxInt;
	public float minFloat, maxFloat;
	
	public RangeTest(int min, int max)
	{
		minInt = min;
		maxInt = max;
	}
	
	public RangeTest(float min, float max)
	{
		minFloat = min;
		maxFloat = max;
	}
}
