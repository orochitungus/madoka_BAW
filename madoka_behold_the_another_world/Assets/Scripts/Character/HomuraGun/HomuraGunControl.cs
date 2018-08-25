using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キャラクター「暁美 ほむら（銃）を制御するためのスクリプト
/// </summary>
public class HomuraGunControl : CharacterControlBase
{
	/// <summary>
	/// メイン射撃撃ち終わり時間
	/// </summary>
	private float MainshotEndtime;

	/// <summary>
	/// サブ射撃撃ち終わり時間
	/// </summary>
	private float SubshotEndtime;

	/// <summary>
	/// 前格闘撃ち終わり時間
	/// </summary>
	private float FrontWrestleEndTime;

	/// <summary>
	/// 特殊射撃撃ち終わり時間
	/// </summary>
	private float ExShotEndtime;
	
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
	/// 前格闘のアイコン
	/// </summary>
	public Sprite FrontWrestleIcon;

	/// <summary>
	/// 通常射撃の弾丸
	/// </summary>
	public GameObject NormalShotBullet;

	/// <summary>
	/// チャージ射撃の弾丸
	/// </summary>
	public GameObject ChargeShotBullet;

	/// <summary>
	/// サブ射撃の弾丸
	/// </summary>
	public GameObject SubShotBullet;

	/// <summary>
	/// サブ射撃射撃派生の弾丸
	/// </summary>
	public GameObject SubShotDeriveBullet;

	/// <summary>
	/// 前格闘の弾丸
	/// </summary>
	public GameObject FrontWrestleBullet;

	/// <summary>
	/// パラメーター読み取り用のインデックス
	/// </summary>
	private const int CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA;

	/// <summary>
	/// チャージ射撃のフック（この場所にチャージ射撃の弾丸が生成される）
	/// </summary>
	public GameObject ChargeShotRoot;

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
	public int ChargeShotID;            // 19
	public int SubShotID;               // 20
	public int EXShotID;                // 21
	public int FollowThrowShotID;       // 22
	public int FollowThrowRunShotID;    // 23
	public int FollowThrowAirShotID;    // 24
	public int FollowThrowChargeShotID; // 25
	public int FollowThrowSubShotID;    // 26
	public int FollowThrowEXShotID;     // 27
	public int Wrestle1ID;              // 28
	public int Wrestle2ID;              // 29
	public int Wrestle3ID;              // 30
	public int FrontWrestleID;          // 31
	public int LeftWrestleID;           // 32
	public int RightWrestleID;          // 33
	public int BackWrestleID;           // 34
	public int AirDashWrestleID;        // 35
	public int EXWrestleID;             // 36
	public int EXFrontWrestleID;        // 37
	public int EXBackWrestleID;         // 38
	public int ReversalID;              // 39
	public int ArousalAttackID;         // 40
	public int DamageID;                // 41
	public int DownID;                  // 42
	public int BlowID;                  // 43
	public int SpinDownID;              // 44
	public int FollowThrowFrontWrestle; // 45
	public int FollowThrowSideWrestle;	// 46



	private void Awake()
	{
		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if (Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}

		// 各ハッシュIDを取得する
		IdleID = Animator.StringToHash("Base Layer.homuragun_idle_copy");
		WalkID = Animator.StringToHash("Base Layer.homuragun_walk_copy");
		JumpID = Animator.StringToHash("Base Layer.homuragun_jump_copy");
		JumpingID = Animator.StringToHash("Base Layer.homuragun_jumping_copy");
		FallID = Animator.StringToHash("Base Layer.homuragun_fall_copy");
		LandingID = Animator.StringToHash("Base Layer.homuragun_landing_copy");
		RunID = Animator.StringToHash("Base Layer.homuragun_run_copy");
		AirDashID = Animator.StringToHash("Base Layer.homuragun_airdash_copy");
		FrontStepID = Animator.StringToHash("Base Layer.homuragun_frontstep_copy");
		LeftStepID = Animator.StringToHash("Base Layer.homuragun_leftstep_copy");
		RightStepID = Animator.StringToHash("Base Layer.homuragun_rightstep_copy");
		BackStepID = Animator.StringToHash("Base Layer.homuragun_backstep_copy");
		FrontStepBackID = Animator.StringToHash("Base Layer.homuragun_frontstepback_copy");
		LeftStepBackID = Animator.StringToHash("Base Layer.homuragun_leftstepback_copy");
		RightStepBackID = Animator.StringToHash("Base Layer.homuragun_rightstepback_copy");
		BackStepBackID = Animator.StringToHash("Base Layer.homuragun_rightstepback_copy");
		ShotID = Animator.StringToHash("Base Layer.homuragun_normalshot_copy");
		RunShotID = Animator.StringToHash("Base Layer.homuragun_normalshot_toponly_copy");
		//AirShotID = Animator.StringToHash("Base Layer.homuragun_normalshot_toponly_copy");
		ChargeShotID = Animator.StringToHash("Base Layer.homuragun_chargeshot_copy");
		SubShotID = Animator.StringToHash("Base Layer.homuragun_subshot_copy");
		EXShotID = Animator.StringToHash("Base Layer.homuragun_exshot_copy");
		FollowThrowShotID = Animator.StringToHash("Base Layer.homuragun_normalshotfs_copy");
		FollowThrowRunShotID = Animator.StringToHash("Base Layer.homuragun_normalshotfs_toponly_copy");
		//FollowThrowAirShotID = Animator.StringToHash("Base Layer.homuragun_normalshotfs_toponly_copy");
		FollowThrowChargeShotID = Animator.StringToHash("Base Layer.homuragun_chargeshotfs_copy");
		FollowThrowSubShotID = Animator.StringToHash("Base Layer.homuragun_subshotfs_copy");
		//FollowThrowEXShotID = Animator.StringToHash("Base Layer.homuragun_subshotfs_copy");
		Wrestle1ID = Animator.StringToHash("Base Layer.homuragun_wrestle1_copy");
		Wrestle2ID = Animator.StringToHash("Base Layer.homuragun_wrestle2_copy");
		Wrestle3ID = Animator.StringToHash("Base Layer.homuragun_wrestle2_copy");
		FrontWrestleID = Animator.StringToHash("Base Layer.homuragun_frontwrestle_copy");
		LeftWrestleID = Animator.StringToHash("Base Layer.homuragun_sidewrestle_copy");
		RightWrestleID = Animator.StringToHash("Base Layer.homuragun_sidewrestle_copy");
		BackWrestleID = Animator.StringToHash("Base Layer.homuragun_backwrestle_copy");
		AirDashWrestleID = Animator.StringToHash("Base Layer.homuragun_bdwrestle_copy");
		EXWrestleID = Animator.StringToHash("Base Layer.homuragun_exwrestle_copy");
		EXFrontWrestleID = Animator.StringToHash("Base Layer.homuragun_exwrestle_copy");
		EXBackWrestleID = Animator.StringToHash("Base Layer.homuragun_bexw_copy");
		ReversalID = Animator.StringToHash("Base Layer.homuragun_reversal_copy");
		ArousalAttackID = Animator.StringToHash("Base Layer.homuragun_fm_copy");
		DamageID = Animator.StringToHash("Base Layer.homuragun_damage_copy");
		DownID = Animator.StringToHash("Base Layer.homuragun_down_copy");
		BlowID = Animator.StringToHash("Base Layer.homuragun_blow");
		SpinDownID = Animator.StringToHash("Base Layer.homuragun_spindown_copy");
		FollowThrowFrontWrestle = Animator.StringToHash("Base Layer.homuragun_frontwrestlefs_copy");
		FollowThrowSideWrestle = Animator.StringToHash("Base Layer.homuragun_sidewrestlefs_copy");

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

		// 共通パラメーター初期化
		InitializeCommonParameter(CharacterIndex);

		// ショットのステート
		Shotmode = ShotMode.NORMAL;

		// 弾のステート
		BulletMoveDirection = Vector3.zero;

		// メイン射撃撃ち終わり時間
		MainshotEndtime = 0.0f;
		// サブ射撃撃ち終わり時間
		SubshotEndtime = 0.0f;
		// 特殊射撃撃ち終わり時間
		FrontWrestleEndTime = 0.0f;

		// 共通ステートを初期化
		FirstSetting(AnimatorUnit, 0);

		// チャージショットチャージ時間
		ChargeMax = (int)ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.CHARGE_SHOT].ReloadTime * 60;

		// カメラ制御

		// インターフェース制御
		this.UpdateAsObservable().Where(_ => IsPlayer == CHARACTERCODE.PLAYER).Subscribe(_ =>
		{
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
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_DEVIL_HOMURA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.DEVILHOMURA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA_GODSIBB)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.SAYAKA_GODSIBB);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_NAGISA)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.NAGISA);
				}
				else if ((Character_Spec.CHARACTER_NAME)savingparameter.GetNowParty(i) == Character_Spec.CHARACTER_NAME.MEMBER_MICHEL)
				{
					Battleinterfacecontroller.Playerbg[i].SelectPlayerBG(BustupSelect.MICHEL);
				}

				// 現在HP
				Battleinterfacecontroller.NowPlayerHP[i] = savingparameter.GetNowHP(savingparameter.GetNowParty(i));

				// 最大HP
				Battleinterfacecontroller.MaxPlayerHP[i] = savingparameter.GetMaxHP(savingparameter.GetNowParty(i));

				// PCのみ
				// 名前
				Battleinterfacecontroller.CharacterName.text = "暁美　ほむら";
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
				// インフォメーション表示内容
				//Battleinterfacecontroller.InformationText.text = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;

				// 武装ゲージ
				// 5番目は消す
				Battleinterfacecontroller.Weapon5.gameObject.SetActive(false);

				// 射撃
				Battleinterfacecontroller.Weapon4.Kind.text = "Shot";
				Battleinterfacecontroller.Weapon4.WeaponGraphic.sprite = ShotIcon;
				Battleinterfacecontroller.Weapon4.NowBulletNumber = BulletNum[0];
				Battleinterfacecontroller.Weapon4.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].OriginalBulletNum;
				// チャージ射撃ゲージ
				Battleinterfacecontroller.Weapon4.UseChargeGauge = true;
				// 1発でも使えれば使用可能
				if (BulletNum[(int)ShotType.NORMAL_SHOT] > 0)
				{
					Battleinterfacecontroller.Weapon4.Use = true;
				}
				else
				{
					Battleinterfacecontroller.Weapon4.Use = false;
				}
				// チャージ射撃
				Battleinterfacecontroller.Weapon4.NowChargeValue = ShotCharge / (float)(ChargeMax);

				// サブ射撃
				Battleinterfacecontroller.Weapon3.Kind.text = "Sub Shot";
				Battleinterfacecontroller.Weapon3.WeaponGraphic.sprite = SubShotIcon;
				Battleinterfacecontroller.Weapon3.NowBulletNumber = BulletNum[1];
				Battleinterfacecontroller.Weapon3.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].OriginalBulletNum;
				// チャージ格闘ゲージ
				Battleinterfacecontroller.Weapon3.UseChargeGauge = true;
				// 1発でも使えれば使用可能(リロード時は0になる)
				if (BulletNum[1] != 0)
				{
					Battleinterfacecontroller.Weapon3.Use = true;
				}
				else
				{
					Battleinterfacecontroller.Weapon3.Use = false;
				}

				// 特殊射撃
				Battleinterfacecontroller.Weapon2.Kind.text = "EX Shot";
				Battleinterfacecontroller.Weapon2.WeaponGraphic.sprite = ExShotIcon;
				Battleinterfacecontroller.Weapon2.NowBulletNumber = BulletNum[5];
				Battleinterfacecontroller.Weapon2.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].OriginalBulletNum;
				Battleinterfacecontroller.Weapon2.UseChargeGauge = false;
				// 初期最大値より大きければ使用可能
				if (BulletNum[5] >= ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].OriginalBulletNum)
				{
					Battleinterfacecontroller.Weapon2.Use = true;
				}
				else
				{
					Battleinterfacecontroller.Weapon2.Use = false;
				}

				// 前格闘
				Battleinterfacecontroller.Weapon1.Kind.text = "Front Wrestle";
				Battleinterfacecontroller.Weapon1.WeaponGraphic.sprite = FrontWrestleIcon;
				Battleinterfacecontroller.Weapon1.NowBulletNumber = BulletNum[3];
				Battleinterfacecontroller.Weapon1.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[15].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[15].OriginalBulletNum;
				Battleinterfacecontroller.Weapon1.UseChargeGauge = false;
				// 1発でも使えれば使用可能（リロード時は0になる）
				if (BulletNum[3] != 0)
				{
					Battleinterfacecontroller.Weapon1.Use = true;
				}
				else
				{
					Battleinterfacecontroller.Weapon1.Use = false;
				}

			}
		});


	}

	// Update is called once per frame
	void Update () 
	{
		// 共通アップデート処理(時間停止系の状態になると入力は禁止)
		bool isspindown = false;
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("spindown"))
		{
			isspindown = true;
		}
		if (Update_Core(isspindown, AnimatorUnit, DownID, AirDashID, AirShotID, JumpingID, FallID, IdleID, BlowID, RunID, FrontStepID, LeftStepID, RightStepID, BackStepID, DamageID, (int)HomuraGunBattleDefine.Idx.homuragun_leftstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_reversal_copy))
		{
			// リロード実行           
			// メイン射撃
			// 手動回復なのでなし

			// サブ射撃
			ReloadSystem.AllTogether(ref BulletNum[2], Time.time,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].ReloadTime, ref SubshotEndtime);
			// 特殊射撃
			ReloadSystem.AllTogether(ref BulletNum[5], Time.time,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[5].ReloadTime, ref ExShotEndtime);
			// 前格闘
			ReloadSystem.AllTogether(ref BulletNum[14], Time.time,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[14].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[14].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[14].ReloadTime, ref ExShotEndtime);
		}
	}

	void LateUpdate()
	{
		UpdateAnimation();
	}

	void UpdateAnimation()
	{
		ShowAirDashEffect = false;
		// 通常
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("idle"))
		{
			Animation_Idle(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_down_copy, (int)HomuraGunBattleDefine.Idx.homuragun_run_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("walk"))
		{
			Animation_Walk(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jump"))
		{
			Animation_Jump(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
		{
			Animation_Jumping(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_landing_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("fall"))
		{
			Animation_Fall(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_landing_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("landing"))
		{
			Animation_Landing(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("run"))
		{
			Animation_Run(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdash"))
		{
			ShowAirDashEffect = true;
			Animation_AirDash(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)HomuraGunBattleDefine.Idx.homuragun_frontstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)HomuraGunBattleDefine.Idx.homuragun_frontstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)HomuraGunBattleDefine.Idx.homuragun_frontstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)HomuraGunBattleDefine.Idx.homuragun_frontstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstepback_copy, (int)HomuraGunBattleDefine.Idx.homuragun_jump_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstepback"))
		{
			Animation_StepBack(AnimatorUnit,(int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetAnimatorTransitionInfo(0).IsName("backstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("shot"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("runshot"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airshot"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("chargeshot"))
		{
			ChargeShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("subshot"))
		{
			SubShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exshot"))
		{
			ExShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("shotft"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("runshotft"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airshotft"))
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("chargeshotft"))
		{
			ChargeShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("subshotft"))
		{
			SubShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exshotfth"))
		{
			ExShot((int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle1"))
		{
			Wrestle1(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle2"))
		{
			Wrestle2(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle3"))
		{
			Wrestle3(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontwrestle"))
		{
			FrontWrestle1(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftwrestle"))
		{
			LeftWrestle1(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightwrestle"))
		{
			RightWrestle1(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backwrestle"))
		{
			BackWrestle(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdashwrestle"))
		{
			ShowAirDashEffect = true;
			AirDashWrestle(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exwrestle"))
		{
			ExWrestle1();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontexwrestle"))
		{
			FrontExWrestle1(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy, (int)HomuraGunBattleDefine.Idx.homuragun_frontstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_leftstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_rightstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_backstep_copy, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backexwrestle"))
		{
			BackExWrestle(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy, (int)HomuraGunBattleDefine.Idx.homuragun_landing_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("reversal"))
		{
			Reversal();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("magicburst"))
		{
			ArousalAttack();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("damage"))
		{
			Damage(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy, (int)HomuraGunBattleDefine.Idx.homuragun_damage_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("down"))
		{
			Down(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_reversal_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("blow"))
		{
			Blow(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_down_copy, (int)HomuraGunBattleDefine.Idx.homuragun_reversal_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("spindown"))
		{
			SpinDown(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_down_copy);
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
	/// Jumpingへ移行する（JumpのAnimationの最終フレームで実行する）
	/// </summary>
	public void JumpingMigration()
	{
		AnimatorUnit.Play("jumping");
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
			SetIsArmor(true);

			// TODO:覚醒技処理

			return true;
		}
		// サブ射撃でサブ射撃へ移行
		else if (HasSubShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
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
				EmagencyStop(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
			}
			// 特殊射撃実行
			EXShotDone();
			return true;
		}
		// 特殊格闘で特殊格闘へ移行
		else if (HasExWrestleInput)
		{
			// 前特殊格闘(ブーストがないと実行不可)
			if (HasFrontInput && Boost > 0)
			{
				// 前特殊格闘実行
				//FrontEXWrestleDone(AnimatorUnit, 13);
			}
			// 空中で後特殊格闘(ブーストがないと実行不可）
			else if (HasBackInput && !IsGrounded && Boost > 0)
			{
				// 後特殊格闘実行
				//BackEXWrestleDone(AnimatorUnit, 14);
			}
			// それ以外
			else
			{
				// 特殊格闘実行
				EmagencyStop(AnimatorUnit, 0);
				AnimatorUnit.Play("exwrestle");
			}
			return true;
		}
		// 射撃チャージでチャージ射撃へ移行
		else if (HasShotChargeInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit,(int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
			}
			// チャージショット実行
			ChagerShotDone();
			return true;
		}
		// 格闘チャージでチャージ格闘へ移行
		else if(HasWrestleChargeInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
			}
			// TODO:チャージ格闘実行
			return true;
		}
		// 射撃で射撃へ移行
		else if (HasShotInput)
		{
			if (run)
			{
				if (GetIsRockon())
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
						EmagencyStop(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy);
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
				AirDashWrestleDone(AnimatorUnit, AirDashSpeed, 13, "bdwrestle");
			}
			// 前格闘で前格闘へ移行
			else if (HasFrontInput)
			{
				// 前格闘実行（射撃扱いになる）
				ChargeWrestleDone();
			}
			// 左格闘で左格闘へ移行
			else if (HasLeftInput)
			{
				// 左格闘実行（射撃扱いになる）
			}
			// 右格闘で右格闘へ移行
			else if (HasRightInput)
			{
				// 右格闘実行(射撃扱いになる)
				
			}
			// 後格闘で後格闘へ移行
			else if (HasBackInput)
			{
				// 後格闘実行（ガード）(Character_Spec.cs参照)
				GuardDone(AnimatorUnit, 12, "backwrestle");
			}
			else
			{
				// それ以外ならN格闘実行(2段目と3段目の追加入力はWrestle1とWrestle2で行う
				WrestleDone(AnimatorUnit, 6, "wrestle1");
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// 通常射撃の装填開始
	/// </summary>
	/// <param name="ruhshot">走行射撃であるか否か</param>
	public void ShotDone(bool runshot)
	{
		// 弾切れの場合はリロードへ移行する
		if(BulletNum[(int)ShotType.NORMAL_SHOT] == 0)
		{
			EmagencyStop(AnimatorUnit, 0);
			AnimatorUnit.Play("exwrestle");
			return;
		}

		// 歩行時は上半身のみにして走行（下半身のみ）と合成
		if (runshot)
		{
			// アニメーション合成処理＆再生
			AnimatorUnit.Play("runshot");
			AnimatorUnit.Play("runshot", 1);
		}
		// 立ち射撃か空中射撃
		else
		{
			AnimatorUnit.Play("shot");
		}
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
	}

	/// <summary>
	/// チャージショットの装填開始
	/// </summary>
	public void ChagerShotDone()
	{
		AnimatorUnit.Play("chargeshot");
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

		// ロックオン時本体の方向を相手に向ける       
		if (GetIsRockon())
		{
			RotateToTarget();
		}
	}

	/// <summary>
	/// チャージ射撃を開始する（chargeshotの最終フレームにインポートする）
	/// </summary>
	public void ChargeShotStart()
	{
		AnimatorUnit.Play("chargeshotft");
	}

	/// <summary>
	/// チャージ格闘の装填開始
	/// </summary>
	public void ChargeWrestleDone()
	{
		AnimatorUnit.Play("chargewrestle");
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	/// <summary>
	/// サブ射撃の装填開始
	/// </summary>
	public void SubShotDone()
	{
		AnimatorUnit.Play("subshot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	/// <summary>
	/// 特殊射撃の装填開始
	/// </summary>
	public void EXShotDone()
	{
		AnimatorUnit.Play("exshot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	/// <summary>
	/// ニュートラル状態の処理
	/// </summary>
	/// <param name="animator"></param>
	protected override void Animation_Idle(Animator animator, int downIndex, int runIndex, int airdashIndex, int jumpIndex, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int fallIndex)
	{
		// 攻撃したかフラグ
		bool attack = false;

		// 覚醒技が発動失敗した場合カメラと視点とアーマーを戻す
		SetIsArmor(false);

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
			CancelDashDone(AnimatorUnit, airdashIndex);
		}
		// 攻撃した場合はステートが変更されるので、ここで終了
		if (!attack)
		{
			base.Animation_Idle(animator, downIndex, runIndex, airdashIndex, jumpIndex, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, fallIndex);
		}
	}

	protected override void Animation_Jumping(Animator animator, int airdashIndex, int fallIndex, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int landingIndex)
	{
		base.Animation_Jumping(animator, airdashIndex, fallIndex, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, landingIndex);
		AttackDone();
	}

	protected override void Animation_Fall(Animator animator, int airdashIndex, int jumpIndex, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int landingIndex)
	{
		base.Animation_Fall(animator, airdashIndex, jumpIndex, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, landingIndex);
		AttackDone();
	}

	protected override void Animation_Run(Animator animator, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int idleIndex, int jumpIndex, int fallIndex)
	{
		base.Animation_Run(animator, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, idleIndex, jumpIndex, fallIndex);
		AttackDone(true, false);
	}

	protected override void Animation_AirDash(Animator animator, int fallIndex)
	{
		base.Animation_AirDash(animator,fallIndex);
		AttackDone(false, true);
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
			CancelDashDone(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_airdash_copy);
		}
		base.Shot();
	}

	/// <summary>
	/// Idle状態に戻す
	/// </summary>
	public void ReturnToIdle()
	{
		// 合成していた場合元に戻す
		AnimatorUnit.Play("None", 1);
		// 矢や格闘判定も消しておく
		DestroyArrow();
		DestroyWrestle();
		ReturnMotion(AnimatorUnit, (int)HomuraGunBattleDefine.Idx.homuragun_run_copy, (int)HomuraGunBattleDefine.Idx.homuragun_idle_copy, (int)HomuraGunBattleDefine.Idx.homuragun_fall_copy);

		// 重力を戻す
		GetComponent<Rigidbody>().useGravity = true;
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
			if (GetIsRockon())
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[0]--;
			

			// 弾丸の出現ポジションをフックと一致させる
			Vector3 pos = MainShotRoot.transform.position;
			Quaternion rot = Quaternion.Euler(MainShotRoot.transform.rotation.eulerAngles.x, MainShotRoot.transform.rotation.eulerAngles.y, MainShotRoot.transform.rotation.eulerAngles.z);
			// 弾丸を出現させる			
			GameObject obj = Instantiate(NormalShotBullet, pos, rot);
			// 親子関係を再設定する(=弾丸をフックの子にする）
			if (obj.transform.parent == null)
			{
				obj.transform.parent = MainShotRoot.transform;
				// 弾丸の親子関係を付けておく
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
		// 通常射撃の弾丸
		var arrow = GetComponentInChildren<HomuraGunNormalShot>();


		if (arrow != null)
		{
			// 矢のスクリプトの速度を設定する
			// メイン弾速設定
			arrow.ShotSpeed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].MoveSpeed;

			// 矢の方向を決定する(本体と同じ方向に向けて打ち出す。ただしノーロックで本体の向きが0のときはベクトルが0になるので、このときだけはカメラの方向に飛ばす）
			if (GetIsRockon() && RunShotDone)
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
				Vector3 addrot = mainrot.eulerAngles;
				Quaternion qua = Quaternion.Euler(addrot);
				// forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
				Vector3 normalizeRot = (qua) * Vector3.forward;
				// 移動ベクトルを確定する
				arrow.MoveDirection = Vector3.Normalize(normalizeRot);
			}
			// ロックオンしているとき
			else if (GetIsRockon())
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
			OffensivePowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].GrowthCoefficientStr * (StrLevel - 1);
			// ダウン値を決定する
			DownratioPowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].DownPoint;
			// 覚醒ゲージ増加量を決定する
			ArousalRatioOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].Arousal;

			Shotmode = ShotMode.SHOT;

			// 固定状態を解除
			// ずれた本体角度を戻す(Yはそのまま）
			transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
			transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

			// 硬直時間を設定
			AttackTime = Time.time;
			// 射出音を再生する
			AudioManager.Instance.PlaySE("gunshot");
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
			AnimatorUnit.Play("runshotft");
			AnimatorUnit.Play("runshotft", 1);
		}
		// 通常時
		else
		{
			AnimatorUnit.Play("shotft");
		}

	}

	/// <summary>
	/// メイン射撃の弾丸をリロードする
	/// </summary>
	public void MainShotReloadDone()
	{
		AudioManager.Instance.PlaySE("reload");
		BulletNum[(int)ShotType.NORMAL_SHOT] = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].OriginalBulletNum;
	}

	/// <summary>
	/// チャージ射撃の装填を行う（アニメーションファイルの装填フレームにインポートする）
	/// </summary>
	public void ShootloadChargeShot()
	{
		// 弾丸の出現ポジションをフックと一致させる
		Vector3 pos = ChargeShotRoot.transform.position;
		Quaternion rot = Quaternion.Euler(ChargeShotRoot.transform.rotation.eulerAngles.x, ChargeShotRoot.transform.rotation.eulerAngles.y, ChargeShotRoot.transform.rotation.eulerAngles.z);
		// 弾丸を出現させる			
		GameObject obj = Instantiate(ChargeShotBullet, pos, rot);
		// 親子関係を再設定する(=弾丸をフックの子にする）
		if (obj.transform.parent == null)
		{
			obj.transform.parent = ChargeShotRoot.transform;
			// 弾丸の親子関係を付けておく
			obj.transform.GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	/// <summary>
	/// チャージ射撃射出（アニメーションの射出フレームにインポートする）
	/// </summary>
	public void ChargeShotShooting()
	{
		// チャージ射撃の弾丸（通常射撃の弾丸と共通）
		var arrow = GetComponentInChildren<HomuraGunNormalShot>();

		if (arrow != null)
		{
			// 弾丸のスクリプトの速度を設定する
			// メイン弾速設定
			arrow.ShotSpeed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].MoveSpeed;

			// 弾丸の方向を決定する(本体と同じ方向に向けて打ち出す。ただしノーロックで本体の向きが0のときはベクトルが0になるので、このときだけはカメラの方向に飛ばす）
			if (GetIsRockon() && RunShotDone)
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
				Vector3 addrot = mainrot.eulerAngles;
				Quaternion qua = Quaternion.Euler(addrot);
				// forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
				Vector3 normalizeRot = (qua) * Vector3.forward;
				// 移動ベクトルを確定する
				arrow.MoveDirection = Vector3.Normalize(normalizeRot);
			}
			// ロックオンしているとき
			else if (GetIsRockon())
			{
				// ロックオン対象の座標を取得
				var target = GetComponentInChildren<Player_Camera_Controller>();
				// 対象の座標を取得
				Vector3 targetpos = target.Enemy.transform.position;
				// 本体の回転角度を拾う
				Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
				// 正規化して代入する
				Vector3 normalizeRot = mainrot * Vector3.forward;

				// チャージ射撃の弾丸
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

					// チャージ射撃の弾丸
					if (arrow != null)
					{
						arrow.MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
					}
				}
				// それ以外は本体の角度を射出角にする
				else
				{
					// チャージ射撃の弾丸
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
			OffensivePowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].GrowthCoefficientStr * (StrLevel - 1);
			// ダウン値を決定する
			DownratioPowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].DownPoint;
			// 覚醒ゲージ増加量を決定する
			ArousalRatioOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].Arousal;

			Shotmode = ShotMode.SHOT;

			// 固定状態を解除
			// ずれた本体角度を戻す(Yはそのまま）
			transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
			transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

			// 硬直時間を設定
			AttackTime = Time.time;

			// 射出音再生
			AudioManager.Instance.PlaySE("gunshot");
		}

	}
}
