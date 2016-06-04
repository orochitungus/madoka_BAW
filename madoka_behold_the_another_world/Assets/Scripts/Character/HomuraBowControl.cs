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

    public Animator AnimatorUnit;

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
    /// 各種アニメのハッシュID
    /// </summary>
    public int IdleID;
    public int WalkID;
    public int JumpID;
    public int JumpingID;
    public int FallID;
    public int LandingID;
    public int RunID;
    public int AirDashID;
    public int FrontStepID;
    public int LeftStepID;
    public int RightStepID;
    public int BackStepID;
    public int FrontStepBackID;
    public int LeftStepBackID;
    public int RightStepBackID;
    public int BackStepBackID;
    public int ShotID;
    public int RunShotID;
    public int AirShotID;
    public int ChargeShotID;
    public int SubShotID;
    public int EXShotID;
    public int FollowThrowShotID;
    public int FollowThrowRunShotID;
    public int FollowThrowAirShotID;
    public int FollowThrowChargeShotID;
    public int FollowThrowSubShotID;
    public int FollowThrowEXShotID;
    public int Wrestle1ID;
    public int Wrestle2ID;
    public int Wrestle3ID;
    public int FrontWrestleID;
    public int LeftWrestleID;
    public int RightWrestleID;
    public int BackWrestleID;
    public int AirDashWrestleID;
    public int EXWrestleID;
    public int EXFrontWrestleID;
    public int EXBackWrestleID;
    public int ReversalID;
    public int ArousalAttackID;
    public int DamageID;
    public int DownID;
    public int BlowID;
    public int SpinDownID;

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
        IdleID = Animator.StringToHash("HomuraBowIdle");
        WalkID = Animator.StringToHash("HomuraBowWalk");
        JumpID = Animator.StringToHash("HomuraBowJump");
        JumpingID = Animator.StringToHash("HomuraBowJumping");
        FallID = Animator.StringToHash("HomuraBowFall");
        LandingID = Animator.StringToHash("HomuraBowRun");
        RunID = Animator.StringToHash("HomuraBowRun");
        AirDashID = Animator.StringToHash("HomuraBowAirDash");
        FrontStepID = Animator.StringToHash("HomuraBowFrontStep");
        FrontStepBackID = Animator.StringToHash("HomurabowFrontStepBack");
        LeftStepID = Animator.StringToHash("HomuraBowLeftStep");
        LeftStepBackID = Animator.StringToHash("HomuraBowLeftStepBack");
        RightStepID = Animator.StringToHash("HomuraBowRightStep");
        RightStepBackID = Animator.StringToHash("HomuraBowRightStepBack");
        BackStepID = Animator.StringToHash("HomuraBowBackStep");
        BackStepBackID = Animator.StringToHash("HomuraBowBackStepBack");
        ShotID = Animator.StringToHash("HomuraBowShot");
        RunShotID = Animator.StringToHash("HomuraBowRunShot");
        AirShotID = Animator.StringToHash("HomuraBowAirShot");
        ChargeShotID = Animator.StringToHash("HomuraBowChargeShot");
        SubShotID = Animator.StringToHash("HomuraBowSubShot");
        EXShotID = Animator.StringToHash("HomuraBowEXShot");
        FollowThrowShotID = Animator.StringToHash("HomuraBowShotForrowThrow");
        FollowThrowRunShotID = Animator.StringToHash("HomuraBowRunShotForrowThrow");
        FollowThrowAirShotID = Animator.StringToHash("HomuraBowAirShotForrowThrow");
        FollowThrowChargeShotID = Animator.StringToHash("HowmuraBowChargeShotFollowThrow");
        FollowThrowSubShotID = Animator.StringToHash("HomuraBowSubShotFollowThrow");
        FollowThrowEXShotID = Animator.StringToHash("HomuraBowEXShotFollowThrow");
        Wrestle1ID = Animator.StringToHash("HomuraBowWrestle1");
        Wrestle2ID = Animator.StringToHash("HomuraBowWrestle2");
        Wrestle3ID = Animator.StringToHash("HomuraBowWrestle3");
        FrontWrestleID = Animator.StringToHash("HomuraBowFrontWrestle");
        LeftWrestleID = Animator.StringToHash("HomuraBowLeftWrestle");
        RightWrestleID = Animator.StringToHash("HomuraBowRightWrestle");
        BackWrestleID = Animator.StringToHash("HomuraBowBackWrestle");
        AirDashWrestleID = Animator.StringToHash("HomuraBowBDWrestle");
        EXWrestleID = Animator.StringToHash("HomuraBowEXWrestle");
        EXFrontWrestleID = Animator.StringToHash("HomuraBowFrontEXWrestle");
        EXBackWrestleID = Animator.StringToHash("HomuraBowBackExWrestle");
        ReversalID = Animator.StringToHash("HomuraBowReversal");
        ArousalAttackID = Animator.StringToHash("HomuraBowArousalAttack");
        DamageID = Animator.StringToHash("HomuraBowDamage");
        DownID = Animator.StringToHash("HomurBowDown");
        BlowID = Animator.StringToHash("HomuraBowBlow");
        SpinDownID = Animator.StringToHash("HomuraBowSpindown");
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
        AirDashSpeed = 20.0f;                        // 移動速度（空中ダッシュの場合）
        AirMoveSpeed = 7.0f;                         // 移動速度（空中慣性移動の場合）
        RiseSpeed = 5.0f;                            // 上昇速度

        // ブースト消費量
        JumpUseBoost = 10;       // ジャンプ時
        DashCancelUseBoost = 10;   // ブーストダッシュ時
        StepUseBoost = 10;         // ステップ時
        BoostLess = 0.5f;        // ジャンプの上昇・BD時の1F当たりの消費量

        // ステップ移動距離
        StepMoveLength = 10.0f;

        // ステップ初速（X/Z軸）
        StepInitialVelocity = 30.0f;
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
        FirstSetting(AnimatorUnit, IdleID);

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
		if(Update_Core(isspindown, AnimatorUnit, DownID, AirDashID,AirShotID,JumpingID,FallID,IdleID,BlowID,RunID))
		{
			
		}
	}

	void UpdateAnimation()
	{
		// 通常
		if (AnimatorUnit.GetHashCode() == IdleID)
		{
			int[] stepanimations = { FrontStepID, LeftStepID, RightStepID, BackStepID };
			Animation_Idle(AnimatorUnit, DownID, RunID,stepanimations,FallID);
		}
		else if (AnimatorUnit.GetHashCode() == WalkID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == JumpID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == JumpingID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FallID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == LandingID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == RunID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == AirDashID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FrontStepID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == LeftStepID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == RightStepID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == BackStepID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FrontStepBackID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == LeftStepBackID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == RightStepBackID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == ShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == RunShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == AirShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == ChargeShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == SubShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == EXShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowRunShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowAirShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowChargeShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowSubShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FollowThrowEXShotID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == Wrestle1ID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == Wrestle2ID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == Wrestle3ID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == FrontWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == LeftWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == RightWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == BackWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == AirDashWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == EXWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == EXFrontWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == EXBackWrestleID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == ReversalID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == ArousalAttackID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == DamageID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == DownID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == BlowID)
		{

		}
		else if (AnimatorUnit.GetHashCode() == SpinDownID)
		{

		}
	}
}
