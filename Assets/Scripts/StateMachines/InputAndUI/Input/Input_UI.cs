using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Input_UI : InputMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){
		InputMachine.instance.SetRoom (null);
	}
	public override void EnterState(StateMachine checkMachine){
		InputMachine.instance.SetRoom (checkMachine.GetComponent<InputMachine> ().myRoom);
		InputMachine.instance.mainUI.SetActive (true);
		InputMachine.instance.loadingUI.SetActive (false);
	}

	public override void SwipeUp(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (InputMachine.instance.swipeForward, checkMachine);
	}
	public override void SwipeDown(GameObject obj, Vector3 point, StateMachine checkMachine){
		UIManager.instance.SetDown ();
	}
	public override void SwipeForward(GameObject obj, Vector3 point, StateMachine checkMachine){
		UIManager.instance.SetForward ();
	}
	public override void SwipeBack(GameObject obj, Vector3 point, StateMachine checkMachine){
		UIManager.instance.SetBackwards ();
	}
	public override void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){
	}
	public override void Release(GameObject obj, Vector3 point, StateMachine checkMachine){}
}
