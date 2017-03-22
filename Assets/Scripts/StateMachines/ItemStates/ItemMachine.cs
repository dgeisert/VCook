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
    public int phase = 0;
    public bool sendRelease = true;
    public AudioSource audio;
    public AudioClip defaultClip;
    public Dictionary<ItemMachine, AudioClip> specialtyClips = new Dictionary<ItemMachine, AudioClip>();

	public void Init(){
		if (itemID == null || itemID == "") {
			itemID = "";
			for (int i = 0; i < 10; i++) {
				itemID += glyphs [Random.Range (0, glyphs.Length)];
			}
		}
        if(audio == null)
        {
            audio = GetComponent<AudioSource>();
        }
		NetworkManager.instance.allObjects.Add (itemID, this);
	}
	public void SetID(string SetID){
		itemID = SetID;
		NetworkManager.instance.allObjects.Add (itemID, this);
    }

    public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
	{
        base.OnInteractableObjectGrabbed(e);
		transform.SetParent (e.interactingObject.transform);
        sendRelease = true;
        if (e.interactingObject != null)
        {
            int hand = (int)e.interactingObject.GetComponentInParent<SteamVR_TrackedObject>().index;
            NetworkManager.instance.SendGrabObject(this, hand);
        }
    }
    public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
    {
        if (e.interactingObject != null)
        {
            NetworkManager.instance.SendReleaseObject(this);
        }
        base.OnInteractableObjectUngrabbed(e);
        transform.SetParent(null);
    }
    public override void OnInteractableObjectUsed(InteractableObjectEventArgs e)
    {
        NetworkManager.instance.SendUseObject(this);
        base.OnInteractableObjectUsed(e);
    }

    void OnCollisionEnter (Collision col){
		ItemMachine im = col.collider.GetComponentInParent<ItemMachine> ();
		if (im != null)
        {
            audio.clip = defaultClip;
            foreach(KeyValuePair<ItemMachine, AudioClip> kvp in specialtyClips)
            {
                if (im.itemName == kvp.Key.itemName)
                {
                    audio.clip = kvp.Value;
                }
            }
            if(audio.clip != null)
            {
                audio.Play();
            }
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

    public void Use()
    {
        Debug.Log("Using item: " + itemID);
    }
}
