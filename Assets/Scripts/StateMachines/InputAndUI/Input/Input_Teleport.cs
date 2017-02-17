using UnityEngine;
using System.Collections;

public class Input_Teleport : HandMachine {

	public override void CheckUpdate(StateMachine checkMachine){
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}

	public override void ExitState(StateMachine checkMachine){
	}
	public override void EnterState(StateMachine checkMachine){
		checkMachine.timer.StartTimer (0.3f);
	}

	public override void SwipeUp(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (InputMachine.instance.swipeUp, checkMachine);
	}
	public override void SwipeDown(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (InputMachine.instance.swipeDown, checkMachine);
	}
	public override void SwipeForward(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (InputMachine.instance.swipeForward, checkMachine);
	}
	public override void SwipeBack(GameObject obj, Vector3 point, StateMachine checkMachine){
		checkMachine.UpdateState (InputMachine.instance.swipeBack, checkMachine);
	}
	public override void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){
		
	}
	public override void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj == null) {
			canInteract = false;
		}
		else if (obj.GetComponent<Blocked>() != null) {
			canInteract = false;
		}
		else if (obj.GetComponent<Platform>() != null) {
			canInteract = true;
		}/*
		else if (InputMachine.instance.reticle.is_nearObjects
			|| !InputMachine.instance.reticle.is_onGround) {
			canInteract = false;
		} */else {
			canInteract = true;
		}
	}
	public override void Release(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (obj != null) {/*
			if (obj.GetComponent<Platform> () ||
			    (!InputMachine.instance.reticle.is_nearObjects
					&& InputMachine.instance.reticle.is_onGround)) {
				StartCoroutine (Teleport(point, 0.1f));
			}*/
		}
	}

	IEnumerator Teleport(Vector3 point, float duration){
		//InputMachine.instance.DipToColor (new Color(0f,0f,0f,0.5f), duration);
		PlayerMachine.instance.SavePlayerPosition ();
		yield return new WaitForSeconds (duration);
		PlayerMachine.playerObject.transform.position = point
			+ PlayerMachine.playerObject.transform.up * InputMachine.playerHeight;
	}
}
