using UnityEngine;
using System.Collections;

public class Animal_RunningAway : AnimalMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){}
	public override void EnterState(StateMachine checkMachine){}
	public override void InstanceUpdate(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().LookMove ();
	}
	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().SetTarget(
			checkMachine.transform.position 
			+ new Vector3(
				InputMachine.instance.transform.forward.x
				, checkMachine.transform.position.y
				, InputMachine.instance.transform.forward.z
			).normalized * runAwayDistance);
	}
}
