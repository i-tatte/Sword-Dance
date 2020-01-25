using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeAnchor : MonoBehaviour {
	// Start is called before the first frame update
	public GameObject Blade;
	void Start () { }

	// Update is called once per frame
	void Update () {
		this.transform.position = Blade.transform.position;
		this.transform.rotation = Blade.transform.rotation;
	}
}
