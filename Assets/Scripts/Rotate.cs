using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public bool rotateX, rotateY, rotateZ;
	public float rotateSpeed = 30;

	void Update (){
		transform.Rotate (Time.deltaTime * (rotateX ? rotateSpeed : 0), Time.deltaTime * (rotateY ? rotateSpeed : 0), Time.deltaTime * (rotateZ ? rotateSpeed : 0));
	}
}
