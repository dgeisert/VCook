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

	IEnumerator SetFirstScene(){
		yield return null;
		SceneManager.LoadSceneAsync (scene);
	}
}
