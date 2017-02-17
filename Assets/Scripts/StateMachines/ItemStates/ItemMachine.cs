using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMachine : StateMachine {

	public SurfaceMachine holdingSurface;
	public TransformationType transformationType = TransformationType.None;
	public int phases = 3 , value = 10;
	public string itemName;

	public override void InstanceInitiate(StateMachine checkMachine){
		
	}

	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputPickUp };
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		if (holdingSurface != null) {
			if (TransformationManager.instance.Transformation (this)) {
				return;
			}
		}
		//InputMachine.instance.PickUpItem (this);
		//currentState.InstanceInteract (obj, point, checkMachine);
	}
}
