using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

public class DiveJava : MonoBehaviour {

	private string cacheDir = "Push to get cache dir";
	private string startURI = "Push to get startURI";
	private static int start_once;
	public float time_since_last_fullscreen=0;

	private static DiveJava instance;

	private static bool initiated = false;

#if UNITY_ANDROID
	private static AndroidJavaClass javadivepluginclass;
	private static AndroidJavaClass javaunityplayerclass;
	private static AndroidJavaObject currentactivity;
	public static AndroidJavaObject javadiveplugininstance;
#endif

	void Start(){
		if (!initiated) {
			init ();
		}
	}



	public void Update(){
		if (start_once>0)start_once--;

		time_since_last_fullscreen+=Time.deltaTime;

		if (time_since_last_fullscreen >8){
			DiveJava.setFullscreen ();
			time_since_last_fullscreen=0;
		}
	}

		
	
	public static void setFullscreen(){
		#if UNITY_EDITOR
		
		#elif UNITY_ANDROID
		String answer;
		answer= javadiveplugininstance.Call<string>("setFullscreen");
		#elif UNITY_IPHONE
		
		#endif 	
	}

	public static void init(){
		start_once=0;
		#if UNITY_EDITOR
		
		#elif UNITY_ANDROID
		
		javadivepluginclass = new AndroidJavaClass("com.shoogee.divejava.divejava") ;
		javaunityplayerclass= new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		currentactivity = javaunityplayerclass.GetStatic<AndroidJavaObject>("currentActivity");
		javadiveplugininstance = javadivepluginclass.CallStatic<AndroidJavaObject>("instance");
		object[] args={currentactivity};
		javadiveplugininstance.Call<string>("set_activity",args);

		String answer;
		answer= javadiveplugininstance.Call<string>("setFullscreen");
		
		#elif UNITY_IPHONE
		
		#endif 	
		initiated = true;
	}

}
