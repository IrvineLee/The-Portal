using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour 
{
	public GameObject MainCamera;
	public AudioClip[] AudioArray;
	
	AudioSource[] mSpeakers = new AudioSource[2];
	int mID;
	float mTimer;
	
	void Awake () 
	{
		mSpeakers = MainCamera.GetComponents<AudioSource>();
//		mSpeakers[0].Play ();
//		mSpeakers[1].Play ();
	}
	
	void Update () 
	{
		mTimer += Time.deltaTime;
		if(mTimer >= mSpeakers[0].audio.clip.length - (Time.deltaTime * 1.5f))
		{
			PlayBGM();
		}
//		if(!MainCamera.audio.isPlaying)
//		{
//			PlayBGM();
//		}
	}
	
	public void PlayBGM()
	{
//		AudioSource[] temp = MainCamera.GetComponents<AudioSource>();
//		foreach(AudioSource camAudio in temp)
//		{
//			Debug.Log (camAudio.clip);
//			if(camAudio.audio.clip == null) // Use audio which is empty.
//			{
				mSpeakers[0].audio.clip = AudioArray[mID];
				CheckAndSetToLoop();
				mSpeakers[0].Play();
				if(mID < AudioArray.Length - 1) mID += 1;
				else this.enabled = false;
//			}
//			else // Play pre-determined audio.
//			{
				mSpeakers[1].Play ();
//				camAudio.audio.Play();
//				Debug.Log ("AAA");
//			}
//		}
	}
	
	void CheckAndSetToLoop()
	{
		if(MainCamera.audio.clip.name == "MidLoop") MainCamera.audio.loop = true;
		else if(MainCamera.audio.clip.name == "End") MainCamera.audio.loop = false;
	}
}
