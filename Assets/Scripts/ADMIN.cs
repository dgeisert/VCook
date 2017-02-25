using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ADMIN : StateMachine {

	public static ADMIN instance;

	void Start(){
		ADMIN.instance = this;
	}
	
	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputInteract };
	}

	public string action = "";

	public override bool InstancePoke(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		return true;
	}
	public void SetText(string txt){
		GetComponent<TextMesh> ().text = txt;
	}
}
