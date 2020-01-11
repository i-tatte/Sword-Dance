using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

	/// <summary>
	/// ベクタv1, v2の外積を求める。
	/// v1 × v2 の値が返される。言わずもがな、外積計算に交換法則は成り立たないので順番に注意。
	public static Vector3 OuterProduct (Vector3 v1, Vector3 v2) {
		return new Vector3 (v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
	}

	/// <summary>
	/// ベクタv1, v2の外積の平方根を求める。
	/// v1 × v2 の値が返される。言わずもがな、外積計算に交換法則は成り立たないので順番に注意。
	public static Vector3 OuterProductRoot (Vector3 v1, Vector3 v2) {
		Vector3 ret = new Vector3 (0, 0, 0);
		ret = new Vector3 (v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
		ret.x = Mathf.Sqrt (ret.x);
		ret.y = Mathf.Sqrt (ret.y);
		ret.z = Mathf.Sqrt (ret.z);
		return ret;
	}

	/// <summary>
	/// ベクタvの各成分の平方根をとったベクタを返す。
	/// </summary>
	public static Vector3 VectorRoot (Vector3 v) {
		Vector3 ret = v;
		ret.x = Mathf.Sign (ret.x) * Mathf.Sqrt (Mathf.Abs (ret.x));
		ret.y = Mathf.Sign (ret.y) * Mathf.Sqrt (Mathf.Abs (ret.y));
		ret.z = Mathf.Sign (ret.z) * Mathf.Sqrt (Mathf.Abs (ret.z));
		return ret;
	}
}
