using UnityEngine;
using System.Collections;

public class Animal_Following : AnimalMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){}
	public override void EnterState(StateMachine checkMachine){
		
	}
	public override void InstanceUpdate(StateMachine checkMachine){
		if (Vector3.Distance (checkMachine.GetComponent<AnimalMachine>().targetLocation, checkMachine.transform.position) < 0.5f) {
			checkMachine.GetComponent<AnimalMachine>().cc.SimpleMove(new Vector3 (0, checkMachine.GetComponent<AnimalMachine>().yVelocity, 0));
		} else if (Vector3.Distance (PlayerMachine.playerObject.transform.position, checkMachine.transform.position) < 45) {
			checkMachine.GetComponent<AnimalMachine>().LookMove ();
		} else {

		}
	}
	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		JumpStart (checkMachine);
	}

	public void JumpStart(StateMachine checkMachine){
		checkMachine.GetComponent<AnimalMachine>().yVelocity = jumpSpeed;
	}
}
