using UnityEngine;
using System.Collections;

public class DoorMachine : StateMachine {

	public Transform toPosition;
	public RoomMachine room, toRoom;

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		if (toPosition != null) {
			PlayerMachine.playerObject.transform.position = toPosition.position + Vector3.up * InputMachine.playerHeight;
			InputMachine.instance.SetRoom (toRoom);
		}

	}
}
