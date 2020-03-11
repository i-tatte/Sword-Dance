using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCubeDestroyer : MonoBehaviour {
	public GameObject Fragment;
	public AudioClip audioClip;
	OVRHapticsClip hapticsClip;
	public double HitPoint = 1;
	public double damage = 0.05;
	public Vector3 velocity;
	void Start () {
		hapticsClip = new OVRHapticsClip (audioClip);
		velocity = this.GetComponent<Rigidbody> ().velocity;
	}

	void Update () {

	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Blade") {
			PhotonSwordController attacker = other.transform.parent.parent.GetComponent<PhotonSwordController> ();
			if (Vector3.Magnitude (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) > 0) {
				if (attacker.isEquipedOnRightHand) {
					if (GVContainer.IsRightSwordActive) {
						DestroyTarget (other, attacker);
					}
				} else if (GVContainer.IsLeftSwordActive) {
					DestroyTarget (other, attacker);
				}
			}
		} else if (other.gameObject.tag == "Bullet") {
			Destroy (other.gameObject);
			this.GetComponent<Rigidbody> ().AddForce (velocity, ForceMode.VelocityChange);
			HitPoint -= damage;
			GetComponent<Renderer> ().material.color = new Color ((int) (1 - HitPoint) * 255, 0, 0, 1);
		}
	}

	void OnTriggerStay (Collider other) { }

	void DestroyTarget (Collision other, PhotonSwordController attacker) {
		Vector3 outerProduct = Utility.OuterProduct (attacker.velocity + this.GetComponent<Rigidbody> ().velocity, attacker.direction);
		//MeshCut.Cut (this.gameObject, new Vector3 (0, 0, 0), Vector3.Normalize (outerProduct), this.gameObject.GetComponent<Renderer> ().material);
		// GameObject fragment = Instantiate (Fragment, this.transform.position, Quaternion.LookRotation (outerProduct)) as GameObject;
		// GameObject fragment1 = fragment.transform.GetChild (0).gameObject;
		// GameObject fragment2 = fragment.transform.GetChild (1).gameObject;
		// fragment1.transform.rotation = Quaternion.LookRotation (outerProduct);
		// fragment2.transform.rotation = Quaternion.LookRotation (outerProduct);
		// fragment1.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (outerProduct + Utility.VectorRoot (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
		// 	ForceMode.Impulse);
		// fragment2.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (-outerProduct + Utility.VectorRoot (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
		// 	ForceMode.Impulse);

		// if (other.transform.parent.parent.GetComponent<PhotonSwordController> ().isEquipedOnRightHand) {
		// 	OVRHaptics.RightChannel.Mix (hapticsClip);
		// } else {
		// 	OVRHaptics.LeftChannel.Mix (hapticsClip);
		// }

		// Destroy (this.gameObject);
	}
}
