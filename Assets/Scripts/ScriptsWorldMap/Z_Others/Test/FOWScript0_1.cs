using UnityEngine;
using System.Collections;

public class FOWScript0_1 : MonoBehaviour 
{
	[HideInInspector]
	public Mesh mesh;
	[HideInInspector]
	public Color[] colors;
	
	void Awake () 
	{
		mesh = GetComponent<MeshFilter>().mesh;
		colors = new Color[mesh.vertices.Length];
		
		for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0.0f,0.0f,0.0f,1.0f);
        }
		mesh.colors = colors;
	}
}
