using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour {
	public AudioClip audioClip;
	OVRHapticsClip hapticsClip;
	void Start () {
		hapticsClip = new OVRHapticsClip (audioClip);
	}
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Fragment" || other.tag == "Cube") {
			var meshCut = other.gameObject.GetComponent<MeshCut> ();
			var controller = this.transform.parent.parent.GetComponent<PhotonSwordController> ();
			if (meshCut == null) { return; }
			var cutPlane = new Plane (Vector3.Cross (controller.direction, controller.velocity).normalized, transform.position);
			meshCut.Cut (cutPlane);
			if (this.transform.parent.parent.GetComponent<PhotonSwordController> ().isEquipedOnRightHand) {
				OVRHaptics.RightChannel.Mix (hapticsClip);
			} else {
				OVRHaptics.LeftChannel.Mix (hapticsClip);
			}
		}
	}

}