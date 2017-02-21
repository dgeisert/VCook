using UnityEngine;
using System.Collections;

public class DoorMachine : StateMachine {

	public Transform toPosition;
	public RoomMachine room, toRoom;

	public override bool InstancePoint(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		if (toPosition != null) {
			PlayerMachine.playerObject.transform.position = toPosition.position;
			InputMachine.instance.SetRoom (toRoom);
			return true;
		}
		return false;
	}

	public override bool InstanceTeleport(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		if (toPosition != null) {
			PlayerMachine.playerObject.transform.position = toPosition.position;
			InputMachine.instance.SetRoom (toRoom);
			return true;
		}
		return false;
	}
}
