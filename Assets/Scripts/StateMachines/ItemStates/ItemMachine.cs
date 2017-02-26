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

    public void Start()
    {
        if (!NetworkManager.instance.allObjects.ContainsKey(itemID))
        {
            Init();
        }
    }
	public void Init(){
		if (itemID == null || itemID == "") {
			itemID = "";
			for (int i = 0; i < 10; i++) {
				itemID += glyphs [Random.Range (0, glyphs.Length)];
			}
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
        NetworkManager.instance.SendString(itemID, InterpretationType.GrabObject);
    }
    public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
    {
        base.OnInteractableObjectUngrabbed(e);
        float[] f = new float[] { transform.position.x, transform.position.y, transform.position.z, transform.rotation.w, transform.rotation.x, transform.rotation.y, transform.rotation.z, rb.velocity.x, rb.velocity.y, rb.velocity.z, rb.angularVelocity.x, rb.angularVelocity.y, rb.angularVelocity.z};
        NetworkManager.instance.SendStringFloat(itemID, f, InterpretationType.ReleaseObject);
    }

    void OnCollisionEnter (Collision col){
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
}
