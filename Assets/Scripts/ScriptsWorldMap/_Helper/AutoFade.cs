// AutoFade.cs
using UnityEngine;
using System.Collections;

public class AutoFade : MonoBehaviour
{
    private static AutoFade m_Instance = null;
    private Material m_Material = null;
    private string m_LevelName = "";
    private int m_LevelIndex = -1;
    private bool m_Fading = false;
    private bool m_IsFadeOutSolid = false;

    private static AutoFade Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("AutoFade")).AddComponent<AutoFade>();
            }
            return m_Instance;
        }
    }
	
    public static bool Fading
    {
        get { return Instance.m_Fading; }
    }
	
	public static bool IsFadeOutSolid
    {
        get { return Instance.m_IsFadeOutSolid; }
    }
	
    private void Awake()
    {
        DontDestroyOnLoad(this);
        m_Instance = this;
        m_Material = new Material("Shader \"Plane/No zTest\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off Fog { Mode Off } BindChannels { Bind \"Color\",color } } } }");
    }

    private void DrawQuad(Color aColor,float aAlpha)
    {
        aColor.a = aAlpha;
        m_Material.SetPass(0);
        GL.Color(aColor);
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, -1);
        GL.Vertex3(0, 1, -1);
        GL.Vertex3(1, 1, -1);
        GL.Vertex3(1, 0, -1);
        GL.End();
        GL.PopMatrix();
    }

    private IEnumerator Fade(float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        float t = 0.0f;
        while (t<1.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t + Time.deltaTime / aFadeOutTime);
            DrawQuad(aColor,t);
        }
		
		m_IsFadeOutSolid = true;
		
		if(aWaitTime <= 0.0f)
		{
			if(aWait1FrameDuringSolid) yield return new WaitForEndOfFrame();
		}
		else if(aWaitTime > 0.0f)
		{
			float time = 0.0f;
			while(time < aWaitTime)
			{
				yield return new WaitForEndOfFrame();
				DrawQuad(aColor,t);
				time += Time.deltaTime;
			}
		}
		
		if (m_LevelName != "")
            Application.LoadLevel(m_LevelName);
        else if (m_LevelIndex != -1)
            Application.LoadLevel(m_LevelIndex);
		
        while (t>0.0f)
        {
            yield return new WaitForEndOfFrame();
            t = Mathf.Clamp01(t - Time.deltaTime / aFadeInTime);
            DrawQuad(aColor,t);
        }
		
		m_IsFadeOutSolid = false;
        m_Fading = false;
    }
    private void StartFade(float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        m_Fading = true;
        StartCoroutine(Fade(aFadeOutTime, aWaitTime, aFadeInTime, aColor, aWait1FrameDuringSolid));
    }
	
	// Different variation of LoadLevel using Scene Name.---------------------------------------------------------------------------------------------
	
	public static void LoadLevel(string aLevelName,float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.m_LevelName = aLevelName;
        Instance.StartFade(aFadeOutTime, 0.0f, aFadeInTime, aColor, false);
    }
	
	public static void LoadLevel(string aLevelName,float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.m_LevelName = aLevelName;
        Instance.StartFade(aFadeOutTime, aWaitTime, aFadeInTime, aColor, false);
    }
	
	public static void LoadLevel(string aLevelName,float aFadeOutTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        if (Fading) return;
        Instance.m_LevelName = aLevelName;
        Instance.StartFade(aFadeOutTime, 0.0f, aFadeInTime, aColor, aWait1FrameDuringSolid);
    }
	
	public static void LoadLevel(string aLevelName,float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        if (Fading) return;
        Instance.m_LevelName = aLevelName;
        Instance.StartFade(aFadeOutTime, aWaitTime, aFadeInTime, aColor, aWait1FrameDuringSolid);
    }
	
	// Different variation of LoadLevel using Scene Index.---------------------------------------------------------------------------------------------
   
	public static void LoadLevel(int aLevelIndex,float aFadeOutTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.m_LevelName = "";
        Instance.m_LevelIndex = aLevelIndex;
        Instance.StartFade(aFadeOutTime, 0.0f, aFadeInTime, aColor, false);
    }
	
	public static void LoadLevel(int aLevelIndex,float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor)
    {
        if (Fading) return;
        Instance.m_LevelName = "";
        Instance.m_LevelIndex = aLevelIndex;
        Instance.StartFade(aFadeOutTime, aWaitTime, aFadeInTime, aColor, false);
    }
	
	public static void LoadLevel(int aLevelIndex,float aFadeOutTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        if (Fading) return;
        Instance.m_LevelName = "";
        Instance.m_LevelIndex = aLevelIndex;
        Instance.StartFade(aFadeOutTime, 0.0f, aFadeInTime, aColor, aWait1FrameDuringSolid);
    }
	
	public static void LoadLevel(int aLevelIndex,float aFadeOutTime, float aWaitTime, float aFadeInTime, Color aColor, bool aWait1FrameDuringSolid)
    {
        if (Fading) return;
        Instance.m_LevelName = "";
        Instance.m_LevelIndex = aLevelIndex;
        Instance.StartFade(aFadeOutTime, aWaitTime, aFadeInTime, aColor, aWait1FrameDuringSolid);
    }
}