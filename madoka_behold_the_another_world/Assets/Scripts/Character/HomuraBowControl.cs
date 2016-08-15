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
	/// サブ射撃用弾丸の左フック
	/// </summary>
	public GameObject SubShotRootL;

	/// <summary>
	/// サブ射撃弾丸用の右フック
	/// </summary>
	public GameObject SubShotRootR;

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

    /// <summary>
    /// 覚醒技の羽根のフック
    /// </summary>
    private GameObject WingHock;

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
    /// 各種アニメのハッシュID.コメント内はAnimatorの管理用ID.武装系以外は全員共通にすること
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
		// 覚醒技専用カメラをOFFにする
		ArousalAttackCamera1.enabled = false;
		ArousalAttackCamera2.enabled = false;

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
        RightWrestleID = Animator.StringToHash("HomuraBowRightWrestle");
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
        BulletPos = Vector3.zero;

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
            Battleinterfacecontroller.MaxArousal = ArousalLevel * ArousalGrowth;
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
		if(Update_Core(isspindown, AnimatorUnit, DownID, AirDashID,AirShotID,JumpingID,FallID,IdleID,BlowID,RunID,FrontStepID,LeftStepID,RightStepID,BackStepID))
		{
            UpdateAnimation();
            // リロード実行           
            // メイン射撃
            ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_reloadtime, ref MainshotEndtime);
            // サブ射撃
            ReloadSystem.AllTogether(ref BulletNum[(int)ShotType.SUB_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)CharacterName][(int)ShotType.SUB_SHOT].m_reloadtime, ref SubshotEndtime);
            // 特殊射撃
            ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.EX_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)CharacterName][(int)ShotType.EX_SHOT].m_reloadtime, ref ExshotEndtime);
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
			Animation_Idle(AnimatorUnit, 42, 6,stepanimations,4,2,7);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == WalkID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpID)
		{
			Animation_Jump(AnimatorUnit, 2);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == JumpingID)
		{
			int[] stepanimations = { 8, 9, 10, 11 };
			Animation_Jumping(AnimatorUnit, 4, stepanimations, 7, 5);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FallID)
		{
			int[] stepanimations = { 8, 9, 10, 11 };
			Animation_Fall(AnimatorUnit, 7, 2, stepanimations, 5);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LandingID)
		{
			Animation_Landing(AnimatorUnit, 0);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunID)
		{
            int[] stepanimations = { 8, 9, 10, 11 };
            Animation_Run(AnimatorUnit, 4, stepanimations, 0, 2);
        }
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirDashID)
		{
            Animation_AirDash(AnimatorUnit, 2, 4, 5);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, 12, LeftStepID, 13, RightStepID, 14, BackStepID, 15,7,2);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, 12, LeftStepID, 13, RightStepID, 14, BackStepID, 15, 7, 2);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, 12, LeftStepID, 13, RightStepID, 14, BackStepID, 15, 7, 2);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackStepID)
		{
			Animation_StepDone(AnimatorUnit, FrontStepID, 12, LeftStepID, 13, RightStepID, 14, BackStepID, 15, 7, 2);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontStepBackID)
		{
			Animation_StepBack(AnimatorUnit, 0);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftStepBackID)
		{
			Animation_StepBack(AnimatorUnit, 0);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightStepBackID)
		{
			Animation_StepBack(AnimatorUnit, 0);
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ShotID)
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RunShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirShotID)
		{
			Shot();
		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ChargeShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SubShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowRunShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowAirShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowChargeShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowSubShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FollowThrowEXShotID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle1ID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle2ID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == Wrestle3ID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == FrontWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == LeftWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == RightWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BackWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == AirDashWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXFrontWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == EXBackWrestleID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ReversalID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == ArousalAttackID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DamageID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == DownID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == BlowID)
		{

		}
		else if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == SpinDownID)
		{

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
	public void AttackDone(bool run = false, bool AirDash = false)
	{
		// サブ射撃でサブ射撃へ移行
		if (HasSubShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// 強制停止実行
				EmagencyStop(AnimatorUnit);
			}
			// サブ射撃実行
			SubShotDone();
		}
		// 特殊射撃で特殊射撃へ移行
		else if (HasExShotInput)
		{
			// 歩き撃ちはしないので、強制停止
			if (run || AirDash)
			{
				// TODO:強制停止実行
			}
			// TODO:特殊射撃実行
		}
		// 特殊格闘で特殊格闘へ移行
		else if (HasExWrestleInput)
		{
			// 前特殊格闘
			if (HasFrontInput)
			{
				// TODO:前特殊格闘実行
			}
			// 空中で後特殊格闘
			else if (HasBackInput && !IsGrounded)
			{
				// TODO:後特殊格闘実行
			}
			// それ以外
			else
			{
				// TODO:特殊格闘実行
			}
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
		}
		// 射撃で射撃へ移行
		else if (HasShotInput)
		{
			if (run)
			{
				if (this.IsRockon)
				{
					// ①　transform.TransformDirection(Vector3.forward)でオブジェクトの正面の情報を得る
					var forward = this.transform.TransformDirection(Vector3.forward);
					// ②　自分の場所から対象との距離を引く
					// カメラからEnemyを求める
					var target = MainCamera.transform.GetComponentInChildren<Player_Camera_Controller>();
					var targetDirection = target.Enemy.transform.position - transform.position;
					// ③　①と②の角度をVector3.Angleで取る　			
					float angle = Vector3.Angle(forward, targetDirection);
					// 角度60度以内なら上体回しで撃つ（歩き撃ち限定で上記の矢の方向ベクトルを加算する）
					if (angle < 60)
					{
						RunShotDone = true;
						// TODO:走行射撃実行
					}
					// それ以外なら強制的に停止して（立ち撃ちにして）撃つ
					else
					{
						// TODO:強制停止実行
						// TODO:射撃実行
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
		}
		// 格闘で格闘へ移行
		else if (HasWrestleInput)
		{
			if (AirDash)
			{
				// TODO:空中ダッシュ格闘実行
			}
			else
			{
				// 前格闘と横格闘への分岐はここで（2段目・3段目の分岐はやっているときに）

				// 前格闘で前格闘へ移行
				if (HasFrontInput)
				{
					// TODO:前格闘実行
				}
				// 左格闘で左格闘へ移行
				else if (HasLeftInput)
				{
					// TODO:左格闘実行
				}
				// 右格闘で右格闘へ移行
				else if (HasRightInput)
				{
					// TODO:右格闘実行
				}
				// 後格闘で後格闘へ移行
				else if (HasBackInput)
				{
					// TODO:後格闘実行（ガード）
				}
				else
				{
					// N格闘1段目  
					// TODO:N格闘実行
				}
			}
		}
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
	}

	public void SubShotDone()
	{
		AnimatorUnit.SetTrigger("SubShot");
	}

	protected override void Animation_Idle(Animator animator, int downID, int runID, int[] stepanimations, int fallID, int jumpID, int airdashID)
	{
		base.Animation_Idle(animator, downID, runID, stepanimations, fallID, jumpID, airdashID);
        // くっついている弾丸系のエフェクトを消す
        DestroyArrow();
        // 羽フックを壊す
        Destroy(WingHock);
        // 格闘の累積時間を初期化
        Wrestletime = 0;
        // 地上にいるか？(落下開始時は一応禁止）
        if (IsGrounded)
        {
            AttackDone();
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
	}

    protected override void Animation_Jumping(Animator animator, int fallID, int[] stepanimations, int airdashID, int landinghashID)
    {
        base.Animation_Jumping(animator, fallID, stepanimations, airdashID, landinghashID);
        AttackDone();
    }

    protected override void Animation_Fall(Animator animator, int airdashID, int jumpID, int[] stepanimations, int landingID)
    {
        base.Animation_Fall(animator, airdashID, jumpID, stepanimations, landingID);
        AttackDone();
    }

    protected override void Animation_Run(Animator animator, int fallhashID, int[] stepanimations, int idleID, int jumpID)
    {
        base.Animation_Run(animator, fallhashID, stepanimations, idleID, jumpID);
        AttackDone(true, false);
    }

    protected override void Animation_AirDash(Animator animator, int jumpID, int fallID, int landingID)
    {
        base.Animation_AirDash(animator, jumpID, fallID, landingID);
        AttackDone(false, true);
    }

    protected override void Shot()
    {       
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
				var leftArrow = (GameObject)Instantiate(SubShotArrow, posLArrow, rotLArrow);
				if(leftArrow.transform.parent == null)
				{
					leftArrow.transform.parent = SubShotRootL.transform;
					leftArrow.transform.GetComponent<Rigidbody>().isKinematic = true;
					leftArrow.name = "SubShotLeft";
				}
				// 右の矢を作る
				var rightArrow = (GameObject)Instantiate(SubShotArrow, posRArrow, rotRArrow);
				if(rightArrow.transform.parent == null)
				{
					rightArrow.transform.parent = SubShotRootR.transform;
					rightArrow.transform.GetComponent<Rigidbody>().isKinematic = true;
					rightArrow.name = "SubShotRight";
				}
			}
			// 特殊射撃
			else if (type == ShotType.EX_SHOT)
			{
				// TODO:特殊射撃の矢を作る処理
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
					// 矢の親子関係をつけておく
					//obj.transform.GetComponent<Rigidbody>().isKinematic = true;
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
		// サブ射撃左右の矢を取得
		var subshotArrowLeft = SubShotRootL.GetComponentInChildren<HomuraBowSubShot>();
		var subshotArrowRight = SubShotRootR.GetComponentInChildren<HomuraBowSubShot>();

		if (arrow != null || subshotArrowCenter != null)
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
				if(subshotArrowLeft != null)
				{
					subshotArrowLeft.ShotSpeed = Character_Spec.cs[(int)CharacterName][2].m_Movespeed;
				}
				if(subshotArrowRight != null)
				{
					subshotArrowRight.ShotSpeed = Character_Spec.cs[(int)CharacterName][2].m_Movespeed;
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
				if(subshotArrowLeft != null)
				{
					// 角度
					Vector3 normalizeRotLOR = mainrot.eulerAngles;
					// 角度再調整
					Vector3 normalizeRotL = new Vector3(normalizeRotLOR.x - 20, normalizeRotLOR.y, normalizeRotLOR.z);
					subshotArrowLeft.MoveDirection = Vector3.Normalize(normalizeRotL);
				}
				// サブ射撃の矢右
				if(subshotArrowRight != null)
				{
					// 角度
					Vector3 normalizeRotROR = mainrot.eulerAngles;
					// 角度再調整
					Vector3 normalizeRotR = new Vector3(normalizeRotROR.x + 20, normalizeRotROR.y, normalizeRotROR.z);
					subshotArrowRight.MoveDirection = Vector3.Normalize(normalizeRotR);
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
						Vector3 normalizeRotL = new Vector3(rotateOR.x - 20, rotateOR.y, rotateOR.z);
						subshotArrowLeft.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotL) * Vector3.forward);
					}
					// サブ射撃の矢右
					if(subshotArrowRight != null)
					{
						// 角度再調整
						Vector3 normalizeRotR = new Vector3(rotateOR.x + 20, rotateOR.y, rotateOR.z);
						subshotArrowRight.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotR) * Vector3.forward);
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
						Vector3 normalizeRotL = new Vector3(mainrotOR.x - 20, mainrotOR.y, mainrotOR.z);
						subshotArrowLeft.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotL) * Vector3.forward);
					}
					// サブ射撃の矢右
					if(subshotArrowRight != null)
					{
						// 本体角度算出
						Vector3 mainrotOR = transform.rotation.eulerAngles;
						// 角度再調整
						Vector3 normalizeRotR = new Vector3(mainrotOR.x + 20, mainrotOR.y, mainrotOR.z);
						subshotArrowRight.MoveDirection = Vector3.Normalize(Quaternion.Euler(normalizeRotR) * Vector3.forward);
					}
				}
			}
			// 矢のフックの位置に弾の位置を代入する
			// メイン射撃位置・サブ射撃中央位置
			BulletPos = MainShotRoot.transform.position;
			
			// 同じく回転角を代入する
			if (arrow != null)
			{
				BulletMoveDirection = arrow.MoveDirection;
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
				SetOffensivePower(SkillType_Homura_B.EX_SHOT);
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
        ReturnMotion(AnimatorUnit);
	}

	// 射撃攻撃の攻撃力とダウン値を決定する
	// kind[in]     :射撃の種類
	private void SetOffensivePower(SkillType_Homura_B kind)
	{
		// 攻撃力を決定する(ここの2がスキルのインデックス。下も同様）
		this.OffensivePowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)kind].m_OriginalStr + Character_Spec.cs[(int)CharacterName][(int)kind].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ダウン値を決定する
		this.DownratioPowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)kind].m_DownPoint;
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


}
