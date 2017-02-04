using UnityEngine;
using System.Collections;

public class TutorialMachine : StateMachine {

	public GameObject[] centerImages;
	public GameObject[] titles;
	public GameObject[] bottomText;
	public int currentTutorialStep = 0;

	void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		titles = new GameObject[transform.GetChild (0).GetChild (0).childCount];
		centerImages = new GameObject[transform.GetChild (0).GetChild (1).childCount];
		bottomText = new GameObject[transform.GetChild (0).GetChild (2).childCount];
		for (int i = 0; i < transform.GetChild (0).GetChild (0).childCount; i++) {
			titles[i] = transform.GetChild (0).GetChild (0).GetChild (i).gameObject;
		}
		for (int i = 0; i < transform.GetChild (0).GetChild (1).childCount; i++) {
			centerImages[i] = transform.GetChild (0).GetChild (1).GetChild (i).gameObject;
		}
		for (int i = 0; i < transform.GetChild (0).GetChild (2).childCount; i++) {
			bottomText[i] = transform.GetChild (0).GetChild (2).GetChild (i).gameObject;
		}
		titles [0].SetActive (true);
		centerImages [0].SetActive (true);
		bottomText [0].SetActive (true);
	}

	public bool NextTutorialStep(){
		centerImages [currentTutorialStep].SetActive (false);
		titles [currentTutorialStep].SetActive (false);
		bottomText [currentTutorialStep].SetActive (false);
		currentTutorialStep++;
		if (currentTutorialStep >= centerImages.Length
		   || currentTutorialStep >= titles.Length
		   || currentTutorialStep >= bottomText.Length) {
			return false;
		}
		centerImages [currentTutorialStep].SetActive (true);
		titles [currentTutorialStep].SetActive (true);
		bottomText [currentTutorialStep].SetActive (true);
		return true;
	}

	public override void InstanceUpdate(StateMachine checkMachine) {
	}

	public void OnTriggerStay(Collider col){
	}
}
