using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ADMIN : StateMachine {
	
	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputInteract };
	}

	public string action = "";

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		switch (action) {
		case "SetText":
			SetText ();
			break;
		default:
			break;
		}
	}
	public void SetText(){
		GetComponent<TextMesh> ().text = "Coins: " + PlayerMachine.instance.GetResource ("COINS");
	}
}
