using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPU時のControllerManagerに相当。CPU制御キャラはこのクラスとXXCPUクラスを取り付け、XXCPUクラスはこのクラスのフラグを書き換え、CharacterControlBaseはこのクラスのフラグを元に動きを作る
/// </summary>
public class CPUController : MonoBehaviour 
{
	// 各種キーの入力の有無

	/// <summary>
	/// 方向キー/左スティック上（コントローラー.斜めは複数同時）
	/// </summary>	
	public bool Top;

	/// <summary>
	/// 方向キー上を離した
	/// </summary>
	public bool TopUp;

	/// <summary>
	/// 方向キー上を長押した
	/// </summary>
	public bool TopLongPress;

	/// <summary>
	/// 方向キー/左スティック下
	/// </summary>
	public bool Under;

	/// <summary>
	/// 方向キー下を離した
	/// </summary>
	public bool UnderUp;

	/// <summary>
	/// 方向キー下を長押した
	/// </summary>
	public bool UnderLongPress;

	/// <summary>
	/// 方向キー/左スティック左
	/// </summary>
	public bool Left;

	/// <summary>
	/// 方向キー左を離した
	/// </summary>
	public bool LeftUp;

	/// <summary>
	/// 方向キー左を長押した
	/// </summary>
	public bool LeftLongPress;

	/// <summary>
	/// 方向キー/左スティック右
	/// </summary>
	public bool Right;

	/// <summary>
	/// 方向キー右を離した
	/// </summary>
	public bool RightUp;

	/// <summary>
	/// 方向キー右を長押した
	/// </summary>
	public bool RightLongPress;

	/// <summary>
	/// 左上を押した
	/// </summary>
	public bool LeftUpper;

	/// <summary>
	/// 左上を離した
	/// </summary>
	public bool LeftUpperUp;

	/// <summary>
	/// 左下を押した
	/// </summary>
	public bool LeftUnder;

	/// <summary>
	/// 左下を離した
	/// </summary>
	public bool LeftUnderUp;

	/// <summary>
	/// 右下を押した
	/// </summary>
	public bool RightUnder;

	/// <summary>
	/// 右下を離した
	/// </summary>
	public bool RightUnderUp;

	/// <summary>
	/// 右上を押した
	/// </summary>
	public bool RightUpper;

	/// <summary>
	/// 右上を押した
	/// </summary>
	public bool RightUpperUp;



	/// <summary>
	/// 射撃・決定
	/// </summary>
	public bool Shot;
	public bool ShotKeyboad;
	public bool ShotController;
	public bool ShotUp;

	/// <summary>
	/// 射撃長押し
	/// </summary>
	public bool Shotting;

	/// <summary>
	/// 格闘
	/// </summary>
	public bool Wrestle;
	public bool WrestleKeyboard;
	public bool WrestleController;
	public bool WrestleUp;

	/// <summary>
	/// 格闘長押し
	/// </summary>
	public bool Wrestling;

	/// <summary>
	/// ジャンプ・キャンセル
	/// </summary>
	public bool Jump;
	public bool JumpKeyboard;
	public bool JumpController;
	public bool JumpUp;

	/// <summary>
	/// ジャンプ長押し
	/// </summary>
	public bool Jumping;

	/// <summary>
	/// サーチ
	/// </summary>
	public bool Search;
	public bool SearchKeyboard;
	public bool SearchController;
	public bool SearchUp;

	/// <summary>
	/// コマンド
	/// </summary>
	public bool Command;
	public bool CommandKeyboard;
	public bool CommandController;
	public bool CommandUp;

	/// <summary>
	/// アイテム
	/// </summary>
	public bool Item;
	public bool ItemKeyboard;
	public bool ItemController;
	public bool ItemUp;

	/// <summary>
	/// メニュー
	/// </summary>
	public bool Menu;
	public bool MenuKeyboard;
	public bool MenuController;
	public bool MenuUp;

	/// <summary>
	/// サブ射撃
	/// </summary>
	public bool SubShot;
	public bool SubShotKeyboard;
	public bool SubShotController;
	public bool SubShotUp;

	/// <summary>
	/// 特殊射撃
	/// </summary>
	public bool EXShot;
	public bool EXShotKeyboard;
	public bool EXShotController;
	public bool EXShotUp;

	/// <summary>
	/// 特殊格闘
	/// </summary>
	public bool EXWrestle;
	public bool EXWrestleKeyboard;
	public bool EXWrestleController;
	public bool EXWrestleUp;

	/// <summary>
	/// 視点回転上(仰角）
	/// </summary>
	public bool ElevationAngleUpper;
	public bool ElevationAngleUpperKeyboard;
	public bool ElevationAngleUpperController;
	public bool ElevationAngleUpperUp;

	/// <summary>
	/// 視点回転下(仰角)
	/// </summary>
	public bool ElevationAngleDown;
	public bool ElevationAngleDownKeyboard;
	public bool ElevationAngleDownController;
	public bool ElevationAngleDownUp;

	/// <summary>
	/// 視点回転左(方位角)
	/// </summary>
	public bool AzimuthLeft;
	public bool AzimuthLeftKeyboard;
	public bool AzimuthLeftController;
	public bool AzimuthLeftUp;

	/// <summary>
	/// 視点回転右(方位角)
	/// </summary>
	public bool AzimuthRight;
	public bool AzimuthRightKeyboard;
	public bool AzimuthRightController;
	public bool AzimuthRightUp;

	/// <summary>
	/// ブーストダッシュ（ジャンプボタン２連打）検出
	/// </summary>
	public bool BoostDash;

	/// <summary>
	/// フロントステップ（前2回）検出
	/// </summary>
	public bool FrontStep;

	/// <summary>
	/// 左前ステップ（左上2回）検出
	/// </summary>
	public bool LeftFrontStep;

	/// <summary>
	/// 左ステップ（左2回）検出
	/// </summary>
	public bool LeftStep;

	/// <summary>
	/// 左後ステップ（左下2回）検出
	/// </summary>
	public bool LeftBackStep;

	/// <summary>
	/// 後ステップ（下2回）検出
	/// </summary>
	public bool BackStep;

	/// <summary>
	/// 右後ステップ（右下2回）検出
	/// </summary>
	public bool RightBackStep;

	/// <summary>
	/// 右ステップ（右2回）検出
	/// </summary>
	public bool RightStep;

	/// <summary>
	/// 右前ステップ（右上2回）検出
	/// </summary>
	public bool RightFrontStep;

	/// <summary>
	/// サーチボタン長押しによるアンロック
	/// </summary>
	public bool Unlock;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// 全フラグを折る
	/// </summary>
	public void Reset()
	{
		/// <summary>
		/// 方向キー/左スティック上（コントローラー.斜めは複数同時）
		/// </summary>	
		Top = false;

		/// <summary>
		/// 方向キー上を離した
		/// </summary>
		TopUp = false;

		/// <summary>
		/// 方向キー上を長押した
		/// </summary>
		TopLongPress = false;

		/// <summary>
		/// 方向キー/左スティック下
		/// </summary>
		Under = false;

		/// <summary>
		/// 方向キー下を離した
		/// </summary>
		UnderUp = false;

		/// <summary>
		/// 方向キー下を長押した
		/// </summary>
		UnderLongPress = false;

		/// <summary>
		/// 方向キー/左スティック左
		/// </summary>
		Left = false;

		/// <summary>
		/// 方向キー左を離した
		/// </summary>
		LeftUp = false;

		/// <summary>
		/// 方向キー左を長押した
		/// </summary>
		LeftLongPress = false;

		/// <summary>
		/// 方向キー/左スティック右
		/// </summary>
		Right = false;

		/// <summary>
		/// 方向キー右を離した
		/// </summary>
		RightUp = false;

		/// <summary>
		/// 方向キー右を長押した
		/// </summary>
		RightLongPress = false;

		/// <summary>
		/// 左上を押した
		/// </summary>
		LeftUpper = false;

		/// <summary>
		/// 左上を離した
		/// </summary>
		LeftUpperUp = false;

		/// <summary>
		/// 左下を押した
		/// </summary>
		LeftUnder = false;

		/// <summary>
		/// 左下を離した
		/// </summary>
		LeftUnderUp = false;

		/// <summary>
		/// 右下を押した
		/// </summary>
		RightUnder = false;

		/// <summary>
		/// 右下を離した
		/// </summary>
		RightUnderUp = false;

		/// <summary>
		/// 右上を押した
		/// </summary>
		RightUpper = false;

		/// <summary>
		/// 右上を押した
		/// </summary>
		RightUpperUp = false;



		/// <summary>
		/// 射撃・決定
		/// </summary>
		Shot = false;
		ShotKeyboad = false;
		ShotController = false;
		ShotUp = false;

		/// <summary>
		/// 射撃長押し
		/// </summary>
		Shotting = false;

		/// <summary>
		/// 格闘
		/// </summary>
		Wrestle = false;
		WrestleKeyboard = false;
		WrestleController = false;
		WrestleUp = false;

		/// <summary>
		/// 格闘長押し
		/// </summary>
		Wrestling = false;

		/// <summary>
		/// ジャンプ・キャンセル
		/// </summary>
		Jump = false;
		JumpKeyboard = false;
		JumpController = false;
		JumpUp = false;

		/// <summary>
		/// ジャンプ長押し
		/// </summary>
		Jumping = false;

		/// <summary>
		/// サーチ
		/// </summary>
		Search = false;
		SearchKeyboard = false;
		SearchController = false;
		SearchUp = false;

		/// <summary>
		/// コマンド
		/// </summary>
		Command = false;
		CommandKeyboard = false;
		CommandController = false;
		CommandUp = false;

		/// <summary>
		/// アイテム
		/// </summary>
		Item = false;
		ItemKeyboard = false;
		ItemController = false;
		ItemUp = false;

		/// <summary>
		/// メニュー
		/// </summary>
		Menu = false;
		MenuKeyboard = false;
		MenuController = false;
		MenuUp = false;

		/// <summary>
		/// サブ射撃
		/// </summary>
		SubShot = false;
		SubShotKeyboard = false;
		SubShotController = false;
		SubShotUp = false;

		/// <summary>
		/// 特殊射撃
		/// </summary>
		EXShot = false;
		EXShotKeyboard = false;
		EXShotController = false;
		EXShotUp = false;

		/// <summary>
		/// 特殊格闘
		/// </summary>
		EXWrestle = false;
		EXWrestleKeyboard = false;
		EXWrestleController = false;
		EXWrestleUp = false;

		/// <summary>
		/// 視点回転上(仰角）
		/// </summary>
		ElevationAngleUpper = false;
		ElevationAngleUpperKeyboard = false;
		ElevationAngleUpperController = false;
		ElevationAngleUpperUp = false;

		/// <summary>
		/// 視点回転下(仰角)
		/// </summary>
		ElevationAngleDown = false;
		ElevationAngleDownKeyboard = false;
		ElevationAngleDownController = false;
		ElevationAngleDownUp = false;

		/// <summary>
		/// 視点回転左(方位角)
		/// </summary>
		AzimuthLeft = false;
		AzimuthLeftKeyboard = false;
		AzimuthLeftController = false;
		AzimuthLeftUp = false;

		/// <summary>
		/// 視点回転右(方位角)
		/// </summary>
		AzimuthRight = false;
		AzimuthRightKeyboard = false;
		AzimuthRightController = false;
		AzimuthRightUp = false;

		/// <summary>
		/// ブーストダッシュ（ジャンプボタン２連打）検出
		/// </summary>
		BoostDash = false;

		/// <summary>
		/// フロントステップ（前2回）検出
		/// </summary>
		FrontStep = false;

		/// <summary>
		/// 左前ステップ（左上2回）検出
		/// </summary>
		LeftFrontStep = false;

		/// <summary>
		/// 左ステップ（左2回）検出
		/// </summary>
		LeftStep = false;

		/// <summary>
		/// 左後ステップ（左下2回）検出
		/// </summary>
		LeftBackStep = false;

		/// <summary>
		/// 後ステップ（下2回）検出
		/// </summary>
		BackStep = false;

		/// <summary>
		/// 右後ステップ（右下2回）検出
		/// </summary>
		RightBackStep = false;

		/// <summary>
		/// 右ステップ（右2回）検出
		/// </summary>
		RightStep = false;

		/// <summary>
		/// 右前ステップ（右上2回）検出
		/// </summary>
		RightFrontStep = false;

		/// <summary>
		/// サーチボタン長押しによるアンロック
		/// </summary>
		Unlock = false;
	}
}

/// <summary>
/// 現在のCPUのモード
/// </summary>
public enum CPUNowMode
{
	NONE,		// CPUを起動しない
	CPU,		// CPUを起動する
}
