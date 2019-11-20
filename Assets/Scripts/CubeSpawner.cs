using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour {
	public GameObject Cube;
	public int Delay = 2;
	float now = 0;

	// Start is called before the first frame update
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		now += Time.deltaTime;
		if (now > Delay) {
			now %= Delay;
			Summon ();
		}
	}

	void Summon () {
		GameObject SummonedCube = Instantiate (Cube, this.transform.position, this.transform.rotation) as GameObject;
		SummonedCube.GetComponent<Rigidbody> ().velocity = new Vector3 (Random.value - 0.5f, Random.value + 4.5f, Random.value - 0.5f);
	}
}
