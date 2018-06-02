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
	/// パラメーター読み取り用のインデックス
	/// </summary>
	private const int CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA;

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
				Battleinterfacecontroller.InformationText.text = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;

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
				Battleinterfacecontroller.Weapon2.NowBulletNumber = BulletNum[2];
				Battleinterfacecontroller.Weapon2.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[6].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[6].OriginalBulletNum;
				Battleinterfacecontroller.Weapon2.UseChargeGauge = false;
				// 初期最大値より大きければ使用可能
				if (BulletNum[2] > ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[6].OriginalBulletNum)
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
		
	}

	void LateUpdate()
	{

	}

	void UpdateAnimation()
	{

	}
}
