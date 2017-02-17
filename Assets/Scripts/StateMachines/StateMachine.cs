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
	public static StateMaster master;
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
	public virtual void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){}
	public virtual List<InputMachine> InstanceHover(){
		return null;
	}

	public void Initiate(){
		timer = new Timer (this);
		InstanceInitiate (this);
	}

	public void Load(Transform tr, SaveObject so){
		parentGround = so.parentGround;
		transform.SetParent(MovingGround.instance.ground[parentGround]);
		transform.localScale = tr.localScale;
		transform.localRotation = tr.localRotation;
		transform.localPosition = tr.localPosition;
		phase = so.phase;
		name = so.objName;
		InputMachine.instance.gos.Add (gameObject);
		InputMachine.instance.SetObjectParent (transform, true, this);
		timer = new Timer (this);
		if (so.secondaryTimerDuration != 0 && so.secondaryTimerDuration != null) {
			secondaryTimer = new Timer (this);
		}
		UpdateState((StateMachine) StateMaster.instance.GetComponent(so.state), this);
		timer.timerStart = so.timerStart;
		timer.timerDuration = so.timerDuration;
		if (so.secondaryTimerDuration != 0 && so.secondaryTimerDuration != null) {
			secondaryTimer.timerStart = so.secondaryTimerStart;
			secondaryTimer.timerDuration = so.secondaryTimerDuration;
		}
	}

	public void Update(){
		if (GetComponent<StateMaster> () != null) {
			return;
		}
		if (currentState != null) {
			currentState.CheckUpdate (this);
		}
		InstanceUpdate (this);
	}

	public void Interact(GameObject obj, Vector3 point, HandMachine hand){
		if (GetComponent<StateMaster> () != null) {
			return;
		}
		InstanceInteract(obj, point, this, hand);
	}
}
