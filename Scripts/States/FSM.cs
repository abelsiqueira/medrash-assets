using UnityEngine;
using System.Collections;

public class FSM {
	private Entity mOwner;
	private State  mCurrentState;
	private State  mPreviousState;
	private State  mGlobalState;
	
	public FSM (Entity owner) {
		mOwner = owner;
		mCurrentState = null;
		mPreviousState = null;
		mGlobalState = null;
	}
	
	public void SetCurrentState (State s) {
		mCurrentState = s;
	}
	
	public void SetGlobalState (State s) {
		mGlobalState = s;
	}
	
	public void SetPreviousState (State s) {
		mPreviousState = s;
	}
	
	public IEnumerator UpdateFSM() {
		while (true) {
			if (mGlobalState != null)
				mGlobalState.Execute (mOwner);
			if (mCurrentState != null)
				mCurrentState.Execute (mOwner);
			
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public void ChangeState (State newState) {
		if (newState == null)
			return;
		mPreviousState = mCurrentState;
		mCurrentState.Exit (mOwner);
		mCurrentState = newState;
		mCurrentState.Enter (mOwner);
	}
	
	public void RevertState () {
		ChangeState (mPreviousState);
	}
	
	public State.states GetCurrentState () {
		return mCurrentState.GetState();
	}
	
	public State.states GetGlobalState () {
		return mGlobalState.GetState();
	}
	
	public State.states GetPreviousState () {
		return mPreviousState.GetState();
	}
}
