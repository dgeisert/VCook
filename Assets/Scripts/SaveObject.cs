using UnityEngine;
using System.Collections;
using System;

public class SaveObject {

	public SaveObject(){}
	public SaveObject(StateMachine sm) {
		phase = sm.phase;
		if(sm.timer != null){
			timerStart = sm.timer.timerStart;
			timerDuration = sm.timer.timerDuration;
		}
		if(sm.secondaryTimer != null){
			secondaryTimerStart = sm.secondaryTimer.timerStart;
			secondaryTimerDuration = sm.secondaryTimer.timerDuration;
		}
		objName = sm.gameObject.name;
		parentGround = sm.parentGround;
		if (sm.currentState != null) {
			state = sm.currentState.GetType().ToString();
		} else {
			state = "";
		}
	}
	public int phase;
	public DateTime timerStart;
	public float timerDuration;
	public DateTime secondaryTimerStart;
	public float secondaryTimerDuration;
	public string objName;
	public string state;
	public Vector2 parentGround;
}
