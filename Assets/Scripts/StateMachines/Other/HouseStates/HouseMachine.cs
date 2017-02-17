using UnityEngine;
using System.Collections;

public class HouseMachine : StateMachine {

	void Start () {
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		UpdateState (StateMaster.instance.houseOpen, this);
		timer.StartTimer ();
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		currentState.InstanceInteract (obj, point, this, hand);
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		currentState.InstanceUpdate (this);
	}
}
