using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetYAdjusterIK : MonoBehaviour {

	Vector3 targetPosition;
	Animator anim;
	Transform hand;
	Transform trans;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		hand = anim.GetBoneTransform (HumanBodyBones.Chest);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		targetPosition = Camera.main.ScreenPointToRay (Input.mousePosition).direction;
		hand.LookAt (targetPosition);
		//hand.Rotate (90, 90, 0);

	}
}
