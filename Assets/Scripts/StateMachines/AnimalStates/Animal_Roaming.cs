using UnityEngine;
using System.Collections;

public class Animal_Roaming : AnimalMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){}
	public override void EnterState(StateMachine checkMachine){
		NewTarget (checkMachine);
	}
	public override void InstanceUpdate(StateMachine checkMachine){
		if (Vector3.Distance (checkMachine.GetComponent<AnimalMachine>().targetLocation, checkMachine.transform.position) < 0.5f) {
			checkMachine.GetComponent<AnimalMachine>().cc.SimpleMove(new Vector3 (0, checkMachine.GetComponent<AnimalMachine>().yVelocity, 0));
		} else if (Vector3.Distance (PlayerMachine.playerObject.transform.position, checkMachine.transform.position) < 45) {
			checkMachine.GetComponent<AnimalMachine>().LookMove ();
		} else {
			
		}
		if (checkMachine.timer.CheckTimer()) {
			NewTarget (checkMachine);
		}
	}
	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		JumpStart (checkMachine);
	}

	public void JumpStart(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().yVelocity = jumpSpeed;
	}

	public void NewTarget(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine> ().SetTarget (checkMachine.transform.position + new Vector3 ((Random.value - 0.5f) * 15, 0, (Random.value - 0.5f) * 15));
		checkMachine.timer.StartTimer (Random.value * 10);
	}
}
