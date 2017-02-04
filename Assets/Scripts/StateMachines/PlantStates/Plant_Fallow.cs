using UnityEngine;
using System.Collections;

public class Plant_Fallow : PlantMachine {

	public override void EnterState(StateMachine checkMachine){
		checkMachine.timer.StartTimer (checkMachine.GetComponent<PlantMachine> ().growTime);
		checkMachine.phase = 0;
	}
	public override void ExitState(StateMachine checkMachine){

	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.timer.CheckTimer()) {
			Destroy (checkMachine.gameObject);
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (StateMaster.instance.plantPlanted, checkMachine);
	}
}
