using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Holoville.HOTween;

public class InputMachine: StateMachine {

	public HandMachine Right, Left, Headset;

	public GameObject[] spawners;

	public Camera thisCamera;
	public RoomMachine currentRoom;
	public float renderDistance = 50;
	public float deltaTime;
	public TextMesh fpsText;
	public bool canInteract;

	public RoomMachine myRoom;
	public GameObject mainUI;
	public GameObject loadingUI;
	public float maxDistance = 10f;
	public static float playerHeight = 1.2f;
	public float canvasDistance = 3f;
	public float canvasWidth = 3f;
	public float holdingTimeSwipeNegation = 0.3f;
	public float waitToSpawnUI = 1;
	public static InputMachine instance;
	public InputMachine swipeUp, swipeForward, swipeBack, swipeDown;
	public List<GameObject> gos;
	public List<RoomMachine> rooms;
	public List<Transform> repositionWithMoves;

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
		Right.Initiate ();
		Left.Initiate ();
		Headset.Initiate ();
		StartCoroutine ("InitialCheckObjects");
	}

	public IEnumerator InitialCheckObjects(){
		yield return new WaitForSeconds(0.5f);
		CheckObjects ();
		CheckObjects ();
	}
	public virtual void Tap(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public virtual void Release(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public virtual void CheckInteract(GameObject obj, Vector3 point, StateMachine checkMachine){}
	public override void InstanceUpdate(StateMachine checkMachine){
		if(Input.GetKeyDown(KeyCode.A)){
			PlayerMachine.playerObject.transform.Rotate (0, -45, 0);
		}
		if(Input.GetKeyDown(KeyCode.D)){
			PlayerMachine.playerObject.transform.Rotate (0, 45, 0);
		}
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
			Headset.blackout.SetActive (false);
		} else {
			foreach (GameObject go in gos) {
				go.SetActive (false);
			}
			Headset.blackout.SetActive (true);
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
}