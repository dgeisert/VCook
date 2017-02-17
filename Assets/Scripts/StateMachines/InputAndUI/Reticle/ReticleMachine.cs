using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReticleMachine : StateMachine {

	public HandMachine hand;
	public bool is_nearObjects = false, is_onGround;
	private InputMachine currentInteractionState;
	private ReticleInstance reticleInstance;

	public override void InstanceInitiate(StateMachine checkMachine){
		timer.timerDuration = 0.1f;
	}

	public void SetReticle(InputMachine setValue){
		if (currentInteractionState == setValue && reticleInstance != null) {
			return;
		}
		if (reticleInstance != null) {
			Destroy (reticleInstance.gameObject);
		}
		GameObject newReticle = (GameObject)GameObject.Instantiate (Resources.Load ("Inputs/Reticles/" + setValue.GetType().ToString()) as GameObject, transform.position, transform.rotation, transform);
		reticleInstance = newReticle.GetComponent<ReticleInstance>();
	}

	public Transform getTimerLocation(){
		if (reticleInstance != null) {
			if (reticleInstance.timerHolder == null) {
				return reticleInstance.transform;
			} else {
				return reticleInstance.timerHolder;
			}
		}
		return transform;
	}

	public void Gaze(Vector3 point){
		transform.position = point;
	}

	public override void InstanceUpdate(StateMachine checkMachine) {
		Vector3 targetLocation = transform.position + 2 * (transform.position - PlayerMachine.playerObject.transform.position);
		if (reticleInstance != null) {
			reticleInstance.transform.localEulerAngles = new Vector3 (0, InputMachine.instance.transform.localEulerAngles.y, 0);
		}
		if (timer.CheckTimer()) {
			is_nearObjects = false;
		}
		if (reticleInstance != null) {
			if (!hand.is_holding) {
				reticleInstance.SetNoHold ();
			} else {
				if (hand.canInteract) {
					reticleInstance.SetHoldTrue (transform.position, targetLocation);
				} else {
					reticleInstance.SetHoldFalse ();
				}
			}
		}
	}

	public void OnTriggerStay(Collider col){
		if (col.GetComponent<Ground> () == null && col.GetComponent<PlayerMachine> () == null) {
			timer.StartTimer ();
			is_nearObjects = true;
		}
		if (col.GetComponent<Ground> () != null) {
			is_onGround = true;
		}
	}

	public void OnTriggerExit(Collider col){
		if (col.GetComponent<Ground> () != null) {
			is_onGround = false;
		}
	}
}