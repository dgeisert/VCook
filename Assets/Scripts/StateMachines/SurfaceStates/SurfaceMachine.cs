using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRTK;

public class SurfaceMachine : VRTK_InteractableObject {

	public Transform holdPosition, specialHoldPosition;
	public List<ItemMachine> specialHold = new List<ItemMachine>();
	public bool spawnSpecialOnStart = false;
	public ItemMachine heldItem, specialHeldItem;
	public TransformationType transformationType = TransformationType.None;
	public bool holding = false, focus = false;
    public Timer secondaryTimer, timer;

	public void Start()
    {
        secondaryTimer = new Timer();
    }

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        if (usingObject != null)
        {
            if(usingObject.GetComponentInParent<SteamVR_TrackedObject>() != null)
            {
                switch (transformationType)
                {
                    case TransformationType.Bin:
                        ItemMachine im = PlayerMachine.instance.CreateItem(specialHold[0].gameObject, Vector3.zero, Quaternion.identity, true, usingObject.transform, "", true);
                        break;
                        /*
                    case TransformationType.Sell:
                        if (hand.heldItem == null)
                        {
                            return false;
                        }
                        return SellItem(hand.heldItem);
                        break;
                    case TransformationType.Trash:
                        Destroy(hand.heldItem.gameObject);
                        break;
                        */
                    default:

                        break;
                }
            }
        }
	}

	public bool SellItem(ItemMachine item){
		foreach (ItemMachine im in specialHold) {
			if (im.itemName == item.itemName) {
                return TransformationManager.instance.Sell(item);
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
				specialHeldItem.transform.SetParent (specialHoldPosition);
				specialHeldItem.transform.localPosition = Vector3.zero;
				specialHeldItem.transform.localEulerAngles = new Vector3 (0, specialHeldItem.transform.localEulerAngles.y, 0);
				return;
			}
		}
		heldItem = item;
		heldItem.rb.isKinematic = true;
		heldItem.rb.useGravity = false;
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
				heldItem = null;
				return;
			}
		}
		if (specialHeldItem != null && transformationType != TransformationType.Bin) {
			if (specialHeldItem.itemName == item.itemName) {
				specialHeldItem.rb.isKinematic = false;
				specialHeldItem.rb.useGravity = true;
				specialHeldItem = null;
				return;
			}
		}
	}
}
