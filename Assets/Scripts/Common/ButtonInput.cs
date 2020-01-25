using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInput : MonoBehaviour {
	// Start is called before the first frame update
	void Start () { }

	// Update is called once per frame
	void Update () {
		GVContainer.ButtonADown = OVRInput.GetDown (OVRInput.RawButton.A);
		GVContainer.ButtonBDown = OVRInput.GetDown (OVRInput.RawButton.B);
		GVContainer.ButtonXDown = OVRInput.GetDown (OVRInput.RawButton.X);
		GVContainer.ButtonYDown = OVRInput.GetDown (OVRInput.RawButton.Y);

		GVContainer.ButtonA = OVRInput.Get (OVRInput.RawButton.A);
		GVContainer.ButtonB = OVRInput.Get (OVRInput.RawButton.B);
		GVContainer.ButtonX = OVRInput.Get (OVRInput.RawButton.X);
		GVContainer.ButtonY = OVRInput.Get (OVRInput.RawButton.Y);

		GVContainer.RightTrigger = OVRInput.Get (OVRInput.RawButton.RIndexTrigger);
		GVContainer.RightTriggerDown = OVRInput.GetDown (OVRInput.RawButton.RIndexTrigger);
		GVContainer.LeftTrigger = OVRInput.Get (OVRInput.RawButton.LIndexTrigger);
		GVContainer.LeftTriggerDown = OVRInput.GetDown (OVRInput.RawButton.LIndexTrigger);

		if (OVRInput.Get (OVRInput.RawButton.RIndexTrigger)) {
			Debug.Log ("にゃーん");
		}
	}
}
