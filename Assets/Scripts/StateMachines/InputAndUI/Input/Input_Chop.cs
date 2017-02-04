using UnityEngine;
using System.Collections;

public class Input_Chop : InputMachine {

	public override void CheckUpdate(StateMachine checkMachine){
		//checkMachine.UpdateState (StateMaster.instance.animalRunningAway, checkMachine);
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){
	}
	public override void EnterState(StateMachine checkMachine){
		checkMachine.timer.StartTimer (0.3f);
	}

	public override void SwipeUp(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (checkMachine.GetComponent<InputMachine> ().swipeUp, checkMachine);
	}
	public override void SwipeDown(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (checkMachine.GetComponent<InputMachine> ().swipeDown, checkMachine);
	}
	public override void SwipeForward(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (checkMachine.GetComponent<InputMachine> ().swipeForward, checkMachine);
	}
	public override void SwipeBack(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (checkMachine.GetComponent<InputMachine> ().swipeBack, checkMachine);
	}
	public override void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj == null) {
			return;
		}
		TreeMachine tm = obj.GetComponentInParent<TreeMachine> ();
		if (tm != null) {
			canInteract = tm.CanChop ();
			if (tm.CanChop ()) {
				tm.timer.StartTimer (tm.chopTime, true, InputMachine.instance.reticle.getTimerLocation (), numbers: false);
			}
			return;
		}
	}
	public override void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj == null) {
			canInteract = false;
			return;
		}
		TreeMachine tm = obj.GetComponentInParent<TreeMachine> ();
		if (tm != null) {
			tm.Interact (obj, point);
			canInteract = tm.CanChop ();
			return;
		}
		canInteract = false;
	}
	public override void Release(GameObject obj, Vector3 point, StateMachine checkMachine){
		TimerObject tObj = InputMachine.instance.reticle.getTimerLocation ().GetComponentInChildren<TimerObject> ();
		if (tObj != null) {
			Destroy(tObj.gameObject);
		}
	}
}