using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentDestroyer : MonoBehaviour {
	// Start is called before the first frame update
	public int time = 10;
	float elapsedTime = 0;
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if (time < elapsedTime) {
			this.gameObject.SetActive (false);
		}
	}
}
