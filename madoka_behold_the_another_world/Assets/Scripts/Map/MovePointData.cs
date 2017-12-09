using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovePointData
{
	public static MovePoint []MovepointMitakihara = new MovePoint[]
	{
		new MovePoint("見滝原市立病院","Mitakihara Hospital", new Vector3(1316.8f,162,740.3f), new Vector3(21.808f, 203.191f, 0), 0,9999,30),
		new MovePoint("歩道橋", "Footbridge", new Vector3(1060,26,572), new Vector3(9.889001f,267.362f,0), 0, 9999, 30),
	};
}

public class MovePoint
{
	/// <summary>
	/// 表示される名前日本語
	/// </summary>
	public string NameJP;
	
	/// <summary>
	/// 表示される名前英語
	/// </summary>
	public string NameEN;

	/// <summary>
	/// カメラ座標
	/// </summary>
	public Vector3 CameraPosition;

	/// <summary>
	/// カメラ角度
	/// </summary>
	public Vector3 CameraRotation;

	/// <summary>
	/// 登場時期最小
	/// </summary>
	public int LowXStory;

	/// <summary>
	/// 登場時期最大
	/// </summary>
	public int HighXStory;

	/// <summary>
	/// 画角
	/// </summary>
	public float FieldOfView;

	/// <summary>
	/// 現在位置であるか否か
	/// </summary>
	public bool NowPosition;

	
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="nameJP"></param>
	/// <param name="nameEN"></param>
	/// <param name="cameraPosition"></param>
	/// <param name="cameraRotation"></param>
	/// <param name="lowXStory"></param>
	/// <param name="highXStory"></param>
	public MovePoint(string nameJP, string nameEN, Vector3 cameraPosition, Vector3 cameraRotation, int lowXStory, int highXStory, float fieldOfView)
	{
		NameJP = nameJP;
		NameEN = nameEN;
		CameraPosition = cameraPosition;
		CameraRotation = cameraRotation;
		LowXStory = lowXStory;
		HighXStory = highXStory;
		FieldOfView = fieldOfView;
	}
}