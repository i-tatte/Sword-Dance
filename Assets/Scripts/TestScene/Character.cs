using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
	bool w, a, s, d;
	public float speed = 2.5f;
	void Start () { }

	// Update is called once per frame
	void Update () {
		Move ();
	}

	void Move () {
		if (true) {
			if (Input.GetKeyDown (KeyCode.W)) {
				w = true;
			}
			if (Input.GetKeyUp (KeyCode.W)) {
				w = false;
			}
			if (Input.GetKeyDown (KeyCode.A)) {
				a = true;
			}
			if (Input.GetKeyUp (KeyCode.A)) {
				a = false;
			}
			if (Input.GetKeyDown (KeyCode.S)) {
				s = true;
			}
			if (Input.GetKeyUp (KeyCode.S)) {
				s = false;
			}
			if (Input.GetKeyDown (KeyCode.D)) {
				d = true;
			}
			if (Input.GetKeyUp (KeyCode.D)) {
				d = false;
			}
		}

		if (w) {
			this.transform.position += this.transform.forward * Time.deltaTime * speed;
		}
		if (a) {
			this.transform.position -= this.transform.right * Time.deltaTime * speed;
		}
		if (s) {
			this.transform.position -= this.transform.forward * Time.deltaTime * speed;
		}
		if (d) {
			this.transform.position += this.transform.right * Time.deltaTime * speed;
		}
	}
}