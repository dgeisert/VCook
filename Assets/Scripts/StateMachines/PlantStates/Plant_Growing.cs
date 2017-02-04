using UnityEngine;
using System.Collections;

public class Plant_Growing : PlantMachine {

	public override void EnterState(StateMachine checkMachine){
		checkMachine.timer.StartTimer (checkMachine.GetComponent<PlantMachine> ().growTime);
	}
	public override void ExitState(StateMachine checkMachine){
		if (checkMachine.timer.timerObject != null) {
			Destroy (checkMachine.timer.timerObject.gameObject);
		}
	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.timer.CheckTimer()) {
			if (checkMachine.GetComponent<PlantMachine> ().growPhases > checkMachine.phase) {
				checkMachine.phase++;
				checkMachine.UpdateState (StateMaster.instance.plantDry, checkMachine);
			} else {
				checkMachine.UpdateState (StateMaster.instance.plantGrown, checkMachine);
			}
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		
	}
}
