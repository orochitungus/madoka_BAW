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
	/// 覚醒技のビームを放つパワーボール
	/// </summary>
	public GameObject ArousalCore;

	/// <summary>
	/// パワーボールのアニメーター
	/// </summary>
	public Animator ArousalCoreAnimator;

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
	/// パラメーター読み取り用のインデックス
	/// </summary>
	private const int CharacterIndex = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;

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
		ExshotEndtime = 0.0f;

		// 共通ステートを初期化
		FirstSetting(AnimatorUnit, 0);

		this.UpdateAsObservable().Subscribe(_ =>
		{
			// N格３段目時のみカメラ変更
			if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle3"))
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
			// インフォメーション表示内容
			//Battleinterfacecontroller.InformationText.text = ParameterManager.Instance.EntityInformation.sheets[0].list[savingparameter.story].text;
			// 武装ゲージ
			// 4番目と5番目は消す
			Battleinterfacecontroller.Weapon4.gameObject.SetActive(false);
			Battleinterfacecontroller.Weapon5.gameObject.SetActive(false);
			// 射撃
			Battleinterfacecontroller.Weapon3.Kind.text = "Shot";
			Battleinterfacecontroller.Weapon3.WeaponGraphic.sprite = ShotIcon;
			Battleinterfacecontroller.Weapon3.NowBulletNumber = BulletNum[(int)ShotType.NORMAL_SHOT];
			Battleinterfacecontroller.Weapon3.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].OriginalBulletNum;
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
			Battleinterfacecontroller.Weapon2.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].OriginalBulletNum;
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
			Battleinterfacecontroller.Weapon1.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].OriginalBulletNum;
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
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("spindown"))
		{
			isspindown = true;
		}
		if (Update_Core(isspindown, AnimatorUnit, DownID, AirDashID, AirShotID, JumpingID, FallID, IdleID, BlowID, RunID, FrontStepID, LeftStepID, RightStepID, BackStepID, DamageID,(int)SconosciutoBattleDefine.Idx.Scono_leftstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rebirsal_copy))
		{
			
			// リロード実行           
			// メイン射撃
			ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].ReloadTime, ref MainshotEndtime);
			// サブ射撃
			ReloadSystem.AllTogether(ref BulletNum[1], Time.time, ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].ReloadTime, ref SubshotEndtime);
			// 特殊射撃
			ReloadSystem.OneByOne(ref BulletNum[2], Time.time, ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].ReloadTime, ref ExshotEndtime);
		}
	}

	private void LateUpdate()
	{
		UpdateAnimation();
	}

	void UpdateAnimation()
	{
		ShowAirDashEffect = false;
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("idle"))
		{
			Animation_Idle(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_Down_copy, (int)SconosciutoBattleDefine.Idx.Scono_run_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("walk"))
		{
			Animation_Walk(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jump"))
		{
			Animation_Jump(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
		{
			Animation_Jumping(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_landing_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("fall"))
		{
			Animation_Fall(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_landing_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("landing"))
		{
			Animation_Landing(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("run"))
		{
			Animation_Run(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdash"))
		{
			ShowAirDashEffect = true;
			Animation_AirDash(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_fall_copy);
		}
		// 前ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID,(int)SconosciutoBattleDefine.Idx.Scono_frontstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 左ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 右ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 後ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backstep"))
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_back_copy, (int)SconosciutoBattleDefine.Idx.Scono_jump_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 前ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstepback"))
		{
			Animation_StepBack(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		// 左ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		// 右ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		// 後ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		// リバーサル
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("reversal"))
		{
			Reversal();
		}
		// ダメージ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("damage"))
		{
			Damage(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy, (int)SconosciutoBattleDefine.Idx.Scono_Damage_copy);
		}
		// ダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("down"))
		{
			Down(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_rebirsal_copy);
		}
		// 吹き飛び
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("blow"))
		{
			Blow(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_Down_copy, (int)SconosciutoBattleDefine.Idx.Scono_rebirsal_copy);
		}
		// きりもみダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("spindown"))
		{
			SpinDown(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_Down_copy);
		}
		// 射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("shot"))
		{
			Shot();
		}
		// 走行射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("runshot"))
		{
			Shot();
		}
		// 空中射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airshot"))
		{
			Shot();
		}
		// 射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("shotft"))
		{
			Shot();
		}
		// 走行射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("runshotft"))
		{
			Shot();
		}
		// 空中射撃フォロースルー
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airshotft"))
		{
			Shot();
		}
		// サブ射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("subshot"))
		{
			SubShot((int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// サブ射撃フォロースルー
		else if(AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("subshotft"))
		{
			
		}
		// 特殊射撃
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exshot"))
		{
			ExShot((int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 特殊射撃フォロースルー
		else if(AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exshotft"))
		{
			
		}
		// 通常格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle1"))
		{
			Wrestle1(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle2"))
		{
			Wrestle2(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("wrestle3"))
		{
			Wrestle3(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 前格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontwrestle"))
		{
			FrontWrestle1(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy,(int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 横格闘(左）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftwrestle"))
		{
			LeftWrestle1(AnimatorUnit,(int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 横格闘(右）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightwrestle"))
		{
			RightWrestle1(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
		}
		// 後格闘（ガード）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backwrestle"))
		{			
			BackWrestle(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
		}
		// 特殊格闘
        else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("exwrestle"))
        {
            ExWrestle1();
        }
        // 前特殊格闘
        else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontexwrestle"))
        {
            FrontExWrestle1(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
        }
        // 後特殊格闘
        else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backexwrestle"))
        {
            BackExWrestle(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy, (int)SconosciutoBattleDefine.Idx.Scono_landing_copy);
        }
        // 空中ダッシュ格闘
        else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdashwrestle"))
        {
            ShowAirDashEffect = true;
            AirDashWrestle(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy);
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
	protected override void Animation_Idle(Animator animator, int downIndex, int runIndex, int airdashIndex, int jumpIndex, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int fallIndex)
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
			CancelDashDone(AnimatorUnit,airdashIndex);
		}
		// 攻撃した場合はステートが変更されるので、ここで終了
		if (!attack)
		{
			base.Animation_Idle(animator,downIndex, runIndex, airdashIndex, jumpIndex, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, fallIndex);
		}
	}

	/// <summary>
	/// Jumpingへ移行する（JumpのAnimationの最終フレームで実行する）
	/// </summary>
	public void JumpingMigration()
	{
		AnimatorUnit.Play("jumping");
	}

	protected override void Animation_Run(Animator animator, int frontstepIndex, int leftstepIndex, int rightstepIndex, int backstepIndex, int idleIndex, int jumpIndex, int fallIndex)
	{
		base.Animation_Run(animator, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, idleIndex, jumpIndex, fallIndex);
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
			SetIsArmor(true);

			// モーションを覚醒技にする
			ArousalAttackDone();

			return true;
		}
		// サブ射撃でサブ射撃へ移行
		else if (HasSubShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
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
				EmagencyStop(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
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
                // 前特殊格闘実行
                FrontEXWrestleDone(AnimatorUnit, 12);
                // スコノの前特格と空中ダッシュ格闘はループアニメなので、WrestleStartをanimファイルに貼るという方法が使えない（無限にくっつく）
                // そこでここで判定を生成し、fallに入ったら判定を消す
                // 判定を作る
                // 判定の場所
                Vector3 pos = WrestleRoot[8].transform.position;
                // 判定の角度（0固定）
                Quaternion rot = WrestleRoot[8].transform.rotation;
                // 判定を生成する
                var obj = Instantiate(WrestleObject[8], pos, rot);
                // 判定を子オブジェクトにする
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = WrestleRoot[8].transform;
                    // 親子関係を付けておく
                    obj.transform.GetComponent<Rigidbody>().isKinematic = true;
                }
                // 判定に値をセットする
                // インデックス
                int characterName = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
				// 
                obj.gameObject.GetComponent<Wrestle_Core>().SetStatus(
                    ParameterManager.Instance.Characterskilldata.sheets[characterName].list[12].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[characterName].list[12].GrowthCoefficientStr * (StrLevel - 1),    // offensive    [in]:攻撃力
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[12].DownPoint,                                                                    // downR        [in]:ダウン値
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[12].Arousal,                                                                                         // arousal      [in]:覚醒ゲージ増加量
					ParameterManager.Instance.GetHitType(characterName, 12),                                                                                          // hittype      [in]:ヒットタイプ
                    ObjectName.CharacterFileName[(int)CharacterName],
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[12].AntiBoostStr
					);
            }
            // 空中で後特殊格闘(ブーストがないと実行不可）
            else if (HasBackInput && !IsGrounded && Boost > 0)
			{
                // 後特殊格闘実行
                BackEXWrestleDone(AnimatorUnit, 9);
			}
			// それ以外
			else
			{
                // 特殊格闘実行
                EXWrestleDone(AnimatorUnit, 11);
			}
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
						EmagencyStop(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy);
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
                WrestleDone(AnimatorUnit, 10,"bdwrestle");
				// こちらもループアニメなので、animに関数を貼る手段は使えないため判定をここで作る
				// 判定の場所
				Vector3 pos = WrestleRoot[10].transform.position;
				// 判定の角度
				Quaternion rot = WrestleRoot[10].transform.rotation;
				// 判定を生成する
				var obj = Instantiate(WrestleObject[10], pos, rot);
				// 判定を子オブジェクトにする
				if (obj.transform.parent == null)
				{
					obj.transform.parent = WrestleRoot[10].transform;
					// 親子関係を付けておく
					obj.transform.GetComponent<Rigidbody>().isKinematic = true;
				}
				// 判定に値をセットする
				// インデックス
				int characterName = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
				obj.gameObject.GetComponent<Wrestle_Core>().SetStatus(
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[10].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[characterName].list[10].GrowthCoefficientStr * (StrLevel - 1),    // offensive    [in]:攻撃力
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[10].DownPoint,                                                                                       // downR        [in]:ダウン値
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[10].Arousal,                                                                                        // arousal      [in]:覚醒ゲージ増加量
					ParameterManager.Instance.GetHitType(characterName,10),                                                                                         // hittype      [in]:ヒットタイプ
					ObjectName.CharacterFileName[(int)CharacterName],
					ParameterManager.Instance.Characterskilldata.sheets[characterName].list[10].AntiBoostStr
					);
				
			}
			// 前格闘で前格闘へ移行
			else if (HasFrontInput)
			{
				// 前格闘実行(Character_Spec.cs参照)
				WrestleDone(AnimatorUnit, 6, "frontwrestle");
			}
			// 左格闘で左格闘へ移行
			else if (HasLeftInput)
			{
				// 左格闘実行(Character_Spec.cs参照)
				WrestleDone(AnimatorUnit, 7, "leftwrestle");
			}
			// 右格闘で右格闘へ移行
			else if (HasRightInput)
			{
				// 右格闘実行(Character_Spec.cs参照)
				WrestleDone(AnimatorUnit, 8, "rightwrestle");
			}
			// 後格闘で後格闘へ移行
			else if (HasBackInput)
			{
				// 後格闘実行（ガード）(Character_Spec.cs参照)
				GuardDone(AnimatorUnit, 9, "backwrestle");
			}
			else
			{
				// それ以外ならN格闘実行(2段目と3段目の追加入力はWrestle1とWrestle2で行う
				WrestleDone(AnimatorUnit, 3, "wrestle1");
			}
			return true;
		}
		return false;
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

	protected override void Animation_AirDash(Animator animator, int fallIndex)
	{
		base.Animation_AirDash(animator,fallIndex);
		AttackDone(false, true);
	}

	/// <summary>
	/// Idle状態に戻す
	/// </summary>
	public void ReturnToIdle()
	{
		// 合成したモーションも戻す
		AnimatorUnit.Play("None", 1);
		// 矢や格闘判定も消しておく
		SetIsArmor(false);
		DestroyArrow();
		DestroyWrestle();
		ReturnMotion(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_run_copy, (int)SconosciutoBattleDefine.Idx.Scono_neutral_copy, (int)SconosciutoBattleDefine.Idx.Scono_fall_copy);
		// 重力を戻す
		GetComponent<Rigidbody>().useGravity = true;
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
			CancelDashDone(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
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
			if (GetIsRockon())
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[0]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[0] == ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].OriginalBulletNum - 1)
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
                Vector3 addrot = mainrot.eulerAngles;// + normalizeRot_OR - new Vector3(0, 74.0f, 0);
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
			// 対ブースト攻撃力を決定する
			AntiBoostOffensivePowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[0].AntiBoostStr;

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
	/// サブ射撃実行
	/// </summary>
	public void SubShotDone()
	{
		AnimatorUnit.Play("subshot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

    protected override void SubShot(int airdashIndex)
    {
        // キャンセルダッシュ受付
        if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (IsGrounded)
            {
                transform.Translate(new Vector3(0, 1, 0));
            }
            CancelDashDone(AnimatorUnit,airdashIndex);
        }
        base.SubShot(airdashIndex);
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
			if (GetIsRockon())
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[1]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[1] == ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].OriginalBulletNum - 1)
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
			arrow.ShotSpeed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].MoveSpeed;
			// ロックオンしているとき
			if (GetIsRockon())
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
			OffensivePowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].OriginalStr + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].GrowthCoefficientStr * (StrLevel - 1);
			// ダウン値を決定する
			DownratioPowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].DownPoint;
			// 覚醒ゲージ増加量を決定する
			ArousalRatioOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].Arousal;
			// 対ブースト攻撃力を決定する
			AntiBoostOffensivePowerOfBullet = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[1].AntiBoostStr;

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
		AnimatorUnit.Play("subshotft");
	}

	/// <summary>
	/// 特殊射撃実行
	/// </summary>
	public void ExShotDone()
	{
		AnimatorUnit.Play("exshot");
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
			if (GetIsRockon())
			{
				RotateToTarget();
			}
			// 弾を消費する
			BulletNum[2]--;
			// 撃ち終わった時間を設定する                
			// メイン（弾数がMax-1のとき）
			if (BulletNum[2] == ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[2].OriginalBulletNum - 1)
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
		AnimatorUnit.Play("exshotft");
	}

	/// <summary>
	/// N格闘1段目実行時、キャンセルや派生の入力を受け取る
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="airdashID"></param>
	/// <param name="stepanimations"></param>
	protected override void Wrestle1(Animator animator, int frontStepIndex, int leftStepIndex, int rightStepIndex, int backStepIndex, int airdashIndex)
	{
		base.Wrestle1(animator,frontStepIndex, leftStepIndex,rightStepIndex,backStepIndex, airdashIndex);
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
	protected override void Wrestle2(Animator animator, int frontStepIndex, int leftStepIndex, int rightStepIndex, int backStepIndex, int airdashIndex)
	{
		base.Wrestle2(animator, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
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
				WrestleDone(AnimatorUnit, 4, "wrestle2");
			}
			// N格闘３段目派生
			else if (nextmotion == 1)
			{
				WrestleDone(AnimatorUnit, 5, "wrestle3");
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
	protected override void LeftWrestle1(Animator animator, int frontStepIndex, int leftStepIndex, int rightStepIndex, int backStepIndex, int airdashIndex)
	{
		base.LeftWrestle1(animator, frontStepIndex, leftStepIndex, rightStepIndex, backStepIndex, airdashIndex);
		if (GetIsRockon())
		{
			// MoveDirectionを再設定
			// ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
			if(transform.rotation.eulerAngles.y != 0)
			{
				// ロックオン対象を取得
				var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
				// ロックオン対象の座標
				Vector3 targetpos = target.transform.position;
				// 上記の座標は足元を向いているので、自分の高さに補正する
				targetpos.y = transform.position.y;
				// 自機の座標
				Vector3 mypos = transform.position;
				// 自機をロックオン対象に向ける
				transform.rotation = Quaternion.LookRotation(mypos - targetpos);
				// 方向ベクトルを向けた方向に合わせる            
				MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
				MoveDirection.x = 0;
			}
			// 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
			else
			{
				// ただしそのままだとカメラが下を向いているため、一旦その分は補正する
				Quaternion rotateOR = MainCamera.transform.rotation;
				Vector3 rotateOR_E = rotateOR.eulerAngles;
				rotateOR_E.x = 0;
				rotateOR = Quaternion.Euler(rotateOR_E);
				MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
				MoveDirection.x = 0;
			}
		}
	}

	/// <summary>
	/// 右格闘時の本体動作
	/// </summary>
	/// <param name="animator"></param>
	protected override void RightWrestle1(Animator animator, int frontStepIndex, int leftStepIndex, int rightStepIndex, int backStepIndex, int airdashIndex)
	{
		base.RightWrestle1(animator, frontStepIndex, leftStepIndex, rightStepIndex, backStepIndex, airdashIndex);
		if (GetIsRockon())
		{
			// MoveDirectionを再設定
			// ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
			if (transform.rotation.eulerAngles.y != 0)
			{
				// ロックオン対象を取得
				var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
				// ロックオン対象の座標
				Vector3 targetpos = target.transform.position;
				// 上記の座標は足元を向いているので、自分の高さに補正する
				targetpos.y = transform.position.y;
				// 自機の座標
				Vector3 mypos = transform.position;
				// 自機をロックオン対象に向ける
				transform.rotation = Quaternion.LookRotation(mypos - targetpos);
				// 方向ベクトルを向けた方向に合わせる            
				MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
				MoveDirection.x = 0;
			}
			// 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
			else
			{
				// ただしそのままだとカメラが下を向いているため、一旦その分は補正する
				Quaternion rotateOR = MainCamera.transform.rotation;
				Vector3 rotateOR_E = rotateOR.eulerAngles;
				rotateOR_E.x = 0;
				rotateOR = Quaternion.Euler(rotateOR_E);
				MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
				MoveDirection.x = 0;
			}
		}
	}

    /// <summary>
    /// 特殊格闘を実行する
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="skillindex"></param>
    public void EXWrestleDone(Animator animator, int skillindex)
    {
        // 追加入力フラグをカット
        AddInput = false;
        // 移動速度
        float movespeed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[skillindex].MoveSpeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
        if (GetIsRockon() && transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 上記の座標は足元を向いているので、自分の高さに補正する
            targetpos.y = transform.position.y;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを向けた方向に合わせる            
            MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
        }
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR = Quaternion.Euler(rotateOR_E);
            MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
        }
        // それ以外は本体の角度を移動方向にする
        else
        {
            MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
        }
        // アニメーション速度
        float speed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[skillindex].AnimSpeed;

		// アニメーションを再生する
		animator.Play("exwrestle");

        // アニメーションの速度を調整する
        animator.speed = speed;
        // 移動速度を調整する
        WrestlSpeed = movespeed;
    }

    /// <summary>
    /// 前特殊格闘を実行する
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="skillindex"></param>
    public void FrontEXWrestleDone(Animator animator, int skillindex)
    {
        // 追加入力フラグをカット
        AddInput = false;
        // 移動速度（上方向に垂直上昇する）

        // アニメーション速度
        animator.speed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[skillindex].AnimSpeed;

		// アニメーションを再生する
		animator.Play("frontexwrestle");
    }

    /// <summary>
	/// 特殊格闘
	/// </summary>
	protected override void ExWrestle1()
    {
        base.ExWrestle1();
        Wrestletime += Time.deltaTime;
        StepCancel(AnimatorUnit, (int)SconosciutoBattleDefine.Idx.Scono_frontstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_leftstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_rightstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_backstep_copy, (int)SconosciutoBattleDefine.Idx.Scono_AirDash_copy);
    }

    /// <summary>
    /// 前特殊格闘
    /// </summary>
    /// <param name="animator"></param>
    protected override void FrontExWrestle1(Animator animator, int fallIndex, int frontStepIndex, int leftStepIndex, int rightStepIndex, int backStepIndex, int airdashIndex)
    {
        base.FrontExWrestle1(animator, fallIndex, frontStepIndex, leftStepIndex, rightStepIndex, backStepIndex, airdashIndex);
        Wrestletime += Time.deltaTime;
        // レバー入力カットか特殊格闘入力カットで落下に移行する
        if (ControllerManager.Instance.TopUp || ControllerManager.Instance.EXWrestleUp)
        {
            FallDone(Vector3.zero, animator, fallIndex);
        }
        // 移動速度（上方向に垂直上昇する）
        float movespeed = 100.0f;

        // 移動方向（移動目的のため、とりあえず垂直上昇させる）
        MoveDirection = Vector3.Normalize(new Vector3(0, 1, 0));

        // 移動速度を調整する
        WrestlSpeed = movespeed;
    }

    /// <summary>
    /// 後特殊格闘
    /// </summary>
    /// <param name="animator"></param>
    protected override void BackExWrestle(Animator animator, int fallIndex, int landingIndex)
    {
        base.BackExWrestle(animator, fallIndex, landingIndex);
        // レバー入力カットか特殊格闘入力カットで落下に移行する
        if (ControllerManager.Instance.UnderUp || ControllerManager.Instance.EXWrestleUp)
        {
            FallDone(Vector3.zero, animator, fallIndex);
        }
        // 移動速度（上方向に垂直上昇する）
        float movespeed = 100.0f;

        // 移動方向（移動目的のため、とりあえず垂直下降させる）
        MoveDirection = Vector3.Normalize(new Vector3(0, -1, 0));

        // 移動速度を調整する
        WrestlSpeed = movespeed;
    }

    /// <summary>
    /// 後特殊格闘を実行する
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="skillindex"></param>
    public void BackEXWrestleDone(Animator animator, int skillindex)
    {
        // 追加入力フラグをカット
        AddInput = false;

        // アニメーション速度
        animator.speed = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[skillindex].AnimSpeed;

		// アニメーションを再生する
		animator.Play("backexwrestle");
    }


	/// <summary>
	/// 覚醒技を実行する
	/// </summary>
	public void ArousalAttackDone()
	{
		AnimatorUnit.Play("finalmagic");
		ArousalCoreAnimator.SetTrigger("Wait");
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	/// <summary>
	/// パワーボールを出現させてアニメーションを再生する
	/// </summary>
	public void AppearArousalBall()
	{
		ArousalCore.SetActive(true);
		ArousalCoreAnimator.SetTrigger("ArousalAttack");
	}

	/// <summary>
	/// パワーボールを消滅させてアニメーションを終える
	/// </summary>
	public void EraseArousalBall()
	{
		ArousalCoreAnimator.SetTrigger("Wait");
		ArousalCore.SetActive(false);
	}
}
