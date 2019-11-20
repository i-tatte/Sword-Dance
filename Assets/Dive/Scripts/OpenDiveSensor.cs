//Durovis Dive Head Tracking 
//copyright by Shoogee GmbH & Co. KG Refer to LICENCE.txt 
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class OpenDiveSensor : MonoBehaviour
{
	
	//if true, rotation of a GameObject will be added
	//for example tilting the camera while going along a racetrack or rollercoaster
	public bool AddRotationGameobject = false;
	//e.g.: the head of the player, or a waggon of a rollercoaster
	public GameObject RotationGameobject;
	//Texture to display when no gyro is found
	public Texture NoGyroTexture;
	//if true, yaw will be rotated to 0 of a scene (e.g. when you load another scene)
	public bool correctCenterTransition = false;
	//script to learn about a device's default orientation, needed for axis correction on some tablets
	public NaturalOrientation no;

	private bool mbShowErrorMessage, mbUseGyro;
	private float q0, q1, q2, q3;
	private float m0, m1, m2;
	private float magnet_value = 0;
	private int magnet_trigger = 0;
	private int magnet_detected;
	private Quaternion rot;
	private Quaternion centerTransition = Quaternion.identity;

	private int is_tablet;

#if UNITY_EDITOR
#elif UNITY_ANDROID
	private static AndroidJavaObject javadiveplugininstance;

	[DllImport("divesensor")]
	private static extern int dive_set_path(string path);

	[DllImport("divesensor")]
	private static extern void initialize_sensors();

	[DllImport("divesensor")]
	private static extern int get_q(ref float q0, ref float q1, ref float q2, ref float q3);

	[DllImport("divesensor")]
	private static extern int process();

	[DllImport("divesensor")]
	private static extern void set_application_name(string name);
	
	[DllImport("divesensor")]
	private static extern int get_magnet(ref int detected,ref int t1,ref float t2);

	[DllImport("divesensor")]
	private static extern int get_m(ref float m0,ref float m1,ref float m2);

	[DllImport("divesensor")]
	private static extern void use_udp(int switchon);

	[DllImport("divesensor")]
	private static extern void get_version(string msg, int maxlength);

	[DllImport("divesensor")]
	private static extern int get_error();
	
	[DllImport("divesensor")]   
	private static extern void dive_command(string command);

#elif UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void initialize_sensors();

	[DllImport("__Internal")]
	private static extern void stop_sensors();

	[DllImport("__Internal")]	
	private static extern float get_q0();

	[DllImport("__Internal")]	
	private static extern float get_q1();

	[DllImport("__Internal")]	
	private static extern float get_q2();

	[DllImport("__Internal")]	
	private static extern float get_q3();

	[DllImport("__Internal")]	
	private static extern void DiveUpdateGyroData();

	[DllImport("__Internal")]	
	private static extern int get_q(ref float q0,ref float q1,ref float q2,ref float q3);

	[DllImport("__Internal")]	
	private static extern int get_magnet(ref int detected,ref int t1,ref float t2);

	[DllImport("__Internal")]	
	private static extern int get_m(ref float m0,ref float m1,ref float m2);
#endif
	void Start()
	{
		mbShowErrorMessage = true;
		mbUseGyro = false;

		//load some settings from PlayerPrefs
		DInput.load ();

		rot = Quaternion.identity;

		//Disable screen dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//set Frame Rate hint to 60 FPS
		Application.targetFrameRate = 60;
#if UNITY_EDITOR
#elif UNITY_ANDROID
		// Java part
		DiveJava.init ();
		dive_set_path(Application.persistentDataPath);
		Network.logLevel = NetworkLogLevel.Full;
		use_udp(1);
		initialize_sensors();
		int err = get_error();
		if(err == 0)
		{
			mbShowErrorMessage = false;
			mbUseGyro = true;
			if(correctCenterTransition){
				get_q(ref q0, ref q1, ref q2, ref q3);
				rot.x = -q2;
				rot.y = q3;
				rot.z = -q1;
				rot.w = q0;
				Quaternion temp = Quaternion.identity;
				temp.eulerAngles = new Vector3(0,rot.eulerAngles.y,0);
				this.centerTransition = Quaternion.identity * Quaternion.Inverse(temp);
			}
	
			if (no.GetDeviceDefaultOrientation() == NaturalOrientation.LANDSCAPE){
				is_tablet=1;
				Debug.Log("Dive Unity Tablet Mode activated");
			}
			else{
				Debug.Log("Dive Phone Mode activated");
			}
		}
		else
		{
			mbShowErrorMessage = true;
			mbUseGyro = false;
		}
#elif UNITY_IPHONE
		initialize_sensors();
		mbShowErrorMessage = false;
		mbUseGyro = true;
#endif
	}

	//Eventhandler for Magnet Trigger; example usage:
	//private void myFunction(object sender, EventArgs e){...}
	//this.MagnetTriggered += myFunction;
	public event EventHandler MagnetTriggered;

	protected virtual void OnMagnetTriggered()
	{
		if(MagnetTriggered != null)
			MagnetTriggered(this, EventArgs.Empty);
	}
	
	private bool mbMagnetDown = false;
	
	void Update()
	{
#if UNITY_EDITOR
#elif UNITY_ANDROID
		process();
		get_q(ref q0, ref q1, ref q2, ref q3);
		rot.x = -q2;
		rot.y = q3;
		rot.z = -q1;
		rot.w = q0;

		get_magnet(ref magnet_detected,ref magnet_trigger,ref magnet_value);
#elif UNITY_IPHONE
		DiveUpdateGyroData();
		get_q(ref q0,ref q1,ref q2,ref q3);
		rot.x=-q2;
		rot.y=q3;
		rot.z=-q1;
		rot.w=q0;

		get_magnet(ref magnet_detected,ref magnet_trigger,ref magnet_value);
		get_m(ref m0,ref m1,ref m2);
#endif

		if(mbUseGyro)
		if(Time.timeSinceLevelLoad > 0.1f)
			if(correctCenterTransition)
			{
				if(AddRotationGameobject){
					if (is_tablet==1){
						transform.rotation = RotationGameobject.transform.rotation * (centerTransition * rot)* Quaternion.AngleAxis(90,Vector3.forward);
					}else{
						transform.rotation = RotationGameobject.transform.rotation * (centerTransition * rot);
					}
				}else{
					if (is_tablet==1){
						transform.rotation = centerTransition * rot * Quaternion.AngleAxis(90,Vector3.forward);
					}else{
						transform.rotation = centerTransition * rot;
					}
				}
			}
		else
		{
			if(AddRotationGameobject)
			if (is_tablet==1){
				transform.rotation= RotationGameobject.transform.rotation * rot * Quaternion.AngleAxis(90,Vector3.forward);
			} else transform.rotation = RotationGameobject.transform.rotation * rot;
			else if (is_tablet==1){
				transform.rotation= rot * Quaternion.AngleAxis(90,Vector3.forward);
			} else transform.rotation = rot;
		}


		if(DInput.magnet_trigger != magnet_trigger && magnet_trigger == 1)
			DInput.magnet_trigger_posedge = true;
		else
			DInput.magnet_trigger_posedge = false;
		if(DInput.magnet_trigger != magnet_trigger && magnet_trigger == 0)
			DInput.magnet_trigger_negedge = true;
		else
			DInput.magnet_trigger_negedge = false;
		DInput.magnet_trigger = magnet_trigger;

		if(DInput.use_analog_value)
		{
			DInput.magnet_value = magnet_value;
			if(DInput.invert)
				DInput.magnet_value = 1 - magnet_value;
		}

		{
			if(!mbMagnetDown)
			{
#if UNITY_EDITOR
				if(Input.GetKey(KeyCode.Space) || Input.GetMouseButtonDown(0))
#else
				if(DInput.magnet_trigger == 1 || Input.GetMouseButtonDown(0))
#endif
				{
					mbMagnetDown = true;
				}
			}
			else
			{
#if UNITY_EDITOR
				if(!Input.GetKey(KeyCode.Space))
#else
				if(DInput.magnet_trigger != 1)
#endif
				{
					OnMagnetTriggered();
					mbMagnetDown = false;
				}
			}
		}
	}

	void OnGUI()
	{
		if(mbShowErrorMessage){
			if(GUI.Button(new Rect(0, 0, Screen.width, Screen.height), "button"))
				mbShowErrorMessage = false;

			if(NoGyroTexture != null){
				int liHeight = (int)(Screen.height * 0.9);
				GUI.DrawTexture(new Rect((Screen.width - liHeight) / 2, (Screen.height - liHeight) / 2, liHeight, liHeight), NoGyroTexture, ScaleMode.StretchToFill, true, 0);
			}
		}
	}

	void OnApplicationQuit(){
#if UNITY_EDITOR
#elif UNITY_IOS
		stop_sensors();
#endif
	}

}

public static class DInput
{
	public static bool use_cardboard_trigger = true;
	public static bool use_analog_value = false;
	public static bool magnet_detected = false;
	public static int magnet_trigger = 0;
	public static bool magnet_trigger_posedge = false;
	public static bool magnet_trigger_negedge = false;
	public static float magnet_value = 0.0f;
	public static bool invert = false;
	public static bool use_IPD_Correction;
	public static float IPDCorrectionValue = 0;
	
	public static void save()
	{
		PlayerPrefs.SetInt("dive_use_cardboard_trigger", (use_cardboard_trigger ? 1 : 0));
		PlayerPrefs.SetInt("dive_use_cardboard_analog_value", (use_analog_value ? 1 : 0));
		PlayerPrefs.SetInt("dive_invert_axis", (invert ? 1 : 0));
		PlayerPrefs.SetInt("dive_use_ipd_correction", (use_IPD_Correction ? 1 : 0));
		PlayerPrefs.SetFloat("dive_ipd_correction_value", IPDCorrectionValue);
	}
	
	public static void load()
	{
		use_cardboard_trigger = (PlayerPrefs.GetInt("dive_use_cardboard_trigger") != 0);
		use_analog_value = (PlayerPrefs.GetInt("dive_use_cardboard_analog_value") != 0);
		invert = (PlayerPrefs.GetInt("dive_invert_axis") != 0);
		use_IPD_Correction = (PlayerPrefs.GetInt("dive_use_ipd_correction") != 0);
		IPDCorrectionValue = (PlayerPrefs.GetFloat("dive_ipd_correction_value"));
	}
}
