using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMachine : StateMachine {

	public SurfaceMachine holdingSurface;
	public HandMachine holdingHand;
	public TransformationType transformationType = TransformationType.None;
	public int phases = 3 , value = 10;
	public string itemName;
	public Rigidbody rb;

	public override void InstanceInitiate(StateMachine checkMachine){
	}

	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputPickUp };
	}

	public override bool InstanceGrab(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		if (holdingSurface != null) {
			if (TransformationManager.instance.Transformation (this, hand.heldItem)) {
				return true;
			}
		}
		hand.PickUpItem (this);
		return true;
		//currentState.InstanceInteract (obj, point, checkMachine);
	}

	void OnCollisionEnter (Collision col){
		ItemMachine im = col.collider.GetComponentInParent<ItemMachine> ();
		if (im == null) {
			SurfaceMachine sm = col.collider.GetComponentInParent<SurfaceMachine> ();
			if (sm == null) {
				return;
			}
			if (sm.transformationType != TransformationType.None) {
				TransformationManager.instance.Transformation (this, sm);
				return;
			}
		}
		RecipeManager.instance.RecipeOutput (this, im);
		if (this.transformationType != TransformationType.None) {
			TransformationManager.instance.Transformation (im, this);
		}
	}
}
