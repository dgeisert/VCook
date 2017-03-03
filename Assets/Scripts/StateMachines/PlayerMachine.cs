using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMachine : MonoBehaviour {

	public static GameObject playerObject;
	public static PlayerMachine instance;
	public Transform headset, left, right;
	private Dictionary<string, int> resources;
	private Dictionary<string, GameObject> loadedResources;
	public GameObject timerObject;

	public void Start(){
		Init ();
	}

	public void Init(){
		PlayerMachine.playerObject = gameObject;
		PlayerMachine.instance = this;
		DontDestroyOnLoad (gameObject);
		if (ES2.Exists ("resources")) {
			resources = ES2.LoadDictionary<string, int> ("resources");
		} else {
			resources = new Dictionary<string, int> ();
		}
		Object[] objs = Resources.LoadAll ("");
		loadedResources = new Dictionary<string, GameObject> ();
		foreach (Object obj in objs) {
			if (obj.name.Substring (0, 3) == "obj") {
				loadedResources.Add (obj.name, (GameObject)obj);
			}
		}
		/*
		if (ES2.Exists ("playerLocation")) {
			Transform tr = ES2.Load<Transform> ("playerLocation");
			transform.position = tr.position;
			transform.localScale = tr.localScale;
			transform.localRotation = tr.localRotation;
			Destroy (tr.gameObject);
		} else {
			transform.position = AreaStartStateMachine.instance.transform.position;
			transform.rotation = AreaStartStateMachine.instance.transform.rotation;
		}*/
	}

	public int GetResource(string resource){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		return resources [resource];
	}
	private void CreateResource(string resource){
		resources.Add (resource, 0);
	}
	public void SetResource(string resource, int count){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		resources [resource] = count;
		SaveResources ();
	}
	public void AddResource(string resource, int increment){
		resource = resource.ToLower ();
		if(!resources.ContainsKey(resource)) {
			CreateResource (resource);
		}
		resources [resource] += increment;
		SaveResources ();
	}
	public void SavePlayerPosition(){
		ES2.Save (transform, "playerLocation");
	}
	public void SaveResources(){
		SavePlayerPosition ();
		ES2.Save(resources, "resources");
	}

	public ItemMachine CreateItem(GameObject baseItem, Vector3 position, Quaternion rotation, bool isLocal = false, Transform parent = null, string SetID = ""){
		if (NetworkManager.instance.IsHost ()) {
			GameObject go = (GameObject)GameObject.Instantiate (baseItem);
			ItemMachine im = go.GetComponent<ItemMachine> ();
			if (SetID != "") {
				if (!NetworkManager.instance.allObjects.ContainsKey (SetID)) {
					im.SetID (SetID);
				}
			} else {
				im.Init ();
			}
			Transform t = im.transform;
			t.SetParent (parent);
			OtherPlayerObject opo = parent.GetComponentInParent<OtherPlayerObject> ();
			if (opo != null) {
				im.Grabbed (parent.gameObject);
			} else {
				VRTK.VRTK_TrackedController vrtkTC = parent.GetComponentInParent<VRTK.VRTK_TrackedController> ();
				if (vrtkTC != null) {
					im.Grabbed (vrtkTC.gameObject);
				}
			}
			if (isLocal) {
				t.localPosition = position;
				t.localRotation = rotation;
			} else {
				t.position = position;
				t.rotation = rotation;
			}
			NetworkManager.instance.SendInstantiateObject (im);
			return im;
		}
		return null;
	}
}
