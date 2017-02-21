using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaStartStateMachine : StateMachine {

	public GameObject eventSystem;
	public GameObject usedRig;
	public RoomMachine startRoom;
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
		foreach (MovingGround mg in GameObject.FindObjectsOfType<MovingGround>()) {
			mg.Setup ();
		}
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
			go.GetComponent<PlayerMachine> ().Initiate ();
		} else {
			if (PlayerMachine.instance == null) {
				PlayerMachine.instance = FindObjectOfType<PlayerMachine> ();
			}
			PlayerMachine.instance.Initiate ();
		}
		StartCoroutine ("SetRooms");	
	}
	IEnumerator SetRooms(){
		yield return null;
		foreach (RoomMachine room in FindObjectsOfType<RoomMachine> ()) {
			InputMachine.instance.rooms.Add (room);
			room.gameObject.SetActive (false);
		}
		InputMachine.instance.SetRoom (startRoom);
	}
}