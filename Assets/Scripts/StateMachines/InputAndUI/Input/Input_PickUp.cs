using UnityEngine;
using System.Collections;

public class Input_PickUp : HandMachine {

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
	public override void Tap(GameObject obj, Vector3 point, StateMachine checkMachine, InteractionButton interaction, bool is_distant){
		if (((HandMachine)checkMachine).canInteract) {
			if (obj == null) {
				return;
			}
			obj.GetComponentInParent<StateMachine> ().Interact (obj, point, (HandMachine) checkMachine, interaction, is_distant);
		}
	}
	public override void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj == null) {
			((HandMachine)checkMachine).canInteract = false;
			return;
		}
		if (obj.GetComponentInParent<ItemMachine> () != null) {
			((HandMachine)checkMachine).canInteract = true;
		}
		if (obj.GetComponentInParent<SurfaceMachine> () != null) {
			((HandMachine)checkMachine).canInteract = true;//obj.GetComponentInParent<SurfaceMachine> ().heldItem != null;
		}
	}
	public override void Release(GameObject obj, Vector3 point, StateMachine checkMachine, InteractionButton interaction, bool is_distant){
	}
}