using UnityEngine;
using System.Collections;

public class PL_Anim : MonoBehaviour 
{
	public enum State
	{
		IDLE = 0,
		WALK,
		ATK,
		GOT_HIT
	};
	public State mState = State.IDLE;
	
	protected Animator animator;
	
	bool mIsGotHit = false;
	
	void Start () 
	{
		animator = GetComponent<Animator>();
	}
	
	void Update()
	{
		if(!mIsGotHit) return;
		
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		
		if(stateInfo.nameHash == Animator.StringToHash("Base Layer.Idle"))
		{
			animator.SetBool("mIsIdle", true );
			animator.SetBool("mIsMove", false );
			animator.SetBool("mIsAtk", false );
			animator.SetBool("mIsGotHit", false );
			mIsGotHit = false;
		}
	}
	
	public void SetAnim(State toState)
	{
		//get the current state
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		
		if(stateInfo.nameHash == Animator.StringToHash("Base Layer.Idle"))
		{
			if(toState == State.IDLE) 
			{
				animator.SetBool("mIsIdle", true );
				animator.SetBool("mIsMove", false );
				animator.SetBool("mIsAtk", false );
				animator.SetBool("mIsGotHit", false );
			}
			else if(toState == State.WALK) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsGotHit", false );
				animator.SetBool("mIsMove", true );
			}
			else if(toState == State.ATK) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsGotHit", false );
				animator.SetBool("mIsAtk", true );
			}
			else if(toState == State.GOT_HIT) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsAtk", false );
				animator.SetBool("mIsMove", false );
				animator.SetBool("mIsGotHit", true );
			}
		}
		else if(stateInfo.nameHash == Animator.StringToHash("Base Layer.Move"))
		{
			if(toState == State.IDLE) 
			{
				animator.SetBool("mIsIdle", true );
				animator.SetBool("mIsMove", false );
				animator.SetBool("mIsAtk", false );
				animator.SetBool("mIsGotHit", false );
			}
			else if(toState == State.ATK) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsGotHit", false );
				animator.SetBool("mIsAtk", true );
			}
			else if(toState == State.GOT_HIT) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsAtk", false );
				animator.SetBool("mIsMove", false );
				animator.SetBool("mIsGotHit", true );
			}
		}
		else if(stateInfo.nameHash == Animator.StringToHash("Base Layer.Atk"))
		{
			if(toState == State.GOT_HIT) 
			{
				animator.SetBool("mIsIdle", false );
				animator.SetBool("mIsAtk", false );
				animator.SetBool("mIsMove", false );
				animator.SetBool("mIsGotHit", true );
				mIsGotHit = true;
			}
		}
	}
}
