using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMachine : StateMachine {

	public TransformationType transformationType = TransformationType.None;
	public int phases = 3 , value = 10;
	public string itemName;
	public Rigidbody rb;
	public string itemID;
	static string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
	public int updatePriority = 0;

	public override void InstanceInitiate(StateMachine checkMachine){
		if (itemID == null || itemID == "") {
			itemID = "";
			for (int i = 0; i < 10; i++) {
				itemID += glyphs [Random.Range (0, glyphs.Length)];
			}
		}
		NetworkManager.instance.allObjects.Add (itemID, this);
	}
	public void SetID(string SetID){
		itemID = SetID;
		NetworkManager.instance.allObjects.Add (itemID, this);
	}

	public void Grab(){
		//send message
	}

	public void Release(){
		//send message
	}

	void OnCollisionEnter (Collision col){
		ItemMachine im = col.collider.GetComponentInParent<ItemMachine> ();
		if (im == null) {
			SurfaceMachine sm = col.collider.GetComponentInParent<SurfaceMachine> ();
			if (sm == null) {
				return;
			}
			if (sm.transformationType != TransformationType.None) {
				TransformationManager.instance.Transformation (this, sm);
				return;
			}
		}
		RecipeManager.instance.RecipeOutput (this, im);
		if (this.transformationType != TransformationType.None) {
			TransformationManager.instance.Transformation (im, this);
		}
	}
}
