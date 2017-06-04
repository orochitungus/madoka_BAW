using UnityEngine;
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
	/// サブ射撃用の矢左
	/// </summary>
	public GameObject SubShotArrowL;

	/// <summary>
	/// サブ射撃用の矢右
	/// </summary>
	public GameObject SubShotArrowR;

	/// <summary>
	/// サブ射撃用弾丸の左フック
	/// </summary>
	public GameObject SubShotRootL;

	/// <summary>
	/// サブ射撃弾丸用の右フック
	/// </summary>
	public GameObject SubShotRootR;

	/// <summary>
	/// 特殊射撃の矢
	/// </summary>
	public GameObject EXShotArrow;

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
	///  射出する弾の方向ベクトル(サブ射撃左）
	/// </summary>
	public Vector3 BulletMoveDirectionL;

	/// <summary>
	///  射出する弾の方向ベクトル(サブ射撃右）
	/// </summary>
	public Vector3 BulletMoveDirectionR;

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
	private const int _MaxboneNum = 34;

	/// <summary>
	/// 覚醒技専用カメラ1個目
	/// </summary>
	public Camera ArousalAttackCamera1;

	/// <summary>
	/// 覚醒技専用カメラ2個目
	/// </summary>
	public Camera ArousalAttackCamera2;

	/// <summary>
	/// Ｎ格闘３段目専用カメラ
	/// </summary>
	public Camera WrestleCamera;

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

	/// <summary>
	/// 覚醒技の羽
	/// </summary>
	public GameObject ArousalAttackWing;

	/// <summary>
	/// 覚醒技の判定フック
	/// </summary>
	public GameObject ArousalAttackHock;

	/// <summary>
	/// 覚醒技の攻撃判定
	/// </summary>
	public HomuraBowArousalAttack ArousalAttackObject;

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

    void Awake()
	{
		// 空中ダッシュＩＤを保持（CharacterControlBaseで使う)
		CancelDashID = 7;
		// 覚醒技専用カメラをOFFにする
		ArousalAttackCamera1.enabled = false;
		ArousalAttackCamera2.enabled = false;

		// 格闘専用カメラをOFFにする
		WrestleCamera.enabled = false;

		// 羽をディアクティブにする
		ArousalAttackWing.SetActive(false);

		// 戦闘用インターフェースを取得する
		Battleinterfacecontroller = GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>();
		if(Battleinterfacecontroller == null)
		{
			Debug.LogError("Caution!! BattleInterfaceCanvas is Nothing!!");
		}

        // ハッシュID取得
        IdleID = Animator.StringToHash("Base Layer.HomuraBowIdle");
        WalkID = Animator.StringToHash("Base Layer.HomuraBowWalk");
        JumpID = Animator.StringToHash("Base Layer.HomuraBowJump");
        JumpingID = Animator.StringToHash("Base Layer.HomuraBowJumping");
        FallID = Animator.StringToHash("Base Layer.HomuraBowFall");
        LandingID = Animator.StringToHash("Base Layer.HomuraBowLanding");
        RunID = Animator.StringToHash("Base Layer.HomuraBowRun");
        AirDashID = Animator.StringToHash("Base Layer.HomuraBowAirDash");
        FrontStepID = Animator.StringToHash("Base Layer.HomuraBowFrontStep");
        FrontStepBackID = Animator.StringToHash("Base Layer.HomurabowFrontStepBack");
        LeftStepID = Animator.StringToHash("Base Layer.HomuraBowLeftStep");
        LeftStepBackID = Animator.StringToHash("Base Layer.HomuraBowLeftStepBack");
        RightStepID = Animator.StringToHash("Base Layer.HomuraBowRightStep");
        RightStepBackID = Animator.StringToHash("Base Layer.HomuraBowRightStepBack");
        BackStepID = Animator.StringToHash("Base Layer.HomuraBowBackStep");
        BackStepBackID = Animator.StringToHash("Base Layer.HomuraBowBackStepBack");
        ShotID = Animator.StringToHash("Base Layer.HomuraBowShot");
        RunShotID = Animator.StringToHash("Base Layer.HomuraBowRunShot");
        AirShotID = Animator.StringToHash("Base Layer.HomuraBowAirShot");
        ChargeShotID = Animator.StringToHash("Base Layer.HomuraBowChargeShot");
        SubShotID = Animator.StringToHash("Base Layer.HomuraBowSubShot");
        EXShotID = Animator.StringToHash("Base Layer.HomuraBowEXShot");
        FollowThrowShotID = Animator.StringToHash("Base Layer.HomuraBowShotForrowThrow");
        FollowThrowRunShotID = Animator.StringToHash("Base Layer.HomuraBowRunShotForrowThrow");
        FollowThrowAirShotID = Animator.StringToHash("Base Layer.HomuraBowAirShotForrowThrow");
        FollowThrowChargeShotID = Animator.StringToHash("Base Layer.HowmuraBowChargeShotFollowThrow");
        FollowThrowSubShotID = Animator.StringToHash("Base Layer.HomuraBowSubShotFollowThrow");
        FollowThrowEXShotID = Animator.StringToHash("Base Layer.HomuraBowEXShotFollowThrow");
        Wrestle1ID = Animator.StringToHash("Base Layer.HomuraBowWrestle1");
        Wrestle2ID = Animator.StringToHash("Base Layer.HomuraBowWrestle2");
        Wrestle3ID = Animator.StringToHash("Base Layer.HomuraBowWrestle3");
        FrontWrestleID = Animator.StringToHash("Base Layer.HomuraBowFrontWrestle");
        LeftWrestleID = Animator.StringToHash("Base Layer.HomuraBowLeftWrestle");
        RightWrestleID = Animator.StringToHash("Base Layer.HomuraBowRightWrestle");
        BackWrestleID = Animator.StringToHash("Base Layer.HomuraBowBackWrestle");
        AirDashWrestleID = Animator.StringToHash("Base Layer.HomuraBowBDWrestle");
        EXWrestleID = Animator.StringToHash("Base Layer.HomuraBowEXWrestle");
        EXFrontWrestleID = Animator.StringToHash("Base Layer.HomuraBowFrontEXWrestle");
        EXBackWrestleID = Animator.StringToHash("Base Layer.HomuraBowBackExWrestle");
        ReversalID = Animator.StringToHash("Base Layer.HomuraBowReversal");
        ArousalAttackID = Animator.StringToHash("Base Layer.HomuraBowArousalAttack");
        DamageID = Animator.StringToHash("Base Layer.HomuraBowDamage");
        DownID = Animator.StringToHash("Base Layer.HomurBowDown");
        BlowID = Animator.StringToHash("Base Layer.HomuraBowBlow");
        SpinDownID = Animator.StringToHash("Base Layer.HomuraBowSpindown");

		// リバーサルとダウンのIDを取得
		ReversalHash = ReversalID;
		DownHash = DownID;
    }

	// Use this for initialization
	void Start () 
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
        

        // HPを初期化

        // メイン射撃撃ち終わり時間
        MainshotEndtime = 0.0f;
        // サブ射撃撃ち終わり時間
        SubshotEndtime = 0.0f;
        // 特殊射撃撃ち終わり時間
        ExshotEndtime = 0.0f;
        
        // 共通ステートを初期化
        FirstSetting(AnimatorUnit, 0);

		// チャージショットチャージ時間
		ChargeMax = (int)Character_Spec.cs[(int)CharacterName][(int)(int)ShotType.CHARGE_SHOT].m_reloadtime*60;

		
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
            // 覚醒技発動中のみカメラ変更
            if(AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ArousalAttackID)
            {
                MainCamera.GetComponent<Player_Camera_Controller>().Maincameramode = MainCameraMode.HOMURABOWAROUSAL;
            }
            else
            {
                MainCamera.GetComponent<Player_Camera_Controller>().Maincameramode = MainCameraMode.NORMAL;
            }
        });

        this.UpdateAsObservable().Where(_ => IsPlayer == CHARACTERCODE.PLAYER).Subscribe(_ => 
		{
			// インターフェース制御
            // PC/僚機共通
            for(int i=0; i<3; i++)
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
            Battleinterfacecontroller.CharacterName.text = "暁美　ほむら";
            // プレイヤーレベル
            Battleinterfacecontroller.PlayerLv.text = "Level - " +  savingparameter.GetNowLevel(savingparameter.GetNowParty(0)).ToString();
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
            // 武装ゲージ(マミとさやかにモードチェンジに伴う武装ゲージ変化があるのでこっちに入れる）
            // 4番目と5番目は消す
            Battleinterfacecontroller.Weapon4.gameObject.SetActive(false);
            Battleinterfacecontroller.Weapon5.gameObject.SetActive(false);
            // 射撃
            Battleinterfacecontroller.Weapon3.Kind.text = "Shot";
            Battleinterfacecontroller.Weapon3.WeaponGraphic.sprite = ShotIcon;
            Battleinterfacecontroller.Weapon3.NowBulletNumber = BulletNum[(int)ShotType.NORMAL_SHOT];
            Battleinterfacecontroller.Weapon3.MaxBulletNumber = Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum;
            Battleinterfacecontroller.Weapon3.UseChargeGauge = true;
            // 1発でも使えれば使用可能
            if(BulletNum[(int)ShotType.NORMAL_SHOT] > 0)
            {
                Battleinterfacecontroller.Weapon3.Use = true;
            }
            else
            {
                Battleinterfacecontroller.Weapon3.Use = false;
            }

			// チャージ射撃
			Battleinterfacecontroller.Weapon3.NowChargeValue = (float)(ShotCharge)/(float)(ChargeMax);

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
	void Update()
	{
		// 共通アップデート処理(時間停止系の状態になると入力は禁止)
		bool isspindown = false;
		if(AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SpinDownID)
		{
			isspindown = true;
		}
		if(Update_Core(isspindown, AnimatorUnit, DownID, AirDashID,AirShotID,JumpingID,FallID,IdleID,BlowID,RunID,FrontStepID,LeftStepID,RightStepID,BackStepID,DamageID))
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

	void LateUpdate()
	{
		
	}

	void UpdateAnimation()
	{
		ShowAirDashEffect = false;		
		// 通常
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
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, LeftStepID, RightStepID, BackStepID);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		else if(AnimatorUnit.GetAnimatorTransitionInfo(0).fullPathHash == BackStepBackID)
		{
			Animation_StepBack(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ShotID)
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunShotID)
		{
            Shot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirShotID)
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ChargeShotID)
		{
            ChargeShot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SubShotID)
		{
            SubShot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXShotID)
		{
			ExShot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowShotID)
		{
            Shot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowRunShotID)
		{
            Shot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowAirShotID)
		{
            Shot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowChargeShotID)
		{
            ChargeShot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowSubShotID)
		{
            SubShot();
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowEXShotID)
		{
			ExShot();
		}
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
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontWrestleID)
		{
			FrontWrestle1(AnimatorUnit, 7);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftWrestleID)
		{
			LeftWrestle1(AnimatorUnit, 7);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightWrestleID)
		{
			RightWrestle1(AnimatorUnit, 7);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackWrestleID)
		{
			BackWrestle(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirDashWrestleID)
		{
			ShowAirDashEffect = true;
			AirDashWrestle(AnimatorUnit, 7, 4);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXWrestleID)
		{
			ExWrestle1();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXFrontWrestleID)
		{
            FrontExWrestle1(AnimatorUnit, 7, 4);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXBackWrestleID)
		{
			BackExWrestle(AnimatorUnit, 7, 4, 5);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ReversalID)
		{
			Reversal();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ArousalAttackID)
		{
			ArousalAttack();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DamageID)
		{
			Damage(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DownID)
		{
			Down(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BlowID)
		{
			Blow(AnimatorUnit);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SpinDownID)
		{
			SpinDown(AnimatorUnit);
		}

		if(ShowAirDashEffect)
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
        AnimatorUnit.SetTrigger("Jumping");
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

            // 覚醒前処理を未実行に変更
            Arousalattackstate = ArousalAttackState.INITIALIZE;
            // ステートを変更
            AnimatorUnit.SetTrigger("Walk");
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
                FrontEXWrestleDone(AnimatorUnit, 13);
			}
			// 空中で後特殊格闘(ブーストがないと実行不可）
			else if (HasBackInput && !IsGrounded && Boost > 0)
			{
				// 後特殊格闘実行
				BackEXWrestleDone(AnimatorUnit, 14);
			}
			// それ以外
			else
			{
				// 特殊格闘実行
				EXWrestleDone(AnimatorUnit, 12);
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
				EmagencyStop(AnimatorUnit);
			}
			// チャージショット実行
			ChagerShotDone();
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
				WrestleDone(AnimatorUnit, 7, "FrontWrestle");
			}
			// 左格闘で左格闘へ移行
			else if (HasLeftInput)
			{
				// 左格闘実行(Character_Spec.cs参照)
				WrestleDone_GoAround_Left(AnimatorUnit, 8);
			}
			// 右格闘で右格闘へ移行
			else if (HasRightInput)
			{
				// 右格闘実行(Character_Spec.cs参照)
				WrestleDone_GoAround_Right(AnimatorUnit, 9);
			}
			// 後格闘で後格闘へ移行
			else if (HasBackInput)
			{
				// 後格闘実行（ガード）(Character_Spec.cs参照)
				GuardDone(AnimatorUnit, 10);
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
	
	/// <summary>
	/// 射撃の装填開始
	/// </summary>
	/// <param name="ruhshot">走行射撃であるか否か</param>
	public void ShotDone(bool runshot)
	{
		// 歩行時は上半身のみにして走行（下半身のみ）と合成
		if(runshot)
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

	public void ChagerShotDone()
	{
		AnimatorUnit.SetTrigger("ChargeShot");
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	public void SubShotDone()
	{
		AnimatorUnit.SetTrigger("SubShot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	public void EXShotDone()
	{
		AnimatorUnit.SetTrigger("EXShot");
		// 装填状態へ移行
		Shotmode = ShotMode.RELORD;
		// 位置固定を行う
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
	}

	protected override void Animation_Idle(Animator animator)
	{
        // 攻撃したかフラグ
        bool attack = false;
        // くっついている弾丸系のエフェクトを消す
        DestroyArrow();
		// 羽フックを壊す
		var wh = gameObject.transform.FindChild("WINGHOCK");
		if (wh != null)
		{
			Destroy(wh);
		}
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

    protected override void Animation_Run(Animator animator)
    {
        base.Animation_Run(animator);
        AttackDone(true, false);
    }

    protected override void Animation_AirDash(Animator animator)
    {
        base.Animation_AirDash(animator);
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
            CancelDashDone(AnimatorUnit);
        }
        base.Shot();
    }

	/// <summary>
	/// 装填を行う（アニメーションファイルの装填フレームにインポートする）
	/// </summary>
	/// <param name="type">射撃がどれであるか</param>
	public void Shootload(ShotType type)
	{
		// 弾があるとき限定（チャージショット除く）
		if (BulletNum[(int)type] > 0 || type == ShotType.CHARGE_SHOT)
		{
			// ロックオン時本体の方向を相手に向ける       
			if (IsRockon)
			{
				RotateToTarget();
			}
			// 弾を消費する（サブ射撃なら1、特殊射撃なら2）
			// チャージ射撃除く
			if (type != ShotType.CHARGE_SHOT)
			{
				BulletNum[(int)type]--;
				// 撃ち終わった時間を設定する                
				// メイン（弾数がMax-1のとき）
				if (type == ShotType.NORMAL_SHOT && BulletNum[(int)type] == Character_Spec.cs[(int)CharacterName][(int)type].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)type].m_OriginalBulletNum - 1)
				{
					MainshotEndtime = Time.time;
				}
				// サブ（弾数が0のとき）
				else if (type == ShotType.SUB_SHOT&& BulletNum[(int)type] == 0)
				{
					SubshotEndtime = Time.time;
				}
				// 特殊（弾数がMax-1のとき）
				else if (type == ShotType.EX_SHOT && BulletNum[(int)type] == Character_Spec.cs[(int)CharacterName][(int)type].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)type].m_OriginalBulletNum - 1)
				{
					ExshotEndtime = Time.time;
				}
			}
			// 矢の出現ポジションをフックと一致させる
			Vector3 pos = MainShotRoot.transform.position;
			Quaternion rot = Quaternion.Euler(MainShotRoot.transform.rotation.eulerAngles.x, MainShotRoot.transform.rotation.eulerAngles.y, MainShotRoot.transform.rotation.eulerAngles.z);
			// 矢を出現させる
			// サブ射撃
			if (type == ShotType.SUB_SHOT)
			{
				// 左の矢の角度
				Vector3 posLArrow = SubShotRootL.transform.position;
				Quaternion rotLArrow = Quaternion.Euler(SubShotRootL.transform.rotation.eulerAngles.x, SubShotRootL.transform.rotation.eulerAngles.y, SubShotRootL.transform.rotation.eulerAngles.z);
				// 右の矢の角度
				Vector3 posRArrow = SubShotRootR.transform.position;
				Quaternion rotRArrow = Quaternion.Euler(SubShotRootR.transform.rotation.eulerAngles.x, SubShotRootR.transform.rotation.eulerAngles.y, SubShotRootR.transform.rotation.eulerAngles.z);
				// 中央の矢を作る(配置位置は通常射撃の場所と同じ）
				var centerArrow = (GameObject)Instantiate(SubShotArrow, pos, rot);
				if(centerArrow.transform.parent == null)
				{
					centerArrow.transform.parent = MainShotRoot.transform;
					centerArrow.transform.GetComponent<Rigidbody>().isKinematic = true;
					centerArrow.name = "SubShotCenter";
				}
				// 左の矢を作る
				var leftArrow = (GameObject)Instantiate(SubShotArrowL, posLArrow, rotLArrow);
				if (leftArrow.transform.parent == null)
				{
					leftArrow.transform.parent = SubShotRootL.transform;
					leftArrow.transform.GetComponent<Rigidbody>().isKinematic = true;
					leftArrow.name = "SubShotLeft";
				}
				// 右の矢を作る
				var rightArrow = (GameObject)Instantiate(SubShotArrowR, posRArrow, rotRArrow);
				if (rightArrow.transform.parent == null)
				{
					rightArrow.transform.parent = SubShotRootR.transform;
					rightArrow.transform.GetComponent<Rigidbody>().isKinematic = true;
					rightArrow.name = "SubShotRight";
				}
			}
			// 特殊射撃
			else if (type == ShotType.EX_SHOT)
			{
				// 特殊射撃の矢を作る処理
				var obj = (GameObject)Instantiate(EXShotArrow, pos, rot);
				// 親子関係を再設定する
				if (obj.transform.parent == null)
				{
					obj.transform.parent = MainShotRoot.transform;
					// 矢の親子関係を付けておく
					obj.transform.GetComponent<Rigidbody>().isKinematic = true;
				}
			}
			// 通常射撃
			else if (type == ShotType.NORMAL_SHOT)
			{
				var obj = (GameObject)Instantiate(NormalArrow, pos, rot);
				// 親子関係を再設定する(=矢をフックの子にする）
				if (obj.transform.parent == null)
				{
					obj.transform.parent = MainShotRoot.transform;
					// 矢の親子関係を付けておく
					obj.transform.GetComponent<Rigidbody>().isKinematic = true;
				}
			}
			// チャージ射撃（これはBulletではなくLaserなので通常射撃・サブ射撃とは処理が異なり、この時点で発射される）
			else if (type == ShotType.CHARGE_SHOT)
			{
				// チャージ射撃の矢を作る処理
				var obj = (GameObject)Instantiate(ChargeShotArrow, pos, rot);
				// 親子関係を再設定する(=矢をフックの子にする)
				if(obj.transform.parent == null)
				{
					obj.transform.parent = MainShotRoot.transform;
				}
			}
		}
	}

	
	
	/// <summary>
	/// 射撃（射出）射撃のアニメーションファイルの射出フレームにインポートする
	/// </summary>
	/// <param name="type"></param>
    public void Shooting(ShotType type)
	{
		// 通常射撃の矢
		var arrow = GetComponentInChildren<HomuraBowNormalShot>();
		// サブ射撃中央の矢
		var subshotArrowCenter = MainShotRoot.GetComponentInChildren<HomuraBowSubShot>();
		// サブ射撃左右の矢
		var subshotArrowLeft = SubShotRootL.GetComponentInChildren<HomuraBowSubShotL>();
		var subshotArrowRight = SubShotRootR.GetComponentInChildren<HomuraBowSubShotR>();
		// 特殊射撃の矢
		var exshotArrow = GetComponentInChildren<HomuraBowEXShot>();


		if (arrow != null || subshotArrowCenter != null || exshotArrow != null)
		{
			// 矢のスクリプトの速度を設定する
			// チャージ射撃は若干速く
			if (type == ShotType.CHARGE_SHOT)
			{
				arrow.ShotSpeed = Character_Spec.cs[(int)CharacterName][1].m_Movespeed;
			}
			else
			{
				// メイン弾速設定
				if (arrow != null)
				{
					arrow.ShotSpeed = Character_Spec.cs[(int)CharacterName][0].m_Movespeed;
				}
				// サブ射撃の矢の弾速設定
				if(subshotArrowCenter != null)
				{
					subshotArrowCenter.ShotSpeed = Character_Spec.cs[(int)CharacterName][2].m_Movespeed;
				}
				if (subshotArrowLeft != null)
				{
					subshotArrowLeft.ShotSpeed = Character_Spec.cs[(int)CharacterName][2].m_Movespeed;
				}
				if (subshotArrowRight != null)
				{
					subshotArrowRight.ShotSpeed = Character_Spec.cs[(int)CharacterName][2].m_Movespeed;
				}
				// 特殊射撃の矢の弾速設定
				if(exshotArrow != null)
				{
					exshotArrow.ShotSpeed = Character_Spec.cs[(int)CharacterName][3].m_Movespeed;
				}
			}
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
				Vector3 addrot = mainrot.eulerAngles + normalizeRot_OR - new Vector3(0, 74.0f, 0);
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
				if (arrow != null)
				{
					arrow.MoveDirection = Vector3.Normalize(normalizeRot);
				}
				// サブ射撃の矢中央
				if(subshotArrowCenter != null)
				{
					subshotArrowCenter.MoveDirection = Vector3.Normalize(normalizeRot);
				}
				// サブ射撃の矢左
				if (subshotArrowLeft != null)
				{
					// 角度
					Vector3 normalizeRotLOR = mainrot.eulerAngles;
					// 角度再調整
					Vector3 normalizeRotL = new Vector3(normalizeRotLOR.x, normalizeRotLOR.y - 20, normalizeRotLOR.z);
					subshotArrowLeft.MoveDirection = Vector3.Normalize(normalizeRotL);
				}
				// サブ射撃の矢右
				if (subshotArrowRight != null)
				{
					// 角度
					Vector3 normalizeRotROR = mainrot.eulerAngles;
					// 角度再調整
					Vector3 normalizeRotR = new Vector3(normalizeRotROR.x, normalizeRotROR.y + 20, normalizeRotROR.z);
					subshotArrowRight.MoveDirection = Vector3.Normalize(normalizeRotR);
				}
				// 特殊射撃の矢
				if(exshotArrow != null)
				{
					// 相手の頭上に向けて飛んでいく
					Vector3 landingpos = new Vector3(targetpos.x, targetpos.y + 50, targetpos.z);
					// 到達座標を代入する
					exshotArrow.FunnelInjectionTargetPos = landingpos;
					// 本体の回転角度を拾う
					Quaternion mainrot2 = Quaternion.LookRotation(landingpos - transform.position);
					// 正規化して代入する
					Vector3 normalizerot2 = mainrot2 * Vector3.forward;
					exshotArrow.MoveDirection = Vector3.Normalize(normalizerot2);		
				}
			}
			// ロックオンしていないとき
			else
			{
				// 本体角度が0の場合カメラの方向を射出角とし、正規化して代入する
				if (this.transform.rotation.eulerAngles == Vector3.zero)
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
					// サブ射撃の矢中央
					if (subshotArrowCenter != null)
					{
						subshotArrowCenter.MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
					}
					// サブ射撃の矢左
					if (subshotArrowLeft != null)
					{
						// 角度再調整
						Vector3 normalizeRotL = new Vector3(rotateOR.x , rotateOR.y-20, rotateOR.z);
						subshotArrowLeft.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotL) * Vector3.forward);
					}
					// サブ射撃の矢右
					if (subshotArrowRight != null)
					{
						// 角度再調整
						Vector3 normalizeRotR = new Vector3(rotateOR.x, rotateOR.y + 20, rotateOR.z);
						subshotArrowRight.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotR) * Vector3.forward);
					}
					// 特殊射撃の矢
					if(exshotArrow != null)
					{
						// 角度再調整
						Vector3 exshotRot = new Vector3(rotateOR.x - 30, rotateOR.y, rotateOR.z);
                        // 到達位置は角度を抜いた直線距離が規定距離になったところになる（r*cos(30)=距離になった場所。rは射出点と現在の距離）
                        // 到達位置は直線距離100の場所
                        // 到達位置算出
                        float x = 100 * Mathf.Sin(30 * Mathf.Deg2Rad) * Mathf.Sin(rotateOR.y * Mathf.Deg2Rad) + transform.position.x;
                        float y = 100 * Mathf.Cos(30 * Mathf.Deg2Rad) + transform.position.y;
                        float z = 100 * Mathf.Sin(30 * Mathf.Deg2Rad) * Mathf.Cos(rotateOR.y * Mathf.Deg2Rad) + transform.position.z;
                        exshotArrow.FunnelInjectionTargetPos = new Vector3(x, y, z);
                        // 射出ベクトル決定
                        exshotArrow.MoveDirection = Vector3.Normalize(Quaternion.Euler(exshotRot) * Vector3.forward);
                    }
				}
				// それ以外は本体の角度を射出角にする
				else
				{
					// 通常射撃の矢
					if (arrow != null)
					{
						arrow.MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
					}
					// サブ射撃の矢中央
					if (subshotArrowCenter != null)
					{
						subshotArrowCenter.MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
					}
					// サブ射撃の矢左
					if (subshotArrowLeft != null)
					{
						// 本体角度算出
						Vector3 mainrotOR = transform.rotation.eulerAngles;
						// 角度再調整
						Vector3 normalizeRotL = new Vector3(mainrotOR.x, mainrotOR.y - 20, mainrotOR.z);
						subshotArrowLeft.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotL) * Vector3.forward);
					}
					// サブ射撃の矢右
					if (subshotArrowRight != null)
					{
						// 本体角度算出
						Vector3 mainrotOR = transform.rotation.eulerAngles;
						// 角度再調整
						Vector3 normalizeRotR = new Vector3(mainrotOR.x, mainrotOR.y + 20, mainrotOR.z);
						subshotArrowRight.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotR) * Vector3.forward);
					}
                    // 特殊射撃の矢
                    if (exshotArrow != null)
                    {
                        // 本体角度算出
                        Vector3 mainrotOR = transform.rotation.eulerAngles;
                        // 角度再調整
                        Vector3 exshotRot = new Vector3(mainrotOR.x - 30, mainrotOR.y, mainrotOR.z);
						// 到達位置算出
						float x = 100 * Mathf.Sin(30 * Mathf.Deg2Rad) * Mathf.Sin(mainrotOR.y * Mathf.Deg2Rad);
						float y = 100 * Mathf.Cos(30 * Mathf.Deg2Rad);
						float z = 100 * Mathf.Sin(30 * Mathf.Deg2Rad) * Mathf.Cos(mainrotOR.y * Mathf.Deg2Rad);
						exshotArrow.FunnelInjectionTargetPos = new Vector3(x, y, z);
                        // 射出ベクトル決定
                        exshotArrow.MoveDirection = Vector3.Normalize(Quaternion.Euler(exshotRot) * Vector3.forward);
                    }
                }
			}
						
			// 矢の移動ベクトルを代入する
			// 通常射撃
			if (arrow != null)
			{
				BulletMoveDirection = arrow.MoveDirection;
			}
			// サブ射撃中央
			if(subshotArrowCenter != null)
			{
				BulletMoveDirection = subshotArrowCenter.MoveDirection;
			}
			// サブ射撃左
			if(subshotArrowLeft != null)
			{
				BulletMoveDirectionL = subshotArrowLeft.MoveDirection;
			}
			// サブ射撃右
			if (subshotArrowRight != null)
			{
				BulletMoveDirectionR = subshotArrowRight.MoveDirection;
			}
            // 特殊射撃
            if(exshotArrow != null)
            {
                BulletMoveDirection = exshotArrow.MoveDirection;
            }

			// 攻撃力を代入する
			if (type == ShotType.NORMAL_SHOT)
			{
				SetOffensivePower(SkillType_Homura_B.SHOT);
			}
			else if (type == ShotType.CHARGE_SHOT)
			{
				SetOffensivePower(SkillType_Homura_B.CHARGE_SHOT);
			}
			else if (type == ShotType.SUB_SHOT)
			{
				SetOffensivePower(SkillType_Homura_B.SUB_SHOT);
			}
			else if (type == ShotType.EX_SHOT)
			{
				// 特殊射撃はShotSpecに入れるためSetOffensivePowerは使えない
				// 覚醒ゲージ上昇量
				exshotArrow.Shotspec.ArousalRatio = Character_Spec.cs[(int)CharacterName][3].m_arousal;
				// 射出したのは誰であるか
				exshotArrow.Shotspec.CharacterIndex = (int)CharacterName;
				// ダウン値
				exshotArrow.Shotspec.DownRatio = Character_Spec.cs[(int)CharacterName][3].m_DownPoint;
				// ヒット時の挙動
				exshotArrow.Shotspec.Hittype = Character_Spec.cs[(int)CharacterName][3].m_Hittype;
				// 射出したのは誰であるか（ゲームオブジェクト）
				exshotArrow.Shotspec.ObjOR = this.gameObject;
				// 攻撃力
				exshotArrow.Shotspec.OffensivePower = Character_Spec.cs[(int)CharacterName][3].m_OriginalStr + Character_Spec.cs[(int)CharacterName][3].m_GrowthCoefficientStr * (this.StrLevel - 1);
			}

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
		if(type == ShotType.NORMAL_SHOT)
		{
			// 走行時
			if (RunShotDone)
			{
                AnimatorUnit.SetTrigger("RunFollowThrow");
			}
			// 通常時
			else
			{
                AnimatorUnit.SetTrigger("FollowThrow");
			}
		}
		else if(type == ShotType.SUB_SHOT)
		{
            AnimatorUnit.SetTrigger("SubShotFollowThrow");
		}
		else if(type == ShotType.EX_SHOT)
		{
            AnimatorUnit.SetTrigger("EXShotFollowThrow");
		}
	}

	/// <summary>
	/// チャージショットの場合フォロースルーに移行する
	/// </summary>
	public void ChargeShotFollowThrow()
	{
		AnimatorUnit.SetTrigger("ChargerShotFollowThrow");
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

	// 射撃攻撃の攻撃力とダウン値を決定する（特殊射撃除く）
	// kind[in]     :射撃の種類
	private void SetOffensivePower(SkillType_Homura_B kind)
	{
		// 攻撃力を決定する(ここの2がスキルのインデックス。下も同様）
		OffensivePowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)kind].m_OriginalStr + Character_Spec.cs[(int)CharacterName][(int)kind].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ダウン値を決定する
		DownratioPowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)kind].m_DownPoint;
		// 覚醒ゲージ増加量を決定する
		ArousalRatioOfBullet = Character_Spec.cs[(int)CharacterName][(int)kind].m_arousal;
	}

	/// <summary>
	/// キャンセルなどで本体に矢があった時消す
	/// </summary>
	protected override void DestroyArrow()
	{
		base.DestroyArrow();
		// サブ射撃Ｌ
		int ChildCountL = SubShotRootL.transform.childCount;
		for (int i = 0; i<ChildCountL; i++)
		{
			Transform left = SubShotRootL.transform.GetChild(i);
			Destroy(left.gameObject);
		}
		// サブ射撃Ｒ
		int ChildCountR = SubShotRootR.transform.childCount;
		for (int i = 0; i<ChildCountR; i++)
		{
			Transform right = SubShotRootR.transform.GetChild(i);
			Destroy(right.gameObject);
		}
	}

    /// <summary>
    /// 格闘攻撃終了後、派生を行う
    /// </summary>
    /// <param name="nextmotion"></param>
    public void WrestleFinish(WrestleType nextmotion )
    {
		// 判定オブジェクトを全て破棄
		DestroyWrestle();
		// 格闘の累積時間を初期化
		Wrestletime = 0;
		// 派生入力があった場合は派生する
		if (AddInput)
        {
            // N格闘２段目派生
            if (nextmotion == WrestleType.WRESTLE_2)
            {
                WrestleDone(AnimatorUnit, 5, "Wrestle2");
            }
            // N格闘３段目派生
            else if (nextmotion == WrestleType.WRESTLE_3)
            {
                WrestleDone(AnimatorUnit, 6, "Wrestle3");
            }
        }
		// なかったら戻す
		else
		{
			ReturnToIdle();
		}
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
	/// 特殊格闘
	/// </summary>
	protected override void ExWrestle1()
	{
		base.ExWrestle1();
		Wrestletime += Time.deltaTime;
		StepCancel(AnimatorUnit);
	}

    /// <summary>
    /// 前特殊格闘
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    /// <param name="fallid"></param>
    protected override void FrontExWrestle1(Animator animator, int airdashhash, int fallid)
    {
        base.FrontExWrestle1(animator, airdashhash, fallid);
        Wrestletime += Time.deltaTime; 
        // レバー入力カットか特殊格闘入力カットで落下に移行する
        if(ControllerManager.Instance.TopUp || ControllerManager.Instance.EXWrestleUp)
        {
            FallDone(Vector3.zero, animator);
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
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    /// <param name="fallid"></param>
    /// <param name="landingid"></param>
    protected override void BackExWrestle(Animator animator, int airdashhash, int fallid, int landingid)
    {
        base.BackExWrestle(animator, airdashhash, fallid, landingid);
        // レバー入力カットか特殊格闘入力カットで落下に移行する
        if (ControllerManager.Instance.UnderUp || ControllerManager.Instance.EXWrestleUp)
        {
            FallDone(Vector3.zero, animator);
        }
		// 移動速度（上方向に垂直上昇する）
		float movespeed = 100.0f;

		// 移動方向（移動目的のため、とりあえず垂直上昇させる）
		MoveDirection = Vector3.Normalize(new Vector3(0, -1, 0));

		// 移動速度を調整する
		WrestlSpeed = movespeed;
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
			MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
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
	/// 弾丸全回復処理
	/// </summary>
	public void FullReload()
	{		
		// 弾丸を回復させる
		for (int i = 0; i < Character_Spec.cs[(int)CharacterName].Length; i++)
		{
			// 使用の可否を初期化
			WeaponUseAble[i] = true;
			// 弾があるものは残弾数を初期化
			if (Character_Spec.cs[(int)CharacterName][i].m_OriginalBulletNum > 0)
			{
				BulletNum[i] = Character_Spec.cs[(int)CharacterName][i].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][i].m_OriginalBulletNum;
			}
			// 硬直時間があるものは硬直時間を初期化
			if (Character_Spec.cs[(int)CharacterName][i].m_WaitTime > 0)
			{
				BulletWaitTime[i] = Character_Spec.cs[(int)CharacterName][i].m_GrowthCoefficientBul * (BulLevel - 1) + Character_Spec.cs[(int)CharacterName][i].m_WaitTime;
			}
		}
	}

	/// <summary>
	/// 覚醒技中のステート定義
	/// </summary>
	private enum ArousalAttackState
	{
		INITIALIZE,     // モーションを歩行へ変更&重力無効化＆羽フックとりつけ
		FEATHERSET,     // 移動しつつエフェクトと判定を順番に取り付け
		ATTACK,         // （ロックオンしている相手に向けて）飛行
		END             // 覚醒ゲージが空になると、Armorを戻してfallへ移行
	};

	/// <summary>
	/// 覚醒技中のステート
	/// </summary>
	private ArousalAttackState Arousalattackstate;

	/// <summary>
	/// 覚醒技発動処理
	/// </summary>
	/// <param name="animator"></param>
	protected override void Animation_Walk(Animator animator)
	{
		base.Animation_Walk(animator);
		switch (Arousalattackstate)
		{
			case ArousalAttackState.INITIALIZE: // 重力を無効化する&本体の羽をアクティブにする&&威力を初期化する
				// 重力無効
				RigidBody.useGravity = false;
				// 羽をアクティブにする
				ArousalAttackWing.SetActive(true);
				Arousalattackstate = ArousalAttackState.FEATHERSET;
				// 変数を初期化する
				_WingAppearCounter = 0;
				_WingboneCounter = 1;
				_LeftwingSet = false;
				ArousalAttackTime = 0;
				// 専用カメラをONにする
				ArousalAttackCamera1.enabled = true;
				// 覚醒技演出フラグをONにする
				ArousalAttackProduction = true;
				// 火力設定
				int offensive = 300 + StrLevel * 30;
				// 威力を初期化する
				ArousalAttackObject.SetStatus(offensive, 5, CharacterSkill.HitType.BEND_BACKWARD);
				break;
			case ArousalAttackState.FEATHERSET: // カメラを切り替えつつ前進させる
				// 前方向に向けて歩き出す
				// ロックオン時は強制的に相手の方向を向く
				if (IsRockon)
				{
					// 敵（ロックオン対象）の座標を取得
					var targetspec = GetComponentInChildren<Player_Camera_Controller>();
					Vector3 targetpos = targetspec.Enemy.transform.position;
					targetpos = new Vector3(targetpos.x, 0, targetpos.z);
					// 自分の座標を取得
					Vector3 myPos = this.transform.position;
					myPos = new Vector3(myPos.x, 0, myPos.z);
					transform.rotation = Quaternion.LookRotation(targetpos - myPos);
				}
				MoveDirection = transform.rotation * Vector3.forward;
				RigidBody.position = GetComponent<Rigidbody>().position + MoveDirection * WalkSpeed * Time.deltaTime;
			
				// m_wingappearTime経過して、左の羽根をつけていないなら左の羽根と判定をつける
				if (_WingAppearCounter < 11 && !_LeftwingSet)
				{
					_LeftwingSet = true;
				}
				// さらにm_wingapperTime経過したら右の羽根と判定をつけてリセット
				else if (_WingAppearCounter > _WingappearTime * 2)
				{
					_WingAppearCounter = 0;
					_LeftwingSet = false;
					_WingboneCounter++;
					// 半分出したらカメラを横に
					if (_WingboneCounter > 9)
					{
						ArousalAttackCamera1.enabled = false;
						ArousalAttackCamera2.enabled = true;
					}
					// 全部出したらカメラを戻して飛行ポーズにして次へ行く
					if (_WingboneCounter > 17)
					{
						ArousalAttackCamera2.enabled = false;
						AnimatorUnit.SetTrigger("ArousalAttack");
						Arousalattackstate = ArousalAttackState.ATTACK;
						// 演出フラグを折る
						ArousalAttackProduction = false;
					}
				}
				_WingAppearCounter += Time.deltaTime;
				break;			
		}
	}

	
	/// <summary>
	/// 覚醒技処理
	/// </summary>
	protected override void ArousalAttack()
	{
		base.ArousalAttack();
		switch (Arousalattackstate)
		{			
			case ArousalAttackState.ATTACK: // （ロックオンしている相手に向けて）飛行開始
				// 前方向に向けて飛行開始
				RigidBody.position = RigidBody.position + MoveDirection * AirDashSpeed * Time.deltaTime;
				ArousalAttackTime += Time.deltaTime;
				// 飛行時間を終えたらENDへ移行
				if (ArousalAttackTime > ArousalAttackTotal)
				{
					ArousalAttackWing.SetActive(false);
					Arousalattackstate = ArousalAttackState.END;
				}
				break;
			case ArousalAttackState.END:    // 覚醒ゲージが空になったので、Armorを戻してIdleへ移行
				// アーマーフラグ解除
				IsArmor = false;
				AnimatorUnit.SetTrigger("Idle");
				break;
		}
	}

	protected override void Down(Animator animator)
	{
		// 死んだ時は羽を消しておく
		ArousalAttackWing.SetActive(false);
		base.Down(animator);
	}

}
