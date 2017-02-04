using UnityEngine;
using System.Collections;

public class Animal_Crying : AnimalMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().anim.SetBool ("IsCrying", false);
	}
	public override void EnterState(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().anim.SetBool ("IsCrying", true);
	}
	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (StateMaster.instance.animalRunningAway, checkMachine);
	}
}
