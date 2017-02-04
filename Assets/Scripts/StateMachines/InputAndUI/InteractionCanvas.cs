using UnityEngine;
using System.Collections;

public class InteractionCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Destroy(){
		Debug.Log ("destroying UI");
		GameObject.Destroy (gameObject);
	}
}
