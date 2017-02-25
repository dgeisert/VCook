using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : StateMachine {

	public void Interact(RaycastHit hit){
	}
	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputTeleport};
	}
}
