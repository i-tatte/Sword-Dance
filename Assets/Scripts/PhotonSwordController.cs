using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSwordController : MonoBehaviour {
	public GameObject LeftHandTracker;
	public GameObject RightHandTracker;
	public bool isEquipedOnRightHand;
	public Vector3 velocity;
	public Vector3 direction;
	Vector3 latestPos;
	//public GameObject PhotonSword;
	// Start is called before the first frame update
	void Start () { }

	// Update is called once per frame
	void Update () {

		Move ();

		VectorCalculation ();

	}

	void Move () {
		if (isEquipedOnRightHand) {
			transform.position = RightHandTracker.transform.position + new Vector3 (0, 1f, 0);
			transform.rotation = RightHandTracker.transform.rotation;
			transform.Rotate (48, 0, 0);
		} else {
			transform.position = LeftHandTracker.transform.position + new Vector3 (0, 1f, 0);
			transform.rotation = LeftHandTracker.transform.rotation;
			transform.Rotate (48, 0, 0);
		}
	}

	void VectorCalculation () {
		Transform blade = this.gameObject.transform.GetChild (2).GetChild (0); //PhotonSword.Blade.Cylinder
		if (!isEquipedOnRightHand) {
			GVContainer.LeftSwordDirection = transform.up;
			GVContainer.LeftSwordVelocity = ((blade.position - latestPos) / Time.deltaTime);
		} else {
			GVContainer.RightSwordDirection = transform.up;
			GVContainer.RightSwordVelocity = ((blade.position - latestPos) / Time.deltaTime);
		}
		direction = transform.up;
		velocity = ((blade.position - latestPos) / Time.deltaTime);
		latestPos = blade.position;
	}
}
