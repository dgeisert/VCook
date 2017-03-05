using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRTK;

public class ItemMachine : VRTK_InteractableObject {

	public TransformationType transformationType = TransformationType.None;
	public int phases = 3 , value = 10;
	public string itemName;
	public Rigidbody rb;
	public string itemID;
	static string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
	public int updatePriority = 0;
    public int phase = 0;

	public void Init(){
		if (itemID == null || itemID == "") {
			itemID = "";
			for (int i = 0; i < 10; i++) {
				itemID += glyphs [Random.Range (0, glyphs.Length)];
			}
		}
		NetworkManager.instance.allObjects.Add (itemID, this);
	}

	public void Update(){
		if (NetworkManager.instance.IsInLobby () && NetworkManager.instance.IsHost()) {
			if (transform.parent != null) {
				if (GetComponentInParent<OtherPlayerObject> () != null) {
					updatePriority = 0;
					return;
				}
			}
			updatePriority += Mathf.CeilToInt (rb.velocity.magnitude * 100);
			updatePriority += Mathf.CeilToInt (rb.angularVelocity.magnitude * 100);
			updatePriority++;
		}
	}
	public bool ShouldUpdate(){
		if (updatePriority > 1000) {
			return true;
			updatePriority = 0;
		} else {
			return false;
		}
	}
	public void SetID(string SetID){
		itemID = SetID;
		NetworkManager.instance.allObjects.Add (itemID, this);
    }

    public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
	{
		rb.isKinematic = true;
		rb.useGravity = false;
        base.OnInteractableObjectGrabbed(e);
		transform.SetParent (e.interactingObject.transform);
		updatePriority = 0;
        int hand = 1;
        if (e.interactingObject != null)
        {
            if (e.interactingObject.GetComponentInParent<SteamVR_TrackedObject>() != null && e.interactingObject.GetComponentInParent<OtherPlayerObject>() == null)
            {
                hand = (int)e.interactingObject.GetComponentInParent<SteamVR_TrackedObject>().index;
                NetworkManager.instance.SendGrabObject(this, hand);
            }
        }
    }
    public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
    {
        if (e.interactingObject != null)
        {
            if (e.interactingObject.GetComponentInParent<SteamVR_TrackedObject>() != null && e.interactingObject.GetComponentInParent<OtherPlayerObject>() == null && !NetworkManager.instance.IsHost())
            {
                NetworkManager.instance.SendReleaseObject(this);
            }
        }

        rb.isKinematic = false;
		rb.useGravity = true;
		base.OnInteractableObjectUngrabbed(e);
		transform.SetParent (null);
		updatePriority = 1001;
    }

	void OnCollisionEnter (Collision col){
		updatePriority = 1001;
		ItemMachine im = col.collider.GetComponentInParent<ItemMachine> ();
		if (im != null)
        {
            RecipeManager.instance.RecipeOutput(this, im);
            if (this.transformationType != TransformationType.None)
            {
                TransformationManager.instance.Transformation(im, this);
            }
        }
	}

    public void SetRB(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
    {
        if (transform.GetComponentInParent<SteamVR_TrackedObject>() != null || transform.GetComponentInParent<OtherPlayerObject>() != null)
        {
            return;
        }
        transform.position = pos;
        transform.rotation = rot;
        rb.velocity = vel;
        rb.angularVelocity = angVel;
    }

    public void OnDestroy()
    {
        NetworkManager.instance.DestroyObject(this);
    }
}
