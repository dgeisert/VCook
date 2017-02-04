using UnityEngine;
using System.Collections;

public class RepositionWithMove : MonoBehaviour {
	void Start(){
		InputMachine.instance.repositionWithMoves.Add (transform);
	}
}