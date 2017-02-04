using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ground : StateMachine {

	public GameObject basePlant;
	public Dictionary<Vector3, GameObject> splats = new Dictionary<Vector3, GameObject> ();

	public void Interact(RaycastHit hit){
	}
	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputTeleport};
	}
}
