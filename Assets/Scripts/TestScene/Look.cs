using UnityEngine;

public class Look : MonoBehaviour {

	public Transform verRot;
	public Transform horRot;
	public float Sensibility = 1.0f;

	// Use this for initialization
	void Start () {

		verRot = transform.parent;
		horRot = GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () {
		float X_Rotation = -Input.GetAxis ("Mouse X") * Sensibility;
		float Y_Rotation = -Input.GetAxis ("Mouse Y") * Sensibility;
		verRot.transform.Rotate (0, -X_Rotation, 0);
		horRot.transform.Rotate (Y_Rotation, 0, 0);
	}
}