using UnityEngine;
using System.Collections;
using System;

public class Timer {

	public DateTime timerStart;
	public bool addativeTime = false;
	public float timerDuration = 1f;
	public TimerObject timerObject;
	public StateMachine sm;

	public Timer(StateMachine setSm){
		timerStart = DateTime.Now;
		sm = setSm;
	}
	public Timer(){
		timerStart = DateTime.Now;
	}

	public void StartTimer(float duration = -1, bool hasTimerObject = false, Transform parent = null, bool bar = true, bool numbers = true){
		if (duration != -1) {
			timerDuration = duration;
		}
		timerStart = DateTime.Now;
		if (hasTimerObject) {
			if (timerObject == null) {
				GameObject go = (GameObject) GameObject.Instantiate(StateMaster.instance.timer);
				timerObject = go.GetComponent<TimerObject> ();
				if (parent != null) {
					go.transform.SetParent (parent);
				}
				go.transform.localPosition = Vector3.zero;
				go.transform.localEulerAngles = Vector3.zero;
				timerObject.sm = sm;
				timerObject.timer = this;
				timerObject.StartTimer (timerDuration, bar, numbers);
			}
		}
	}
	public bool CheckTimer(bool immediate = false){
		if (immediate) {
			return ((DateTime.Now - timerStart).TotalSeconds > timerDuration) && ((DateTime.Now - timerStart).TotalSeconds < timerDuration + 0.2f);
		} else {
			return ((DateTime.Now - timerStart).TotalSeconds > timerDuration);
		}
	}
	public bool CheckTimer(float time){
		return (DateTime.Now - timerStart).TotalSeconds > timerDuration;
	}
	public bool TimerActive(){
		return (DateTime.Now - timerStart).TotalSeconds < timerDuration;
	}
	public float TimerPercent(){
		return Mathf.Min (Mathf.Max (((float)(DateTime.Now - timerStart).TotalSeconds)/timerDuration, 0), 1);
	}
	public float TimerRemaining(){
		return (float)(DateTime.Now - timerStart).TotalSeconds;
	}
}
