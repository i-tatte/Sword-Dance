using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAccelerator : MonoBehaviour {
	// Start is called before the first frame update
	/// <summary>
	/// ブレードとボールの質量比。
	/// </summary>
	public float mass = 10;
	void Start () { }

	// Update is called once per frame
	void Update () { }

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Blade") {
			PhotonSwordController attacker = other.transform.parent.parent.GetComponent<PhotonSwordController> ();
			// if (Vector3.Magnitude (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) > 1) {
			// 	Vector3 outerProduct = Utility.OuterProduct (attacker.velocity + this.GetComponent<Rigidbody> ().velocity, attacker.direction);
			// 	GameObject fragment = Instantiate (Fragment, this.transform.position, Quaternion.LookRotation (outerProduct)) as GameObject;
			// 	GameObject fragment1 = fragment.transform.GetChild (0).gameObject;
			// 	GameObject fragment2 = fragment.transform.GetChild (1).gameObject;
			// 	fragment1.transform.rotation = Quaternion.LookRotation (outerProduct);
			// 	fragment2.transform.rotation = Quaternion.LookRotation (outerProduct);
			// 	fragment1.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (outerProduct + Utility.VectorRoot (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
			// 		ForceMode.Impulse);
			// 	fragment2.GetComponent<Rigidbody> ().AddForce (Utility.VectorRoot (-outerProduct + Utility.VectorRoot (attacker.velocity + this.GetComponent<Rigidbody> ().velocity) + this.gameObject.GetComponent<Rigidbody> ().velocity),
			// 		ForceMode.Impulse);

			// 	Destroy (this.gameObject);
			// }
			Vector3 sum = (attacker.velocity) * mass + this.GetComponent<Rigidbody> ().velocity;
			this.GetComponent<Rigidbody> ().AddForce (sum, ForceMode.Impulse);
		}
	}
}
