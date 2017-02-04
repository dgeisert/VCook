using UnityEngine;
using System.Collections;

public class TimerObject : MonoBehaviour {

	public TextMesh number;
	public Transform complete, incomplete;
	public StateMachine sm;
	public Timer timer;
	bool bar = false, numbers = false;

	public void SetTime(float setTime, float setPercent){
		if (bar) {
			complete.localPosition = new Vector3 (complete.localPosition.x, complete.localPosition.y, setPercent - 1);
			incomplete.localPosition = new Vector3 (incomplete.localPosition.x, incomplete.localPosition.y, setPercent);
			complete.localScale = new Vector3 (complete.localScale.x, setPercent, complete.localScale.z);
			incomplete.localScale = new Vector3 (incomplete.localScale.x, 1 - setPercent, incomplete.localScale.z);
		}
		if (numbers) {
			if (setTime > 3600) {

			} else if (setTime <= 0) {
				TurnOff ();
			} else {
				number.text = Mathf.CeilToInt (setTime).ToString ();
			}
		}
	}
	public void TurnOff(){
		Destroy (gameObject);
	}
	public void StartTimer(float setTime, bool useBar = false, bool useNumbers = false){
		bar = useBar;
		numbers = useNumbers;
		complete.gameObject.SetActive (bar);
		incomplete.gameObject.SetActive (bar);
		number.gameObject.SetActive (numbers);
		SetTime (setTime, 0);
	}

	public void Update(){
		if (timer != null) {
			SetTime (timer.TimerRemaining (), timer.TimerPercent ());
		}
	}
}
