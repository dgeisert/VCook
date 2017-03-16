using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour {

	[SerializeField] public List<Recipe> Recipes = new List<Recipe>();
	public Dictionary<string, ItemMachine> itemList;
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

    public void RecipeOutput(ItemMachine heldItem, ItemMachine surfaceItem)
    {
        if ((NetworkManager.instance.IsInLobby() && NetworkManager.instance.IsHost()) || !NetworkManager.instance.IsInLobby())
        {
            if (heldItem == null || surfaceItem == null)
            {
                return;
            }
            if (!CheckRecipes(heldItem, surfaceItem))
            {
                return;
            }
            if (heldItem.IsGrabbed())
            {
                heldItem.ForceStopInteracting();
            }
            if (surfaceItem.IsGrabbed())
            {
                surfaceItem.ForceStopInteracting();
            }
            Recipe r = RecipeChecker[heldItem.itemName + surfaceItem.itemName];
            PlayerMachine.instance.CreateItem(r.outC.gameObject, heldItem.transform.localPosition, heldItem.transform.localRotation, true, heldItem.transform.parent);
            if (r.outD != null)
            {
                PlayerMachine.instance.CreateItem(r.outD.gameObject, surfaceItem.transform.localPosition, surfaceItem.transform.localRotation, true, surfaceItem.transform.parent);
            }
            Destroy(heldItem);
            Destroy(surfaceItem);
        }
    }
}
