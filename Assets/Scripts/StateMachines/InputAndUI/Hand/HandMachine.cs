using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using Holoville.HOTween;

[System.Serializable]
public enum Hand {
	Right,
	Left,
	Headset
}
public enum InteractionButton {
	Trigger,
	Grip,
	A,
	B
}

public class HandMachine : InputMachine {

	[SerializeField] public Hand hand = Hand.Headset;
	public ReticleMachine reticle;
	public Transform raycastTransform, holdPosition;
	public bool is_holding = false;
	private InputMachine currentInteractionState;
	private HandInstance handInstance;
	public SteamVR_Controller.Device controller;
	private KeyValuePair<float, Vector3> twoTenthsBack, oneTenthBack;

	public GameObject blackout;
	public SpriteRenderer blackoutDip;

	public ItemMachine heldItem;
	public float holdStart;
	float lastSwipeTime;
	bool is_swipe = false;
	public bool canInteract;
	public StateMachine touchedObject;
	public float timeToResetTouchedObject = 0.2F;

	public virtual void Tap(GameObject obj, Vector3 point, StateMachine checkMachine, InteractionButton interaction, bool is_distant){}
	public virtual void Release(GameObject obj, Vector3 point, StateMachine checkMachine, InteractionButton interaction, bool is_distant){}
	public virtual void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){}

	HandMachine GetCurrentState(){
		if (currentState == null) {
			return StateMaster.instance.inputInspect;
		}
		return (HandMachine) currentState;
	}

	void OnEnable(){
		if (hand != Hand.Headset) {
			controller = SteamVR_Controller.Input ((int)(GetComponent<SteamVR_TrackedObject> ().index));
		}
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		if (hand == Hand.Headset) {
			HOTween.To (blackoutDip, 5f, new TweenParms ().Prop ("color", new Color (0f, 0f, 0f, 0f)));
		}
		else{
			SetHand(StateMaster.instance.inputInteract);
		}
		if (reticle != null) {
			reticle.Initiate ();
		}
		secondaryTimer = new Timer (this);
		oneTenthBack = new KeyValuePair<float, Vector3> (Time.time, transform.position);
		twoTenthsBack = new KeyValuePair<float, Vector3> (Time.time, transform.position);
	}

	public void SetHand(InputMachine setValue){
		reticle.SetReticle (setValue);
		if (currentInteractionState == setValue) {
			return;
		}
		if (handInstance != null) {
			Destroy (handInstance.gameObject);
		}
		currentInteractionState = setValue;
		GameObject newHand = (GameObject)GameObject.Instantiate (Resources.Load ("Inputs/Hands/Primary/" + setValue.GetType().ToString()) as GameObject, transform.position, transform.rotation, transform);
		if (handInstance != null) {
			Destroy (handInstance.gameObject);
		}
		handInstance = newHand.GetComponent<HandInstance> ();
		switch(hand){
		case Hand.Right:
			//set right
			break;
		case Hand.Left:
			newHand.transform.localScale = new Vector3 (-1, 1, 1);
			break;
		default:
			break;
		}
	}

	void OnTriggerEnter(Collider col){
		HandMachine hm = col.GetComponentInParent<HandMachine> ();
		if (hm != null) {
			if (hm.heldItem != null && heldItem != null) {
				if (!TransformationManager.instance.Transformation (heldItem, hm.heldItem)) {
					RecipeManager.instance.RecipeOutput (heldItem, hm.heldItem);
				}
			}
		}
	}

	void OnTriggerStay(Collider col){
		StateMachine sm = col.GetComponentInParent<StateMachine> ();
		if (sm == null) {
			return;
		}
		if (sm.GetComponent<ReticleMachine> () != null) {
			return;
		}
		if (sm.GetComponent<HandMachine> () != null) {
			return;
		}
		if (touchedObject == null) {
			touchedObject = sm;
			secondaryTimer.StartTimer (timeToResetTouchedObject);
			return;
		}
		if (touchedObject == sm) {
			return;
			secondaryTimer.StartTimer (timeToResetTouchedObject);
		}
		if (Vector3.Distance(transform.position, sm.transform.position) < Vector3.Distance(transform.position, touchedObject.transform.position)) {
			touchedObject = sm;
			secondaryTimer.StartTimer (timeToResetTouchedObject);
		}
	}

	KeyValuePair<Vector3, GameObject> GetSightedPoint(Transform raycastTransform){
		if (touchedObject != null) {
			return new KeyValuePair<Vector3, GameObject> (touchedObject.transform.position, touchedObject.gameObject);
		}
		RaycastHit hit;
		Ray inputRay = new Ray(raycastTransform.position, raycastTransform.forward);
		if (Physics.Raycast (inputRay, out hit, InputMachine.instance.maxDistance)) {
			if (hit.transform != null) {
				return new KeyValuePair<Vector3, GameObject> (hit.point, hit.transform.gameObject);
			} else {
				return new KeyValuePair<Vector3, GameObject> (transform.position + transform.forward * InputMachine.instance.maxDistance, null);
			}
		} else {
			return new KeyValuePair<Vector3, GameObject> (transform.position + transform.forward * InputMachine.instance.maxDistance, null);
		}
	}

	public override void InstanceUpdate(StateMachine checkMachine) {
		KeyValuePair<Vector3, GameObject> kvp = GetSightedPoint (raycastTransform);
		GameObject sightedObject = kvp.Value;
		Vector3 sightedPoint = kvp.Key;
		if (lastSwipeTime + 0.1f < Time.time) {
			is_swipe = false;
		}
		if (reticle != null) {
			reticle.Gaze (sightedPoint);
		}
		if (controller != null) {
			bool is_distant = touchedObject == null;
			if (!is_distant) {
				is_distant = sightedObject != touchedObject.gameObject;
			}
			if (controller.GetHairTriggerDown()) {
				holdStart = Time.time;
				GetCurrentState ().Tap (sightedObject, sightedPoint, this, InteractionButton.Trigger, is_distant);
			}
			if (controller.GetHairTriggerUp()) {
				if (!is_swipe) {
					GetCurrentState ().Release (sightedObject, sightedPoint, this, InteractionButton.Trigger, is_distant);
				}
			}
			if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip)) {
				UpdateState (StateMaster.instance.inputPickUp, this);
				is_holding = true;
				holdStart = Time.time;
				GetCurrentState ().Tap (sightedObject, sightedPoint, this, InteractionButton.Grip, is_distant);
			}
			if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_Grip)) {
				is_holding = false;
				if (heldItem != null) {
					heldItem.transform.SetParent (null);
					heldItem.holdingHand = null;
					heldItem.rb.isKinematic = false;
					heldItem.rb.useGravity = true;
					heldItem.rb.AddForce ((transform.position - twoTenthsBack.Value) / (Time.time - twoTenthsBack.Key) * 150);
					heldItem = null;
				}
			}
			if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_A)) {
				UpdateState (StateMaster.instance.inputTeleport, this);
				holdStart = Time.time;
				GetCurrentState ().Tap (sightedObject, sightedPoint, this, InteractionButton.A, is_distant);
			}
			if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_A)) {
				if (!is_swipe) {
					GetCurrentState ().Release (sightedObject, sightedPoint, this, InteractionButton.A, is_distant);
				}
			}
		}
		if (is_holding) {
			GetCurrentState().CheckInteract (sightedObject, sightedPoint, this);
		} else {
			CheckInteractionChange (sightedObject);
		}
		if (handInstance != null) {
			if (!is_holding) {
				handInstance.SetNoHold ();
			} else {
				if (canInteract) {
					handInstance.SetHoldTrue ();
				} else {
					handInstance.SetHoldFalse ();
				}
			}
		}
		if (heldItem != null) {
			heldItem.transform.localPosition = Vector3.zero;
		}
		if (secondaryTimer.CheckTimer ()) {
			touchedObject = null;
		}
		if (oneTenthBack.Key + 0.1F < Time.time) {
			twoTenthsBack = oneTenthBack;
			oneTenthBack = new KeyValuePair<float, Vector3> (Time.time, transform.position);
		}
	}

	public void DipToColor(Color col, float duration){
		if (enabled) {
			StartCoroutine (DipToColorStart (col, duration));
		}
	}
	IEnumerator DipToColorStart(Color col, float duration){
		blackoutDip.gameObject.SetActive(true);
		blackoutDip.color = new Color (col.r, col.g, col.b, 0f);
		HOTween.To(blackoutDip, duration, new TweenParms().Prop("color", col));
		yield return new WaitForSeconds (duration);
		InputMachine.instance.CheckObjects ();
		HOTween.To(blackoutDip, duration, new TweenParms().Prop("color", new Color(col.r, col.g, col.b, 0f)));
		yield return new WaitForSeconds (duration);
		blackoutDip.gameObject.SetActive(false);
	}

	public virtual void SwipeUp(GameObject obj, Vector3 point, StateMachine checkMachine){
		//GetCurrentState ().SwipeUp (obj, point, checkMachine);
		is_swipe = true;
		lastSwipeTime = Time.time;
	}
	public virtual void SwipeDown(GameObject obj, Vector3 point, StateMachine checkMachine){
		//GetCurrentState().SwipeDown (obj, point, checkMachine);
		is_swipe = true;
		lastSwipeTime = Time.time;
	}
	public virtual void SwipeForward(GameObject obj, Vector3 point, StateMachine checkMachine){
		//GetCurrentState().SwipeForward (obj, point, checkMachine);
		is_swipe = true;
		lastSwipeTime = Time.time;
	}
	public virtual void SwipeBack(GameObject obj, Vector3 point, StateMachine checkMachine){
		//GetCurrentState().SwipeBack (obj, point, checkMachine);
		is_swipe = true;
		lastSwipeTime = Time.time;
	}

	public void CheckInteractionChange(GameObject sightedObject){
		if (sightedObject != null) {
			StateMachine sm = sightedObject.GetComponentInParent<StateMachine> ();
			if (sm != null) {
				List<InputMachine> inputs = sm.InstanceHover ();
				if (inputs != null) {
					if (inputs.Count == 1) {
						UpdateState (inputs [0], this);
						reticle.SetReticle (inputs [0]);
					}
				}
			}
		}
	}

	public void PickUpItem(ItemMachine item){
		if (heldItem != null) {
			RecipeManager.instance.RecipeOutput (heldItem, item);
		} else {
			heldItem = item;
			heldItem.holdingHand = this;
			heldItem.rb.isKinematic = true;
			heldItem.rb.useGravity = false;
			if (heldItem.holdingSurface != null) {
				heldItem.holdingSurface.RemoveItem (item);
			}
			item.transform.SetParent (transform);
			heldItem.transform.localPosition = Vector3.zero;
			heldItem.transform.localRotation = Quaternion.identity;
			NetworkManager.instance.SendGrab (heldItem.itemID, (int)hand);
		}
	}
	public void PlaceObject(SurfaceMachine surface){
		if (surface.heldItem != null && surface.specialHeldItem != null) {
			return;
		}
		if (heldItem == null) {
			return;
		}
		foreach (ItemMachine im in surface.specialHold) {
			if (im.itemName == heldItem.itemName && surface.specialHeldItem == null) {
				surface.SetItem (heldItem);
				heldItem = null;
				return;
			}
		}
		if (surface.heldItem == null) {
			surface.SetItem (heldItem);
			heldItem = null;
		}
	}
}
