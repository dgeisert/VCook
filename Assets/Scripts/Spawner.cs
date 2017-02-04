using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject spawnPrefab;
	public float spread = 100;
	public int spawns = 100;
	public bool randomDirection = true;
	public Vector2 scalingBounds = Vector2.one;

	// Use this for initialization
	void Start () {
		StartCoroutine ("Spawn");
	}
	public IEnumerator Spawn(){
		yield return null;
		for (int i = 0; i < spawns; i++) {
			Vector3 newPos = transform.position + new Vector3((Random.value - 0.5f) * spread, 0.5f, (Random.value - 0.5f) * spread);
			GameObject go = (GameObject)GameObject.Instantiate (spawnPrefab, newPos, Quaternion.identity);
			go.transform.SetParent (transform.parent);
			if (randomDirection) {
				go.transform.localEulerAngles += new Vector3 (0, Random.value * 360, 0);
			}
			go.transform.localScale *= (Random.value * (scalingBounds.y - scalingBounds.x)) + scalingBounds.x;
			go.name = spawnPrefab.name;
			StateMachine sm = go.GetComponent<StateMachine> ();
			if (sm != null) {
				sm.Initiate ();
			}
			InputMachine.instance.gos.Add (go);
			InputMachine.instance.SetObjectParent (go.transform, true, sm);
		}
		InputMachine.instance.CheckObjects ();
		Destroy (gameObject);
	}
}
