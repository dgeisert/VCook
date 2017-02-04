using UnityEngine;
using System.Collections;

public class ConstantMovement : MonoBehaviour {

	public Vector3 velocity = Vector3.zero;
	void Update () {
		transform.position += velocity / 60 * Time.deltaTime;
	}
}
