using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : StateMachine { 

	GameObject currentSelected;
	public GameObject outline;
	GameObject currentOutline;
	public GameObject middleTop, middleBottom, leftTop, leftBottom, rightTop, rightBottom;
	public Image swipeUp, swipeDown, swipeBack, swipeForward;
	public static UIManager instance;
	public GameObject[] Buttons;

	public void Start(){
		Initiate ();
	}

	public override void InstanceInitiate(StateMachine checkMachine){
		middleTop.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		middleBottom.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		leftTop.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		leftBottom.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		rightTop.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		rightBottom.GetComponent<Canvas> ().worldCamera = InputMachine.instance.thisCamera;
		UIManager.instance = this;
	}

	public void CloseUI(){
		InputMachine.instance.UpdateState (InputMachine.instance.swipeForward, InputMachine.instance);
	}

	public void ReportName(string name){
		if (name.Length > 4) {
			if (name.Substring (0, 5) == "swipe") {
				switch (name) {
				case "swipeBack":
					SetBackwards ();
					break;
				case "swipeForward":
					SetForward ();
					break;
				case "swipeDown":
					SetDown ();
					break;
				default:
					break;
				}
			} else {
				SetSelected ();
			}
		} else {
			SetSelected ();
		}
	}
	void SetSelected(){
		if (currentOutline != null) {
			Destroy (currentOutline);
		}
		currentSelected = EventSystem.current.currentSelectedGameObject;
		currentOutline = (GameObject)GameObject.Instantiate (outline, currentSelected.transform.position, currentSelected.transform.rotation);
		currentOutline.transform.SetParent (currentSelected.transform);
		currentOutline.transform.localScale = 1.1f * Vector3.one;
	}

	public void SetForward(){
		if (GetSelected() == null) {
			return;
		}
		InputMachine.instance.swipeForward = GetSelected ();
		UIManager.instance.swipeForward.sprite = currentSelected.GetComponent<Image> ().sprite;
	}
	public void SetBackwards(){
		if (GetSelected() == null) {
			return;
		}
		InputMachine.instance.swipeBack = GetSelected ();
		UIManager.instance.swipeBack.sprite = currentSelected.GetComponent<Image> ().sprite;
	}
	public void SetDown(){
		if (GetSelected() == null) {
			return;
		}
		InputMachine.instance.swipeDown = GetSelected ();
		UIManager.instance.swipeDown.sprite = currentSelected.GetComponent<Image> ().sprite;
	}
	InputMachine GetSelected(){
		InputMachine newAction = null;
		if (currentSelected == null) {
			return newAction;
		}
		return newAction;
	}
}
