using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurfaceMachine : StateMachine {

	public Transform holdPosition, specialHoldPosition;
	public List<ItemMachine> specialHold = new List<ItemMachine>();
	public bool spawnSpecialOnStart = false;
	public ItemMachine heldItem, specialHeldItem;
	public TransformationType transformationType = TransformationType.None;
	public bool holding = false, focus = false;

	public void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		secondaryTimer = new Timer (this);
		if (spawnSpecialOnStart && specialHold.Count > 0) {
			GameObject go = (GameObject) GameObject.Instantiate (specialHold [0].gameObject);
			SetItem (go.GetComponent<ItemMachine> ());
			if (transformationType == TransformationType.Bin) {
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				foreach (Collider col in go.GetComponentsInChildren<Collider>()) {
					Destroy (col);
				}
			}
		}
	}

	public override List<InputMachine> InstanceHover(){
		return new List<InputMachine>(){StateMaster.instance.inputPickUp };
	}

	public override bool InstanceGrab(GameObject obj, Vector3 point, StateMachine checkMachine, HandMachine hand){
		switch (transformationType) {
		case TransformationType.Bin:
			if (hand.heldItem == null && heldItem == null) {
				GameObject go = (GameObject)GameObject.Instantiate (specialHold [0].gameObject);
				hand.PickUpItem (go.GetComponent<ItemMachine>());
				return true;
			}
			break;
		case TransformationType.Sell:
			if (hand.heldItem == null) {
				return false;
			}
			return SellItem (hand.heldItem);
			break;
		case TransformationType.Trash:
			Destroy (hand.heldItem.gameObject);
			break;
		default:
			hand.PlaceObject(this);
			break;
		}
		return true;
	}

	public bool SellItem(ItemMachine item){
		foreach (ItemMachine im in specialHold) {
			if (im.itemName == item.itemName) {
				PlayerMachine.instance.AddResource ("coins", item.value);
				Destroy (item.gameObject);
				return true;
			}
		}
		return false;
	}

	public void SetItem(ItemMachine item){
		foreach(ItemMachine im in specialHold){
			if (specialHeldItem == null && im.itemName == item.itemName) {
				specialHeldItem = item;
				specialHeldItem.rb.isKinematic = true;
				specialHeldItem.rb.useGravity = false;
				specialHeldItem.holdingSurface = this;
				specialHeldItem.transform.SetParent (specialHoldPosition);
				specialHeldItem.transform.localPosition = Vector3.zero;
				specialHeldItem.transform.localEulerAngles = new Vector3 (0, specialHeldItem.transform.localEulerAngles.y, 0);
				return;
			}
		}
		heldItem = item;
		heldItem.rb.isKinematic = true;
		heldItem.rb.useGravity = false;
		heldItem.holdingSurface = this;
		heldItem.transform.SetParent (holdPosition);
		heldItem.transform.localPosition = Vector3.zero;
		heldItem.transform.localEulerAngles = new Vector3 (0, heldItem.transform.localEulerAngles.y, 0);
		heldItem.transform.localScale = Vector3.one;
	}

	public void RemoveItem(ItemMachine item){
		if (heldItem == null && specialHeldItem == null) {
			return;
		}
		if (heldItem != null) {
			if (heldItem.itemName == item.itemName) {
				heldItem.rb.isKinematic = false;
				heldItem.rb.useGravity = true;
				heldItem.holdingSurface = null;
				heldItem = null;
				return;
			}
		}
		if (specialHeldItem != null && transformationType != TransformationType.Bin) {
			if (specialHeldItem.itemName == item.itemName) {
				specialHeldItem.rb.isKinematic = false;
				specialHeldItem.rb.useGravity = true;
				specialHeldItem.holdingSurface = null;
				specialHeldItem = null;
				return;
			}
		}
	}
}
