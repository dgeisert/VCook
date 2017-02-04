using UnityEngine;
using System.Collections;

public class StateMaster : MonoBehaviour {

	public PlantMachine plantDry, plantFallow, plantGrown, plantGrowing, plantPlanted, plantWithered;
	public TreeMachine treeGrowing, treeFallen;
	public AnimalMachine animalRoaming, animalCrying, animalFollowing, animalRunningAway;
	public InputMachine inputUI, inputTeleport, inputInteract
	, inputPickUp, inputInspect, inputChop, inputGather;
	public HouseMachine houseOpen;
	public GameObject number, timer;

	public static StateMaster instance;

	public void Setup(){
		StateMaster.instance = this;
		timer = (GameObject)Resources.Load<GameObject> ("Timer");


		plantDry = gameObject.AddComponent<Plant_Dry>();
		plantFallow = gameObject.AddComponent<Plant_Fallow>();
		plantGrowing = gameObject.AddComponent<Plant_Growing>();
		plantGrown = gameObject.AddComponent<Plant_Grown>();
		plantPlanted = gameObject.AddComponent<Plant_Planted>();
		plantWithered = gameObject.AddComponent<Plant_Withered>();

		treeGrowing = gameObject.AddComponent<Tree_Growing>();
		treeFallen = gameObject.AddComponent<Tree_Fallen>();

		animalCrying = gameObject.AddComponent<Animal_Crying>();
		animalFollowing = gameObject.AddComponent<Animal_Following>();
		animalRoaming = gameObject.AddComponent<Animal_Roaming>();
		animalRunningAway = gameObject.AddComponent<Animal_RunningAway>();

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
