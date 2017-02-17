using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationManager : MonoBehaviour {

	[SerializeField] public List<Transformation> Transformations = new List<Transformation>();
	private Dictionary<string, ItemMachine> itemList;
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
		return TransformationChecker.ContainsKey(item.itemName + tranType.ToString());
	}

	public bool Transformation(ItemMachine item){
		if (InputMachine.instance.Right.heldItem != null) {
			if (CheckTransformation (item, item.holdingSurface.transformationType) && item.holdingSurface.transformationType == InputMachine.instance.Right.heldItem.transformationType) {
				item.phase++;
				if (item.phase >= item.phases) {
					GameObject go = (GameObject)GameObject.Instantiate (TransformationChecker [item.itemName + item.holdingSurface.transformationType.ToString ()].outItem.gameObject);
					item.holdingSurface.heldItem = null;
					item.holdingSurface.SetItem (go.GetComponent<ItemMachine> ());
					Destroy (item.gameObject);
					return true;
				}
				return true;
			}
		}
		return false;
	}

}
