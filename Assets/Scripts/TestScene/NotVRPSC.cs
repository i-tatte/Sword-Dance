using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotVRPSC : MonoBehaviour {
	public Vector3 velocity;
	public Vector3 direction;
	Vector3 latestPos;
	Animator anim;
	bool isActive = true;
	//public GameObject PhotonSword;
	// Start is called before the first frame update
	void Start () {
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {

		Move ();

		VectorCalculation ();

		Activate ();

	}

	void Move () { }

	void VectorCalculation () {
		Transform blade = this.gameObject.transform.GetChild (2).GetChild (0); //PhotonSword.Blade.Cylinder
		GVContainer.RightSwordDirection = transform.up;
		GVContainer.RightSwordVelocity = ((blade.position - latestPos) / Time.deltaTime);
		direction = transform.up;
		velocity = ((blade.position - latestPos) / Time.deltaTime);
		latestPos = blade.position;
	}

	void Activate () {
		isActive = GVContainer.IsRightSwordActive;
		if (isActive && Input.GetKeyDown (KeyCode.Q)) {
			//animationしてbladeしまう
			anim.SetBool ("isActive", false);
			isActive = false;
			GVContainer.IsRightSwordActive = false;
		} else if (!isActive && Input.GetKeyDown (KeyCode.Q)) {
			//animationしてblade出す
			anim.SetBool ("isActive", true);
			isActive = true;
			GVContainer.IsRightSwordActive = true;
		}
	}

}