﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キャラクター「暁美　ほむら（弓）」を制御するためのスクリプト
/// </summary>
public class HomuraBowControl : CharacterControlBase 
{
	/// <summary>
	/// 通常射撃用の用の矢
	/// </summary>
	public GameObject NormalArrow;

	/// <summary>
	/// チャージ射撃用の矢
	/// </summary>
	public GameObject ChargeShotArrow;

	/// <summary>
	/// サブ射撃用の矢
	/// </summary>
	public GameObject SubShotArrow;

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
	/// 種類（キャラごとに技数は異なるので別々に作らないとアウト
	/// </summary>
	public enum SkillType_Homura_B
	{
		// 攻撃系
		// 射撃属性
		SHOT,                   // 通常射撃
		CHARGE_SHOT,            // 射撃チャージ
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
								// アビリティ系
		DISABLE_BLUNT_FOOT,         // 鈍足無効
		DISABLE_ROCKON_IMPOSSIBLE,  // ロックオン不可無効
		DISABLE_DESTRUCTION_MAGIC,  // 魔力破壊無効
									// なし(派生がないとき用）
		NONE
	}

	/// <summary>
	/// 覚醒技基礎攻撃力
	/// </summary>
	private const int _BasisOffensive = 270;

	/// <summary>
	/// 覚醒技攻撃力成長係数
	/// </summary>
	private const int _GrowthcOffecientStr = 5;

	/// <summary>
	/// 覚醒技ダウン値
	/// </summary>
	private const int _DownratioArousal = 5;

	/// <summary>
	/// 羽根と判定が出るときの間隔
	/// </summary>
	private const float _WingappearTime = 0.1f;

	/// <summary>
	/// 羽根と判定が出るときのタイマー
	/// </summary>
	private float _WingAppearCounter;

	/// <summary>
	/// 羽根と判定が何番目のボーンであるかを示すカウンター
	/// </summary>
	private int _WingboneCounter;

	/// <summary>
	/// 左側の羽根と判定をセットしたフラグ
	/// </summary>
	private bool _LeftwingSet;

	/// <summary>
	/// ボーンの最大数
	/// </summary>
	private const int _MaxboneNum = 17;

	/// <summary>
	/// 専用カメラ1個目
	/// </summary>
	public Camera ArousalAttackCamera1;

	/// <summary>
	/// 専用カメラ2個目
	/// </summary>
	public Camera ArousalAttackCamera2;

	/// <summary>
	/// 総発動時間(秒）
	/// </summary>
	private const float ArousalAttackTotal = 10.0f;

	/// <summary>
	/// 累積発動時間（秒）
	/// </summary>
	private float ArousalAttackTime;

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

	void Awake()
	{
		// 覚醒技専用カメラをOFFにする
		ArousalAttackCamera1.enabled = false;
		ArousalAttackCamera2.enabled = false;

		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if(Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}
	}

	// Use this for initialization
	void Start () 
	{		

		this.UpdateAsObservable().Where(_ => IsPlayer == CHARACTERCODE.PLAYER).Subscribe(_ => 
		{
			// インターフェース制御		
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
