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
			Battleinterfacecontroller.Weapon2.NowBulletNumber = BulletNum[1];
			Battleinterfacecontroller.Weapon2.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][1].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][1].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon2.UseChargeGauge = false;
			// 1発でも使えれば使用可能(リロード時は0になる)
			if (BulletNum[1] != 0)
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
			Battleinterfacecontroller.Weapon1.NowBulletNumber = BulletNum[2];
			Battleinterfacecontroller.Weapon1.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][2].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][2].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon1.UseChargeGauge = false;
			// 1発でも使えれば使用可能
			if (BulletNum[2] > 0)
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
			ReloadSystem.AllTogether(ref BulletNum[1], Time.time, Character_Spec.cs[(int)CharacterName][1].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][1].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][1].m_reloadtime, ref SubshotEndtime);
			// 特殊射撃
			ReloadSystem.OneByOne(ref BulletNum[2], Time.time, Character_Spec.cs[(int)CharacterName][2].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][2].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][2].m_reloadtime, ref ExshotEndtime);
		}
	}

	void UpdateAnimation()
	{
		ShowAirDashEffect = false;
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
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunID)
		{
			Animation_Run(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirDashID)
		{
			ShowAirDashEffect = true;
			Animation_AirDash(AnimatorUnit);
		}
		// 前ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		// 左ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		// 右ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		// 後ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		// 前ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		// 左ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		// 右ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		// 後ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		// リバーサル
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ReversalID)
		{
			Reversal();
		}
		// ダメージ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DamageID)
		{
			Damage(AnimatorUnit);
		}
		// ダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DownID)
		{
			Down(AnimatorUnit);
		}
		// 吹き飛び
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BlowID)
		{
			Blow(AnimatorUnit);
		}
		// きりもみダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SpinDownID)
		{
			SpinDown(AnimatorUnit);
		}
		// 射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ShotID)
		{
			Shot();
		}
		// 走行射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunShotID)
		{
			Shot();
		}
		// 空中射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirShotID)
		{
			Shot();
		}
		// 射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowShotID)
		{
			Shot();
		}
		// 走行射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowRunShotID)
		{
			Shot();
		}
		// 空中射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowAirShotID)
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowSubShotID)
		{
			SubShot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowEXShotID)
		{
			ExShot();
		}
		// 通常格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle1ID)
		{
			Wrestle1(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle2ID)
		{
			Wrestle2(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle3ID)
		{
			Wrestle3(AnimatorUnit);
		}
		// 前格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontWrestleID)
		{
			FrontWrestle1(AnimatorUnit);
		}
		// 横格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftWrestleID)
		{
			LeftWrestle1(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightWrestleID)
		{
			RightWrestle1(AnimatorUnit);
		}
		// 後格闘（ガード）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackWrestleID)
		{
			BackWrestle(AnimatorUnit);
		}
		if (ShowAirDashEffect)
		{
			AirDashEffect.SetActive(true);
		}
		else
		{
			AirDashEffect.SetActive(false);
		}
	}

	/// <summary>
	/// アイドル時のアニメーションを制御
	/// </summary>
	/// <param name="animator"></param>
	protected override void Animation_Idle(Animator animator)
	{
		// 攻撃したかフラグ
		bool attack = false;
		// くっついている弾丸系のエフェクトを消す
		DestroyArrow();

		// 格闘の累積時間を初期化
		Wrestletime = 0;
		// 地上にいるか？(落下開始時は一応禁止）
		if (IsGrounded)
		{
			attack = AttackDone();
		}
		// キャンセルダッシュ受付
		if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
		{
			// 地上でキャンセルすると浮かないので浮かす
			if (IsGrounded)
			{
				transform.Translate(new Vector3(0, 1, 0));
			}
			CancelDashDone(AnimatorUnit);
		}
		// 攻撃した場合はステートが変更されるので、ここで終了
		if (!attack)
		{
			base.Animation_Idle(animator);
		}
	}

	/// <summary>
	/// Jumpingへ移行する（JumpのAnimationの最終フレームで実行する）
	/// </summary>
	public void JumpingMigration()
	{
		AnimatorUnit.SetTrigger("Jumping");
	}

	protected override void Animation_Run(Animator animator)
	{
		base.Animation_Run(animator);
		AttackDone(true, false);
	}

	/// <summary>
	/// 攻撃行動全般(RunとAirDashは特殊なので使用しない）
	/// </summary>
	/// <param name="run"></param>
	/// <param name="AirDash"></param>
	public bool AttackDone(bool run = false, bool AirDash = false)
	{
		DestroyWrestle();
		DestroyArrow();
		// 覚醒中は覚醒技を発動
		if (HasArousalAttackInput && IsArousal)
		{
			// アーマーをONにする
			IsArmor = true;

			// TODO:覚醒技前処理をここに入れる
			
			return true;
		}
		// サブ射撃でサブ射撃へ移行
		else if (HasSubShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit);
			}
			// サブ射撃実行
			SubShotDone();
			return true;
		}
		// 特殊射撃で特殊射撃へ移行
		else if (HasExShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit);
			}
			// 特殊射撃実行
			ExShotDone();
			return true;
		}
		// 特殊格闘で特殊格闘へ移行
		else if (HasExWrestleInput)
		{
			// 前特殊格闘(ブーストがないと実行不可)
			if (HasFrontInput && Boost > 0)
			{
				// TODO:前特殊格闘実行
				
			}
			// 空中で後特殊格闘(ブーストがないと実行不可）
			else if (HasBackInput && !IsGrounded && Boost > 0)
			{
				// TODO:後特殊格闘実行
			}
			// それ以外
			else
			{
				// TODO:特殊格闘実行
			}
			return true;
		}
		// 射撃で射撃へ移行
		else if (HasShotInput)
		{
			if (run)
			{
				if (IsRockon)
				{
					// ①　transform.TransformDirection(Vector3.forward)でオブジェクトの正面の情報を得る
					var forward = transform.TransformDirection(Vector3.forward);
					// ②　自分の場所から対象との距離を引く
					// カメラからEnemyを求める
					var target = MainCamera.transform.GetComponentInChildren<Player_Camera_Controller>();
					var targetDirection = target.Enemy.transform.position - transform.position;
					// ③　①と②の角度をVector3.Angleで取る　			
					float angle = Vector3.Angle(forward, targetDirection);
					// 角度60度以内なら上体回しで撃つ（歩き撃ち限定で上記の矢の方向ベクトルを加算する）
					if (angle < 60)
					{
						// TODO:上体補正

						RunShotDone = true;
						// 走行射撃実行
						ShotDone(true);
					}
					// それ以外なら強制的に停止して（立ち撃ちにして）撃つ
					else
					{
						// 強制停止実行
						EmagencyStop(AnimatorUnit);
						// 射撃実行
						ShotDone(false);
					}
				}
				// 非ロック状態なら歩き撃ちフラグを立てる
				else
				{
					RunShotDone = true;
					// 射撃実行
					ShotDone(true);
				}
			}
			else
			{
				// 射撃実行
				ShotDone(false);
			}
			return true;
		}
		// 格闘で格闘へ移行
		else if (HasWrestleInput)
		{
			// 空中ダッシュ中で空中ダッシュ格闘へ移行
			if (AirDash)
			{
				// 空中ダッシュ格闘実行
				AirDashWrestleDone(AnimatorUnit, AirDashSpeed, 11);
			}
			// 前格闘で前格闘へ移行
			else if (HasFrontInput)
			{
				// 前格闘実行(Character_Spec.cs参照)
				WrestleDone(AnimatorUnit, 4, "FrontWrestle");
			}
			// 左格闘で左格闘へ移行
			else if (HasLeftInput)
			{
				// 左格闘実行(Character_Spec.cs参照)
				WrestleDone_GoAround_Left(AnimatorUnit, 7);
			}
			// 右格闘で右格闘へ移行
			else if (HasRightInput)
			{
				// 右格闘実行(Character_Spec.cs参照)
				WrestleDone_GoAround_Right(AnimatorUnit, 8);
			}
			// 後格闘で後格闘へ移行
			else if (HasBackInput)
			{
				// 後格闘実行（ガード）(Character_Spec.cs参照)
				GuardDone(AnimatorUnit, 9);
			}
			else
			{
				// それ以外ならN格闘実行(2段目と3段目の追加入力はWrestle1とWrestle2で行う
				WrestleDone(AnimatorUnit, 4, "Wrestle1");
			}
			return true;
		}
		return false;
	}

	protected override void Animation_Jumping(Animator animator)
	{
		base.Animation_Jumping(animator);
		AttackDone();
	}

	protected override void Animation_Fall(Animator animator)
	{
		base.Animation_Fall(animator);
		AttackDone();
	}	

	protected override void Animation_AirDash(Animator animator)
	{
		base.Animation_AirDash(animator);
		AttackDone(false, true);
	}

	/// <summary>
	/// Idle状態に戻す
	/// </summary>
	public void ReturnToIdle()
	{
		// 矢や格闘判定も消しておく
		DestroyArrow();
		DestroyWrestle();
		ReturnMotion(AnimatorUnit);
	}

	protected override void DestroyArrow()
	{
		base.DestroyArrow();
		// サブ射撃
		int ChildCount = SubShotHock.transform.childCount;
		for (int i = 0; i < ChildCount; i++)
		{
			Transform child = SubShotHock.transform.GetChild(i);
			Destroy(child.gameObject);
		}
		// 特殊射撃
		int childcount2 = EXShotHock.transform.childCount;
		for(int i=0; i<childcount2; i++)
		{
			Transform child = EXShotHock.transform.GetChild(i);
			Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// メイン射撃の装填開始
	/// </summary>
	/// <param name="ruhshot">走行射撃であるか否か</param>
	public void ShotDone(bool runshot)
	{
		// 歩行時は上半身のみにして走行（下半身のみ）と合成
		if (runshot)
		{
			// アニメーション合成処理＆再生
			AnimatorUnit.SetTrigger("RunShot");
			Debug.Log("RunShotDone!!");
		}
		// 立ち射撃か空中射撃
		else
		{
			AnimatorUnit.SetTrigger("Shot");
		}
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
	}

    
	protected override void Shot()
	{
		// キャンセルダッシュ受付
		if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
		{
			// 地上でキャンセルすると浮かないので浮かす
			if (IsGrounded)
			{
				transform.Translate(new Vector3(0, 1, 0));
			}
			CancelDashDone(AnimatorUnit);
		}
		base.Shot();
	}

	/// <summary>
	/// 通常射撃の装填を行う（アニメーションファイルの装填フレームにインポートする）
	/// </summary>
	public void ShootloadNormalShot()
	{
		// 弾があるとき限定（チャージショット除く）
		if (BulletNum[0] > 0)
		{
			// ロックオン時本体の方向を相手に向ける       
			if (IsRockon)
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[0]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[0] == Character_Spec.cs[(int)CharacterName][0].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][0].m_OriginalBulletNum - 1)
			{
				MainshotEndtime = Time.time;
			}

			// 矢の出現ポジションをフックと一致させる
			Vector3 pos = MainShotRoot.transform.position;
			Quaternion rot = Quaternion.Euler(MainShotRoot.transform.rotation.eulerAngles.x, MainShotRoot.transform.rotation.eulerAngles.y, MainShotRoot.transform.rotation.eulerAngles.z);
			// 矢を出現させる			
			var obj = Instantiate(NormalShotBullet, pos, rot);
			// 親子関係を再設定する(=矢をフックの子にする）
			if (obj.transform.parent == null)
			{
				obj.transform.parent = MainShotRoot.transform;
				// 矢の親子関係を付けておく
				obj.transform.GetComponent<Rigidbody>().isKinematic = true;
			}			
		}
	}

	/// <summary>
	/// 射撃（射出）射撃のアニメーションファイルの射出フレームにインポートする
	/// </summary>
	/// <param name="type"></param>
	public void Shooting()
	{
		// 通常射撃の矢
		var arrow = GetComponentInChildren<SconosciutoNormalShot>();
		

		if (arrow != null)
		{
			// 矢のスクリプトの速度を設定する
			// メイン弾速設定
			arrow.ShotSpeed = Character_Spec.cs[(int)CharacterName][0].m_Movespeed;			
				
			// 矢の方向を決定する(本体と同じ方向に向けて打ち出す。ただしノーロックで本体の向きが0のときはベクトルが0になるので、このときだけはカメラの方向に飛ばす）
			if (IsRockon && RunShotDone)
			{
				// ロックオン対象の座標を取得
				var target = GetComponentInChildren<Player_Camera_Controller>();
				// 対象の座標を取得
				Vector3 targetpos = target.Enemy.transform.position;
				// 補正値込みの胸部と本体の回転ベクトルを取得
				// 本体
				Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
				// 胸部
				Vector3 normalizeRot_OR = BrestObject.transform.rotation.eulerAngles;
                // 本体と胸部と矢の補正値分回転角度を合成
                Vector3 addrot = mainrot.eulerAngles;// + normalizeRot_OR - new Vector3(0, 74.0f, 0);
				Quaternion qua = Quaternion.Euler(addrot);
				// forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
				Vector3 normalizeRot = (qua) * Vector3.forward;
				// 移動ベクトルを確定する
				arrow.MoveDirection = Vector3.Normalize(normalizeRot);
			}
			// ロックオンしているとき
			else if (IsRockon)
			{
				// ロックオン対象の座標を取得
				var target = GetComponentInChildren<Player_Camera_Controller>();
				// 対象の座標を取得
				Vector3 targetpos = target.Enemy.transform.position;
				// 本体の回転角度を拾う
				Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
				// 正規化して代入する
				Vector3 normalizeRot = mainrot * Vector3.forward;

				// 通常射撃の矢
				arrow.MoveDirection = Vector3.Normalize(normalizeRot);				
			}
			// ロックオンしていないとき
			else
			{
				// 本体角度が0の場合カメラの方向を射出角とし、正規化して代入する
				if (transform.rotation.eulerAngles == Vector3.zero)
				{
					// ただしそのままだとカメラが下を向いているため、一旦その分は補正する
					Quaternion rotateOR = MainCamera.transform.rotation;
					Vector3 rotateOR_E = rotateOR.eulerAngles;
					rotateOR_E.x = 0;
					rotateOR = Quaternion.Euler(rotateOR_E);

					// 通常射撃の矢
					if (arrow != null)
					{
						arrow.MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
					}
				}
				// それ以外は本体の角度を射出角にする
				else
				{
					// 通常射撃の矢
					arrow.MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);										
				}				
			}
			// 弾丸の向きを合わせる
			arrow.transform.rotation = transform.rotation;
			Debug.Log(arrow.transform.rotation.eulerAngles);

			// 矢の移動ベクトルを代入する
			// 通常射撃
			if (arrow != null)
			{
				BulletMoveDirection = arrow.MoveDirection;
			}


			// 攻撃力を代入する
			// 攻撃力を決定する(ここの2がスキルのインデックス。下も同様）
			OffensivePowerOfBullet = Character_Spec.cs[(int)CharacterName][0].m_OriginalStr + Character_Spec.cs[(int)CharacterName][0].m_GrowthCoefficientStr * (this.StrLevel - 1);
			// ダウン値を決定する
			DownratioPowerOfBullet = Character_Spec.cs[(int)CharacterName][0].m_DownPoint;
			// 覚醒ゲージ増加量を決定する
			ArousalRatioOfBullet = Character_Spec.cs[(int)CharacterName][0].m_arousal;

			Shotmode = ShotMode.SHOT;

			// 固定状態を解除
			// ずれた本体角度を戻す(Yはそのまま）
			transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
			transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

			// 硬直時間を設定
			AttackTime = Time.time;
			// 射出音を再生する
			AudioManager.Instance.PlaySE("shot_hand_gun");
		}
		// 弾がないときはとりあえずフラグだけは立てておく
		else
		{
			// 硬直時間を設定
			AttackTime = Time.time;
			Shotmode = ShotMode.SHOTDONE;
		}

		// フォロースルーへ移行する(チャージショットの場合は別タイミング）
		// 走行時
		if (RunShotDone)
		{
			AnimatorUnit.SetTrigger("RunFollowThrow");
		}
		// 通常時
		else
		{
			AnimatorUnit.SetTrigger("ShotFollowThrow");
		}
		
	}

	/// <summary>
	/// サブ射撃実行
	/// </summary>
	public void SubShotDone()
	{
		AnimatorUnit.SetTrigger("SubShot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

    protected override void SubShot()
    {
        // キャンセルダッシュ受付
        if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (IsGrounded)
            {
                transform.Translate(new Vector3(0, 1, 0));
            }
            CancelDashDone(AnimatorUnit);
        }
        base.SubShot();
    }

    /// <summary>
    /// サブ射撃の装填を行う（アニメーションファイルの装填フレームにインポートする）
    /// </summary>
    public void ShootLoadSubShot()
    {
		// 弾があるとき限定
		if (BulletNum[1] > 0)
		{
			// ロックオン時本体の方向を相手に向ける       
			if (IsRockon)
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[1]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[1] == Character_Spec.cs[(int)CharacterName][1].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][1].m_OriginalBulletNum - 1)
			{
				SubshotEndtime = Time.time;
			}

			// 矢の出現ポジションをフックと一致させる
			Vector3 pos = SubShotHock.transform.position;
			Quaternion rot = Quaternion.Euler(SubShotHock.transform.rotation.eulerAngles.x, SubShotHock.transform.rotation.eulerAngles.y, SubShotHock.transform.rotation.eulerAngles.z);
			// 矢を出現させる			
			var obj = Instantiate(SubShotBullet, pos, rot);
			// 親子関係を再設定する(=矢をフックの子にする）
			if (obj.transform.parent == null)
			{
				obj.transform.parent = SubShotHock.transform;
				// 矢の親子関係を付けておく
				obj.transform.GetComponent<Rigidbody>().isKinematic = true;
			}
		}

	}

	/// <summary>
	/// サブ射撃の射出のところにインポートする
	/// </summary>
	public void ShootingSubShot()
	{
		// サブ射撃の弾丸
		var arrow = GetComponentInChildren<SconosciutoSubShot>();
		if (arrow != null)
		{
			// 弾速設定
			arrow.ShotSpeed = Character_Spec.cs[(int)CharacterName][1].m_Movespeed;
			// ロックオンしているとき
			if (IsRockon)
			{
				// ロックオン対象の座標を取得
				var target = GetComponentInChildren<Player_Camera_Controller>();
				// 対象の座標を取得
				Vector3 targetpos = target.Enemy.transform.position;
				// 本体の回転角度を拾う
				Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
				// 正規化して代入する
				Vector3 normalizeRot = mainrot * Vector3.forward;

				// サブ射撃の弾丸
				arrow.MoveDirection = Vector3.Normalize(normalizeRot);
			}
			// ロックオンしていないとき
			else
			{
				// 本体角度が0の場合カメラの方向を射出角とし、正規化して代入する
				if (transform.rotation.eulerAngles == Vector3.zero)
				{
					// ただしそのままだとカメラが下を向いているため、一旦その分は補正する
					Quaternion rotateOR = MainCamera.transform.rotation;
					Vector3 rotateOR_E = rotateOR.eulerAngles;
					rotateOR_E.x = 0;
					rotateOR = Quaternion.Euler(rotateOR_E);

					// 通常射撃の矢
					if (arrow != null)
					{
						arrow.MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
					}
				}
				// それ以外は本体の角度を射出角にする
				else
				{
					// 通常射撃の矢
					arrow.MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
				}
			}
			// 弾丸の向きを合わせる
			arrow.transform.rotation = transform.rotation;
			// 矢の移動ベクトルを代入する
			if (arrow != null)
			{
				BulletMoveDirection = arrow.MoveDirection;
			}
			// 攻撃力を代入する
			// 攻撃力を決定する(ここの2がスキルのインデックス。下も同様）
			OffensivePowerOfBullet = Character_Spec.cs[(int)CharacterName][1].m_OriginalStr + Character_Spec.cs[(int)CharacterName][1].m_GrowthCoefficientStr * (StrLevel - 1);
			// ダウン値を決定する
			DownratioPowerOfBullet = Character_Spec.cs[(int)CharacterName][1].m_DownPoint;
			// 覚醒ゲージ増加量を決定する
			ArousalRatioOfBullet = Character_Spec.cs[(int)CharacterName][1].m_arousal;

			Shotmode = ShotMode.SHOT;

			// 固定状態を解除
			// ずれた本体角度を戻す(Yはそのまま）
			transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
			transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

			// 硬直時間を設定
			AttackTime = Time.time;
			// 射出音を再生する
			AudioManager.Instance.PlaySE("shot_hand_gun");
		}
		// 弾がないときはとりあえずフラグだけは立てておく
		else
		{
			// 硬直時間を設定
			AttackTime = Time.time;
			Shotmode = ShotMode.SHOTDONE;
		}
		// フォロースルーへ移行する
		AnimatorUnit.SetTrigger("SubShotFollowThrow");
	}

	/// <summary>
	/// 特殊射撃実行
	/// </summary>
	public void ExShotDone()
	{
		AnimatorUnit.SetTrigger("EXShot");
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	/// <summary>
	/// 特殊射撃の装填を行う(特殊射撃はレーザーなのでこの時点で発射される）
	/// </summary>
	public void ShootLoadEXShot()
	{
		// 弾があるとき限定
		if (BulletNum[2] > 0)
		{
			// ロックオン時本体の方向を相手に向ける       
			if (IsRockon)
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[2]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[2] == Character_Spec.cs[(int)CharacterName][2].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][2].m_OriginalBulletNum - 1)
			{
				SubshotEndtime = Time.time;
			}
			// 矢の出現ポジションをフックと一致させる
			Vector3 pos = EXShotHock.transform.position;
			Quaternion rot = Quaternion.Euler(EXShotHock.transform.rotation.eulerAngles.x, EXShotHock.transform.rotation.eulerAngles.y, EXShotHock.transform.rotation.eulerAngles.z);
			// 矢を出現させる			
			var obj = Instantiate(EXShotLaser, pos, rot);
			// 親子関係を再設定する(=矢をフックの子にする）
			if (obj.transform.parent == null)
			{
				obj.transform.parent = EXShotHock.transform;
				// 矢の親子関係を付けておく
				obj.transform.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
	}

	/// <summary>
	/// 特殊射撃の場合フォロースルーに移行する
	/// </summary>
	public void FollowThrowEXShot()
	{
		DestroyArrow();
		AnimatorUnit.SetTrigger("EXShotFollowThrow");
	}

	/// <summary>
	/// N格闘1段目実行時、キャンセルや派生の入力を受け取る
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="airdashID"></param>
	/// <param name="stepanimations"></param>
	protected override void Wrestle1(Animator animator)
	{
		base.Wrestle1(animator);
		// 追加入力受け取り
		if (HasWrestleInput)
		{
			AddInput = true;
		}
	}

	/// <summary>
	/// N格闘2段目実行時、キャンセルや派生の入力を受け取る
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="airdashhash"></param>
	/// <param name="stepanimations"></param>
	protected override void Wrestle2(Animator animator)
	{
		base.Wrestle2(animator);
		// 追加入力受け取り
		if (HasWrestleInput)
		{
			AddInput = true;
		}
	}



	/// <summary>
	/// 格闘攻撃終了後、派生を行う
	/// </summary>
	/// <param name="nextmotion"></param>
	public void WrestleFinish(int nextmotion)
	{
		// 判定オブジェクトを全て破棄
		DestroyWrestle();
		// 格闘の累積時間を初期化
		Wrestletime = 0;
		// 派生入力があった場合は派生する
		if (AddInput)
		{
			// N格闘２段目派生
			if (nextmotion == 0)
			{
				WrestleDone(AnimatorUnit, 4, "Wrestle2");
			}
			// N格闘３段目派生
			else if (nextmotion == 1)
			{
				WrestleDone(AnimatorUnit, 5, "Wrestle3");
			}
		}
		// なかったら戻す
		else
		{
			ReturnToIdle();
		}
	}

	/// <summary>
	/// 左格闘時の本体動作
	/// </summary>
	/// <param name="animator"></param>
	protected override void LeftWrestle1(Animator animator)
	{
		base.LeftWrestle1(animator);
		if (IsRockon)
		{
			RotateToTarget();
			SideStep(180);
		}
	}

	/// <summary>
	/// 右格闘時の本体動作
	/// </summary>
	/// <param name="animator"></param>
	protected override void RightWrestle1(Animator animator)
	{
		base.RightWrestle1(animator);
		if (IsRockon)
		{
			RotateToTarget();
			SideStep(-180);
		}
	}
}
