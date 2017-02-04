using UnityEngine;
using System.Collections;

public class HandInstance : MonoBehaviour {

	public GameObject noHold, holdTrue, holdFalse;
	public GameObject current;

	public void SetNoHold(){
		if (current == noHold) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = noHold;
		current.SetActive (true);
	}
	public void SetHoldTrue(){
		if (current == holdTrue) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = holdTrue;
		current.SetActive (true);
	}
	public void SetHoldFalse(){
		if (current == holdFalse) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = holdFalse;
		current.SetActive (true);
	}
}
