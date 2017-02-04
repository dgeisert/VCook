using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlantMachine : StateMachine {

	public float growTime = 30f;
	public int growPhases = 2;

	public override void InstanceInitiate(StateMachine checkMachine){
		UpdateState (StateMaster.instance.plantFallow, this);
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		currentState.InstanceInteract (obj, point, this);
	}

	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputGather };
	}

}
