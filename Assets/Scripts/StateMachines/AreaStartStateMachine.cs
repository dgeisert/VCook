using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaStartStateMachine : StateMachine {

	public GameObject eventSystem;
	public GameObject usedRig;
	public static AreaStartStateMachine instance;
	public void Awake(){
		AreaStartStateMachine.instance = this;
		Initiate ();
	}

	public override void CheckUpdate(StateMachine checkMachine){
		
	}

	public override void ExitState(StateMachine checkMachine){}
	public override void EnterState(StateMachine checkMachine){}
	public override void InstanceInitiate(StateMachine checkMachine){
		if (eventSystem == null) {
			eventSystem = (GameObject) Resources.Load("EventSystem", typeof(GameObject));
		}
		GameObject.Instantiate (eventSystem);
		if (FindObjectOfType<PlayerMachine>() == null) {	
			if (usedRig == null) {
				usedRig = (GameObject) Resources.Load("UsedRig", typeof(GameObject));
			}
			Vector3 startPosition = transform.position;
			GameObject go = (GameObject) GameObject.Instantiate (usedRig, startPosition, transform.rotation);
			go.GetComponent<PlayerMachine> ().Init ();
		} else {
			if (PlayerMachine.instance == null) {
				PlayerMachine.instance = FindObjectOfType<PlayerMachine> ();
			}
			PlayerMachine.instance.Init();
		}
		StartCoroutine ("SetRooms");	
	}
}