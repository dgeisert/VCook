﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OtherPlayerObject : MonoBehaviour {

	public Transform head, left, right;
    public AudioSource chatAudio;
    public AudioClip clip;
    public float createTime;
    public float lastAudio;

    public void Init(float timestamp)
    {
        clip = AudioClip.Create("chat", 11025, 1, 11025, false, false);
        chatAudio.clip = clip;
        createTime = timestamp;
        lastAudio = Time.time;
		chatAudio.PlayDelayed (0.2f);
    }

    public void Update()
    {
        if(lastAudio + 0.4f < Time.time)
        {
            clip.SetData(new float[11025], 0);
			lastAudio = Time.time;
        }
        if (chatAudio.timeSamples > 1001)
        {
            clip.SetData(new float[1000], chatAudio.timeSamples - 1001);
        }else
        {
            clip.SetData(new float[500], clip.samples - 501);
        }
    }

    public void PlayAudio(float[] audio, float timestamp)
    {
		clip.SetData (audio, Mathf.Max(0, Mathf.Min(11024, Mathf.FloorToInt (11025 * ((timestamp - createTime)  - Mathf.Floor(timestamp - createTime))))));
		lastAudio = Time.time;
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

	public void GrabObject(ItemMachine im, int hand = 0){
        switch (hand) {
		case 2:
            im.transform.SetParent(head);
            im.transform.localPosition = Vector3.zero;
            break;
		case 4:
		case 1:
            im.transform.SetParent(left);
            if (im.grabAttachMechanicScript.leftSnapHandle != null)
			{
				im.transform.rotation = left.transform.rotation * Quaternion.Euler (im.grabAttachMechanicScript.leftSnapHandle.localEulerAngles);
				im.transform.position = left.transform.position - (im.grabAttachMechanicScript.leftSnapHandle.transform.position - im.transform.position);
            }
            else
            {
                im.transform.localPosition = Vector3.zero;
            }
			break;
		case 3:
		case 0:
            im.transform.SetParent(right);
            if (im.grabAttachMechanicScript.rightSnapHandle != null)
			{
				im.transform.rotation = right.transform.rotation * Quaternion.Euler (im.grabAttachMechanicScript.rightSnapHandle.localEulerAngles);
				im.transform.position = right.transform.position - (im.grabAttachMechanicScript.rightSnapHandle.transform.position - im.transform.position);
            }
            else
            {
                im.transform.localPosition = Vector3.zero;
            }
            break;
		default:
			break;
        }
        im.sendRelease = true;
        im.isKinematic = true;
        im.rb.isKinematic = true;
    }

	public void ReleaseObject(ItemMachine im, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angvel, bool isMine = false)
    {
        im.transform.SetParent(null);
        im.rb.isKinematic = false;
        im.isKinematic = false;
		im.transform.position = pos;
		im.transform.rotation = rot;
		im.rb.velocity = vel;
		im.rb.angularVelocity = angvel;
	}
}
