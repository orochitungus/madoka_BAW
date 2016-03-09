using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager> 
{
	// 各種キーの入力の有無
	/// <summary>
	/// 射撃・決定
	/// </summary>
	public bool Shot;

	/// <summary>
	/// 格闘
	/// </summary>
	public bool Wrestle;

	/// <summary>
	/// ジャンプ・キャンセル
	/// </summary>
	public bool Jump;

	/// <summary>
	/// サーチ
	/// </summary>
	public bool Search;

	/// <summary>
	/// コマンド
	/// </summary>
	public bool Command;

	/// <summary>
	/// メニュー
	/// </summary>
	public bool Menu;

	/// <summary>
	/// サブ射撃
	/// </summary>
	public bool SubShot;

	/// <summary>
	/// 特殊射撃
	/// </summary>
	public bool EXShot;

	/// <summary>
	/// 特殊格闘
	/// </summary>
	public bool EXWrestle;

	/// <summary>
	/// 視点回転上(仰角）
	/// </summary>
	public bool ElevationAngleUpper;

	/// <summary>
	/// 視点回転下(仰角)
	/// </summary>
	public bool ElevationAngleDown;

	/// <summary>
	/// 視点回転左(方位角)
	/// </summary>
	public bool AzimuthLeft;

	/// <summary>
	/// 視点回転右(方位角)
	/// </summary>
	public bool AzimuthRight;

	// 各種キーの入力名（キーボード）
	/// <summary>
	/// 射撃・決定
	/// </summary>
	public string ShotKeyName;

	/// <summary>
	/// 格闘
	/// </summary>
	public string WrestleKeyName;
	
	/// <summary>
	/// ジャンプ・キャンセル
	/// </summary>
	public string JumpKeyName;

	/// <summary>
	/// サーチ
	/// </summary>
	public string SeachKeyName;

	/// <summary>
	/// メニュー
	/// </summary>
	public string MenuKeyName;

	/// <summary>
	/// サブ射撃
	/// </summary>
	public string SubShotKeyName;

	/// <summary>
	/// 特殊射撃
	/// </summary>
	public string EXShotKeyName;

	/// <summary>
	/// 特殊格闘
	/// </summary>
	public string EXWrestleKeyName;

	/// <summary>
	/// 視点回転上
	/// </summary>
	public string ElevationAngleUpKeyName;

	/// <summary>
	/// 視点回転下
	/// </summary>
	public string ElevationAngleDownKeyName;

	/// <summary>
	/// 視点回転左
	/// </summary>
	public string AzimuthLeftKeyName;

	/// <summary>
	/// 視点回転右
	/// </summary>
	public string AzimuthRightKeyName;

	// 各種キーの入力名（ジョイスティック）

	// Use this for initialization
	void Start () 
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
