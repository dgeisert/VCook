using UnityEngine;
using System.Collections;

public class StateMaster : MonoBehaviour {
	public InputMachine inputUI, inputTeleport, inputInteract
	, inputPickUp, inputInspect, inputChop, inputGather;
	public HouseMachine houseOpen;
	public GameObject number, timer;

	public static StateMaster instance;

	public void Setup(){
		StateMaster.instance = this;
		timer = (GameObject)Resources.Load<GameObject> ("Timer");

		inputUI = gameObject.AddComponent<Input_UI>();
		inputTeleport = gameObject.AddComponent<Input_Teleport>();
		inputInteract = gameObject.AddComponent<Input_Interact>();
		inputPickUp = gameObject.AddComponent<Input_PickUp>();
		inputInspect = gameObject.AddComponent<Input_Inspect>();
		inputChop = gameObject.AddComponent<Input_Chop>();
		inputGather = gameObject.AddComponent<Input_Gather>();

		houseOpen = gameObject.AddComponent<House_Open> ();
	}
}
