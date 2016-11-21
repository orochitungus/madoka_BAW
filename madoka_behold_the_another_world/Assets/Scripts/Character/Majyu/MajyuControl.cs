﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キャラクター「魔獣」を制御するためのスクリプト
/// </summary>
public class MajyuControl : CharacterControlBase
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
	public int FrontExWrestleID;        // 32
	public int BackExWrestleID;         // 33
	public int FrontWrestleID;			// 34

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
		FrontExWrestleID = Animator.StringToHash("Base Layer.MajyuFrontExWrestle");
		BackExWrestleID = Animator.StringToHash("Base Layer.MajyuBackExWrestle");
		FrontWrestleID = Animator.StringToHash("Base Layer.MajyuFrontWrestle");
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
		AirDashSpeed = 40.0f;                        // 移動速度（空中ダッシュの場合）
		AirMoveSpeed = 7.0f;                         // 移動速度（空中慣性移動の場合）
		RiseSpeed = 2.0f;                            // 上昇速度

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

		// 共通ステートを初期化
		FirstSetting(AnimatorUnit, 0);

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
			Battleinterfacecontroller.CharacterName.text = "魔獣";
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
			Battleinterfacecontroller.Weapon1.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum;
			Battleinterfacecontroller.Weapon1.UseChargeGauge = false;

			// 1発でも使えれば使用可能
			if (BulletNum[(int)ShotType.NORMAL_SHOT] > 0)
			{
				Battleinterfacecontroller.Weapon3.Use = true;
			}
			else
			{
				Battleinterfacecontroller.Weapon3.Use = false;
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
			ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum,
				Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_reloadtime, ref MainshotEndtime);
		}
	}

	void LateUpdate()
	{

	}

	void UpdateAnimation()
	{
		// 通常
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == IdleID)
		{
			int[] stepanimations = { 8, 9, 10, 11 };
			Animation_Idle(AnimatorUnit, 29, 6, stepanimations, 4, 2, 7);
		}
		// 歩行
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == WalkID)
		{
			Animation_Walk(AnimatorUnit);
		}
		// ジャンプ
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpID)
		{
			Animation_Jump(AnimatorUnit, 2, 7);
		}
		// 上昇
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpingID)
		{
			int[] stepanimations = { 8, 9, 10, 11 };
			Animation_Jumping(AnimatorUnit, 4, stepanimations, 7, 5);
		}
		// 落下
		else if(AnimatorUnit.GetAnimatorTransitionInfo(0).fullPathHash == FallID)
		{
			int[] stepanimations = { 8, 9, 10, 11 };
			Animation_Fall(AnimatorUnit, 7, 2, stepanimations, 5);
		}
		// 着地
		else if(AnimatorUnit.GetAnimatorTransitionInfo(0).fullPathHash == LandingID)
		{
			Animation_Landing(AnimatorUnit, 0);
		}
	}

	/// <summary>
	/// アイドル時のアニメーションを制御
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="downID"></param>
	/// <param name="runID"></param>
	/// <param name="stepanimations"></param>
	/// <param name="fallID"></param>
	/// <param name="jumpID"></param>
	/// <param name="airdashID"></param>
	protected override void Animation_Idle(Animator animator, int downID, int runID, int[] stepanimations, int fallID, int jumpID, int airdashID)
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
		if (HasDashCancelInput)
		{
			// 地上でキャンセルすると浮かないので浮かす
			if (IsGrounded)
			{
				transform.Translate(new Vector3(0, 1, 0));
			}
			CancelDashDone(AnimatorUnit, 7);
		}
		// 攻撃した場合はステートが変更されるので、ここで終了
		if (!attack)
		{
			base.Animation_Idle(animator, downID, runID, stepanimations, fallID, jumpID, airdashID);
		}
	}

	
	

	/// <summary>
	/// 攻撃行動全般(RunとAirDashは特殊なので使用しない）
	/// </summary>
	/// <param name="run"></param>
	/// <param name="AirDash"></param>
	/// <returns></returns>
	public bool AttackDone(bool run = false, bool AirDash = false)
	{
		DestroyWrestle();
		DestroyArrow();
		// サブ射撃・特殊射撃・射撃で射撃へ移行
		if(HasSubShotInput || HasExShotInput || HasShotInput)
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
		// 特殊格闘で(特殊)格闘へ移行
		else if (HasExWrestleInput)
		{
			// 前特殊格闘(ブーストがないと実行不可)
			if (HasFrontInput && Boost > 0)
			{
				// 前特殊格闘実行
				FrontEXWrestleDone(AnimatorUnit, 7);
			}
			// 空中で後特殊格闘(ブーストがないと実行不可）
			else if (HasBackInput && !IsGrounded && Boost > 0)
			{
				// 後特殊格闘実行
				BackEXWrestleDone(AnimatorUnit, 8);
			}
			// それ以外
			else
			{
				// 格闘実行
				WrestleDone(AnimatorUnit, 1, "Wrestle1");
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
				AirDashWrestleDone(AnimatorUnit, AirDashSpeed, 5);
			}
			// 前格闘で前格闘へ移行
			else if (HasFrontInput)
			{
				// 前格闘実行(Character_Spec.cs参照)
				WrestleDone(AnimatorUnit, 4, "FrontWrestle");
			}
			// 後格闘で後格闘へ移行
			else if (HasBackInput)
			{
				// 後格闘実行（ガード）(Character_Spec.cs参照)
				GuardDone(AnimatorUnit, 5);
			}
			else
			{
				// それ以外ならN格闘実行(2段目と3段目の追加入力はWrestle1とWrestle2で行う
				WrestleDone(AnimatorUnit, 1, "Wrestle1");
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
			AnimatorUnit.SetTrigger("RunShot");
		}
		// 立ち射撃か空中射撃
		else
		{
			AnimatorUnit.SetTrigger("Shot");
		}
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
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
		animator.speed = Character_Spec.cs[(int)CharacterName][skillindex].m_Animspeed;

		// アニメーションを再生する
		animator.SetTrigger("FrontEXWrestle");

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
		animator.speed = Character_Spec.cs[(int)CharacterName][skillindex].m_Animspeed;

		// アニメーションを再生する
		animator.SetTrigger("BackEXWrestle");

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
		float movespeed = Character_Spec.cs[(int)CharacterName][skillindex].m_Movespeed;
		// 移動方向
		// ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
		if (IsRockon && this.transform.rotation.eulerAngles.y != 0)
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
		else if (this.transform.rotation.eulerAngles.y == 0)
		{
			// ただしそのままだとカメラが下を向いているため、一旦その分は補正する
			Quaternion rotateOR = MainCamera.transform.rotation;
			Vector3 rotateOR_E = rotateOR.eulerAngles;
			rotateOR_E.x = 0;
			rotateOR = Quaternion.Euler(rotateOR_E);
			this.MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
		}
		// それ以外は本体の角度を移動方向にする
		else
		{
			MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
		}
		// アニメーション速度
		float speed = Character_Spec.cs[(int)CharacterName][skillindex].m_Animspeed;

		// アニメーションを再生する
		animator.SetTrigger("EXWrestle");

		// アニメーションの速度を調整する
		animator.speed = speed;
		// 移動速度を調整する
		WrestlSpeed = movespeed;
	}

	/// <summary>
	/// 上昇中モーション
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="fallID"></param>
	/// <param name="stepanimations"></param>
	/// <param name="airdashID"></param>
	/// <param name="landinghashID"></param>
	protected override void Animation_Jumping(Animator animator, int fallID, int[] stepanimations, int airdashID, int landinghashID)
	{
		base.Animation_Jumping(animator, fallID, stepanimations, airdashID, landinghashID);
		AttackDone();
	}

	/// <summary>
	/// 落下モーション
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="airdashID"></param>
	/// <param name="jumpID"></param>
	/// <param name="stepanimations"></param>
	/// <param name="landingID"></param>
	protected override void Animation_Fall(Animator animator, int airdashID, int jumpID, int[] stepanimations, int landingID)
	{
		base.Animation_Fall(animator, airdashID, jumpID, stepanimations, landingID);
		AttackDone();
	}
}
