using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovePointData
{
	/// <summary>
	/// 見滝原市全体マップからの行先
	/// </summary>
	public static MovePoint []MovepointMitakihara = new MovePoint[]
	{
		new MovePoint("見滝原市立病院","Mitakihara Hospital", new Vector3(1316.8f,162,740.3f), new Vector3(21.808f, 203.191f, 0), 0,9999,30,FROMCODEFORMAP.MITAKIHARAHOSPITAL,"MitakiharaHospitalBycicleshed",8),
		new MovePoint("歩道橋", "Footbridge", new Vector3(1060,26,572), new Vector3(9.889001f,267.362f,0), 0, 9999, 30, FROMCODEFORMAP.FOOTBRIDGE,"FootBridgeArea",10),
	};


}

public enum FROMCODEFORMAP
{
	MITAKIHARAHOSPITAL = 1001,
	FOOTBRIDGE = 1101,
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
	/// どこであるか
	/// </summary>
	public FROMCODEFORMAP Fromcode;

	/// <summary>
	/// 移動先のステージファイルの名前
	/// </summary>
	public string StageFileName;

	/// <summary>
	/// 移動先のステージファイルのインデックス（StageCode.csのインデックスを参照）
	/// </summary>
	public int ForCode;

	
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="nameJP"></param>
	/// <param name="nameEN"></param>
	/// <param name="cameraPosition"></param>
	/// <param name="cameraRotation"></param>
	/// <param name="lowXStory"></param>
	/// <param name="highXStory"></param>
	public MovePoint(string nameJP, string nameEN, Vector3 cameraPosition, Vector3 cameraRotation, int lowXStory, int highXStory, float fieldOfView, FROMCODEFORMAP fromcode, string stagefilename,int forcode)
	{
		NameJP = nameJP;
		NameEN = nameEN;
		CameraPosition = cameraPosition;
		CameraRotation = cameraRotation;
		LowXStory = lowXStory;
		HighXStory = highXStory;
		FieldOfView = fieldOfView;
		Fromcode = fromcode;
		StageFileName = stagefilename;
		ForCode = forcode;
	}
}