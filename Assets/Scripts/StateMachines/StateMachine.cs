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
	public virtual bool InstanceGrab(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){return false;}
	public virtual bool InstancePoint(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){return false;}
	public virtual bool InstanceRemoteGrab(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){return false;}
	public virtual bool InstanceTeleport(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){return false;}
	public virtual bool InstancePoke(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){return false;}
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

	public void Interact(GameObject obj, Vector3 point, HandMachine hand, InteractionButton interactionButton, bool is_distant){
		if (GetComponent<StateMaster> () != null) {
			return;
		}
		if (is_distant) {
			switch (interactionButton) {
			case InteractionButton.Trigger:
				InstancePoint (obj, point, this, hand);
				break;
			case InteractionButton.Grip:
				InstanceRemoteGrab (obj, point, this, hand);
				break;
			case InteractionButton.A:
				InstanceTeleport (obj, point, this, hand);
				break;
			case InteractionButton.B:
				break;
			default:
				break;
			}
		} else {
			switch (interactionButton) {
			case InteractionButton.Trigger:
				if (!InstancePoke (obj, point, this, hand)) {
					InstanceRemoteGrab (obj, point, this, hand);
				};
				break;
			case InteractionButton.Grip:
				if (!InstanceGrab (obj, point, this, hand)){
					InstanceRemoteGrab (obj, point, this, hand);
				}
				break;
			case InteractionButton.A:
				if (!InstanceTeleport (obj, point, this, hand)){

				}
				break;
			case InteractionButton.B:
				break;
			default:
				break;
			}
		}
	}
}
