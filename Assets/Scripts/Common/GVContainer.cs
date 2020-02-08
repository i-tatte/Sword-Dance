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
}
