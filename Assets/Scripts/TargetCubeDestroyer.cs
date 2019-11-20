using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCubeDestroyer : MonoBehaviour {
	public GameObject Fragment;
	// Start is called before the first frame update
	void Start () { }

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider other) {
		if (other.name == "Cylinder") {
			PhotonSwordController attacker = other.transform.parent.parent.GetComponent<PhotonSwordController> ();
			Vector3 outerProduct = Utility.OuterProduct (attacker.velocity, attacker.direction);
			GameObject fragment = Instantiate (Fragment, this.transform.position, Quaternion.LookRotation (outerProduct)) as GameObject;
			GameObject fragment1 = fragment.transform.GetChild (0).gameObject;
			GameObject fragment2 = fragment.transform.GetChild (1).gameObject;
			fragment1.transform.rotation = Quaternion.LookRotation (outerProduct);
			fragment2.transform.rotation = Quaternion.LookRotation (outerProduct);
			fragment1.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (outerProduct + Utility.VectorRoot (attacker.velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
				ForceMode.Impulse);
			fragment2.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (-outerProduct + Utility.VectorRoot (attacker.velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
				ForceMode.Impulse);

			Destroy (this.gameObject);
		}
	}

	void OnTriggerStay (Collider other) { }
}
