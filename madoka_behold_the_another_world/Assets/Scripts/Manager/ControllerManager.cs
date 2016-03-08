using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class ControllerManager : SingletonMonoBehaviour<ControllerManager> 
{
	// 各種キーの入力の有無
	// 射撃・決定
	public bool Shot;
	// 格闘
	public bool Wrestle;
	// ジャンプ・キャンセル
	public bool Jump;
	// サーチ
	public bool Search;
	// コマンド
	public bool Command;
	// メニュー
	public bool Menu;
	// サブ射撃
	public bool SubShot;
	// 特殊射撃
	public bool EXShot;
	// 特殊格闘
	public bool EXWrestle;
	// 視点回転上(仰角）
	public bool ElevationAngleUpper;
	// 視点回転下(仰角)
	public bool ElevationAngleDown;
	// 視点回転左(方位角)
	public bool AzimuthLeft;
	// 視点回転右(方位角)
	public bool AzimuthRight;

	// 各種キーの入力名（キーボード）
	// 射撃・決定
	public string ShotKeyName;
	// 格闘
	public string WrestleKeyName;
	// ジャンプ・キャンセル
	public string JumpKeyName;


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
