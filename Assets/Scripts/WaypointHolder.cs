using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class WaypointHolder : MonoBehaviour {

	public GameObject target;
	int wp = 0;
	public Waypoints[] wps;
	public bool is_loop = false;

	public void Start(){
		for (int i = 0; i < wps.Length; i++) {
			wps [i].wph = this;
			if (wp == i) {
				SetWaypoint (wps [i]);
			} else {
				wps [i].gameObject.SetActive (false);
			}
		}
	}

	public void Next(){
		wps [wp].gameObject.SetActive (false);
		wp++;
		if (wp < wps.Length) {
			SetWaypoint (wps [wp]);
		} else if (is_loop) {
			wp = 0;
			SetWaypoint (wps [wp]);
		}
	}

	public void SetWaypoint(Waypoints waypoint){
		wps [wp].gameObject.SetActive (true);
		wps [wp].newWaypoint.Invoke ();
	}
}