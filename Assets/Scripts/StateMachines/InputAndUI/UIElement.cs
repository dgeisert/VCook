using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UIElement : StateMachine {

	UIManager manager;
	public InputMachine setInput;

	void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		
	}

	public override bool InstancePoint(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		return true;
	}

}
