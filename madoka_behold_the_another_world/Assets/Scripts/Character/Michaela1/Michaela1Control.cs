using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityStandardAssets.ImageEffects;

/// <summary>
/// キャラクター「芸術家の魔女の手下（意欲作）」を制御するためのスクリプト
/// </summary>
public class Michaela1Control : CharacterControlBase
{
	/// <summary>
	/// メイン射撃撃ち終わり時間
	/// </summary>
	private float MainshotEndtime;

	/// <summary>
	/// 戦闘用インターフェース
	/// </summary>
	public BattleInterfaceController Battleinterfacecontroller;

	/// <summary>
	/// 通常射撃のアイコン
	/// </summary>
	public Sprite ShotIcon;

	/// <summary>
	/// パラメーター読み取り用のインデックス
	/// </summary>
	private const int CharacterIndex = (int)Character_Spec.CHARACTER_NAME.ENEMY_MICHAELA1;

	private void Awake()
	{
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

		// 共通ステートを初期化
		FirstSetting(AnimatorUnit);

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
				Battleinterfacecontroller.CharacterName.text = "芸術家の魔女の手下（意欲作）";
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
				// 2,3,4,5番目は消す
				Battleinterfacecontroller.Weapon2.gameObject.SetActive(false);
				Battleinterfacecontroller.Weapon3.gameObject.SetActive(false);
				Battleinterfacecontroller.Weapon4.gameObject.SetActive(false);
				Battleinterfacecontroller.Weapon5.gameObject.SetActive(false);

				// 射撃
				Battleinterfacecontroller.Weapon1.Kind.text = "Shot";
				Battleinterfacecontroller.Weapon1.NowBulletNumber = BulletNum[(int)ShotType.NORMAL_SHOT];
				Battleinterfacecontroller.Weapon1.MaxBulletNumber = ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].OriginalBulletNum;
				Battleinterfacecontroller.Weapon1.UseChargeGauge = false;

				// 1発でも使えれば使用可能
				if (BulletNum[(int)ShotType.NORMAL_SHOT] > 0)
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
		if(Update_Core(isspindown, AnimatorUnit))
		{
			// リロード実行           
			// メイン射撃
			ReloadSystem.OneByOne(
				ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].GrowthCoefficientBul * (BulLevel - 1) + ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].OriginalBulletNum,
				ParameterManager.Instance.Characterskilldata.sheets[(int)CharacterName].list[(int)ShotType.NORMAL_SHOT].ReloadTime,
				ref MainshotEndtime);
		}
	}

	private void LateUpdate()
	{
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		ShowAirDashEffect = false;
		// 通常
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("idle"))
		{
			Animation_Idle(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_down_copy, (int)MajyuBattleDefine.Idx.majyu_b_run_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy);
		}
		// 歩行
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("walk"))
		{
			Animation_Walk(AnimatorUnit);
		}
		// ジャンプ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jump"))
		{
			Animation_Jump(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 上昇
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("jumping"))
		{
			Animation_Jumping(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_landing_copy);
		}
		// 落下
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("fall"))
		{
			Animation_Fall(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_landing_copy);
		}
		// 着地
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("landing"))
		{
			Animation_Landing(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
		}
		// 走行
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("run"))
		{
			Animation_Run(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy);
		}
		// 空中ダッシュ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdash"))
		{
			ShowAirDashEffect = true;
			Animation_AirDash(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy);
		}
		// 前ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstep"))
		{
			Animation_StepDone(AnimatorUnit, 0, 0, 0, 0, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 左ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstep"))
		{
			Animation_StepDone(AnimatorUnit, 0, 0, 0, 0, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 右ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstep"))
		{
			Animation_StepDone(AnimatorUnit, 0, 0, 0, 0, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 後ステップ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backstep"))
		{
			Animation_StepDone(AnimatorUnit, 0, 0, 0, 0, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_back_copy, (int)MajyuBattleDefine.Idx.majyu_b_jump_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 前ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
		}
		// 左ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("leftstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
		}
		// 右ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("rightstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
		}
		// 後ステップ終了
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backstepback"))
		{
			Animation_StepBack(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
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
		// 後格闘（ガード）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backwrestle"))
		{
			BackWrestle(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
		}
		// 空中ダッシュ格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("airdash"))
		{
			ShowAirDashEffect = true;
			AirDashWrestle(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy);
		}
		// 前特殊格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("frontexwrestle"))
		{
			FrontExWrestle1(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy, (int)MajyuBattleDefine.Idx.majyu_b_frontstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_leftstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_rightstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_backstep_copy, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 後特殊格闘
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("backexwrestle"))
		{
			BackExWrestle(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_fall_copy, (int)MajyuBattleDefine.Idx.majyu_b_landing_copy);
		}
		// 起き上がり
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("reversal"))
		{
			Reversal();
		}
		// ダメージ（のけぞり）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("damage"))
		{
			Damage(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy, (int)MajyuBattleDefine.Idx.majyu_b_damage02_copy);
		}
		// ダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("down"))
		{
			Down(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_down_rebirth02_copy);
		}
		// ダメージ（吹き飛び）
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("blow"))
		{
			Blow(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_damage02_copy, (int)MajyuBattleDefine.Idx.majyu_b_down_rebirth02_copy);
		}
		// きりもみダウン
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).IsName("spindown"))
		{
			SpinDown(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_spinDown_copy);
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
	/// Jumpingへ移行する（JumpのAnimationの最終フレームで実行する/地上からダッシュに移行するにはJumpを削除してこれを直接実行のほうがいいかもしれない）
	/// </summary>
	public void JumpingMigration()
	{
		AnimatorUnit.Play("jumping");
	}

	/// <summary>
	/// Idle時共通操作
	/// </summary>
	protected override void Animation_Idle(Animator animator, int downIndex = 0, int runIndex = 0, int airdashIndex = 0, int jumpIndex = 0, int frontstepIndex = 0, int leftstepIndex = 0, int rightstepIndex = 0, int backstepIndex = 0, int fallIndex = 0)
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
			CancelDashDone(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_air_dash_copy);
		}
		// 攻撃した場合はステートが変更されるので、ここで終了
		if (!attack)
		{
			base.Animation_Idle(animator, downIndex, runIndex, airdashIndex, jumpIndex, frontstepIndex, leftstepIndex, rightstepIndex, backstepIndex, fallIndex);
		}
	}

	/// <summary>
	/// 攻撃行動全般
	/// </summary>
	/// <param name="run"></param>
	/// <param name="AirDash"></param>
	/// <returns></returns>
	public bool AttackDone(bool run = false, bool AirDash = false)
	{
		DestroyWrestle();
		DestroyArrow();
		// サブ射撃・特殊射撃・射撃で射撃へ移行
		if (HasSubShotInput || HasExShotInput || HasShotInput)
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
						EmagencyStop(AnimatorUnit, (int)MajyuBattleDefine.Idx.majyu_b_idle_copy);
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


		return false;
	}

	/// <summary>
	/// 射撃の装填開始
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
			AnimatorUnit.Play("airshot");
		}
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
	}
}
