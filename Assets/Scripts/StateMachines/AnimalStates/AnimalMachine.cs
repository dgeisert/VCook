using UnityEngine;
using System.Collections;

public class AnimalMachine : StateMachine {

	public Vector3 targetLocation;
	public float speed = 2f, jumpSpeed = 0.06f, gravity = 0.2f, yVelocity = 0f, runAwayDistance = 30f;
	public CharacterController cc;
	public bool is_crying;
	public Animator anim;

	public override void InstanceInitiate(StateMachine checkMachine){
		if (is_crying) {
			UpdateState (StateMaster.instance.animalCrying, this);
		} else {
			UpdateState (StateMaster.instance.animalRoaming, this);
		}
		timer.StartTimer ();
	}

	void Start(){
		if (is_crying) {
			Initiate ();
		}
	}

	public override void InstanceInteract(GameObject obj, Vector3 point, StateMachine checkMachine){
		currentState.InstanceInteract (obj, point, this);
	}

	public override void InstanceUpdate(StateMachine checkMachine){
		anim.SetFloat ("YVelocity", cc.velocity.y);
		anim.SetFloat ("XZVelocity", Mathf.Abs (cc.velocity.x) + Mathf.Abs (cc.velocity.z));
		anim.SetBool ("IsGrounded", cc.isGrounded);
		if (yVelocity > 0 || !cc.isGrounded) {
			cc.Move (transform.up * yVelocity);
		}
		yVelocity -= gravity * Time.deltaTime;
		if (transform.position.y < 0) {
			transform.position = new Vector3 (transform.position.x, 1, transform.position.z);
			yVelocity = 0;
		}
		/*
		for (int i = tile.childCount - 1; i >= 0; i--) {
			Transform child = tile.GetChild (i);
			if (child.position.x > tile.position.x + squareScale) {
				if (kvp.Key.x + 1 == layout.x) {
					transform.SetParent (ground [new Vector2 (0, kvp.Key.y)]);
				} else {
					transform.SetParent (ground [new Vector2 (kvp.Key.x + 1, kvp.Key.y)]);
				}
			}
			if (child.position.y > tile.position.y + squareScale) {
				if (kvp.Key.y + 1 == layout.x) {
					transform.SetParent (ground [new Vector2 (kvp.Key.x, 0)]);
				} else {
					transform.SetParent (ground [new Vector2 (kvp.Key.x, kvp.Key.y + 1)]);
				}
			}
			if (child.position.x < tile.position.x + squareScale) {
				if (kvp.Key.x == 0) {
					transform.SetParent (ground [new Vector2 (layout.x - 1, kvp.Key.y)]);
				} else {
					transform.SetParent (ground [new Vector2 (kvp.Key.x - 1, kvp.Key.y)]);
				}
			}
			if (child.position.y < tile.position.y + squareScale) {
				if (kvp.Key.y == 0) {
					transform.SetParent (ground [new Vector2 (kvp.Key.x, layout.y - 1)]);
				} else {
					transform.SetParent (ground [new Vector2 (kvp.Key.x, kvp.Key.y - 1)]);
				}
			}
		}*/
		currentState.InstanceUpdate (this);
	}

	public void SetTarget(Vector3 target){
		targetLocation = target;
		transform.LookAt (new Vector3(targetLocation.x, transform.position.y, targetLocation.z));
	}

	public void SetTarget(GameObject target){
		targetLocation = target.transform.position;
		transform.LookAt (new Vector3(targetLocation.x, transform.position.y, targetLocation.z));
	}

	public void LookMove(){
		transform.LookAt (new Vector3(targetLocation.x, transform.position.y, targetLocation.z));
		cc.SimpleMove(new Vector3 ((targetLocation - transform.position).normalized.x * speed, yVelocity, (targetLocation - transform.position).normalized.z * speed));
	}
}
