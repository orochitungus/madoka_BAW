using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キャラクター「スコノシュート」を制御するためのスクリプト
/// </summary>
public class SconosciutoControl : CharacterControlBase
{
	/// <summary>
	/// 通常射撃用の用の弾丸
	/// </summary>
	public GameObject NormalShotBullet;

	/// <summary>
	/// サブ射撃用の弾丸
	/// </summary>
	public GameObject SubShotBullet;

	/// <summary>
	/// 特殊射撃用のレーザー
	/// </summary>
	public GameObject EXShotLaser;

	/// <summary>
	/// サブ射撃フック
	/// </summary>
	public GameObject SubShotHock;

	/// <summary>
	/// 特殊射撃フック
	/// </summary>
	public GameObject EXShotHock;

	/// <summary>
	/// メイン射撃撃ち終わり時間
	/// </summary>
	private float MainshotEndtime;

	/// <summary>
	/// サブ射撃撃ち終わり時間
	/// </summary>
	private float SubshotEndtime;

	/// <summary>
	/// 特殊射撃撃ち終わり時間
	/// </summary>
	private float ExshotEndtime;

	/// <summary>
	/// 攻撃の種類
	/// </summary>
	public enum Skilltype_Sconosciuto
	{
		SHOT,                   // 通常射撃
		SUB_SHOT,               // サブ射撃
		EX_SHOT,                // 特殊射撃
								// 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
		WRESTLE_1,              // N格1段目
		WRESTLE_2,              // N格2段目
		WRESTLE_3,              // N格3段目
		FRONT_WRESTLE_1,        // 前格闘1段目
		LEFT_WRESTLE_1,         // 左横格闘1段目
		RIGHT_WRESTLE_1,        // 右横格闘1段目
		BACK_WRESTLE,           // 後格闘（防御）
		AIRDASH_WRESTLE,        // 空中ダッシュ格闘
		EX_WRESTLE_1,           // 特殊格闘1段目
		EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
		BACK_EX_WRESTLE,        // 後特殊格闘
		NONE                    // なし(派生がないとき用）
	}

	/// <summary>
	/// 覚醒技基礎攻撃力
	/// </summary>
	private const int _BasisOffensive = 470;

	/// <summary>
	/// 覚醒技攻撃力成長係数
	/// </summary>
	private const int _GrowthcOffecientStr = 5;

	/// <summary>
	/// 覚醒技ダウン値
	/// </summary>
	private const int _DownratioArousal = 5;

	/// <summary>
	/// 覚醒技専用カメラ1個目
	/// </summary>
	public Camera ArousalAttackCamera1;

	/// <summary>
	/// Ｎ格闘３段目専用カメラ
	/// </summary>
	public Camera WrestleCamera;

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

	/// <summary>
	/// 通常射撃のアイコン
	/// </summary>
	public Sprite ShotIcon;

	/// <summary>
	/// サブ射撃のアイコン
	/// </summary>
	public Sprite SubShotIcon;

	/// <summary>
	/// 特殊射撃のアイコン
	/// </summary>
	public Sprite ExShotIcon;

	/// <summary>
	/// 各種アニメのID.コメント内はAnimatorの管理用ID.武装系以外は全員共通にすること
	/// </summary>
	public int IdleID;                  // 0
	public int WalkID;                  // 1
	public int JumpID;                  // 2
	public int JumpingID;               // 3
	public int FallID;                  // 4
	public int LandingID;               // 5
	public int RunID;                   // 6
	public int AirDashID;               // 7
	public int FrontStepID;             // 8
	public int LeftStepID;              // 9
	public int RightStepID;             // 10
	public int BackStepID;              // 11
	public int FrontStepBackID;         // 12
	public int LeftStepBackID;          // 13
	public int RightStepBackID;         // 14
	public int BackStepBackID;          // 15
	public int ShotID;                  // 16
	public int RunShotID;               // 17
	public int AirShotID;               // 18
	public int SubShotID;               // 19
	public int EXShotID;                // 20
	public int FollowThrowShotID;       // 21
	public int FollowThrowRunShotID;    // 22
	public int FollowThrowAirShotID;    // 23
	public int FollowThrowSubShotID;    // 24
	public int FollowThrowEXShotID;     // 25
	public int Wrestle1ID;              // 26
	public int Wrestle2ID;              // 27
	public int Wrestle3ID;              // 28
	public int FrontWrestleID;          // 29
	public int LeftWrestleID;           // 30
	public int RightWrestleID;          // 31
	public int BackWrestleID;           // 32
	public int AirDashWrestleID;        // 33
	public int EXWrestleID;             // 34
	public int EXFrontWrestleID;        // 35
	public int EXBackWrestleID;         // 36
	public int ReversalID;              // 37
	public int ArousalAttackID;         // 38
	public int DamageID;                // 39
	public int DownID;                  // 40
	public int BlowID;                  // 41
	public int SpinDownID;              // 42

	void Awake()
	{
		// 空中ダッシュＩＤを保持（CharacterControlBaseで使う)
		CancelDashID = 7;
		// 覚醒技専用カメラをOFFにする
		ArousalAttackCamera1.enabled = false;

		// 格闘専用カメラをOFFにする
		WrestleCamera.enabled = false;

		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if (Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}


	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
