using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandMachine : StateMachine {

	public bool is_right = false;
	public bool is_nearObjects = false, is_onGround;
	private InputMachine currentInteractionState;
	private HandInstance handInstance;

	void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
	}

	public void SetHand(InputMachine setValue){
		if (currentInteractionState == setValue) {
			return;
		}
		if (handInstance != null) {
			Destroy (handInstance.gameObject);
		}
		currentInteractionState = setValue;
		if (is_right) {
			GameObject newHand = (GameObject)GameObject.Instantiate (setValue.setRightHand, transform);
			newHand.transform.localPosition = Vector3.zero;
			newHand.transform.rotation = InputMachine.instance.transform.rotation;
			handInstance = newHand.GetComponent<HandInstance> ();
		} else {
			GameObject newHand = (GameObject)GameObject.Instantiate (setValue.setLeftHand, transform);
			newHand.transform.localPosition = Vector3.zero;
			newHand.transform.localScale = new Vector3 (-1, 1, 1);
			newHand.transform.rotation = InputMachine.instance.transform.rotation;
			handInstance = newHand.GetComponent<HandInstance> ();
		}
	}

	public override void InstanceUpdate(StateMachine checkMachine) {
		if (handInstance != null) {
			if (!InputMachine.instance.is_holding) {
				handInstance.SetNoHold ();
			} else {
				if (currentInteractionState.canInteract) {
					handInstance.SetHoldTrue ();
				} else {
					handInstance.SetHoldFalse ();
				}
			}
		}
	}

	public void OnTriggerStay(Collider col){
	}
}
