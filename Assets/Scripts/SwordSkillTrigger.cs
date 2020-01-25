using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillTrigger : MonoBehaviour {
	public AudioClip audioClip;
	OVRHapticsClip hapticsClip;
	// Start is called before the first frame update
	void Start () {
		hapticsClip = new OVRHapticsClip (audioClip);
	}

	// Update is called once per frame
	void Update () {
		// Aボタンで振動
		if (OVRInput.GetDown (OVRInput.RawButton.A)) {
			OVRHaptics.RightChannel.Mix (hapticsClip);
		}
	}
}
