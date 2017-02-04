using UnityEngine;
using System.Collections;

public class Plant_Dry : PlantMachine {

	public override void EnterState(StateMachine checkMachine){
		checkMachine.timer.StartTimer (checkMachine.GetComponent<PlantMachine> ().growTime * 2);
	}
	public override void ExitState(StateMachine checkMachine){

	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.timer.CheckTimer()) {
			checkMachine.UpdateState (StateMaster.instance.plantWithered, checkMachine);
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (StateMaster.instance.plantGrowing, checkMachine);
	}
}