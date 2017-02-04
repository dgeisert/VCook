using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Waypoints : MonoBehaviour {

	public UnityEvent toWaypoint, enterWaypoint, newWaypoint;
	public WaypointHolder wph;
	public bool is_atWaypoint = false;

	public void Start(){
		wph = GetComponentInParent<WaypointHolder> ();
	}

	public void Update(){
		if (!is_atWaypoint) {
			toWaypoint.Invoke ();
		}
	}

	public void OnTriggerEnter(Collider other){
		if (other.transform == wph.target.transform && !is_atWaypoint) {
			is_atWaypoint = true;
			enterWaypoint.Invoke ();
		}
	}
}
