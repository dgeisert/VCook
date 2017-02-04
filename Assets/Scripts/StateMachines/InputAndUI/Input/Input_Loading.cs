using UnityEngine;
using System.Collections;

public class Input_Loading : InputMachine {

	public override void CheckUpdate(StateMachine checkMachine){
		//checkMachine.UpdateState (StateMaster.instance.animalRunningAway, checkMachine);
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){
	}
	public override void EnterState(StateMachine checkMachine){
	}

	public override void SwipeUp(GameObject obj, Vector3 point, StateMachine checkMachine){
	}
	public override void SwipeDown(GameObject obj, Vector3 point, StateMachine checkMachine){
	}
	public override void SwipeForward(GameObject obj, Vector3 point, StateMachine checkMachine){
	}
	public override void SwipeBack(GameObject obj, Vector3 point, StateMachine checkMachine){
	}
	public override void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public override void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj == null) {
			canInteract = false;
		}
		if (obj.GetComponentInParent<Ground> () != null) {
			canInteract = false;
		}
		if (obj.GetComponentInParent<StateMachine> () == null) {
			canInteract = false;
		}
		canInteract = true;
	}
	public override void Release(GameObject obj, Vector3 point, StateMachine checkMachine){}
}