using UnityEngine;
using System.Collections;

public class Plant_Planted : PlantMachine {

	public override void EnterState(StateMachine checkMachine){
		
	}
	public override void ExitState(StateMachine checkMachine){

	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.timer.CheckTimer()) {

		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (StateMaster.instance.plantGrowing, checkMachine);
	}
}
