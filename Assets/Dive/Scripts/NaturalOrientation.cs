using UnityEngine;
using System.Collections;

public class NaturalOrientation : MonoBehaviour {
	
	public static int ORIENTATION_UNDEFINED = 0x00000000;
	public static int ORIENTATION_PORTRAIT = 0x00000001;
	public static int ORIENTATION_LANDSCAPE = 0x00000002;
	
	public static int ROTATION_0 	= 0x00000000;
	public static int ROTATION_180 	= 0x00000002;
	public static int ROTATION_270 	= 0x00000003;
	public static int ROTATION_90 	= 0x00000001;
	
	public static int PORTRAIT = 0;
	public static int PORTRAIT_UPSIDEDOWN = 1;
	public static int LANDSCAPE = 2;
	public static int LANDSCAPE_LEFT = 3;
	
#if UNITY_ANDROID && !UNITY_EDITOR

    AndroidJavaObject mConfig;
	AndroidJavaObject mWindowManager;

	private static float density;
	private static int densitydpi;
	private static int vpixels;
	private static int hpixels;
	private static float scaledDensity;
	private static float ydpi;
	private static float xdpi;
	private static float xmm;
	private static float ymm;
	private static float mmdist;
	private static float correction_factor = 0.0f;

	private static AndroidJavaObject windowManagerInstance;
	private static AndroidJavaObject displayInstance;
	private static AndroidJavaObject metricsClass;
	private static AndroidJavaObject metricsInstance;

	// Use this for initialization
	void Start () {
		AndroidJavaClass majcUnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject majoDivePluginInstance = majcUnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
		metricsClass= new AndroidJavaClass("android.util.DisplayMetrics");
		metricsInstance= new AndroidJavaObject("android.util.DisplayMetrics");
		windowManagerInstance=majoDivePluginInstance.Call<AndroidJavaObject>("getWindowManager");
		displayInstance=windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay");
		
		displayInstance.Call("getMetrics",metricsInstance);
		
		density=metricsInstance.Get<float>("density");
		densitydpi=metricsInstance.Get<int> ("densityDpi");
		vpixels=metricsInstance.Get<int>("heightPixels");
		vpixels=Screen.height;
		hpixels=metricsInstance.Get<int>("widthPixels");
		//hpixels=displayInstance.Call<int>("getWidth");
		hpixels=Screen.width;
		scaledDensity=metricsInstance.Get<float>("scaledDensity");
		xdpi=metricsInstance.Get<float>("xdpi");
		ydpi=metricsInstance.Get<float>("ydpi");
		xmm=hpixels/xdpi/0.0393701f;
		ymm=vpixels/ydpi/0.0393701f;
		
		
		hpixels=Screen.width;
		vpixels=Screen.height;
		xmm=hpixels/xdpi/0.0393701f;
		ymm=vpixels/ydpi/0.0393701f;
		mmdist=xmm/2;
	}
	
	// Update is called once per frame
	void Update () {
		correction_factor=0.002f*((mmdist-55.0f)/(76.0f-55.0f));
		if(OffsetCenter.instance !=null){
			OffsetCenter.instance.setCorrectionFactor(correction_factor);
		}
	}

	
	//adapted from http://stackoverflow.com/questions/4553650/how-to-check-device-natural-default-orientation-on-android-i-e-get-landscape/4555528#4555528
	public int GetDeviceDefaultOrientation()
	{
		if ((mWindowManager == null) || (mConfig == null))
		{
			using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").
							GetStatic<AndroidJavaObject>("currentActivity"))
			{
				mWindowManager = activity.Call<AndroidJavaObject>("getSystemService","window");
				mConfig = activity.Call<AndroidJavaObject>("getResources").Call<AndroidJavaObject>("getConfiguration");
			}
		}
		
		int lRotation = mWindowManager.Call<AndroidJavaObject>("getDefaultDisplay").Call<int>("getRotation");
		int dOrientation = mConfig.Get<int>("orientation");

		if( (((lRotation == ROTATION_0) || (lRotation == ROTATION_180)) && (dOrientation == ORIENTATION_LANDSCAPE)) ||
		(((lRotation == ROTATION_90) || (lRotation == ROTATION_270)) && (dOrientation == ORIENTATION_PORTRAIT)))
		{
  			return(LANDSCAPE); //TABLET
  		}     

  		return (PORTRAIT); //PHONE
	} 
	
#endif
}