using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGunController : MonoBehaviour {
	public GameObject LeftHandTracker;
	public GameObject RightHandTracker;
	public GameObject Bullet;
	public GameObject ChargeMeter;
	public bool isEquipedOnRightHand;
	public float BulletSpeed = 299_792_458;
	public float ChargedAmount = 1.0f;
	public float charge, cost;
	public AudioClip AudioClip;
	OVRHapticsClip hapticsClip;
	void Start () {
		hapticsClip = new OVRHapticsClip (AudioClip);
	}

	void Update () {

		Move ();

		Charge ();

		if (OVRInput.Get (isEquipedOnRightHand?OVRInput.RawButton.RIndexTrigger : OVRInput.RawButton.LIndexTrigger)) {
			if (ChargedAmount >= cost) {
				Shoot ();
			}
		} else {
			ChargedAmount += charge;
			if (ChargedAmount > 1f) {
				ChargedAmount = 1f;
			}
		}

	}

	void Move () {
		if (isEquipedOnRightHand) {
			transform.position = RightHandTracker.transform.position + new Vector3 (0, 1f, 0);
			transform.rotation = RightHandTracker.transform.rotation;
		} else {
			transform.position = LeftHandTracker.transform.position + new Vector3 (0, 1f, 0);
			transform.rotation = LeftHandTracker.transform.rotation;
		}
	}
	void Shoot () {
		GameObject bullet = Instantiate (Bullet, transform.position + transform.rotation * new Vector3 (0, 0.058f, 0.2f), new Quaternion ()) as GameObject;
		bullet.GetComponent<Rigidbody> ().AddForce (transform.forward * BulletSpeed, ForceMode.VelocityChange);
		if (isEquipedOnRightHand) {
			OVRHaptics.RightChannel.Mix (hapticsClip);
		} else {
			OVRHaptics.LeftChannel.Mix (hapticsClip);
		}
		ChargedAmount -= cost;
	}

	void Charge () {
		ChargeMeter.transform.localPosition = new Vector3 (0, 0, (-0.001f - (0.5f * ChargedAmount)));
		ChargeMeter.transform.localScale = new Vector3 (1.001f, 1.001f, (1f - ChargedAmount));
		if (ChargedAmount == 1.0f) {
			ChargeMeter.transform.localScale = new Vector3 (0, 0, 0);
		}
	}
}
