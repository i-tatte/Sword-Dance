using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotVRCutter : MonoBehaviour {
	void Start () { }
	void OnTriggerEnter (Collider other) {
		if (other.tag == "Fragment" || other.tag == "Cube") {
			var meshCut = other.gameObject.GetComponent<MeshCut> ();
			var controller = this.transform.parent.parent.GetComponent<NotVRPSC> ();
			if (meshCut == null) { return; }
			var cutPlane = new Plane (Vector3.Cross (controller.direction, controller.velocity).normalized, transform.position);
			meshCut.Cut (cutPlane);
			Debug.Log (controller.direction);
			Debug.Log (controller.velocity);
		}
	}

}