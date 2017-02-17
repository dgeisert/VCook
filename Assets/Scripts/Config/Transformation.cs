using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TransformationType {
	None,
	Chop,
	Grill,
	Boil,
	Fry,
	Bin,
	Sell,
	Trash
}

[System.Serializable]
public class Transformation {

	[SerializeField] public ItemMachine inItem, outItem;
	[SerializeField] public TransformationType type;

}
