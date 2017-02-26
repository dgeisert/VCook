using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StateMachine: MonoBehaviour {
	
	public Timer timer;
	public Timer secondaryTimer;
	public int phase = 0;
	public float stepDelay = 0;
	public Action interact;
	public Vector2 parentGround;

	public StateMachine currentState;

	public void UpdateState(StateMachine newState, StateMachine thisMachine){
		if (currentState != null) {
			currentState.ExitState (thisMachine);
		}
		currentState = newState;
		currentState.EnterState (thisMachine);
	}

	public virtual void CheckUpdate(StateMachine checkMachine){}

	public virtual void ExitState(StateMachine checkMachine){}
	public virtual void EnterState(StateMachine checkMachine){}
	public virtual void InstanceInitiate(StateMachine checkMachine){}
	public virtual void InstanceUpdate(StateMachine checkMachine){}

	public void Initiate(){
		timer = new Timer (this);
		InstanceInitiate (this);
	}

	public void Load(Transform tr, SaveObject so){
		parentGround = so.parentGround;
		transform.localScale = tr.localScale;
		transform.localRotation = tr.localRotation;
		transform.localPosition = tr.localPosition;
		phase = so.phase;
		name = so.objName;
		timer = new Timer (this);
		if (so.secondaryTimerDuration != 0 && so.secondaryTimerDuration != null) {
			secondaryTimer = new Timer (this);
		}
		timer.timerStart = so.timerStart;
		timer.timerDuration = so.timerDuration;
		if (so.secondaryTimerDuration != 0 && so.secondaryTimerDuration != null) {
			secondaryTimer.timerStart = so.secondaryTimerStart;
			secondaryTimer.timerDuration = so.secondaryTimerDuration;
		}
	}

	public void Update(){
		if (currentState != null) {
			currentState.CheckUpdate (this);
		}
		InstanceUpdate (this);
	}
}
