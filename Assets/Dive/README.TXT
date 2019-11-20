Durovis Dive Tracking Technology Plugin for Android/iOS  www.durovis.com

This software may be used free of charge for Non-Commercial and Commercial purposes. 
Decompilation or reverse-engineering is explicitly prohibited. 

-== Contents of plugin package ==-
	Dive
		Prefabs
			Dive_Camera.prefab			Prefab including all you need to build a VR-App for Dive
		Scenes
			Dive-Splashscreen.unity		Optional Splashscreen showing the dive logo
		Scripts
			DiveJava.cs					Unity-Java Bridge for Dive, needed for immersive fullscreenmode on devices with softbuttons
			DiveMouseLook.cs			MouseLook Script for debugging your app in editor		
			OffsetCenter.cs				Script to calculate and set an offcenter projection on larger screens
			OpenDiveSensor.cs			Unity-Native Brigde / Rotational Head Tracking (Main Script)
			NaturalOrientation.cs		Fetches a device's default orientation on android
		Textures
			dive_splashscreen.png		Texture for optional Splashscreen
			nogyro.png					Texture for no gyroscope error message
	Plugins
		Android
			libs
				armeabi
					libdivesensor.so	native sensorfusion for android, arm architecture
				armeabi-v7a
					libdivesensor.so	native sensorfusion for android, arm v7 architecture
				mips
					libdivesensor.so	native sensorfusion for android, mips architecture
				x86
					libdivesensor.so	native sensorfusion for android, x86 architecture
			divejava.jar				Dive Java Bridge Library for android
		iOS
			libdivesensor.a				native sensorfusion for iOS, arm & arm64 architecture
	LICENSE.TXT							text file with licensing information
	README.TXT							this text file

-== Setup ==-
 
Just drag the Dive_Camera prefab onto your scene and you will get a Gameobject 
including two cameras (Camera Left, Camera Right) for stereo rendering, 
a script to rotate the Gameobject by Sensor Fusion, also
including magnet trigger functionality (OpenDiveSensor.cs), 
a script to automatically calculate the right projetion matrix 
for bigger screens like tablet displays (OffsetCenter.cs) and 
a script to recognize devices with landscape default orientation needed 
for axis corrention on some tablets (NaturalOrientation.cs). 

For debugging your application in Unity Editor, 
there's also a mouselook script in the prefab, 
which is switched off automatically when running on mobile (DiveMouseLook.cs).
 
DiveJava.cs is not directly included in the prefab but 
called by OpenDiveSensor.cs to set an immersive fullscreen mode on older devices
with soft buttons.

-== Usage ==-
	OpenDiveSensor.cs
		Add Rotation Gameobject
			For simplicity reasons the dive plugin works with global rotation.
			To simulate sitting in a rollercoaster for example, you could either change the script to use local rotation,
			or drag the GameObject in question (in the example of a rollercoaster, a waggon for instance) to RotationGameobject
			and set AddRotationGameObject to true.
		Correct Center Transition
			If you set this to true, the cameras will always rotate to Quaternion.Identity, the first time, the script is started.
			Use this, if you want to reset the rotation after entering another scene.
	OffsetCenter.cs
		Offset
			a correction offset used to project your world in a proper way for bigger screens.
			Without Offcenter Projection your eyes won't be able to converge on bigger screens.
		Autocorrect Offset
			if set, the offset will be calculated and set automatically

 	
17/04/2015
Shoogee GmbH & Co. KG
Hoetteweg 8
48143 Muenster
Germany
