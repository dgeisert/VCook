using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour {

	[SerializeField] public List<Recipe> Recipes = new List<Recipe>();
	private Dictionary<string, ItemMachine> itemList;
	private Dictionary<string, Recipe> RecipeChecker;
	public static RecipeManager instance;

	public void Start(){
		instance = this;
		itemList = new Dictionary<string, ItemMachine> ();
		RecipeChecker = new Dictionary<string, Recipe>();
		foreach (Recipe r in Recipes) {
			if(!itemList.ContainsKey(r.inA.itemName)){
				itemList.Add (r.inA.itemName, r.inA);
			}
			if(!itemList.ContainsKey(r.inB.itemName)){
				itemList.Add (r.inB.itemName, r.inB);
			}
			if(!itemList.ContainsKey(r.outC.itemName)){
				itemList.Add (r.outC.itemName, r.outC);
			}
			if (r.outD != null) {
				if (!itemList.ContainsKey (r.outD.itemName)) {
					itemList.Add (r.outD.itemName, r.outD);
				}
			}
			RecipeChecker.Add (r.inA.itemName + r.inB.itemName, r);
		}
	}

	public bool CheckRecipes (ItemMachine heldItem, ItemMachine surfaceItem){
		return RecipeChecker.ContainsKey(heldItem.itemName + surfaceItem.itemName);
	}

	public void RecipeOutput (ItemMachine heldItem, ItemMachine surfaceItem){
		if (heldItem == null || surfaceItem == null) {
			return;
		}
		if (!CheckRecipes (heldItem, surfaceItem)) {
			return;
		}
		Recipe r = RecipeChecker [heldItem.itemName + surfaceItem.itemName];
		GameObject toHandObject = (GameObject)GameObject.Instantiate (r.outC.gameObject);
		if (heldItem.holdingHand != null) {
			heldItem.holdingHand.heldItem = null;
			heldItem.holdingHand.PickUpItem (toHandObject.GetComponent<ItemMachine> ());
		} else if (heldItem.holdingSurface != null) {
			heldItem.holdingSurface.SetItem (toHandObject.GetComponent<ItemMachine> ());
		} else {
			toHandObject.transform.position = heldItem.transform.position;
			toHandObject.transform.rotation = heldItem.transform.rotation;
		}
		if (r.outD != null) {
			GameObject toSurfaceObject = (GameObject)GameObject.Instantiate (r.outD.gameObject);
			if (surfaceItem.holdingHand != null) {
				surfaceItem.holdingHand.heldItem = null;
				surfaceItem.holdingHand.PickUpItem (toSurfaceObject.GetComponent<ItemMachine> ());
			} else if (surfaceItem.holdingSurface != null) {
				surfaceItem.holdingSurface.SetItem (toSurfaceObject.GetComponent<ItemMachine> ());
			} else {
				toSurfaceObject.transform.position = surfaceItem.transform.position;
				toSurfaceObject.transform.rotation = surfaceItem.transform.rotation;
			}
		}
		Destroy (heldItem.gameObject);
		Destroy (surfaceItem.gameObject);
	}
}
