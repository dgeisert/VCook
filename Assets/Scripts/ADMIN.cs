using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ADMIN : StateMachine {
	
	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputInteract };
	}

	public string action = "";

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		switch (action) {
		case "DestroyGos":
			ADMIN.DestroyGos ();
			break;
		case "CreateGos":
			ADMIN.CreateGos ();
			break;
		case "Save":
			ADMIN.Save ();
			break;
		case "Load":
			ADMIN.Load ();
			break;
		default:
			break;
		}
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		switch (action) {
		case "SetText":
			SetText ();
			break;
		default:
			break;
		}
	}

	public static void DestroyGos(){
		foreach(GameObject go in InputMachine.instance.gos){
			Destroy(go);
		}
		GameObject.FindObjectOfType<ADMIN>().StartCoroutine (GameObject.FindObjectOfType<ADMIN>().ClearGos ());
	}
	public IEnumerator ClearGos(){
		yield return null;
		InputMachine.instance.gos.RemoveAll (item => item == null);
	}
	public static void CreateGos(){
		foreach (GameObject go in InputMachine.instance.spawners) {
			GameObject.Instantiate (go);
		}
		InputMachine.instance.CheckObjects ();
	}
	public static void Save(){
		PlayerMachine.instance.SaveGos ();
	}
	public static void Load(){
		PlayerMachine.instance.LoadGos ();
	}
	public void SetText(){
		GetComponent<TextMesh> ().text = "Wood" + PlayerMachine.instance.GetResource ("Wood");
	}
}
