using UnityEngine;
using System;

public abstract class State {
	
	public enum states {enIdle, enPatrol, enPursue, enFlee, 
		enAttack, enDefense, enDamage, enTigerRunAround, 
		enTigerTired, enTigerWaiting, enTigerSpecialAttack, 
		enDying, enBeeReturnToHive, enAlligatorReturnToPlace,
		enReturnToHome};
	
	protected states state;
	
	public states GetState () {
		return state;
	}
	
	public bool IsState (states st) {
		if (state == st)
			return true;
		else
			return false;
	}
	
	public abstract void Execute (Entity context);
	
	public abstract void Enter (Entity context);
	
	public abstract void Exit (Entity context);
}
