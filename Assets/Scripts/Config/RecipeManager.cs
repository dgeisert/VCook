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
			RecipeChecker.Add (r.inB.itemName + r.inA.itemName, r);
		}
	}

	public bool CheckRecipes (ItemMachine heldItem, ItemMachine surfaceItem){
		return RecipeChecker.ContainsKey(heldItem.itemName + surfaceItem.itemName);
	}

	public void RecipeOutput (ItemMachine heldItem, ItemMachine surfaceItem){
		if (!CheckRecipes (heldItem, surfaceItem)) {
			return;
		}
		Recipe r = RecipeChecker [heldItem.itemName + surfaceItem.itemName];
		ItemMachine toHand = r.outC;
		ItemMachine toSurface = r.outD;
		if (heldItem.itemName == r.inB.itemName) {
			toHand = r.outD;
			toSurface = r.outC;
		}
		if (toHand != null) {
			GameObject toHandObject = (GameObject)GameObject.Instantiate (toHand.gameObject);
			InputMachine.instance.Right.heldItem = null;
			InputMachine.instance.Right.PickUpItem (toHandObject.GetComponent<ItemMachine> ());
		}
		if (toSurface != null) {
			GameObject toSurfaceObject = (GameObject)GameObject.Instantiate (toSurface.gameObject);
			surfaceItem.holdingSurface.SetItem (toSurfaceObject.GetComponent<ItemMachine> ());
		}

		Destroy (heldItem.gameObject);
		Destroy (surfaceItem.gameObject);
	}
}
