﻿using UnityEngine;
using System.Collections;

public class House_Open : HouseMachine {

	// Use this for initialization
	void Start () {

	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}
	public override bool InstancePoint(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		return true;
	}
}
