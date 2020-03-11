using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCut : MonoBehaviour {

	private MeshFilter attachedMeshFilter;
	private Mesh attachedMesh;
	private bool coliBool = false;
	private double delta = 0.000000001f;
	private float skinWidth = 0.05f;
	private bool returnBool = false;
	private bool returnBool2 = false;
	private bool returnBool3 = false;

	void Start () {
		//連続で切ってしまわないように少し遅らせる。適宜調整する
		Invoke ("BoolOn", 0.2f);
		//meshの取得
		attachedMeshFilter = GetComponent<MeshFilter> ();
		attachedMesh = attachedMeshFilter.mesh;
	}

	void BoolOn () {
		coliBool = true;
	}

	public void Cut (Plane cutPlane) {
		//coliboolがfalseの時は何もしない
		if (coliBool == false) {
			return;
		}
		returnBool = false;

		//いろいろ、Vector3は精度のためにdoubleで扱えるようにしたDVector3を使用↓にclassあり
		DVector3 p1, p2, p3;
		bool p1Bool, p2Bool, p3Bool;
		var uvs1 = new List<Vector2> ();
		var uvs2 = new List<Vector2> ();
		var vertices1 = new List<DVector3> ();
		var vertices2 = new List<DVector3> ();
		var triangles1 = new List<int> ();
		var triangles2 = new List<int> ();
		var normals1 = new List<Vector3> ();
		var normals2 = new List<Vector3> ();
		var crossVertices = new List<DVector3> ();

		//カットしたいオブジェクトのメッシュをトライアングルごとに処理
		for (int i = 0; i < attachedMesh.triangles.Length; i += 3) {
			//メッシュの3つの頂点を取得
			p1 = new DVector3 (transform.TransformPoint (attachedMesh.vertices[attachedMesh.triangles[i]]));
			p2 = new DVector3 (transform.TransformPoint (attachedMesh.vertices[attachedMesh.triangles[i + 1]]));
			p3 = new DVector3 (transform.TransformPoint (attachedMesh.vertices[attachedMesh.triangles[i + 2]]));

			//頂点がカットする面のどちら側にあるか
			p1Bool = DVector3.Dot (new DVector3 (cutPlane.normal), p1) + (double) cutPlane.distance > 0 ? true : false;
			p2Bool = DVector3.Dot (new DVector3 (cutPlane.normal), p2) + (double) cutPlane.distance > 0 ? true : false;
			p3Bool = DVector3.Dot (new DVector3 (cutPlane.normal), p3) + (double) cutPlane.distance > 0 ? true : false;

			//3つの頂点が同じ側にある場合はそのまま代入、頂点がカットする場合はその処理を行う
			if (p1Bool && p2Bool && p3Bool) {
				//3つの頂点が同じ側にある、そのままそれぞれの1に代入
				for (int k = 0; k < 3; k++) {
					vertices1.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + k]]));
					uvs1.Add (attachedMesh.uv[attachedMesh.triangles[i + k]]);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + k]]);
					triangles1.Add (vertices1.Count - 1);
				}

			} else if (!p1Bool && !p2Bool && !p3Bool) {
				//3つの頂点が同じ側にある、そのままそれぞれの２に代入
				for (int k = 0; k < 3; k++) {
					vertices2.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + k]]));
					uvs2.Add (attachedMesh.uv[attachedMesh.triangles[i + k]]);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + k]]);
					triangles2.Add (vertices2.Count - 1);
				}
			} else {
				//3つの頂点が同じ側にない場合の処理１、以下仲間外れの頂点をp,それ以外をcとする
				DVector3 p, c1, c2;
				int n1, n2, n3;
				if ((p1Bool && !p2Bool && !p3Bool) || (!p1Bool && p2Bool && p3Bool)) {
					p = p1;
					c1 = p2;
					c2 = p3;
					n1 = 0;
					n2 = 1;
					n3 = 2;

				} else if ((!p1Bool && p2Bool && !p3Bool) || (p1Bool && !p2Bool && p3Bool)) {
					p = p2;
					c1 = p3;
					c2 = p1;
					n1 = 1;
					n2 = 2;
					n3 = 0;

				} else {
					p = p3;
					c1 = p1;
					c2 = p2;
					n1 = 2;
					n2 = 0;
					n3 = 1;

				}

				//カットした面に生じる新しい頂点を計算、カットする平面の法線方向に対するpとcの距離の比からc-pの長さを決める
				DVector3 cross1 = p + (c1 - p) * (((double) cutPlane.distance + DVector3.Dot (new DVector3 (cutPlane.normal), p)) / DVector3.Dot (new DVector3 (cutPlane.normal), p - c1));
				DVector3 cross2 = p + (c2 - p) * (((double) cutPlane.distance + DVector3.Dot (new DVector3 (cutPlane.normal), p)) / DVector3.Dot (new DVector3 (cutPlane.normal), p - c2));

				//新しい頂点のuvを計算、pとcの間で線形補間
				Vector2 cross1Uv = Vector2.Lerp (attachedMesh.uv[attachedMesh.triangles[i + n1]], attachedMesh.uv[attachedMesh.triangles[i + n2]], (float) System.Math.Sqrt ((cross1 - p).sqrMagnitude / (p - c1).sqrMagnitude));
				Vector2 cross2Uv = Vector2.Lerp (attachedMesh.uv[attachedMesh.triangles[i + n1]], attachedMesh.uv[attachedMesh.triangles[i + n3]], (float) System.Math.Sqrt ((cross2 - p).sqrMagnitude / (p - c2).sqrMagnitude));

				//本来はDVector3内でやりたいがよくわからないのでVector3のInverseTransfromPointを使用
				cross1 = new DVector3 (transform.InverseTransformPoint (cross1.ToVector3 ()));
				cross2 = new DVector3 (transform.InverseTransformPoint (cross2.ToVector3 ()));

				//断面をつくるために取っておく
				crossVertices.Add (cross1);
				crossVertices.Add (cross2);

				//pの２通りの処理、カットする面に対してどちらにあるかで異なる
				if ((p1Bool && !p2Bool && !p3Bool) || (!p1Bool && p2Bool && !p3Bool) || (!p1Bool && !p2Bool && p3Bool)) {

					//p側のメッシュを追加
					vertices1.Add (cross1);
					uvs1.Add (cross1Uv);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles1.Add (vertices1.Count - 1);

					vertices1.Add (cross2);
					uvs1.Add (cross2Uv);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles1.Add (vertices1.Count - 1);

					vertices1.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n1]]));
					uvs1.Add (attachedMesh.uv[attachedMesh.triangles[i + n1]]);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles1.Add (vertices1.Count - 1);

					//c側のメッシュを追加１
					vertices2.Add (cross2);
					uvs2.Add (cross2Uv);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles2.Add (vertices2.Count - 1);

					vertices2.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n2]]));
					uvs2.Add (attachedMesh.uv[attachedMesh.triangles[i + n2]]);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n2]]);
					triangles2.Add (vertices2.Count - 1);

					vertices2.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n3]]));
					uvs2.Add (attachedMesh.uv[attachedMesh.triangles[i + n3]]);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n3]]);
					triangles2.Add (vertices2.Count - 1);

					//c側のメッシュを追加2
					vertices2.Add (cross2);
					uvs2.Add (cross2Uv);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles2.Add (vertices2.Count - 1);

					vertices2.Add (cross1);
					triangles2.Add (vertices2.Count - 1);
					uvs2.Add (cross1Uv);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices2.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n2]]));
					uvs2.Add (attachedMesh.uv[attachedMesh.triangles[i + n2]]);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n2]]);
					triangles2.Add (vertices2.Count - 1);
				} else {
					//p側のメッシュを追加
					vertices2.Add (cross1);
					triangles2.Add (vertices2.Count - 1);
					uvs2.Add (cross1Uv);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices2.Add (cross2);
					triangles2.Add (vertices2.Count - 1);
					uvs2.Add (cross2Uv);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices2.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n1]]));
					uvs2.Add (attachedMesh.uv[attachedMesh.triangles[i + n1]]);
					normals2.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);
					triangles2.Add (vertices2.Count - 1);

					//c側のメッシュを追加１
					vertices1.Add (cross2);
					triangles1.Add (vertices1.Count - 1);
					uvs1.Add (cross2Uv);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices1.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n2]]));
					uvs1.Add (attachedMesh.uv[attachedMesh.triangles[i + n2]]);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n2]]);
					triangles1.Add (vertices1.Count - 1);

					vertices1.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n3]]));
					uvs1.Add (attachedMesh.uv[attachedMesh.triangles[i + n3]]);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n3]]);
					triangles1.Add (vertices1.Count - 1);

					//c側のメッシュを追加2
					vertices1.Add (cross2);
					triangles1.Add (vertices1.Count - 1);
					uvs1.Add (cross2Uv);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices1.Add (cross1);
					triangles1.Add (vertices1.Count - 1);
					uvs1.Add (cross1Uv);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n1]]);

					vertices1.Add (new DVector3 (attachedMesh.vertices[attachedMesh.triangles[i + n2]]));
					uvs1.Add (attachedMesh.uv[attachedMesh.triangles[i + n2]]);
					normals1.Add (attachedMesh.normals[attachedMesh.triangles[i + n2]]);
					triangles1.Add (vertices1.Count - 1);
				}
			}
		}

		//meshを減らす処理を行う。(断面以外の処理)、↓に関数あり
		reduceMesh (ref vertices1, ref uvs1, ref normals1, cutPlane);
		//reduceMesh中でCutの中断を判断した場合にreturnする
		if (returnBool) {
			return;
		}
		reduceMesh (ref vertices2, ref uvs2, ref normals2, cutPlane);
		if (returnBool) {
			return;
		}

		//断面をつくる処理
		if (crossVertices.Count != 0) {
			//断面で頂点を減らす処理、直線上にある頂点を2つのみにする
			for (int i = 0; i < crossVertices.Count; i += 2) {
				for (int k = i + 2; k < crossVertices.Count; k += 2) {
					//4つの頂点が一直線上にあるか、まず2つのベクトルが平行かどうか
					if (System.Math.Abs (DVector3.Dot ((crossVertices[i] - crossVertices[i + 1]).normalized, (crossVertices[k] - crossVertices[k + 1]).normalized)) > 1 - delta) {
						//同一の頂点を持つかどうか→一直線上にある
						if ((crossVertices[i] - crossVertices[k]).sqrMagnitude < delta || (crossVertices[i] - crossVertices[k + 1]).sqrMagnitude < delta ||
							(crossVertices[i + 1] - crossVertices[k]).sqrMagnitude < delta || (crossVertices[i + 1] - crossVertices[k + 1]).sqrMagnitude < delta) {
							//以下重なる点に応じた処理、両端を残して後を消去
							if ((crossVertices[i] - crossVertices[k]).sqrMagnitude < (crossVertices[i + 1] - crossVertices[k]).sqrMagnitude) {
								crossVertices.Add (crossVertices[i + 1]);
								if ((crossVertices[i] - crossVertices[k]).sqrMagnitude < (crossVertices[i] - crossVertices[k + 1]).sqrMagnitude) {
									crossVertices.Add (crossVertices[k + 1]);
								} else {
									crossVertices.Add (crossVertices[k]);
								}
								crossVertices.RemoveRange (k, 2);
								crossVertices.RemoveRange (i, 2);
							} else {
								crossVertices.Add (crossVertices[i]);
								if ((crossVertices[i + 1] - crossVertices[k]).sqrMagnitude < (crossVertices[i + 1] - crossVertices[k + 1]).sqrMagnitude) {
									crossVertices.Add (crossVertices[k + 1]);
								} else {
									crossVertices.Add (crossVertices[k]);
								}
								crossVertices.RemoveRange (k, 2);
								crossVertices.RemoveRange (i, 2);

							}
							i -= 2;
							break;
						}

					}

				}
			}

			//断面の三角形を作る処理

			//等しい点を消去
			for (int i = 0; i < crossVertices.Count; i++) {
				for (int j = i + 1; j < crossVertices.Count; j++) {
					if ((crossVertices[i] - crossVertices[j]).sqrMagnitude < delta) {
						crossVertices.RemoveAt (j);
						i--;
						break;
					}
				}
			}

			//外周の頂点を並び替え、crossVertices[0]と[1]を基準としてそれぞれの点を並び替える
			for (int i = 2; i < crossVertices.Count; i++) {
				for (int j = i + 1; j < crossVertices.Count; j++) {
					if (System.Math.Acos (DVector3.Dot ((crossVertices[0] - crossVertices[1]).normalized, (crossVertices[0] - crossVertices[i]).normalized)) >= System.Math.Acos (DVector3.Dot ((crossVertices[0] - crossVertices[1]).normalized, (crossVertices[0] - crossVertices[j]).normalized))) {
						//角度が等しい、一直線上にある場合。本来はないはずだが・・・
						if (System.Math.Acos (DVector3.Dot ((crossVertices[0] - crossVertices[1]).normalized, (crossVertices[0] - crossVertices[i]).normalized)) == System.Math.Acos (DVector3.Dot ((crossVertices[0] - crossVertices[1]).normalized, (crossVertices[0] - crossVertices[j]).normalized))) {
							//基準を変える
							if ((crossVertices[0] - crossVertices[i]).sqrMagnitude > (crossVertices[0] - crossVertices[j]).sqrMagnitude) {
								crossVertices.Insert (0, crossVertices[j]);
								crossVertices.RemoveAt (j + 1);
							} else {
								crossVertices.Insert (0, crossVertices[i]);
								crossVertices.RemoveAt (i + 1);
							}

							i = 1;
							break;
						}
						//並び替え
						crossVertices.Insert (i, crossVertices[j]);
						crossVertices.RemoveAt (j + 1);
						i = 1;
						break;
					}
				}
			}

			for (int i = 1; i < crossVertices.Count - 1; i++) {
				//断面のnormalとuvの設定。uvを特別に設定する場合は変えてください
				for (int j = 0; j < 3; j++) {
					normals1.Add (-cutPlane.normal);
					uvs1.Add (new Vector2 (0, 0));
					normals2.Add (cutPlane.normal);
					uvs2.Add (new Vector2 (0, 0));

				}
				//断面の三角形を追加する　面の表の方向が正しくなるように判断して追加
				if (Vector3.Dot (transform.TransformDirection (DVector3.Cross ((crossVertices[i] - crossVertices[0]).normalized, (crossVertices[i + 1] - crossVertices[i]).normalized).ToVector3 ()), cutPlane.normal) < delta) {
					vertices1.Add (crossVertices[0]);
					vertices1.Add (crossVertices[i]);
					vertices1.Add (crossVertices[i + 1]);

					vertices2.Add (crossVertices[i]);
					vertices2.Add (crossVertices[0]);
					vertices2.Add (crossVertices[i + 1]);
				} else {
					vertices1.Add (crossVertices[i]);
					vertices1.Add (crossVertices[0]);
					vertices1.Add (crossVertices[i + 1]);

					vertices2.Add (crossVertices[0]);
					vertices2.Add (crossVertices[i]);
					vertices2.Add (crossVertices[i + 1]);
				}
			}

		}
		//ひとつのtriangleについてそれぞれ3つずつの頂点を作っているため最後に順番通りにいれる
		triangles1.Clear ();
		for (int i = 0; i < vertices1.Count; i++) {
			triangles1.Add (i);
		}
		triangles2.Clear ();
		for (int i = 0; i < vertices2.Count; i++) {
			triangles2.Add (i);
		}

		//DVector3を通常のVector3に直す、もっと賢いやりかたがありそう
		var list1 = new List<Vector3> ();
		for (int i = 0; i < vertices1.Count; i++) {
			list1.Add (vertices1[i].ToVector3 ());
		}
		var list2 = new List<Vector3> ();
		for (int i = 0; i < vertices2.Count; i++) {
			list2.Add (vertices2[i].ToVector3 ());
		}

		//カット後のオブジェクト生成、いろいろといれる
		GameObject obj = new GameObject ("cut obj", typeof (MeshFilter), typeof (MeshRenderer), typeof (MeshCollider), typeof (Rigidbody), typeof (MeshCut));
		var mesh = new Mesh ();
		mesh.vertices = list1.ToArray ();
		mesh.triangles = triangles1.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.normals = normals1.ToArray ();
		obj.GetComponent<MeshFilter> ().mesh = mesh;
		obj.GetComponent<MeshRenderer> ().materials = GetComponent<MeshRenderer> ().materials;
		obj.GetComponent<MeshCollider> ().sharedMesh = mesh;
		obj.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation;
		obj.GetComponent<MeshCollider> ().convex = true;
		obj.GetComponent<MeshCollider> ().material = GetComponent<Collider> ().material;
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		obj.transform.localScale = transform.localScale;
		obj.GetComponent<Rigidbody> ().velocity = GetComponent<Rigidbody> ().velocity;
		obj.GetComponent<Rigidbody> ().angularVelocity = GetComponent<Rigidbody> ().angularVelocity;
		obj.GetComponent<MeshCut> ().skinWidth = skinWidth;
		obj.tag = "Fragment";

		GameObject obj2 = new GameObject ("cut obj", typeof (MeshFilter), typeof (MeshRenderer), typeof (MeshCollider), typeof (Rigidbody), typeof (MeshCut));
		var mesh2 = new Mesh ();
		mesh2.vertices = list2.ToArray ();
		mesh2.triangles = triangles2.ToArray ();
		mesh2.uv = uvs2.ToArray ();
		mesh2.normals = normals2.ToArray ();
		obj2.GetComponent<MeshFilter> ().mesh = mesh2;
		obj2.GetComponent<MeshRenderer> ().materials = GetComponent<MeshRenderer> ().materials;
		obj2.GetComponent<MeshCollider> ().sharedMesh = mesh2;
		obj2.GetComponent<MeshCollider> ().cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation;
		obj2.GetComponent<MeshCollider> ().convex = true;
		obj2.GetComponent<MeshCollider> ().material = GetComponent<Collider> ().material;
		obj2.transform.position = transform.position;
		obj2.transform.rotation = transform.rotation;
		obj2.transform.localScale = transform.localScale;
		obj2.GetComponent<Rigidbody> ().velocity = GetComponent<Rigidbody> ().velocity;
		obj2.GetComponent<Rigidbody> ().angularVelocity = GetComponent<Rigidbody> ().angularVelocity;
		obj2.GetComponent<MeshCut> ().skinWidth = skinWidth;
		obj2.tag = "Fragment";

		//このオブジェクトをデストロイ
		Destroy (gameObject);

	}

	void reduceMesh (ref List<DVector3> vertices, ref List<Vector2> uvs, ref List<Vector3> normals, Plane cutPlane) {

		var verticeIndices = new List<int> ();
		var pVertices = new List<DVector3> ();
		var pNormals = new List<Vector3> ();
		var pUvs = new List<Vector2> ();

		for (int i = 0; i < vertices.Count; i += 3) {
			//まずは同一平面上にある三角形を見つけ出すが、基準となるiをとりあえず追加
			verticeIndices.Clear ();
			verticeIndices.Add (i);

			for (int j = i + 3; j < vertices.Count; j += 3) {

				//同一の平面上にある三角形かどうか。deltaで調整
				if (DVector3.Dot (DVector3.Cross ((vertices[i + 1] - vertices[i]).normalized, (vertices[i + 2] - vertices[i + 1]).normalized).normalized,
						DVector3.Cross ((vertices[j + 1] - vertices[j]).normalized, (vertices[j + 2] - vertices[j + 1]).normalized).normalized) > 1 - delta) {
					verticeIndices.Add (j);
				}

				//全ての三角形について計算してループの最後に行う処理
				if (j == vertices.Count - 3) {
					//三角形が1つの場合は何もする必要なし
					if (verticeIndices.Count > 1) {
						//平面上の三角形が2つ以上ある場合
						//pの入れ物に三角形を3つの直線にしていれる
						for (int k = 0; k < verticeIndices.Count; k++) {
							for (int l = 0; l < 3; l++) {
								pVertices.Add (vertices[verticeIndices[k] + l]);
								pNormals.Add (normals[verticeIndices[k] + l]);
								pUvs.Add (uvs[verticeIndices[k] + l]);

								pVertices.Add (vertices[verticeIndices[k] + numRep (l + 1)]);
								pNormals.Add (normals[verticeIndices[k] + numRep (l + 1)]);
								pUvs.Add (uvs[verticeIndices[k] + numRep (l + 1)]);
							}

						}

						//等しい直線を消す（面の外周でない線を消す）
						int sameLineCount = 0;
						for (int k = 0; k < pVertices.Count; k += 2) {
							for (int l = k + 2; l < pVertices.Count; l += 2) {
								if (((pVertices[l + 1] - pVertices[k]).sqrMagnitude < delta) && ((pVertices[l] - pVertices[k + 1]).sqrMagnitude < delta)) {
									sameLineCount++;
									pVertices.RemoveRange (l, 2);
									pVertices.RemoveRange (k, 2);
									pNormals.RemoveRange (l, 2);
									pNormals.RemoveRange (k, 2);
									pUvs.RemoveRange (l, 2);
									pUvs.RemoveRange (k, 2);
									k -= 2;

									break;
								}
							}
						}
						//同一平面上のn個の隣接する三角形にはn-1個の等しい直線があるはずが、ない場合(ここまでの処理がうまくいっていない場合)は処理をやめる
						//ここらへんは検討中　→　新しい処理追加
						if (sameLineCount != verticeIndices.Count - 1) {
							//同一平面上にあるが三角形が隣接していない場合（本来同一平面上と判定されるはずの三角形が
							//同一平面上と判定されなかった(切断面の頂点を求める際にある2点がほぼ同じ値を取ってしまった場合)）
							//にcutPlaneをちょっとずらしてもう一度計算させる。

							//一度だけ処理させるように以下の分岐でゴニョゴニョ
							if (returnBool2) {
								returnBool = true;
								returnBool3 = true;
								return;
							}
							returnBool2 = true;
							//ずらす量は暫定的
							Cut (new Plane (cutPlane.normal, -cutPlane.normal * cutPlane.distance + new Vector3 (0.02f, 0.02f, 0.02f)));
							if (returnBool3) {
								returnBool = false;
								returnBool2 = false;
							} else {
								returnBool = true;
							}
							return;

						}
						for (int l = 0; l < pVertices.Count; l += 2) {
							for (int k = l + 2; k < pVertices.Count; k += 2) {
								//4つの頂点が一直線上にあるか、2つのベクトルが平行かどうか
								if (System.Math.Abs (DVector3.Dot ((pVertices[l] - pVertices[l + 1]).normalized, (pVertices[k] - pVertices[k + 1]).normalized)) > 1 - delta) {
									//同一の点を持つ→一直線上にある
									if ((pVertices[l] - pVertices[k]).sqrMagnitude < delta || (pVertices[l] - pVertices[k + 1]).sqrMagnitude < delta ||
										(pVertices[l + 1] - pVertices[k]).sqrMagnitude < delta || (pVertices[l + 1] - pVertices[k + 1]).sqrMagnitude < delta) {

										//以下重なる点に応じた処理、両端を残して後を消去
										if ((pVertices[l] - pVertices[k]).sqrMagnitude < (pVertices[l + 1] - pVertices[k]).sqrMagnitude) {
											pVertices.Add (pVertices[l + 1]);
											pNormals.Add (pNormals[l + 1]);
											pUvs.Add (pUvs[l + 1]);

											if ((pVertices[l] - pVertices[k]).sqrMagnitude < (pVertices[l] - pVertices[k + 1]).sqrMagnitude) {
												pVertices.Add (pVertices[k + 1]);
												pNormals.Add (pNormals[k + 1]);
												pUvs.Add (pUvs[k + 1]);

											} else {
												pVertices.Add (pVertices[k]);
												pNormals.Add (pNormals[k]);
												pUvs.Add (pUvs[k]);

											}
											pVertices.RemoveRange (k, 2);
											pVertices.RemoveRange (l, 2);
											pNormals.RemoveRange (k, 2);
											pNormals.RemoveRange (l, 2);
											pUvs.RemoveRange (k, 2);
											pUvs.RemoveRange (l, 2);
										} else {
											pVertices.Add (pVertices[l]);
											pNormals.Add (pNormals[l]);
											pUvs.Add (pUvs[l]);
											if ((pVertices[l + 1] - pVertices[k]).sqrMagnitude < (pVertices[l + 1] - pVertices[k + 1]).sqrMagnitude) {
												pVertices.Add (pVertices[k + 1]);
												pNormals.Add (pNormals[k + 1]);
												pUvs.Add (pUvs[k + 1]);
											} else {
												pVertices.Add (pVertices[k]);
												pNormals.Add (pNormals[k]);
												pUvs.Add (pUvs[k]);
											}
											pVertices.RemoveRange (k, 2);
											pVertices.RemoveRange (l, 2);
											pNormals.RemoveRange (k, 2);
											pNormals.RemoveRange (l, 2);
											pUvs.RemoveRange (k, 2);
											pUvs.RemoveRange (l, 2);
										}

										l -= 2;
										break;
									}

								}
							}

						}

						//等しい点を消す
						for (int k = 0; k < pVertices.Count; k++) {
							for (int l = k + 1; l < pVertices.Count; l++) {
								if ((pVertices[k] - pVertices[l]).sqrMagnitude < delta) {
									pVertices.RemoveAt (l);
									pNormals.RemoveAt (l);
									pUvs.RemoveAt (l);
								}
							}
						}

						//外周上の点を順番に並び替える.pVertices[0],[1]を基準として並び替え
						for (int k = 2; k < pVertices.Count; k++) {
							for (int l = k + 1; l < pVertices.Count; l++) {
								if (System.Math.Acos (DVector3.Dot ((pVertices[0] - pVertices[1]).normalized, (pVertices[0] - pVertices[k]).normalized)) >= System.Math.Acos (DVector3.Dot ((pVertices[0] - pVertices[1]).normalized, (pVertices[0] - pVertices[l]).normalized))) {
									//等しくなってしまう場合
									if (System.Math.Acos (DVector3.Dot ((pVertices[0] - pVertices[1]).normalized, (pVertices[0] - pVertices[k]).normalized)) == System.Math.Acos (DVector3.Dot ((pVertices[0] - pVertices[1]).normalized, (pVertices[0] - pVertices[l]).normalized))) {
										if ((pVertices[0] - pVertices[k]).sqrMagnitude > (pVertices[0] - pVertices[l]).sqrMagnitude) {
											pVertices.Insert (0, pVertices[l]);
											pVertices.RemoveAt (l + 1);
											pNormals.Insert (0, pNormals[l]);
											pNormals.RemoveAt (l + 1);
											pUvs.Insert (0, pUvs[l]);
											pUvs.RemoveAt (l + 1);
										} else {
											pVertices.Insert (0, pVertices[k]);
											pVertices.RemoveAt (k + 1);
											pNormals.Insert (0, pNormals[k]);
											pNormals.RemoveAt (k + 1);
											pUvs.Insert (0, pUvs[k]);
											pUvs.RemoveAt (k + 1);
										}
										k = 1;
										break;
									}
									//並び替え
									pVertices.Insert (k, pVertices[l]);
									pVertices.RemoveAt (l + 1);
									pNormals.Insert (k, pNormals[l]);
									pNormals.RemoveAt (l + 1);
									pUvs.Insert (k, pUvs[l]);
									pUvs.RemoveAt (l + 1);
									k = 1;
									break;
								}
							}
						}

						//外周上の並び替えられた頂点を全て三角形で結ぶように追加
						for (int k = 1; k < pVertices.Count - 1; k++) {
							vertices.Insert (0, pVertices[k + 1]);
							normals.Insert (0, pNormals[k + 1]);
							uvs.Insert (0, pUvs[k + 1]);

							vertices.Insert (0, pVertices[k]);
							normals.Insert (0, pNormals[k]);
							uvs.Insert (0, pUvs[k]);

							vertices.Insert (0, pVertices[0]);
							normals.Insert (0, pNormals[0]);
							uvs.Insert (0, pUvs[0]);
						}

						//追加したので古いのを消去
						for (int k = verticeIndices.Count - 1; k >= 0; k--) {
							vertices.RemoveRange (verticeIndices[k] + 3 * (pVertices.Count - 2), 3);
							normals.RemoveRange (verticeIndices[k] + 3 * (pVertices.Count - 2), 3);
							uvs.RemoveRange (verticeIndices[k] + 3 * (pVertices.Count - 2), 3);
						}
						//処理した三角形を考慮してループの位置を調整
						i += 3 * (pVertices.Count - 3);
						//初期化しておく
						pVertices.Clear ();
						pNormals.Clear ();
						pUvs.Clear ();

						break;
					}

				}

			}

		}
	}

	//ループで使いたかった関数
	int numRep (int i) {
		if (i % 3 == 0) {
			return 0;
		} else if (i % 3 == 1) {
			return 1;
		} else if (i % 3 == 2) {
			return 2;
		} else {
			return 0;
		}
	}

}

//Vector3をdoubleで使うクラス、使う機能のみ
public class DVector3 {
	public double x;
	public double y;
	public double z;

	public DVector3 (Vector3 a) {
		x = a.x;
		y = a.y;
		z = a.z;
	}
	public DVector3 (double a, double b, double c) {
		x = a;
		y = b;
		z = c;
	}
	public double sqrMagnitude {
		get { return x * x + y * y + z * z; }
	}

	public Vector3 ToVector3 () {
		return new Vector3 ((float) x, (float) y, (float) z);
	}

	public override string ToString () {
		return string.Format ("({0:0.00000}, {1:0.00000}, {2:0.00000})", x, y, z);
	}
	public DVector3 normalized {
		get { return new DVector3 (x / System.Math.Sqrt (this.sqrMagnitude), y / System.Math.Sqrt (this.sqrMagnitude), z / System.Math.Sqrt (this.sqrMagnitude)); }
	}

	public static double Dot (DVector3 a, DVector3 b) {
		return a.x * b.x + a.y * b.y + a.z * b.z;
	}

	public static DVector3 Cross (DVector3 a, DVector3 b) {
		return new DVector3 (a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	}

	public static DVector3 operator - (DVector3 a, DVector3 b) {
		return new DVector3 (a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static DVector3 operator + (DVector3 a, DVector3 b) {
		return new DVector3 (a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static DVector3 operator * (DVector3 a, double b) {
		return new DVector3 (a.x * b, a.y * b, a.z * b);
	}

	public static DVector3 operator / (DVector3 a, double b) {
		return new DVector3 (a.x / b, a.y / b, a.z / b);
	}

}
