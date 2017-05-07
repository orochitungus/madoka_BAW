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
		//ArousalAttackCamera1.enabled = false;

		// 格闘専用カメラをOFFにする
		WrestleCamera.enabled = false;

		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if (Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}
		// ハッシュID取得
		IdleID = Animator.StringToHash("Base Layer.SconosciutoIdle");
		WalkID = Animator.StringToHash("Base Layer.SconosciutoWalk");
		JumpID = Animator.StringToHash("Base Layer.SconosciutoJump");
		JumpingID = Animator.StringToHash("Base Layer.SconosciutoJumping");
		FallID = Animator.StringToHash("Base Layer.SconosciutoFall");
		LandingID = Animator.StringToHash("Base Layer.SconosciutoLanding");
		RunID = Animator.StringToHash("Base Layer.SconosciutoRun");
		AirDashID = Animator.StringToHash("Base Layer.SconosciutoAirDash");
		FrontStepID = Animator.StringToHash("Base Layer.SconosciutoFrontStep");
		FrontStepBackID = Animator.StringToHash("Base Layer.SconosciutoFrontStepBack");
		LeftStepID = Animator.StringToHash("Base Layer.SconosciutoLeftStep");
		LeftStepBackID = Animator.StringToHash("Base Layer.SconosciutoLeftStepBack");
		RightStepID = Animator.StringToHash("Base Layer.SconosciutoRightStep");
		RightStepBackID = Animator.StringToHash("Base Layer.SconosciutoRightStepBack");
		BackStepID = Animator.StringToHash("Base Layer.SconosciutoBackStep");
		BackStepBackID = Animator.StringToHash("Base Layer.SconosciutoBackStepBack");
		ShotID = Animator.StringToHash("Base Layer.SconosciutoShot");		
		SubShotID = Animator.StringToHash("Base Layer.SconosciutoSubShot");
		EXShotID = Animator.StringToHash("Base Layer.SconosciutoEXShot");
		FollowThrowShotID = Animator.StringToHash("Base Layer.SconosciutoShotFollowthrow");
		FollowThrowSubShotID = Animator.StringToHash("Base Layer.SconosciutoSubShotFollwthrow");
		FollowThrowEXShotID = Animator.StringToHash("Base Layer.SconosciutoEXShotFollowthrow");
		Wrestle1ID = Animator.StringToHash("Base Layer.SconoscituoWrestle1");
		Wrestle2ID = Animator.StringToHash("Base Layer.SconoscituoWrestle2");
		Wrestle3ID = Animator.StringToHash("Base Layer.SconoscituoWrestle3");
		FrontWrestleID = Animator.StringToHash("Base Layer.SconocituoFrontWrestle");
		LeftWrestleID = Animator.StringToHash("Base Layer.SconoscituoLeftWrestle");
		RightWrestleID = Animator.StringToHash("Base Layer.SconoscituoRightWrestle");
		BackWrestleID = Animator.StringToHash("Base Layer.SconoscituoBackWrestle");
		AirDashWrestleID = Animator.StringToHash("Base Layer.SconocituoAirDashWrestle");
		EXWrestleID = Animator.StringToHash("Base Layer.SconoscituoEXWrestle");
		EXFrontWrestleID = Animator.StringToHash("Base Layer.SconoscituoFrontEXWrestle");
		EXBackWrestleID = Animator.StringToHash("Base Layer.SconoscituoBackEXWrestle");
		ReversalID = Animator.StringToHash("Base Layer.SconoscituoReversal");
		ArousalAttackID = Animator.StringToHash("Base Layer.SconoscituoArousalAttack");
		DamageID = Animator.StringToHash("Base Layer.SconoscituoDamage");
		DownID = Animator.StringToHash("Base Layer.SconoscituoDown");
		BlowID = Animator.StringToHash("Base Layer.SconoscituoBlow");
		SpinDownID = Animator.StringToHash("Base Layer.SconoscituoSpindown");


		// リバーサルとダウンのIDを取得
		ReversalHash = ReversalID;
		DownHash = DownID;
	}

	// Use this for initialization
	void Start()
	{
		// 誰であるかを定義(インスペクターで拾う)
		// レベル・攻撃力レベル・防御力レベル・残弾数レベル・ブースト量レベル・覚醒ゲージレベルを初期化
		SettingPleyerLevel();

		// ジャンプ硬直
		JumpWaitTime = 0.5f;

		//着地硬直
		LandingWaitTime = 1.0f;

		WalkSpeed = 1.0f;                            // 移動速度（歩行の場合）
		RunSpeed = 15.0f;                            // 移動速度（走行の場合）
		AirDashSpeed = 60.0f;                        // 移動速度（空中ダッシュの場合）
		AirMoveSpeed = 10.0f;                        // 移動速度（空中慣性移動の場合）
		RiseSpeed = 3.0f;                            // 上昇速度

		// ブースト消費量
		JumpUseBoost = 10;       // ジャンプ時
		DashCancelUseBoost = 10;   // ブーストダッシュ時
		StepUseBoost = 10;         // ステップ時
		BoostLess = 0.5f;        // ジャンプの上昇・BD時の1F当たりの消費量

		// ステップ移動距離
		StepMoveLength = 10.0f;

		// ステップ初速（X/Z軸）
		StepInitialVelocity = 100.0f;
		// ステップ時の１F当たりの移動量
		StepMove1F = 1.0f;
		// ステップ終了時硬直時間
		StepBackTime = 0.4f;

		// コライダの地面からの高さ
		ColliderHeight = 1.5f;

		// ロックオン距離
		RockonRange = 100.0f;

		// ロックオン限界距離
		RockonRangeLimit = 200.0f;

		// ショットのステート
		Shotmode = ShotMode.NORMAL;

		// 弾のステート
		BulletMoveDirection = Vector3.zero;

		// メイン射撃撃ち終わり時間
		MainshotEndtime = 0.0f;
		// サブ射撃撃ち終わり時間
		SubshotEndtime = 0.0f;
		// 特殊射撃撃ち終わり時間
		ExshotEndtime = 0.0f;

		// 共通ステートを初期化
		FirstSetting(AnimatorUnit, 0);

		this.UpdateAsObservable().Subscribe(_ =>
		{
			// N格３段目時のみカメラ変更
			if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle3ID)
			{
				WrestleCamera.enabled = true;
			}
			else
			{
				WrestleCamera.enabled = false;
			}
		});

		this.UpdateAsObservable().Where(_ => IsPlayer == CHARACTERCODE.PLAYER).Subscribe(_ =>
		{
			// インターフェース制御
			// PC/僚機共通
			for (int i = 0; i < 3; i++)
			{
				// 顔グラフィック/現在HP/最大HP
				if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.MADOKA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.SAYAKA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.HOMURA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.HOMURA_B);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_MAMI)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.MAMI);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_KYOKO)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.KYOKO);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_YUMA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.YUMA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.KIRIKA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_ORIKO)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.ORIKO);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.ULTIMADOKA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_SCHONO)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.SCONO);
				}
				// 現在HP
				Battleinterfacecontroller.NowPlayerHP[i] = savingparameter.GetNowHP(savingparameter.GetNowParty(i));

				// 最大HP
				Battleinterfacecontroller.MaxPlayerHP[i] = savingparameter.GetMaxHP(savingparameter.GetNowParty(i));
			}

			// PCのみ
			// 名前
			Battleinterfacecontroller.CharacterName.text = "Sconoscituo";
			// プレイヤーレベル
			Battleinterfacecontroller.PlayerLv.text = "Level - " + savingparameter.GetNowLevel(savingparameter.GetNowParty(0)).ToString();
			// ブースト量
			Battleinterfacecontroller.NowBoost = Boost;
			// 最大ブースト量
			Battleinterfacecontroller.MaxBoost = BoostLevel * BoostGrowth + Boost_OR;
			// 覚醒ゲージ量
			Battleinterfacecontroller.NowArousal = Arousal;
			// 最大覚醒ゲージ量
			Battleinterfacecontroller.MaxArousal = (ArousalLevel - 1) * ArousalGrowth + Arousal_OR;
			// 基本覚醒ゲージ量
			Battleinterfacecontroller.BiasArousal = Arousal_OR;
			// 装備アイテム
			Battleinterfacecontroller.ItemName.text = savingparameter.GetNowEquipItemString();
			// 装備アイテム個数
			Battleinterfacecontroller.ItemNumber.text = savingparameter.GetItemNum(savingparameter.GetNowEquipItem()).ToString();

			// 武装ゲージ
			// 4番目と5番目は消す
			Battleinterfacecontroller.Weapon4.gameObject.SetActive(false);
			Battleinterfacecontroller.Weapon5.gameObject.SetActive(false);
			// 射撃
			Battleinterfacecontroller.Weapon3.Kind.text = "Shot";
			Battleinterfacecontroller.Weapon3.WeaponGraphic.sprite = ShotIcon;
			Battleinterfacecontroller.Weapon3.NowBulletNumber = BulletNum[(int)ShotType.NORMAL_SHOT];
			Battleinterfacecontroller.Weapon3.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon3.UseChargeGauge = false;
			// 1発でも使えれば使用可能
			if (BulletNum[(int)ShotType.NORMAL_SHOT] > 0)
			{
				Battleinterfacecontroller.Weapon3.Use = true;
			}
			else
			{
				Battleinterfacecontroller.Weapon3.Use = false;
			}

			// サブ射撃
			Battleinterfacecontroller.Weapon2.Kind.text = "Sub Shot";
			Battleinterfacecontroller.Weapon2.WeaponGraphic.sprite = SubShotIcon;
			Battleinterfacecontroller.Weapon2.NowBulletNumber = BulletNum[(int)ShotType.SUB_SHOT];
			Battleinterfacecontroller.Weapon2.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon2.UseChargeGauge = false;
			// 1発でも使えれば使用可能(リロード時は0になる)
			if (BulletNum[(int)ShotType.SUB_SHOT] != 0)
			{
				Battleinterfacecontroller.Weapon2.Use = true;
			}
			else
			{
				Battleinterfacecontroller.Weapon2.Use = false;
			}

			// 特殊射撃
			Battleinterfacecontroller.Weapon1.Kind.text = "EX Shot";
			Battleinterfacecontroller.Weapon1.WeaponGraphic.sprite = ExShotIcon;
			Battleinterfacecontroller.Weapon1.NowBulletNumber = BulletNum[(int)ShotType.EX_SHOT];
			Battleinterfacecontroller.Weapon1.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon1.UseChargeGauge = false;
			// 1発でも使えれば使用可能
			if (BulletNum[(int)ShotType.EX_SHOT] > 0)
			{
				Battleinterfacecontroller.Weapon1.Use = true;
			}
			else
			{
				Battleinterfacecontroller.Weapon1.Use = false;
			}
		});


	}

	// Update is called once per frame
	void Update () 
	{
		// 共通アップデート処理(時間停止系の状態になると入力は禁止)
		bool isspindown = false;
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SpinDownID)
		{
			isspindown = true;
		}
		if (Update_Core(isspindown, AnimatorUnit, DownID, AirDashID, AirShotID, JumpingID, FallID, IdleID, BlowID, RunID, FrontStepID, LeftStepID, RightStepID, BackStepID, DamageID))
		{
			UpdateAnimation();
			// リロード実行           
			// メイン射撃
			ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_reloadtime, ref MainshotEndtime);
			// サブ射撃
			ReloadSystem.AllTogether(ref BulletNum[(int)ShotType.SUB_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_reloadtime, ref SubshotEndtime);
			// 特殊射撃
			ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.EX_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_reloadtime, ref ExshotEndtime);
		}
	}

	void UpdateAnimation()
	{
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == IdleID)
		{
			Animation_Idle(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == WalkID)
		{
			Animation_Walk(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpID)
		{
			Animation_Jump(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpingID)
		{
			Animation_Jumping(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FallID)
		{
			Animation_Fall(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LandingID)
		{
			Animation_Landing(AnimatorUnit);
		}
	}

	/// <summary>
	/// Jumpingへ移行する（JumpのAnimationの最終フレームで実行する）
	/// </summary>
	public void JumpingMigration()
	{
		AnimatorUnit.SetTrigger("Jumping");
	}
}
