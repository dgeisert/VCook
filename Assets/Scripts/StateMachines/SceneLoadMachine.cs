using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadMachine : StateMachine {

	public string scene;

	void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		if (SceneManager.GetActiveScene ().name == "PreLoad") {
			StartCoroutine ("SetFirstScene");
		}
	}
	public override void InstanceUpdate(StateMachine checkMachine){
	}
	public override bool InstancePoint(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		InputMachine.instance.SetRoom (InputMachine.instance.myRoom);
		InputMachine.instance.mainUI.SetActive (false);
		InputMachine.instance.loadingUI.SetActive (true);
		SceneManager.LoadSceneAsync (scene);
		return true;
	}

	IEnumerator SetFirstScene(){
		yield return null;
		InputMachine.instance.SetLoading ();
		SceneManager.LoadSceneAsync (scene);
	}
}
