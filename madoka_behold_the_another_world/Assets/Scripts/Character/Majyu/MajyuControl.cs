using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キャラクター「魔獣」を制御するためのスクリプト
/// </summary>
public class MajyuControl : MonoBehaviour
{
    /// <summary>
	/// 通常射撃
	/// </summary>
	public GameObject NormalShot;

    /// <summary>
	/// メイン射撃撃ち終わり時間
	/// </summary>
	private float MainshotEndtime;

    /// <summary>
	/// 種類（キャラごとに技数は異なるので別々に作らないとアウト
	/// </summary>
	public enum SkillType_Majyu
    {
        // 攻撃系
        // 射撃属性
        SHOT,                   // 通常射撃
                                // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        BACK_EX_WRESTLE,        // 後特殊格闘
                                // なし(派生がないとき用）
        NONE
    }

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

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
	public int FollowThrowShotID;       // 19
	public int FollowThrowRunShotID;    // 20
	public int FollowThrowAirShotID;    // 21
	public int Wrestle1ID;              // 22
	public int Wrestle2ID;              // 23
	public int Wrestle3ID;              // 24
	public int BackWrestleID;           // 25
	public int AirDashWrestleID;        // 26
	public int ReversalID;              // 27
	public int DamageID;                // 28
	public int DownID;                  // 29
	public int BlowID;                  // 30
	public int SpinDownID;              // 31

	void Awake()
	{
		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if (Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}
		// ハッシュID取得
		IdleID = Animator.StringToHash("Base Layer.MajyuIdle");		
		WalkID = Animator.StringToHash("Base Layer.MajyuWalk");
		JumpID = Animator.StringToHash("Base Layer.MajyuJump");		
		JumpingID = Animator.StringToHash("Base Layer.MajyuJumping");
		FallID = Animator.StringToHash("Base Layer.MajyuFall");
		LandingID = Animator.StringToHash("Base Layer.MajyuLanding");
		RunID = Animator.StringToHash("Base Layer.MajyuRun");
		AirDashID = Animator.StringToHash("Base Layer.MajyuAirDash");
		FrontStepID = Animator.StringToHash("Base Layer.MajyuFrontStep");
		LeftStepID = Animator.StringToHash("Base Layer.MajyuLeftStep");
		RightStepID = Animator.StringToHash("Base Layer.MajyuRightStep");
		BackStepID = Animator.StringToHash("Base Layer.MajyuBackStep");
		FrontStepBackID = Animator.StringToHash("Base Layer.MajyuFrontStepBack");
		LeftStepBackID = Animator.StringToHash("Base Layer.MajyuLeftStepBack");
		RightStepBackID = Animator.StringToHash("Base Layer.MajyuRightStepBack");
		BackStepBackID = Animator.StringToHash("Base Layer.MajyuBackStepBack");
		ShotID = Animator.StringToHash("Base Layer.MajyuShot");
		RunShotID = Animator.StringToHash("Base Layer.MajyuRunShot");
		AirShotID = Animator.StringToHash("Base Layer.MajyuAirShot");
		FollowThrowShotID = Animator.StringToHash("Base Layer.MajyuFollowThrowShot");
		FollowThrowRunShotID = Animator.StringToHash("Base Layer.MajyuFollowThrowRunShot");
		FollowThrowAirShotID = Animator.StringToHash("Base Layer.MajyuFollowThrowAirShot");
		Wrestle1ID = Animator.StringToHash("Base Layer.MajyuWrestle1");
		Wrestle2ID = Animator.StringToHash("Base Layer.MajyuWrestle2");
		Wrestle3ID = Animator.StringToHash("Base Layer.MajyuWrestle3");
		BackWrestleID = Animator.StringToHash("Base Layer.MajyuBackWrestle");
		AirDashWrestleID = Animator.StringToHash("Base Layer.MajyuAirDashWrestle");
		ReversalID = Animator.StringToHash("Base Layer.MajyuReversal");
		DamageID = Animator.StringToHash("Base Layer.MajyuDamage");
		DownID = Animator.StringToHash("Base Layer.MajyuDown");
		BlowID = Animator.StringToHash("Base Layer.MajyuBlow");
		SpinDownID = Animator.StringToHash("Base Layer.MajyuSpinDown");
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
