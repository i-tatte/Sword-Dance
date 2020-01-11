using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeDetector : MonoBehaviour {
	public GameObject HPText;
	// Start is called before the first frame update
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Cube") {
			GVContainer.WallHP--;
			Text text = HPText.GetComponent<Text> ();
			text.text = "HP : " + GVContainer.WallHP;
			if (GVContainer.WallHP <= 0) {
				text.text = "HP : 0  GAME OVER";
			}
		}
	}
}
