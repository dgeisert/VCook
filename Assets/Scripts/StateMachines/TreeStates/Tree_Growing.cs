using UnityEngine;
using System.Collections;

public class Tree_Growing : TreeMachine {

	public override void EnterState(StateMachine checkMachine){
		checkMachine.secondaryTimer.StartTimer (60);
	}
	public override void ExitState(StateMachine checkMachine){

	}

	public override void CheckUpdate(StateMachine checkMachine){
		if (checkMachine.secondaryTimer.CheckTimer (60)) {
			checkMachine.transform.localScale *= 1 + (0.02f / checkMachine.transform.localScale.x);
			checkMachine.secondaryTimer.StartTimer (60);
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		TreeMachine tm = checkMachine.GetComponentInParent<TreeMachine> ();
		if (tm.timer.CheckTimer (true)) {
			if (checkMachine.phase < checkMachine.transform.localScale.x * 10) {
				checkMachine.phase++;
				PlayerMachine.instance.AddResource ("wood", 1);
			} else {
				checkMachine.UpdateState (StateMaster.instance.treeFallen, checkMachine);
				PlayerMachine.instance.AddResource ("wood", 1);
				return;
			}
			tm.timer.StartTimer (tm.chopTime, true, InputMachine.instance.reticle.getTimerLocation(), numbers: false);
		} else if (tm.timer.CheckTimer (false)) {
			tm.timer.StartTimer (tm.chopTime, true, InputMachine.instance.reticle.getTimerLocation(), numbers: false);
		}
	}
}
