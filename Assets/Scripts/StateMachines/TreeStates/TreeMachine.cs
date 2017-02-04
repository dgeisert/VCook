using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeMachine : StateMachine {

	public GameObject logs, tree;
	public float chopTime = 1;

	public override void InstanceInitiate(StateMachine checkMachine){
		secondaryTimer = new Timer (this);
		UpdateState (StateMaster.instance.treeGrowing, this);
	}

	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputChop };
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		currentState.InstanceInteract (obj, point, checkMachine);
	}

	public bool CanChop(){
		return (currentState == StateMaster.instance.treeGrowing);
	}
}
