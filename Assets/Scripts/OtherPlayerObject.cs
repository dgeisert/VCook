using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerObject : MonoBehaviour {

	public Transform head, left, right;
    public AudioSource chatAudio;

    public void Init()
    {
        chatAudio.clip = AudioClip.Create("chat", 11025, 1, 11025, false, false);
    }

	public void InterpretLocation(float[] f){
		if (f.Length == 21) {
			head.transform.position = new Vector3 (f [0], f [1], f [2]);
			head.transform.rotation = new Quaternion (f [3], f [4], f [5], f [6]);
			right.transform.position = new Vector3 (f [7], f [8], f [9]);
			right.transform.rotation = new Quaternion (f [10], f [11], f [12], f [13]);
			left.transform.position = new Vector3 (f [14], f [15], f [16]);
			left.transform.rotation = new Quaternion (f [17], f [18], f [19], f [20]);
		}
	}

	public void GrabObject(ItemMachine im, int hand){
		switch (hand) {
		case 0:
			im.transform.SetParent (head);
			break;
		case 2:
			im.transform.SetParent (left);
			break;
		case 1:
			im.transform.SetParent (right);
			break;
		default:
			break;
		}
		im.rb.isKinematic = true;
		im.rb.useGravity = false;
		im.transform.localPosition = Vector3.zero;
		im.transform.localRotation = Quaternion.identity;
	}

	public void ReleaseObject(ItemMachine im, int hand, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angvel){
		im.transform.SetParent (null);
		im.rb.isKinematic = false;
		im.rb.useGravity = true;
		im.transform.position = pos;
		im.transform.rotation = rot;
		im.rb.velocity = vel;
		im.rb.angularVelocity = angvel;
	}
}
