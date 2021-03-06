﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationManager : MonoBehaviour {

	[SerializeField] public List<Transformation> Transformations = new List<Transformation>();
	public Dictionary<string, ItemMachine> itemList;
	private Dictionary<string, Transformation> TransformationChecker;
	public static TransformationManager instance;

	public void Start(){
		instance = this;
		itemList = new Dictionary<string, ItemMachine> ();
		TransformationChecker = new Dictionary<string, Transformation>();
		foreach (Transformation t in Transformations) {
			if(!itemList.ContainsKey(t.inItem.itemName)){
				itemList.Add (t.inItem.itemName, t.inItem);
			}
			if(!itemList.ContainsKey(t.outItem.itemName)){
				itemList.Add (t.outItem.itemName, t.outItem);
			}
			TransformationChecker.Add (t.inItem.itemName + t.type.ToString(), t);
		}
	}

	public bool CheckTransformation (ItemMachine item, TransformationType tranType){
		if (item == null) {
			return false;
		}
		return TransformationChecker.ContainsKey(item.itemName + tranType.ToString());
	}

	public bool Transformation(ItemMachine item, ItemMachine heldItem){
        if ((NetworkManager.instance.IsInLobby() && NetworkManager.instance.IsHost()) || !NetworkManager.instance.IsInLobby())
        {
            if (heldItem != null)
            {
                if (CheckTransformation(item, heldItem.transformationType) && heldItem.transformationType == heldItem.transformationType)
                {
                    item.phase++;
                    if (item.phase >= item.phases)
                    {
                        PlayerMachine.instance.CreateItem(TransformationChecker[item.itemName + heldItem.transformationType.ToString()].outItem.gameObject, item.transform.position, item.transform.rotation, false, item.transform.parent);
                        Destroy(item);
                        return true;
                    }
                    return true;
                }
            }
            return false;
        }
        return false;
	}

    public bool Sell(ItemMachine item)
    {
        PlayerMachine.instance.AddResource("coins", item.value);
        Destroy(item.gameObject);
        return true;
    }

	public bool Transformation(ItemMachine item, SurfaceMachine surface)
    {
        if ((NetworkManager.instance.IsInLobby() && NetworkManager.instance.IsHost()) || !NetworkManager.instance.IsInLobby())
        {
            if (surface != null)
            {
                if (surface.transformationType == TransformationType.Sell)
                {
                    return surface.SellItem(item);
                }
                if (CheckTransformation(item, surface.transformationType) && surface.transformationType == surface.transformationType)
                {
                    item.phase++;
                    if (item.phase >= item.phases)
                    {
                        PlayerMachine.instance.CreateItem(TransformationChecker[item.itemName + surface.transformationType.ToString()].outItem.gameObject, item.transform.position, item.transform.rotation, false, item.transform.parent);
                        Destroy(item);
                        return true;
                    }
                    return true;
                }
            }
            return false;
        }
        return false;
	}
}
