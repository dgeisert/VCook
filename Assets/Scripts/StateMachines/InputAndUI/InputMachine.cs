using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Holoville.HOTween;

public class InputMachine: StateMachine {

	public GameObject[] spawners;

	public Camera thisCamera;
	public RoomMachine currentRoom;
	public float renderDistance = 50;
	float lastSwipeTime;
	bool is_swipe = false;
	public float deltaTime;
	public TextMesh fpsText;

	public ReticleMachine reticle;
	public GameObject setRightHand, setLeftHand, setReticle;
	public bool canInteract;

	public RoomMachine myRoom;
	public GameObject mainUI;
	public GameObject loadingUI;
	public GameObject blackout;
	public SpriteRenderer blackoutDip;
	public float maxDistance = 10f;
	public static float playerHeight = 1.2f;
	public float canvasDistance = 3f;
	public float canvasWidth = 3f;
	public float holdStart;
	public bool is_holding = false;
	public float holdingTimeSwipeNegation = 0.3f;
	public float waitToSpawnUI = 1;
	public static InputMachine instance;
	public InputMachine swipeUp, swipeForward, swipeBack, swipeDown;
	public List<GameObject> gos;
	public List<RoomMachine> rooms;
	public List<Transform> repositionWithMoves;

	void Start(){
		if (GetComponent<Camera> () == null) {
			string script = this.GetType ().ToString ();
			setRightHand = Resources.Load("Inputs/Hands/Primary/" + script) as GameObject;
			setLeftHand = Resources.Load("Inputs/Hands/Secondary/" + script) as GameObject;
			setReticle = Resources.Load("Inputs/Reticles/" + script) as GameObject;
		}
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		InputMachine.instance = this;
		OVRTouchpad.Create ();
		swipeUp = StateMaster.instance.inputUI;
		swipeDown = StateMaster.instance.inputInteract;
		swipeForward = StateMaster.instance.inputTeleport;
		swipeBack = StateMaster.instance.inputInteract;
		UpdateState (StateMaster.instance.inputTeleport, this);
		EventSystem.current.sendNavigationEvents = false;
		thisCamera.layerCullSpherical = true;
		Application.targetFrameRate = 240;
		HOTween.To(blackoutDip, 5f, new TweenParms().Prop("color", new Color(0f, 0f, 0f, 0f)));
		StartCoroutine ("InitialCheckObjects");
	}

	public IEnumerator InitialCheckObjects(){
		yield return new WaitForSeconds(0.5f);
		CheckObjects ();
		CheckObjects ();
	}

	InputMachine GetCurrentState(){
		return (InputMachine) currentState;
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
	public virtual void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public virtual void Release(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public virtual void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){}


	Vector3 GetSightedPoint(){
		RaycastHit hit;
		Ray inputRay;
		if (VRDevice.isPresent) {
			inputRay = new Ray(transform.position, transform.forward);
		} else {
			inputRay = thisCamera.ScreenPointToRay (Input.mousePosition);
		}
		if (Physics.Raycast (inputRay, out hit, maxDistance)) {
			if (hit.transform != null) {
				return hit.point;
			} else {
				return transform.position + transform.forward * maxDistance;
			}
		} else {
			return transform.position + transform.forward * maxDistance;
		}
	}
	GameObject GetSightedObject(){
		RaycastHit hit;
		Ray inputRay;
		if (VRDevice.isPresent) {
			inputRay = new Ray(transform.position, transform.forward);
		} else {
			inputRay = thisCamera.ScreenPointToRay (Input.mousePosition);
		}
		if (Physics.Raycast (inputRay, out hit, maxDistance)) {
			if (hit.transform != null) {
				return hit.transform.gameObject;
			}
		}
		return null;
	}
	void Gaze(Vector3 point){
		reticle.transform.position = point;
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		GameObject sightedObject = GetSightedObject ();
		Vector3 sightedPoint = GetSightedPoint ();
		if (lastSwipeTime + 0.1f < Time.time) {
			is_swipe = false;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) || OVRInput.GetUp(OVRInput.RawButton.DpadUp)) {
			SwipeUp (sightedObject, sightedPoint, this);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || OVRInput.GetUp(OVRInput.RawButton.DpadDown)) {
			SwipeDown (sightedObject, sightedPoint, this);
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || OVRInput.GetUp(OVRInput.RawButton.DpadLeft)) {
			SwipeBack (sightedObject, sightedPoint, this);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) || OVRInput.GetUp(OVRInput.RawButton.DpadRight)) {
			SwipeForward (sightedObject, sightedPoint, this);
		}
		if (Input.GetMouseButtonDown(0) && !is_swipe) {
			is_holding = true;
			holdStart = Time.time;
			GetCurrentState().Tap (sightedObject, sightedPoint, this);
		}
		if (Input.GetMouseButtonUp(0)) {
			is_holding = false;
			if (!is_swipe) {
				GetCurrentState ().Release (sightedObject, sightedPoint, this);
			}
		}
		if (is_holding) {
			GetCurrentState().CheckInteract (sightedObject, sightedPoint, this);
		} else {
			CheckInteractionChange (sightedObject);
		}
		if(Input.GetKeyDown(KeyCode.A)){
			PlayerMachine.playerObject.transform.Rotate (0, -45, 0);
		}
		if(Input.GetKeyDown(KeyCode.D)){
			PlayerMachine.playerObject.transform.Rotate (0, 45, 0);
		}
		if(Input.GetKeyDown(KeyCode.M)){
			ADMIN.DestroyGos ();
		}
		if(Input.GetKeyDown(KeyCode.N)){
			ADMIN.CreateGos ();
		}
		if(Input.GetKeyDown(KeyCode.K)){
			ADMIN.Save ();
		}
		if(Input.GetKeyDown(KeyCode.L)){
			ADMIN.Load ();
		}
		Gaze (GetSightedPoint ());
		if (currentState != null) {
			currentState.InstanceUpdate (this);
		}
		if (InputMachine.instance != this) {
			return;
		}
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float fps = 1.0f / deltaTime;
		fpsText.text = string.Format("{0:0.} fps", fps);
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

	public void SpawnUI(GameObject prefab){
		Quaternion quat = new Quaternion ();
		quat.eulerAngles = new Vector3 (0, transform.rotation.eulerAngles.y, 0);
	}

	public void CheckObjects(){
		gos.RemoveAll (item => item == null);
		repositionWithMoves.RemoveAll (item => item == null);
		if (currentRoom == null) {
			foreach (GameObject go in gos) {
				if (go != null) {
					float magnitude = Vector3.Magnitude (go.transform.lossyScale);
					if (go.GetComponent<MeshRenderer> () != null) {
						magnitude *= Vector3.Magnitude (go.GetComponent<MeshRenderer> ().bounds.size);
					}
					if ((renderDistance + magnitude) < Vector3.Distance (transform.position, go.transform.position)) {
						go.SetActive (false);
					} else {
						go.SetActive (true);
					}
				}
				SetObjectParent (go.transform, false, go.GetComponent<StateMachine>());
			}
			foreach (Transform tr in repositionWithMoves) {
				SetObjectParent (tr);
			}
		}
	}
	public void SetObjectParent(Transform tr, bool reset = false, StateMachine sm = null){
		Vector2 newParentGround = 
			new Vector2 (
				(Mathf.Round (tr.position.x / MovingGround.instance.squareScale) + MovingGround.instance.layout.x * 1000) % MovingGround.instance.layout.x,
				(Mathf.Round (tr.position.z / MovingGround.instance.squareScale) + MovingGround.instance.layout.y * 1000) % MovingGround.instance.layout.y
			);
		if (tr.parent == null) {
			if (sm != null) {
				sm.parentGround = newParentGround;
			}
			tr.SetParent (MovingGround.instance.ground[newParentGround]);
			return;
		}
		if (tr.position.x - tr.parent.position.x > MovingGround.instance.squareScale / 2) {
			reset = true;
		}
		if (tr.position.x - tr.parent.position.x < -MovingGround.instance.squareScale / 2) {
			reset = true;
		}
		if (tr.position.z - tr.parent.position.z > MovingGround.instance.squareScale / 2) {
			reset = true;
		}
		if (tr.position.z - tr.parent.position.z < -MovingGround.instance.squareScale / 2) {
			reset = true;
		}
		if (reset) {
			if (sm != null) {
				sm.parentGround = newParentGround;
			}
			tr.SetParent (MovingGround.instance.ground[newParentGround]);
			while (tr.position.x - tr.parent.position.x > MovingGround.instance.squareScale / 2) {
				tr.position -= new Vector3(MovingGround.instance.squareScale, 0, 0);
			}
			while (tr.position.x - tr.parent.position.x < -MovingGround.instance.squareScale / 2) {
				tr.position += new Vector3(MovingGround.instance.squareScale, 0, 0);
			}
			while (tr.position.z - tr.parent.position.z > MovingGround.instance.squareScale / 2) {
				tr.position -= new Vector3(0, 0, MovingGround.instance.squareScale);
			}
			while (tr.position.z - tr.parent.position.z < -MovingGround.instance.squareScale / 2) {
				tr.position += new Vector3(0, 0, MovingGround.instance.squareScale);
			}
		}
	}

	public void SetRoom(RoomMachine room){
		if (currentRoom != null) {
			currentRoom.gameObject.SetActive (false);
		}
		currentRoom = room;
		if (currentRoom == null) {
			CheckObjects ();
			blackout.SetActive (false);
		} else {
			foreach (GameObject go in gos) {
				go.SetActive (false);
			}
			blackout.SetActive (true);
			currentRoom.gameObject.SetActive (true);
			Transform tr = currentRoom.transform;
			while(tr.parent != null){
				tr = tr.parent;
				tr.gameObject.SetActive (true);
			}
		}
	}

	public void SetFarClip(float maxDistance){
		renderDistance = maxDistance - 15;
		thisCamera.farClipPlane = maxDistance * 20;
		CheckObjects ();
	}

	public void SetLoading(){

	}

	public void DipToColor(Color col, float duration){
		StartCoroutine (DipToColorStart(col, duration));
	}
	IEnumerator DipToColorStart(Color col, float duration){
		blackoutDip.gameObject.SetActive(true);
		blackoutDip.color = new Color (col.r, col.g, col.b, 0f);
		HOTween.To(blackoutDip, duration, new TweenParms().Prop("color", col));
		yield return new WaitForSeconds (duration);
		CheckObjects ();
		HOTween.To(blackoutDip, duration, new TweenParms().Prop("color", new Color(col.r, col.g, col.b, 0f)));
		yield return new WaitForSeconds (duration);
		blackoutDip.gameObject.SetActive(false);
	}
}