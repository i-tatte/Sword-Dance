using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GVContainer {
	/// <summary>
	/// 壁のHP。なくなるとゲームオーバー。
	/// </summary>
	public static int WallHP = 10;
	/// <summary>
	/// 左(青)の光剣の方向ベクタ
	/// </summary>
	public static Vector3 LeftSwordDirection;

	/// <summary>
	/// 左(青)の光剣の速度ベクタ
	/// </summary>
	public static Vector3 LeftSwordVelocity;

	/// <summary>
	/// 右(赤)の光剣の方向ベクタ
	/// </summary>
	public static Vector3 RightSwordDirection;

	/// <summary>
	/// 右(赤)の光剣の速度ベクタ
	/// </summary>
	public static Vector3 RightSwordVelocity;

	/// <summary>
	/// Aボタンの押下
	/// </summary>
	public static bool ButtonADown;

	/// <summary>
	/// Bボタンの押下
	/// </summary>
	public static bool ButtonBDown;

	/// <summary>
	/// Xボタンの押下
	/// </summary>
	public static bool ButtonXDown;

	/// <summary>
	/// Yボタンの押下
	/// </summary>
	public static bool ButtonYDown;

	/// <summary>
	/// Aボタンの状態
	/// </summary>
	public static bool ButtonA;

	/// <summary>
	/// Bボタンの状態
	/// </summary>
	public static bool ButtonB;

	/// <summary>
	/// Xボタンの状態
	/// </summary>
	public static bool ButtonX;

	/// <summary>
	/// Yボタンの状態
	/// </summary>
	public static bool ButtonY;

	/// <summary>
	/// 右トリガーの押下
	/// </summary>
	public static bool RightTriggerDown;

	/// <summary>
	/// 左トリガーの押下
	/// </summary>
	public static bool LeftTriggerDown;

	/// <summary>
	/// 右トリガーの状態
	/// </summary>
	public static bool RightTrigger;

	/// <summary>
	/// 左トリガーの状態
	/// </summary>
	public static bool LeftTrigger;
}
