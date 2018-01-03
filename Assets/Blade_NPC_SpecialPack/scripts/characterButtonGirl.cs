
	
using UnityEngine;
using System.Collections;

public class characterButtonGirl : MonoBehaviour {

	public GameObject frog;
	
	
	
	private Rect FpsRect ;
	private string frpString;
	
	private GameObject instanceObj;
	public GameObject[] gameObjArray=new GameObject[9];
	public AnimationClip[] AniList  = new AnimationClip[4];
	
	float minimum = 2.0f;
	float maximum = 50.0f;
	float touchNum = 0f;
	string touchDirection ="forward"; 
	private GameObject Villarger_A_Girl_prefab;
	
	// Use this for initialization
	void Start () {
		
		//frog.animation["dragon_03_ani01"].blendMode=AnimationBlendMode.Blend;
		//frog.animation["dragon_03_ani02"].blendMode=AnimationBlendMode.Blend;
		//Debug.Log(frog.GetComponent("dragon_03_ani01"));
		
		//Instantiate(gameObjArray[0], gameObjArray[0].transform.position, gameObjArray[0].transform.rotation);
	}
	
	
 void OnGUI() {
	  if (GUI.Button(new Rect(20, 20, 70, 40),"Idle")){
		 frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Idle");
	  }
	    //if (GUI.Button(new Rect(90, 20, 70, 40),"Greeting")){
		 // frog.animation.wrapMode= WrapMode.Loop;
		  //	frog.animation.CrossFade("Greeting");
	 // }
		   if (GUI.Button(new Rect(90, 20, 70, 40),"Walk")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Walk");
	  }
		   if (GUI.Button(new Rect(160, 20, 70, 40),"L_Walk")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_L_Walk");
	  }
		   if (GUI.Button(new Rect(230, 20, 70, 40),"R_Walk")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_R_Walk");
			
	  }
		   if (GUI.Button(new Rect(300, 20, 70, 40),"B_Walk")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_B_Walk");
	  }
	     if (GUI.Button(new Rect(370, 20, 70, 40),"Talk")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Talk");
	  } 
		if (GUI.Button(new Rect(440, 20, 70, 40),"Run")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Run00");
	  } 
			if (GUI.Button(new Rect(510, 20, 70, 40),"L_Run")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_L_Run00");
	  }
			if (GUI.Button(new Rect(580, 20, 70, 40),"R_Run")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_R_Run00");
	  }
			if (GUI.Button(new Rect(650, 20, 70, 40),"B_Run")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_B_Run00");
	  }
			if (GUI.Button(new Rect(720, 20, 70, 40),"Jump")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Jump_NoBlade");
	  }
			if (GUI.Button(new Rect(20, 60, 70, 40),"DrawBlade")){
		  frog.animation.wrapMode= WrapMode.Once;
		  	frog.animation.CrossFade("BG_DrawBlade");
	  } 
			if (GUI.Button(new Rect(90, 60, 70, 40),"ATK_standy")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_AttackStandy");
	  }
			if (GUI.Button(new Rect(160, 60, 70, 40),"Attack")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Attack00");
	  }
			if (GUI.Button(new Rect(230, 60, 70, 40),"Attack01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Attack");
	  }
		if (GUI.Button(new Rect(300, 60, 70, 40),"Block")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Block");
	  }
			if (GUI.Button(new Rect(370, 60, 70, 40),"Attack02")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Attack01");
	  }
			if (GUI.Button(new Rect(440, 60, 70, 40),"Combo")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_ComboAttack");
	  }
				if (GUI.Button(new Rect(510, 60, 70, 40),"Skill")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  frog.animation.CrossFade("BG_Skill");
		
	  }
				if (GUI.Button(new Rect(580, 60, 70, 40),"M_Avoid")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  frog.animation.CrossFade("BG_M_Avoid");
		
	  }
			if (GUI.Button(new Rect(650, 60, 70, 40),"L_Avoid")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_L_Avoid");
	  }
				if (GUI.Button(new Rect(720, 60, 70, 40),"R_Avoid")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  frog.animation.CrossFade("BG_R_Avoid");
		
	  }
				if (GUI.Button(new Rect(20, 100, 70, 40),"Buff")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  frog.animation.CrossFade("BG_Buff");
		
	  }
			if (GUI.Button(new Rect(90, 100, 70, 40),"Run01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Run");
	  }
		if (GUI.Button(new Rect(160, 100, 70, 40),"L_Run01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_L_Run");
	  }
		if (GUI.Button(new Rect(230, 100, 70, 40),"R_Run01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_R_Run");
	  }
		if (GUI.Button(new Rect(300, 100, 70, 40),"B_Run01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_B_Run");
	  }
		if (GUI.Button(new Rect(370, 100, 70, 40),"Jump01")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_jump");
	  }
		if (GUI.Button(new Rect(440, 100, 70, 40),"PickUp")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Pickup");
	  }
			if (GUI.Button(new Rect(510, 100, 70, 40),"Damage")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_Damage");
	  }
			if (GUI.Button(new Rect(580, 100, 70, 40),"Death")){
		  frog.animation.wrapMode= WrapMode.Once;
		  	frog.animation.CrossFade("BG_Death");
	  }
		if (GUI.Button(new Rect(650, 100, 140, 40),"GangnamStyle")){
		  frog.animation.wrapMode= WrapMode.Loop;
		  	frog.animation.CrossFade("BG_GanamStyle");
	  }
		
		/////////////////////////////////////////////////////////////////////
		
		if (GUI.Button(new Rect(20, 540, 120, 40),"Blad_Warrior")){
	       Application.LoadLevel(0);
	 }
		if (GUI.Button(new Rect(150, 540, 70, 40),"Base")){
	       Application.LoadLevel(0);
	 }  
		if (GUI.Button(new Rect(220, 540, 70, 40),"T01")){
	       Application.LoadLevel(1);
	 } 
		if (GUI.Button(new Rect(290, 540, 70, 40),"T02")){
	       Application.LoadLevel(2);
	 }  
		if (GUI.Button(new Rect(360, 540, 70, 40),"T03")){
	       Application.LoadLevel(3);
	 }  
		if (GUI.Button(new Rect(430, 540, 70, 40),"T04")){
	       Application.LoadLevel(4);
	 }  
		if (GUI.Button(new Rect(500, 540, 70, 40),"T05")){
	       Application.LoadLevel(5);
	 }  
		if (GUI.Button(new Rect(570, 540, 70, 40),"T06")){
	       Application.LoadLevel(6);
	 }  
		if (GUI.Button(new Rect(640, 540, 70, 40),"T07")){
	       Application.LoadLevel(7);
	 }  
		
	 ///////////////////////////////////////////////////////////////////////////////
		
		if (GUI.Button(new Rect(20, 580, 120, 40),"Blad_Girl")){
	       Application.LoadLevel(8);
	 }
			if (GUI.Button(new Rect(150, 580, 70, 40),"Base")){
	       Application.LoadLevel(8);
	 }  
		if (GUI.Button(new Rect(220, 580, 70, 40),"T01")){
	       Application.LoadLevel(9);
	 } 
		if (GUI.Button(new Rect(290, 580, 70, 40),"T02")){
	       Application.LoadLevel(10);
	 }  
		if (GUI.Button(new Rect(360, 580, 70, 40),"T03")){
	       Application.LoadLevel(11);
	 }  
		if (GUI.Button(new Rect(430, 580, 70, 40),"T04")){
	       Application.LoadLevel(12);
	 }  
		if (GUI.Button(new Rect(500, 580, 70, 40),"T05")){
	       Application.LoadLevel(13);
	 }  
		if (GUI.Button(new Rect(570, 580, 70, 40),"T06")){
	       Application.LoadLevel(14);
	 }  
		if (GUI.Button(new Rect(640, 580, 70, 40),"T07")){
	       Application.LoadLevel(15);
	 }  
		
		//		if (GUI.Button(new Rect(580, 440, 140, 40),"V  1.2")){
		//  frog.animation.wrapMode= WrapMode.Loop;
		//  	frog.animation.CrossFade("Idle");
	 // } 
			//	if (GUI.Button(new Rect(640, 540, 140, 40),"Ver 2.0")){
		// frog.animation.wrapMode= WrapMode.Loop;
		 // 	frog.animation.CrossFade("BW_Idle");
	  //}
		
		
 }
	
	// Update is called once per frame
	void Update () {
		
		//if(Input.GetMouseButtonDown(0)){
		
			//touchNum++;
			//touchDirection="forward";
		 // transform.position = new Vector3(0, 0,Mathf.Lerp(minimum, maximum, Time.time));
			//Debug.Log("touchNum=="+touchNum);
		//}
		/*
		if(touchDirection=="forward"){
			if(Input.touchCount>){
				touchDirection="back";
			}
		}
	*/
		 
		//transform.position = Vector3(Mathf.Lerp(minimum, maximum, Time.time), 0, 0);
	if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
		//frog.transform.Rotate(Vector3.up * Time.deltaTime*30);
	}
	
}
