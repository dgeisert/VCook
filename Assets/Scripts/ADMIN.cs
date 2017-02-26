using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ADMIN : StateMachine {

	public static ADMIN instance;

	void Start(){
		ADMIN.instance = this;
	}

	public string action = "";
	public void SetText(string txt){
		GetComponent<TextMesh> ().text = txt;
	}
}
