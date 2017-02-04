using UnityEngine;
using System.Collections;

public class ReticleInstance : MonoBehaviour {

	public GameObject noHold, holdTrue, holdFalse;
	public GameObject current;
	public LineRenderer lineRenderer;
	public Transform timerHolder;

	public void SetNoHold(){
		if (current == noHold) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = noHold;
		current.SetActive (true);
	}
	public void SetHoldTrue(Vector3 fromPoint, Vector3 toPoint){
		if (lineRenderer != null) {
			RenderLine (fromPoint, toPoint);
		}
		if (current == holdTrue) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = holdTrue;
		current.SetActive (true);
	}
	public void SetHoldFalse(){
		if (current == holdFalse) {
			return;
		}
		if (current != null) {
			current.SetActive (false);
		}
		current = holdFalse;
		current.SetActive (true);
	}

	public void RenderLine(Vector3 fromPoint, Vector3 toPoint){  
		for (int i = 0; i < 20 ; i++){
			lineRenderer.SetPosition (i ,MakeLine(fromPoint, toPoint, i));
		}
	}
	private Vector3 MakeLine(Vector3 fromPoint, Vector3 toPoint, float t){
		return new Vector3(fromPoint.x - (fromPoint.x - toPoint.x) / 40 * t,(t * (20 - t))/50 + (10 - t)/8, fromPoint.z - (fromPoint.z - toPoint.z) / 40 * t);
	}
}
