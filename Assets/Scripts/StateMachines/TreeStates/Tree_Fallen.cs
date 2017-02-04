using UnityEngine;
using System.Collections;

public class Tree_Fallen : TreeMachine {

	public override void EnterState(StateMachine checkMachine){
		checkMachine.GetComponent<TreeMachine> ().tree.SetActive (false);
		checkMachine.GetComponent<TreeMachine> ().logs.SetActive (true);
		checkMachine.timer.StartTimer (5);
		if (checkMachine.timer.timerObject != null) {
			Destroy (checkMachine.timer.timerObject.gameObject);
		}
	}
	public override void ExitState(StateMachine checkMachine){

	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.timer.CheckTimer(false)) {
			Destroy (checkMachine.gameObject);
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){}
}
