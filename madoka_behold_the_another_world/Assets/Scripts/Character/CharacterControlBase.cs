using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

// アクションステージで使うキャラの基底クラス
public class CharacterControlBase : MonoBehaviour
{
    /// <summary>
    /// 本体を追従するカメラ
    /// </summary>
    public Camera MainCamera;

    /// <summary>
    /// 覚醒演出用カメラ
    /// </summary>
    public Camera ArousalCamera;

	/// <summary>
	/// CPU時の入力受け取りクラス
	/// </summary>
	public CPUController Cpucontroller;

    /// <summary>
    /// 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    /// </summary>
    public Vector3 LayOriginOffs;

    /// <summary>
    /// レイの長さ
    /// </summary>
    public float Laylength;

    /// <summary>
    /// ground属性をもったレイヤー（この場合8）
    /// </summary>
    protected int LayMask;

    /// <summary>
    /// このオブジェクトのRigidbody
    /// </summary>
    protected Rigidbody RigidBody;

    /// <summary>
    /// 哨戒モードにおける起点(僚機の時はPC）
    /// </summary>
    public GameObject StartingPoint;
	public GameObject EndingPoint;

    /// <summary>
    /// 歩き撃ちフラグ
    /// </summary>
    protected bool RunShotDone;

	/// <summary>
	/// 覚醒技演出中フラグ
	/// </summary>
	public bool ArousalAttackProduction;

	/// <summary>
	/// ロックオンカーソルの表示中心
	/// </summary>
	public GameObject RockonCursorPosition;

    /// <summary>
    /// プレイヤーであるか否か（これがfalseであると描画系の関数全カット）
    /// それぞれの意味は以下の通り
    /// </summary>
    public enum CHARACTERCODE
	{
		PLAYER,         // プレイヤー操作
		PLAYER_ALLY,    // プレイヤー僚機
		ENEMY           // 敵
	};
	public CHARACTERCODE IsPlayer;

    /// <summary>
    /// 弾の種類(CharacterSpecの配置順に合わせること）
    /// </summary>
    public enum ShotType
	{
		NORMAL_SHOT,    // 通常射撃
		CHARGE_SHOT,    // チャージ射撃
		SUB_SHOT,       // サブ射撃
		EX_SHOT,        // 特殊射撃        
	};

    /// <summary>
    /// 射撃時の射出モード
    /// </summary>
    public enum ShotMode
	{
		NORMAL,     // 通常状態
		RELORD,     // 構え
		SHOT,       // 射出
		SHOTDONE,   // 射出完了
		AIRDASHSHOT // 射出（空中ダッシュ射撃）
	};
	public ShotMode Shotmode;

    /// <summary>
    /// 誰であるか
    /// </summary>
    public Character_Spec.CHARACTER_NAME CharacterName;

    /// <summary>
    /// 頭部オブジェクト
    /// </summary>
    public GameObject HeadObject;

    /// <summary>
    /// 胸部オブジェクト
    /// </summary>
    public GameObject BrestObject;

    /// <summary>
    /// 覚醒時エフェクト
    /// </summary>
    public GameObject ArousalEffect;

	/// <summary>
	/// 空中ダッシュエフェクト
	/// </summary>
	public GameObject AirDashEffect;

	/// <summary>
	/// 空中ダッシュエフェクト表示フラグ
	/// </summary>
	public bool ShowAirDashEffect;

    /// <summary>
    /// ポーズコントローラー
    /// </summary>
    private GameObject Pausecontroller;

    // CPU
    /// <summary>
    /// ルーチンを拾う
    /// </summary>
    public AIControlBase.CPUMODE CPUMode;

    /// <summary>
    /// テンキー入力を拾う
    /// </summary>
    public AIControlBase.TENKEY_OUTPUT CPUTenkey;

    /// <summary>
    /// キー入力を拾う
    /// </summary>
    public AIControlBase.KEY_OUTPUT CPUKey;


    // 時間停止時の処理    
    // 時間停止系のENUM
    public enum TimeStopMode
    {
        NORMAL,                 // 通常状態(F単位で加減算系の数値は通常)
        PAUSE,                  // ポーズ（全停止）
        TIME_STOP,              // ほむらの時間停止発動中（master以外全停止（弾丸含む））
        TIME_DELAY,             // キリカの時間遅延発動中（実際に使うか不明）、全速度を1/4にする。こちらも弾丸を含む
        AROUSAL                 // 誰かの覚醒による時間停止
    };
    // 時間停止系のenumの値
    public TimeStopMode Timestopmode;
    // 時間停止・遅延のmasterであるか否か（これがtrueだと時間停止の影響を受けない.発動者にのみtureにする。）
    public bool TimeStopMaster;

    /// <summary>
    /// ロックオン距離（この距離を超えて撃つと誘導や補正が入らない）
    /// </summary>
    public float RockonRange;

    /// <summary>
    /// ロックオン限界距離（この距離を超えるとロックオン判定に入らない）
    /// </summary>
    public float RockonRangeLimit;

    /// <summary>
    /// ジャンプ時間
    /// </summary>
    public float JumpWaitTime;
    public float JumpTime;

    /// <summary>
    /// 着地硬直
    /// </summary>
    public float LandingWaitTime;
    public float LandingTime;

    /// <summary>
    /// 攻撃行動硬直
    /// </summary>
    public float AttackTime;

    // 移動用ステート
    /// <summary>
    /// 移動方向
    /// </summary>
    public Vector3 MoveDirection;

    /// <summary>
    /// 射撃などの射出前における移動方向
    /// </summary>
    public Vector3 MoveDirection_OR;

    /// <summary>
    /// ダウンする時や吹き飛び属性の攻撃を食らった時の方向ベクトル
    /// </summary>
    public Vector3 BlowDirection;

    /// <summary>
    /// 移動速度（歩行の場合）
    /// </summary>
    public float WalkSpeed;

    /// <summary>
    /// 移動速度（走行の場合）
    /// </summary>         
    public float RunSpeed;

    /// <summary>
    /// 移動速度（空中ダッシュの場合）
    /// </summary>
    public float AirDashSpeed;

    /// <summary>
    /// 移動速度（空中慣性移動の場合）
    /// </summary>
    public float AirMoveSpeed;

    /// <summary>
    /// 上昇速度
    /// </summary>         
    public float RiseSpeed;

    /// <summary>
    /// ジャンプ時のブースト消費量
    /// </summary>
    protected float JumpUseBoost;

    /// <summary>
    /// ダッシュキャンセル時のブースト消費量
    /// </summary>
    protected float DashCancelUseBoost;

    /// <summary>
    /// ダッシュキャンセル硬直時間
    /// </summary>
    protected float DashCancelWaittime;

    /// <summary>
    /// ダッシュキャンセル累積時間
    /// </summary>
    protected float DashCancelTime;

    /// <summary>
    /// ステップ時のブースト消費量 
    /// </summary>
    protected float StepUseBoost;

    /// <summary>
    /// ダウン回避時のブースト消費量 
    /// </summary>
    protected float ReversalUseBoost;

    /// <summary>
    /// レベル
    /// </summary>
    public int Level = 1;

    /// <summary>
    /// 取得技（オートスキル含む）
    /// </summary>
    public int SkillUse = 1;

    /// <summary>
    /// 攻撃力レベル
    /// </summary>
    public int StrLevel = 1;

    /// <summary>
    /// 防御力レベル
    /// </summary>
    public int DefLevel = 1;

    /// <summary>
    /// 残弾数レベル
    /// </summary>
    public int BulLevel = 1;

    /// <summary>
    /// ブースト量レベル
    /// </summary>
    public int BoostLevel = 1;

    /// <summary>
    /// 覚醒ゲージレベル
    /// </summary>
    public int ArousalLevel = 1;

    /// <summary>
    /// ブースト量
    /// </summary>
    public float Boost;

    /// <summary>
    /// ブースト量初期値(Lv1の時の値）
    /// </summary>
    public float Boost_OR;

    /// <summary>
    /// ブースト量成長係数
    /// </summary>
    public float BoostGrowth;

    /// <summary>
    /// 覚醒ゲージ量
    /// </summary>
    public float Arousal;

    /// <summary>
    /// 覚醒ゲージ追加
    /// </summary>
    /// <param name="arousal"></param>
    public void AddArousal(float arousal)
    {
        Arousal = Arousal + arousal;
    }

    /// <summary>
    /// 覚醒ゲージ量初期値(LV1の時の値）
    /// </summary>
    public float Arousal_OR;

    /// <summary>
    /// 覚醒ゲージ量成長係数
    /// </summary>
    public float ArousalGrowth;

    /// <summary>
    /// 覚醒状態であるか否か
    /// </summary>
    public bool IsArousal;

    /// <summary>
    /// ブースト消費量（1Fあたり）
    /// </summary>
    public float BoostLess;

    /// <summary>
    /// ステップ移動距離
    /// </summary>
    public float StepMoveLength;

    /// <summary>
    /// ステップ初速（X/Z軸）
    /// </summary>
    public float StepInitialVelocity;

    /// <summary>
    /// ステップ時の１F当たりの移動量
    /// </summary>
    public float StepMove1F;

    /// <summary>
    /// ステップ累積移動距離
    /// </summary>
    public float SteppingLength;

    /// <summary>
    /// ステップ時の移動角度
    /// </summary>
    public Quaternion StepRotation;

    /// <summary>
    /// ステップ終了時の硬直時間
    /// </summary>
    public float StepBackTime;

    // 現在のダウン値
    public float NowDownRatio;

    /// <summary>
    /// 現在のHP
    /// </summary>
    public int NowHitpoint;

    /// <summary>
    /// 現在のHPの表示値
    /// </summary>
    public int DrawHitpoint;

    /// <summary>
    /// HP初期値（Lv1のときの値）
    /// </summary>
    public int NowHitpoint_OR;

    /// <summary>
    /// HP成長係数
    /// </summary>
    public int NowHitpointGrowth;

    /// <summary>
    /// ダウン値の閾値（これを超えるとダウン状態へ移行.基本全員5でOK、一部のボスをそれ以上に）
    /// </summary>
    public float DownRatioBias;

    /// <summary>
    /// ダウン時の打ち上げ量
    /// </summary>
    public float LaunchOffset;

    /// <summary>
    /// ロックオン状態か否か
    /// </summary>
    public bool IsRockon;

    /// <summary>
    /// 移動方向固定状態か否か
    /// </summary>
    protected bool Rotatehold;

    /// <summary>
    /// アーマー状態か否か
    /// </summary>
    public bool IsArmor;

    /// <summary>
    /// 地上にいるか否か
    /// </summary>
    public bool IsGrounded;

    /// <summary>
    /// ステップしているか否か
    /// </summary>
    public bool IsStep;

    /// <summary>
    /// 格闘しているか否か
    /// </summary>
    public bool IsWrestle;

    // 入力関係

    /// <summary>
    /// テンキー入力があったか否か
    /// </summary>
    protected bool HasVHInput;

    /// <summary>
    /// 射撃入力があったか否か
    /// </summary>
    protected bool HasShotInput;

    /// <summary>
    /// ジャンプ入力があったか否か
    /// </summary>
    protected bool HasJumpInput;

	/// <summary>
	/// ジャンプ長押入力があったか否か
	/// </summary>
	protected bool HasJumpingInput;

    /// <summary>
    /// ダッシュキャンセル入力があったか否か
    /// </summary>
    protected bool HasDashCancelInput;

    /// <summary>
    /// 空中ダッシュ入力があったか否か
    /// </summary>
    protected bool HasAirDashInput;

    /// <summary>
    /// サーチ入力があったか否か
    /// </summary>
    protected bool HasSearchInput;

    /// <summary>
    /// サーチキャンセル入力があったか否か
    /// </summary>
    public bool HasSearchCancelInput;

    /// <summary>
    /// 格闘入力があったか否か
    /// </summary>
    protected bool HasWrestleInput;

    /// <summary>
    /// サブ射撃入力があったか否か
    /// </summary>
    protected bool HasSubShotInput;

    /// <summary>
    /// 特殊射撃入力があったか否か
    /// </summary>
    protected bool HasExShotInput;

    /// <summary>
    /// 特殊格闘入力があったか否か
    /// </summary>
    protected bool HasExWrestleInput;

    /// <summary>
    /// アイテム入力があったか否か
    /// </summary>
    protected bool HasItemInput;

    /// <summary>
    /// メニュー入力があったか否か
    /// </summary>
    protected bool HasMenuInput;

    // 覚醒入力があったか否か
    protected bool HasArousalInput;

    /// <summary>
    /// 覚醒技入力があったか否か（コマンドは同じだが、処理順番の関係上別にしておかないとCPUは覚醒した時点で覚醒技を入力したという扱いになってしまう）
    /// </summary>
    protected bool HasArousalAttackInput;

    /// <summary>
    /// 射撃チャージ入力があったか否か
    /// </summary>
    protected bool HasShotChargeInput;

    /// <summary>
    /// 格闘チャージ入力があったか否か
    /// </summary>
    protected bool HasWrestleChargeInput;

    /// <summary>
    /// 前入力があったか否か
    /// </summary>
    protected bool HasFrontInput;

    /// <summary>
    /// 左入力があったか否か
    /// </summary>
    protected bool HasLeftInput;

    /// <summary>
    /// 右入力があったか否か
    /// </summary>
    protected bool HasRightInput;

    /// <summary>
    /// 後入力があったか否か
    /// </summary>
    protected bool HasBackInput;

    /// <summary>
    /// 左ステップ入力があったか否か
    /// </summary>
    protected bool HasLeftStepInput;

    /// <summary>
    /// 右ステップ入力があったか否か
    /// </summary>
    protected bool HasRightStepInput;

    // ステップ入力強制解除
    public void StepStop()
    {
        HasLeftStepInput = false;
        HasRightStepInput = false;
    }

    /// <summary>
    /// 使用可能な武装（弾切れもしくはリロード中は使用不可にする.順番はCharacter_Specのインデックスの射撃武装）
    /// </summary>
    public bool[] WeaponUseAble = new bool[20];

    /// <summary>
    /// 弾数を消費するタイプの武装の場合、残弾数
    /// </summary>
    public int[] BulletNum = new int[20];

    /// <summary>
    /// 射撃攻撃の硬直時間
    /// </summary>
    public float[] BulletWaitTime = new float[20];

    /// <summary>
    /// 追撃可能時間(ダメージ硬直時間）
    /// </summary>
    public float DamagedWaitTime;

	/// <summary>
	/// 追撃可能累積時間
	/// </summary>
	public float DamagedTime;

	/// <summary>
	/// ダウン値復帰時間
	/// </summary>
	public float DownRebirthWaitTime;
	
	/// <summary>
	/// ダウン値累積時間
	/// </summary>
	public float DownRebirthTime;

	/// <summary>
	/// ダウン時間（ダウンしている時間）
	/// </summary>
	public float DownWaitTime;

	/// <summary>
	/// ダウン累積時間
	/// </summary>
	public float DownTime;

	/// <summary>
	/// 累積押し時間（射撃）をカウント
	/// </summary>
	protected int ShotCharge;
    public int GetShotCharge()
    {
        return ShotCharge;
    }

	/// <summary>
	/// 累積押し時間（格闘）をカウント
	/// </summary>
	protected int WrestleCharge;
    public int GetWrestleCharge()
    {
        return WrestleCharge;
    }

	/// <summary>
	/// 1Fあたりの射撃チャージゲージ増加量
	/// </summary>
	protected int ShotIncrease;

	/// <summary>
	/// 1Fあたりの格闘チャージゲージ増加量
	/// </summary>
	protected int WrestleIncrease;

	/// <summary>
	/// 1Fあたりの射撃チャージゲージ減衰量
	/// </summary>
	protected int ShotDecrease;

	/// <summary>
	/// 1Fあたりの格闘チャージゲージ減衰量
	/// </summary>
	protected int WrestleDecrease;

	/// <summary>
	/// チャージ最大値（固定。ここを超えたら発射）
	/// </summary>
	public int ChargeMax;


	/// <summary>
	/// モード状態（モードチェンジがないキャラは常時Normalのまま）
	/// </summary>
	public enum ModeState
	{
		NORMAL,
		ANOTHER_MODE
	}
	/// <summary>
	/// 現在のモード
	/// </summary>
	public ModeState Nowmode;

	/// <summary>
	/// コライダの地面からの高さ
	/// </summary>
	public float ColliderHeight;

	/// <summary>
	///  射出する弾の方向ベクトル(スプレッドのときはこれを基準にしてずらす）
	/// </summary>
	public Vector3 BulletMoveDirection;

	
	/// <summary>
	/// 射出する弾の攻撃力
	/// </summary>
	public int OffensivePowerOfBullet;

	/// <summary>
	/// 射出する弾のダウン値
	/// </summary>
	public float DownratioPowerOfBullet;

	/// <summary>
	/// 射出する弾の覚醒ゲージ増加量
	/// </summary>
	public float ArousalRatioOfBullet;

    /// <summary>
    /// 通常射撃用弾丸の配置用フック(他の専用武器は派生先で用意）
    /// </summary>
    public GameObject MainShotRoot;

	
    
    /// <summary>
    /// リロードクラス
    /// </summary>
    protected Reload ReloadSystem;

  

	/// <summary>
    /// 使用する足音
    /// </summary>
	private StageSetting.FootType _Foottype;
    

    // 格闘関係

    /// <summary>
    /// 格闘時の移動速度
    /// </summary>
    protected float WrestlSpeed;        // N格闘1段目

	/// <summary>
	/// 追加入力の有無を保持。trueであり
	/// </summary>
	protected bool AddInput;

	/// <summary>
	/// 格闘攻撃の種類
	/// </summary>
	public enum WrestleType
    {
        WRESTLE_1,              // 0 N格1段目
        WRESTLE_2,              // 1 N格2段目
        WRESTLE_3,              // 2 N格3段目
        CHARGE_WRESTLE,         // 3 格闘チャージ
        FRONT_WRESTLE_1,        // 4 前格闘1段目
        FRONT_WRESTLE_2,        // 5 前格闘2段目
        FRONT_WRESTLE_3,        // 6 前格闘3段目
        LEFT_WRESTLE_1,         // 7 左横格闘1段目
        LEFT_WRESTLE_2,         // 8 左横格闘2段目
        LEFT_WRESTLE_3,         // 9 左横格闘3段目
        RIGHT_WRESTLE_1,        // 10 右横格闘1段目
        RIGHT_WRESTLE_2,        // 11 右横格闘2段目
        RIGHT_WRESTLE_3,        // 12 右横格闘3段目
        BACK_WRESTLE,           // 13 後格闘（防御）
        AIRDASH_WRESTLE,        // 14 空中ダッシュ格闘
        EX_WRESTLE_1,           // 15 特殊格闘1段目
        EX_WRESTLE_2,           // 16 特殊格闘2段目
        EX_WRESTLE_3,           // 17 特殊格闘3段目
        EX_FRONT_WRESTLE_1,     // 18 前特殊格闘1段目
        EX_FRONT_WRESTLE_2,     // 19 前特殊格闘2段目
        EX_FRONT_WRESTLE_3,     // 20 前特殊格闘3段目
        EX_LEFT_WRESTLE_1,      // 21 左横特殊格闘1段目
        EX_LEFT_WRESTLE_2,      // 22 左横特殊格闘2段目
        EX_LEFT_WRESTLE_3,      // 23 左横特殊格闘3段目
        EX_RIGHT_WRESTLE_1,     // 24 右横特殊格闘1段目
        EX_RIGHT_WRESTLE_2,     // 25 右横特殊格闘2段目
        EX_RIGHT_WRESTLE_3,     // 26 右横特殊格闘3段目
        BACK_EX_WRESTLE,        // 27 後特殊格闘
        NONE,                   // なし
        // キャラごとの特殊な処理はこの後に追加、さやかのスクワルタトーレのような連続で切りつける技など

        WRESTLE_TOTAL
    };


	/// <summary>
	/// N格1段目用判定の配置用フック(キャラごとに設定する。順番は上の列挙体と同じ）
	/// </summary>
	public GameObject[] WrestleRoot = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

	/// <summary>
	/// 格闘判定のオブジェクト
	/// </summary>
	public GameObject[] WrestleObject = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

	// ダメージ関連

	/// <summary>
	/// 死亡エフェクトの存在
	/// </summary>
	private bool Explode;

	/// <summary>
	/// この値×防御力分ダメージが減衰する
	/// </summary>
	private static int DamageLess = 2;

	/// <summary>
	/// 無敵であるか否か（リバーサル等で使う）
	/// </summary>
	public bool Invincible;

    public Animator AnimatorUnit;

	/// <summary>
	/// 空中ダッシュのＩＤ
	/// </summary>
	protected int CancelDashID;

	/// <summary>
	/// ダウン時のAnimatorHash
	/// </summary>
	public int DownHash;

	/// <summary>
	/// リバーサル時のAnimatorHash
	/// </summary>
	public int ReversalHash;

	// ステップ関係
	/// <summary>
	/// 回転用の角度を保存する
	/// </summary>
	private float StepDegree;

	/// <summary>
	/// ステップによる移動先
	/// </summary>
	//private Vector3 StepTarget;

	/// <summary>
	/// ステップ中にフォーカスする座標
	/// </summary>
	private Vector3 StepFocus;

	/// <summary>
	/// ステップの開始位置
	/// </summary>
	private Vector3 StepStartPos;

	/// <summary>
	/// ガード状態であるか否か（各キャラのBackWrestleでこれをONにし、IdleかDamageで折る）
	/// </summary>
	public bool IsGuard;


    /// <summary>
    /// プレイヤーのレベル設定を行う
    /// </summary>
    protected void SettingPleyerLevel()
    {
        // 自機もしくは自機の僚機
        if (IsPlayer != CHARACTERCODE.ENEMY)
        {
            // 0を拾った場合は1に
            // 個別ステートを初期化（インスペクタでもできるけど一応こっちでやっておこう）
            // 後、ブースト量などはここで設定しておかないと下で初期化できない
            // レベル
            Level = savingparameter.GetNowLevel((int)CharacterName);
            // 攻撃力レベル
            StrLevel = savingparameter.GetStrLevel((int)CharacterName);
            // 防御力レベル
            DefLevel = savingparameter.GetDefLevel((int)CharacterName);
            // 残弾数レベル
            BulLevel = savingparameter.GetBulLevel((int)CharacterName);
            // ブースト量レベル
            BoostLevel = savingparameter.GetBoostLevel((int)CharacterName);
            // 覚醒ゲージレベル
            ArousalLevel = savingparameter.GetArousalLevel((int)CharacterName);

        }
        // 敵の場合はインスペクターで設定

        // 1を割っていた場合は1に
        if (Level < 1)
        {
            Level = 1;
        }
        if (StrLevel < 1)
        {
            StrLevel = 1;
        }
        if (DefLevel < 1)
        {
            DefLevel = 1;
        }
        if (BulLevel < 1)
        {
            BulLevel = 1;
        }
        if (BoostLevel < 1)
        {
            BoostLevel = 1;
        }
        if (ArousalLevel < 1)
        {
            ArousalLevel = 1;
        }

        // HP初期値
        NowHitpoint_OR = Character_Spec.HP_OR[(int)CharacterName];
        // HP成長係数
        NowHitpointGrowth = Character_Spec.HP_Grouth[(int)CharacterName];


        // ブースト量初期値(Lv1の時の値）
        Boost_OR = Character_Spec.Boost_OR[(int)CharacterName];
        // ブースト量成長係数
        BoostGrowth = Character_Spec.Boost_Growth[(int)CharacterName];

        // 覚醒ゲージ量初期値(LV1の時の値）
        Arousal_OR = Character_Spec.Arousal_OR[(int)CharacterName];
        // 覚醒ゲージ量成長係数
        ArousalGrowth = Character_Spec.Arousal_Growth[(int)CharacterName];

        // 使用可能武器をセット（初期段階で使用不能にしておきたいものは、各キャラのStartでこの関数を呼んだ後に再定義）
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
        // ダッシュキャンセル硬直時間
        DashCancelTime = 0.2f;
        // プレイヤーでない限りカメラを切っておく
        var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
        if (IsPlayer != CHARACTERCODE.PLAYER)
        {
            target.GetComponent<Camera>().enabled = false;
        }
        else
        {
            target.GetComponent<Camera>().enabled = true;
        }

    }

    /// <summary>
    /// 接地判定関連
    /// </summary>
    protected int Hitcounter;
    bool Hitcounterdone;
    const int HitcounterBias = 20;

	/// <summary>
	/// 撃破目標であるか否か
	/// </summary>
	public bool IsTarget;

    // 接地判定を行う。足元に5本(中心と前後左右)レイを落とし、そのいずれかが接触していれば接地。全部外れていれば落下
    protected bool onGround()
    {
        // 座標
        Vector3[] layStartPos = new Vector3[5];
        // カプセルコライダ
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        // 中心
        layStartPos[0] = transform.position + LayOriginOffs;
        // 左
        layStartPos[1] = new Vector3(layStartPos[0].x + collider.radius, layStartPos[0].y, layStartPos[0].z);
        // 右
        layStartPos[2] = new Vector3(layStartPos[0].x - collider.radius, layStartPos[0].y, layStartPos[0].z);
        // 前
        layStartPos[3] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z + collider.radius);
        // 後
        layStartPos[4] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z - collider.radius);
        int hitcount = 0;
        for (int i = 0; i < layStartPos.Length; i++)
        {
            if (Physics.Raycast(layStartPos[i], -Vector3.up, Laylength + 1.5f, LayMask))
            {
                hitcount++;
            }          
        }
        // 1つでもヒットしたら接地とする
        if (hitcount > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 接地判定を行う。足元に5本(中心と前後左右)レイを落とし、そのいずれかが接触していれば接地。全部外れていれば落下
    /// </summary>
    /// <param name="isSpindown">錐揉みダウンのときは判定が変わるのでそのためのフラグ</param>
    /// <returns></returns>
    protected bool onGround2(bool isSpindown)
    {
        // 座標
        Vector3[] layStartPos = new Vector3[5];
        // カプセルコライダ
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        // 中心
        layStartPos[0] = transform.position + LayOriginOffs;
        // 左
        layStartPos[1] = new Vector3(layStartPos[0].x + collider.radius, layStartPos[0].y, layStartPos[0].z);
        // 右
        layStartPos[2] = new Vector3(layStartPos[0].x - collider.radius, layStartPos[0].y, layStartPos[0].z);
        // 前
        layStartPos[3] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z + collider.radius);
        // 後
        layStartPos[4] = new Vector3(layStartPos[0].x, layStartPos[0].y, layStartPos[0].z - collider.radius);
        int hitcount = 0;
        for (int i = 0; i < layStartPos.Length; i++)
        {
            //Debug.DrawLine(layStartPos[i], new Vector3(layStartPos[i].x, layStartPos[i].y - 0.5f, layStartPos[i].z), Color.red); 
            // 錐揉みダウン時
            if (isSpindown)
            {
                if (Physics.Raycast(layStartPos[i], -Vector3.up, this.Laylength - collider.radius, this.LayMask))
                {
                    hitcount++;
                }
            }
            // 通常時
            else
            {
                if (Physics.Raycast(layStartPos[i], -Vector3.up, this.Laylength + 1.5f, this.LayMask))
                {
                    hitcount++;
                }
            }
        }
        // 1つでもヒットしたら接地とする.但しブーストダッシュの離陸時は例外
        if (hitcount > 0 && !ControllerManager.Instance.BoostDash)
        {                            
            return true;
        }

        return false;
    }

    /// <summary>
    /// 移動キーが押されているかどうかをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetVHInput()
    {
		// PC時のみ。CPU時は何か別の関数を用意
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Top || ControllerManager.Instance.Left || ControllerManager.Instance.Right || ControllerManager.Instance.Under || ControllerManager.Instance.LeftUpper
			|| ControllerManager.Instance.LeftUnder || ControllerManager.Instance.RightUnder || ControllerManager.Instance.RightUpper)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		// CPU時
		else
		{
			if (Cpucontroller.Top || Cpucontroller.Left || Cpucontroller.Right || Cpucontroller.Under || Cpucontroller.LeftUpper || Cpucontroller.LeftUnder || Cpucontroller.RightUnder || Cpucontroller.RightUpper)
			{
				return true;
			}
		}
        return false;
        
    }

    /// <summary>
    /// 方向キー入力（上）が押されているかをチェックする（横が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetFrontInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Top)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Top)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// 方向キー入力（下）が押されているかをチェックする（横が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetBackInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Under)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Under)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// 方向キー入力（左）が押されているかをチェックする（縦が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetLeftInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Left)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Left)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// 方向キー入力（右）が押されているかをチェックする（縦が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetRightInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Right)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Right)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// ショット入力があったか否かをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetShotInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Shot)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Shot)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// 射撃入力が押しっぱなしであるか離されたかをチェックする 
    /// </summary>
    /// <returns>チャージ完了状態で離されたか否か</returns>
    protected bool GetShotChargeInput()
    {
        bool compleate = false;
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 入力中はShotChargeを増加
            if (ControllerManager.Instance.Shotting)
            {
                if (ShotCharge < 0)
                {
                    ShotCharge = 0;
                }
                ShotCharge += ShotIncrease;
            }
            // 解除するとShotChargeを減衰
            else
            {
                if (ShotCharge > 0)
                {
                    ShotCharge -= ShotDecrease;
                }
            }
        }
		else
		{
			if(Cpucontroller.Shotting)
			{
				if (ShotCharge < 0)
				{
					ShotCharge = 0;
				}
				ShotCharge += ShotIncrease;
			}
			else
			{
				if (ShotCharge > 0)
				{
					ShotCharge -= ShotDecrease;
				}
			}
		}
		
        // MAX状態で離されるとチャージ量を0にしてtrue
        if (ShotCharge >= ChargeMax && (!ControllerManager.Instance.Shotting))
        {
            ShotCharge = 0;
            compleate = true;
        }
				
        return compleate;
    }
    
    /// <summary>
    /// 格闘入力が押しっぱなしであるか離されたかをチェックする
    /// </summary>
    /// <returns>チャージ完了状態で離されたか否か</returns>
    protected bool GetWrestleChargeInput()
    {
        bool compleate = false;
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 入力中はm_ShotChargeを増加
            if (ControllerManager.Instance.Wrestling)
            {
                if (WrestleCharge < 0)
                {
                    WrestleCharge = 0;
                }
                WrestleCharge += WrestleIncrease;
            }
            // 解除するとm_ShotChargeを減衰
            else
            {
                if (WrestleCharge > 0)
                {
                    WrestleCharge -= WrestleDecrease;
                }
            }
        }
		else
		{
			if(Cpucontroller.Wrestling)
			{
				if (WrestleCharge < 0)
				{
					WrestleCharge = 0;
				}
				WrestleCharge += WrestleIncrease;
			}
			else
			{
				if (WrestleCharge > 0)
				{
					WrestleCharge -= WrestleDecrease;
				}
			}
		}
        // MAX状態で離されるとチャージ量を0にしてtrue
        if (WrestleCharge >= ChargeMax && (!ControllerManager.Instance.Wrestle))
        {
            WrestleCharge = 0;
            compleate = true;
        }
        return compleate;
    }

    /// <summary>
    /// ジャンプ入力があったか否かをチェックする 
    /// </summary>
    /// <returns></returns>
    protected bool GetJumpInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Jump)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Jump)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// ジャンプ長押し入力があったか否かをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetJumpingInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Jumping)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.Jumping || Cpucontroller.BoostDash)
			{
				return true;
			}
		}
        return false;
    }

    /// <summary>
    /// ダッシュキャンセル入力があったか否かをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetDashCancelInput()
    {
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.BoostDash)
			{
				return true;
			}
		}
		else
		{
			if (Cpucontroller.BoostDash)
			{
				return true;
			}
		}
        return false;
    }

	/// <summary>
	/// 空中ダッシュ入力があったか否かをチェックする（CPU専用）
	/// </summary>
	/// <returns></returns>
	protected bool GetAirDashInput()
	{
		if(IsPlayer == CHARACTERCODE.PLAYER)
		{
			
		}
		else
		{
			if(Cpucontroller.BoostDash)
			{
				return true;
			}
		}
		return false;
	}

    // サーチ入力があったか否かをチェックする
    // カメラ側で取得が要るのでここはpublicに
    public bool GetSearchInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (ControllerManager.Instance.Search && !ControllerManager.Instance.Unlock)
            {
				Debug.Log("SearchInput Done");
				ControllerManager.Instance.Search = false;
                return true;
            }
        }
		else
		{
			if(Cpucontroller == null)
			{
				return false;
			}
			if(Cpucontroller.Search && !Cpucontroller.Unlock)
			{
				return true;
			}
		}
        return false;
    }
		

	// サーチ長押し（ロックオン解除）があったか否かをチェック
	public bool GetUnSerchInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if(ControllerManager.Instance.Unlock)
			{
				return true;
			}
		}
		else
		{
			if(Cpucontroller.Unlock)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 格闘入力があったか否か
	/// </summary>
	/// <returns></returns>
	protected bool GetWrestleInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Wrestle)
			{
				return true;
			}
		}
		else
		{
			if(Cpucontroller.Wrestle)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// サブ射撃入力があったか否か(判定の都合上優先度はノーマル射撃及び格闘より上にすること)
	/// </summary>
	/// <returns></returns>
	protected bool GetSubShotInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			// 同時押し（射撃＋格闘）
			if (ControllerManager.Instance.Wrestle && ControllerManager.Instance.Shot)
			{
				return true;
			}
			// サブ射撃ボタン
			else if (ControllerManager.Instance.SubShot)
			{
				return true;
			}
		}
		// CPU側は直接指定で
		else
		{
			if(Cpucontroller.SubShot)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 特殊射撃入力があったか否か(判定の都合上優先度はノーマル射撃及びジャンプより上にすること)
	/// </summary>
	/// <returns></returns>
	protected bool GetExShotInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			// 同時押し（射撃＋ジャンプ）
			if (ControllerManager.Instance.Shot && ControllerManager.Instance.Jump)
			{
				return true;
			}
			// 特殊射撃ボタン
			else if (ControllerManager.Instance.EXShot)
			{
				return true;
			}
		}
		// CPU側
		else 
		{
			if(Cpucontroller.EXShot)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 特殊格闘入力があったか否か(判定の都合上優先度はノーマル射撃及びジャンプより上にすること)
	/// </summary>
	/// <returns></returns>
	protected bool GetExWrestleInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			// 同時押し（格闘＋ジャンプ）
			if (ControllerManager.Instance.Wrestle && ControllerManager.Instance.Jump)
			{
				return true;
			}
			// 特殊格闘ボタン
			else if (ControllerManager.Instance.EXWrestle)
			{
				return true;
			}
		}
		else
		{
			if(Cpucontroller.EXWrestle)
			{
				return true;
			}
		}
		return false;
	}

	// アイテム入力があったか否か
	protected bool GetItemInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER && NowHitpoint > 0)
		{
			if (ControllerManager.Instance.Item)
			{
				// 装備しているアイテムを取得
				int nowitem = savingparameter.GetNowEquipItem();
				// 装備しているアイテムが何個あるか取得
				int numofitem = savingparameter.GetItemNum(nowitem);
				// アイテムの個数が0なら強制抜け
				if (numofitem < 1)
				{
					return false;
				}
				// 装備しているアイテムが全体用か否か
				bool isall = Item.itemspec[nowitem].IsAll();
				// 無効判定(全体）
				if (isall)
				{
					if (!savingparameter.ItemUseCheckAll(nowitem))
					{
						return false;
					}
				}
				// 無効判定(単体）
				else
				{
					if (!savingparameter.ItemUseCheck(nowitem, 0))
					{
						return false;
					}
				}

				// アイテムの効果を出す
				savingparameter.ItemDone(nowitem, 0);
				// エフェクトとSEを再生する
				UnityEngine.Object ItemEffect = null;
				// HP回復
				if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRETH_HP)
				{
					// エフェクト作成
					ItemEffect = Resources.Load("RebirthHP1");
					// SE再生
					AudioManager.Instance.PlaySE("RebirthHP1");
				}
				// 蘇生（パーティーを組めるようになってから）
				else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH)
				{
					// エフェクト作成
					ItemEffect = Resources.Load("Resurrection1");
					// SE再生
					AudioManager.Instance.PlaySE("Regeneration");
				}
				// 蘇生＋HP回復＋マジックゲージ回復
				else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_SOUL)
				{
					// エフェクト作成
					ItemEffect = Resources.Load("PurificationGem");
					// SE再生
					AudioManager.Instance.PlaySE("SoulGem");
				}
				// 蘇生しつつ完全回復（蘇生はパーティーを組めるようになってから）
				else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
				{
					// エフェクト作成
					ItemEffect = Resources.Load("Resurrection1");
					// SE再生
					AudioManager.Instance.PlaySE("Regeneration");
				}
				// 現在の自分の位置にエフェクトを置く
				var obj = (GameObject)Instantiate(ItemEffect, transform.position, transform.rotation);
				// 親子関係を再設定する
				obj.transform.parent = this.transform;
				// アイテム使用フラグを折る
				ControllerManager.Instance.Item = false;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// メニュー入力があったか否か
	/// </summary>
	/// <returns></returns>
	protected bool GetMenuInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Menu)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 覚醒入力があったか否か(優先度の都合上一番高くする)
	/// </summary>
	/// <returns></returns>
	protected bool GetArousalInput()
	{
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			// 射撃・格闘・ジャンプ
			if (ControllerManager.Instance.Shot && ControllerManager.Instance.Wrestle && ControllerManager.Instance.Jump)
			{
				return true;
			}
			// 特殊射撃＋格闘
			else if (ControllerManager.Instance.EXShot && ControllerManager.Instance.Wrestle)
			{
				return true;
			}
			// 特殊格闘＋射撃
			else if (ControllerManager.Instance.EXWrestle && ControllerManager.Instance.Shot)
			{
				return true;
			}
		}
		else
		{
			if(Cpucontroller.Shot && Cpucontroller.Wrestle && Cpucontroller.Jump)
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// ジャンプ時共通操作
	/// この関数を各キャラで継承させ、そこでアニメーションの再生を行う
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="jumpid">ジャンプのState番号</param>
	protected virtual void JumpDone(Animator animator)
	{
		transform.Translate(new Vector3(0, 1, 0));  // 打ち上げる
		animator.SetTrigger("Jump");
		Boost = Boost - JumpUseBoost;
	}


    /// <summary>
    /// ステップ時共通操作
    /// </summary>
    /// <param name="Yforce">Y方向への上昇量</param>
    /// <param name="inputVector">入力の方向</param>
    /// <param name="animator">キャラクター本体のAnimator</param>
    /// <param name="rainbow">虹エフェクトにするか否か（通常はfalse)</param>
    protected virtual void StepDone(float Yforce, Vector2 inputVector, Animator animator, bool rainbow = false)
	{
		// 固定があった場合解除
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		IsStep = true;
        // エフェクトを生成
        UnityEngine.Object Effect;
        // 通常
        if (!rainbow)
        {
            // エフェクトをロードする
            Effect = Resources.Load("StepEffect");
        }
        // ステップキャンセル時
        else
        {
            // 慣性をいったん消す
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            Effect = Resources.Load("StepEffectCancel");
        }
        // 現在の自分の位置にエフェクトを置くと少し下すぎるので、上にあげる
        Vector3 setpos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);


        var obj = (GameObject)Instantiate(Effect, setpos, transform.rotation);
        // 親子関係を再設定する
        obj.transform.parent = this.transform;
		// 入力の方向ごとに切り分け
		StepInput stepinput = StepInput.NONE;

		// 前を取得した場合
		if(inputVector.x > -0.5f && inputVector.x < 0.5f && inputVector.y > 0.5f)
		{
			stepinput = StepInput.FRONT;
		}
		// 左を取得した場合
		else if(inputVector.x <= -0.5f && inputVector.y <= 0.5f && inputVector.y >= -0.5f)
		{
			stepinput = StepInput.LEFT;
		}
		// 右を取得した場合
		else if(inputVector.x >= 0.5f && inputVector.y <= 0.5f && inputVector.y >= -0.5f)
		{
			stepinput = StepInput.RIGHT;
		}
		// 後を取得した場合
		else if(inputVector.x > -0.5f && inputVector.x < 0.5f && inputVector.y < -0.5f)
		{
			stepinput = StepInput.BACK;
		}

		// ロックオン時は相手の方向を見てステップする
		if(IsRockon)
		{
			// 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
			var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
			// 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
			// このため、高低差がないとみなす
			Vector3 Target_VertualPos = target.Enemy.transform.position;
			Target_VertualPos.y = 0;
			Vector3 Mine_VerturalPos = transform.position;
			Mine_VerturalPos.y = 0;
			transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
			// 前
			if(stepinput == StepInput.FRONT)
			{
				animator.SetTrigger("FrontStep");
			}
			// 左
			else if(stepinput == StepInput.LEFT)
			{
				animator.SetTrigger("LeftStep");
				SideStep(180);
			}
			// 右
			else if(stepinput == StepInput.RIGHT)
			{
				animator.SetTrigger("RightStep");
				SideStep(-180);
			}
			// 後
			else if(stepinput == StepInput.BACK)
			{
				animator.SetTrigger("BackStep");
			}
		}
		// 非ロック時は強制的に前ステップする
		else
		{
			animator.SetTrigger("FrontStep");
			MoveDirection = transform.rotation * Vector3.forward;
		}

		// 開始位置を保存する
		StepStartPos = transform.position;

        
        Boost = Boost - StepUseBoost;
        // ステップ累積距離を0に初期化
        SteppingLength = 0.0f;

        // 空中ステップのために重力無効
        GetComponent<Rigidbody>().useGravity = false;

        // 移動方向取得
        UpdateRotation_step();

                
        _StepStartTime = Time.time;
    }

	public enum StepInput
	{
		NONE,
		FRONT,
		LEFT,
		RIGHT,
		BACK
	}

    /// <summary>
    /// ステップ終了時処理
    /// </summary>
    /// <param name="animator">本体のanimator</param>
    /// <param name="idlehash">idle状態のハッシュコード</param>
    /// <param name="fallhash">fall状態のハッシュコード</param>
    protected virtual void EndStep(Animator animator, int idlehash,int fallhash)
    {
        // 実行中で終了していなかった場合終了まで待つ（硬直扱い）   
        // 終了判定
        if (Time.time > _StepStartTime + 1.35f/*this.m_LandingTime*/)
        {
            // 無効になっていたら重力を復活させる
            GetComponent<Rigidbody>().useGravity = true;
            // 地上にいたら着地
            if (IsGrounded)
            {
                // ブースト量を初期化する
                Boost = GetMaxBoost(BoostLevel);
            }
            // 空中にいたら角度を戻して落下
            else
            {
                FallDone(new Vector3(0, 0, 0),animator);
            }
			// ステップ累積時間を戻す
			_StepStartTime = 0;
			// アニメーションを戻す
			animator.SetTrigger("Idle");
        }
    }

    
    /// <summary>
    /// 通常射撃処理（オーバーライド前提。基本攻撃系処理は空にしておいて、使うところでオーバーライドすること）
    /// </summary>
    protected virtual void Shot()
    {
        if (IsGrounded)
        {
            Boost = GetMaxBoost(BoostLevel);
        }
    }

    /// <summary>
    /// 歩き撃ち処理
    /// </summary>
    protected virtual void ShotRun()
    {

    }

    /// <summary>
    /// 空中ダッシュ通常射撃処理
    /// </summary>
    /// <param name="animator">本体のAnimator</param>
    /// <param name="fallhash">fall時のハッシュコード</param>
    protected virtual void ShotAirDash(Animator animator, int fallhash)
    {
        // ブースト切れ時にFallDone、DestroyWrestleを実行する
        if (Boost <= 0)
        {
            animator.SetTrigger("Fall");
            DestroyWrestle();
        }
        // 着地時にLandingを実行する
        if (IsGrounded)
        {
			Debug.Log("01");
            LandingDone(animator);
        }
    }

    /// <summary>
    /// チャージ射撃処理
    /// </summary>
    protected virtual void ChargeShot()
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
    }

    /// <summary>
    /// サブ射撃処理
    /// </summary>
    protected virtual void SubShot()
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
    }

    /// <summary>
    /// 特殊射撃処理
    /// </summary>
    protected virtual void ExShot()
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
    }

    /// <summary>
    /// 格闘の累積時間
    /// </summary>    
    protected float Wrestletime;
    /// <summary>
    /// N格1段目
    /// </summary>    
    protected virtual void Wrestle1(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// N格2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void Wrestle2(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// N格3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void Wrestle3(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// ステップキャンセル実行時の処理
    /// </summary>
    protected virtual void StepCancel(Animator animator)
    {
        // キャンセルダッシュ入力を受け取ったら、キャンセルして空中ダッシュする
        if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
		{
            AddInput = false;
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            CancelDashDone(animator);
        }
        // ステップ入力を受け取ったら、キャンセルしてステップする
        else if (ControllerManager.Instance.FrontStep || ControllerManager.Instance.LeftFrontStep || ControllerManager.Instance.LeftStep || ControllerManager.Instance.LeftBackStep || ControllerManager.Instance.BackStep ||
            ControllerManager.Instance.RightBackStep || ControllerManager.Instance.RightStep || ControllerManager.Instance.RightFrontStep)
        {
            Vector2 stepinput = Vector2.zero;
            IsStep = true;
            // 入力方向取得
            if(ControllerManager.Instance.FrontStep)
            {
                stepinput.x = 0;
                stepinput.y = 1;
            }
            else if(ControllerManager.Instance.LeftFrontStep)
            {
                stepinput.x = -0.5f;
                stepinput.y = 0.5f;
            }
            else if(ControllerManager.Instance.LeftStep)
            {
                stepinput.x = -1.0f;
                stepinput.y = 0.0f;
            }
            else if(ControllerManager.Instance.LeftBackStep)
            {
                stepinput.x = -0.5f;
                stepinput.y = -0.5f;
            }
            else if(ControllerManager.Instance.BackStep)
            {
                stepinput.x = 0;
                stepinput.y = -1;
            }
            else if(ControllerManager.Instance.RightBackStep)
            {
                stepinput.x = 0.5f;
                stepinput.y = -0.5f;
            }
            else if(ControllerManager.Instance.RightStep)
            {
                stepinput.x = 1.0f;
                stepinput.y = 0.0f;
            }
            else if(ControllerManager.Instance.RightFrontStep)
            {
                stepinput.x = 0.5f;
                stepinput.y = 0.5f;
            }
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            // ステップキャンセル成功の証しとしてエフェクトが虹になる
            StepDone(1, stepinput,animator, true);
        }
    }

    /// <summary>
    /// 格闘チャージ
    /// </summary>
    protected virtual void ChargeWrestle()
    {
    }

    /// <summary>
    /// 前格闘1段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void FrontWrestle1(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 前格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void FrontWrestle2(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 前格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void FrontWrestle3(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 左横格闘1段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void LeftWrestle1(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 左横格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void LeftWrestle2(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 左横格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void LeftWrestle3(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 右横格闘1段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void RightWrestle1(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 右横格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    protected virtual void RightWrestle2(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 右横格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void RightWrestle3(Animator animator)
    {
        IsWrestle = true;
        Wrestletime += Time.deltaTime;
        StepCancel(animator);
    }

    /// <summary>
    /// 後格闘（防御）
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void BackWrestle(Animator animator)
    {
		IsGuard = true;
        IsWrestle = true;
        //1．ブーストゲージを減衰させる
        Boost -= Time.deltaTime * 5.0f;
        //2．ブーストゲージが0になると、強制的にIdleに戻す
        if (Boost <= 0)
        {
            animator.SetTrigger("Idle");
            DestroyWrestle();
			// 格闘フラグを破棄
			IsWrestle = false;
		}
		//3．下入力を離すと、強制的にIdleに戻す
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			// ↓要素が抜けたらガード解除
			if (!ControllerManager.Instance.Under && !ControllerManager.Instance.LeftUnder && !ControllerManager.Instance.RightUnder)
			{
				animator.SetTrigger("Idle");
				DestroyWrestle();
				// 格闘フラグを破棄
				IsWrestle = false;
			}
		}
		else
		{
			if(!Cpucontroller.Under)
			{
				animator.SetTrigger("Idle");
				DestroyWrestle();
				// 格闘フラグを破棄
				IsWrestle = false;
			}
		}
		// 強制的にロック対象の方を向く
		if (IsRockon)
		{
			RotateToTarget();
		}
	}

    /// <summary>
    /// 空中ダッシュ格闘
    /// </summary>
    /// <param name="animator">本体のAnimator</param>
    protected virtual void AirDashWrestle(Animator animator)
    {
		Vector3 RiseSpeed = new Vector3(MoveDirection.x, this.RiseSpeed, MoveDirection.z);
		IsWrestle = true;
		// 空中ダッシュ中はブーストダッシュキャンセル不可にしておく
        //StepCancel(animator, airdashhash);
        // 発動中常時ブースト消費
        Boost = Boost - BoostLess;
		// ブースト切れ時か格闘ボタンを離すとにFallDone、DestroyWrestleを実行する
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (Boost <= 0 || ControllerManager.Instance.WrestleUp)
			{
				FallDone(RiseSpeed, animator);
				animator.SetTrigger("Fall");
				DestroyWrestle();
			}
		}
		else
		{
			if (Boost <= 0 || !Cpucontroller.Wrestle)
			{
				FallDone(RiseSpeed, animator);
				animator.SetTrigger("Fall");
				DestroyWrestle();
			}
		}
    }

    // 特殊格闘1段目（特殊格闘はキャラによっては射撃や回復だったりするのでStepCancelは継承先につけること
    protected virtual void ExWrestle1()
    {
        IsWrestle = true;
    }

    // 特殊格闘2段目
    protected virtual void ExWrestle2()
    {
        IsWrestle = true;
    }

    // 特殊格闘3段目
    protected virtual void ExWrestle3()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 前特殊格闘1段目（全員共通で上昇技）
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void FrontExWrestle1(Animator animator)
    {
        IsWrestle = true;
        // 毎フレームブーストを消費する
        Boost -= Time.deltaTime * 100.0f;
        // 重力無効
        this.GetComponent<Rigidbody>().useGravity = false;
        // ブーストが0になったらFallにする
        if (Boost <= 0)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            animator.SetTrigger("Fall");
            DestroyWrestle();
        }
        StepCancel(animator);
    }

    /// <summary>
    /// 前特殊格闘2段目
    /// </summary>
    protected virtual void FrontExWrestle2()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 前特殊格闘3段目
    /// </summary>
    protected virtual void FrontExWrestle3()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 左横特殊格闘1段目
    /// </summary>
    protected virtual void LeftExWrestle1()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 左横特殊格闘2段目
    /// </summary>
    protected virtual void LeftExWrestle2()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 左横特殊格闘3段目
    /// </summary>
    protected virtual void LeftExWrestle3()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 右横特殊格闘1段目
    /// </summary>
    protected virtual void RightExWrestle1()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 右横特殊格闘2段目
    /// </summary>
    protected virtual void RightExWrestle2()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 右横特殊格闘3段目
    /// </summary>
    protected virtual void RightExWrestle3()
    {
        IsWrestle = true;
    }

    /// <summary>
    /// 後特殊格闘（全員共通で下降技）
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void BackExWrestle(Animator animator)
    {
        IsWrestle = true;
        // 毎フレームブーストを消費する
        Boost -= Time.deltaTime * 100;
        // ブーストが0になったらFallにする
        if (Boost <= 0)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            animator.SetTrigger("Fall");
            DestroyWrestle();
        }
        StepCancel(animator);
        // 接地したらLandingにする
        if (IsGrounded)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
			Debug.Log("02");
			LandingDone(animator);
        }
    }

    /// <summary>
    /// 覚醒技の初期化を実行したか？
    /// </summary>
    protected bool InitializeArousal;

    /// <summary>
    /// 覚醒技発動中
    /// </summary>
    protected virtual void ArousalAttack()
    {
        
    }

    
    /// <summary>
    /// 着地共通動作
    /// <param name="animator">本体のanimator</param>
    /// </summary>
    protected void LandingDone(Animator animator)
    {
        // 格闘判定削除
        DestroyWrestle();
        // ずれた本体角度を戻す(Yはそのまま）
        transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        // 無効になっていたら重力を復活させる
        GetComponent<Rigidbody>().useGravity = true;
        animator.SetTrigger("Landing");
        // 着地したので硬直を設定する
        LandingTime = Time.time;
    }


    /// <summary>
    /// 空中ダッシュ（キャンセルダッシュ）発動共通操作
    /// 弓ほむら・まどかのモーションキャンセルなどはこの前に行うこと
    /// </summary>
    protected virtual void CancelDashDone(Animator animator)
    {
		animator.speed = 1.0f;
		if (Boost > 0)
        {
			// 固定があった場合解除
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            IsStep = false;
            IsWrestle = false;
            // 格闘判定削除
            DestroyWrestle();
            // 一応歩き撃ちフラグはここでも折る
            RunShotDone = false;
            // 上体の角度を戻す
            BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            Rotatehold = false;
            Boost = Boost - DashCancelUseBoost;
            animator.SetTrigger("AirDash");
            // 移動方向取得
            //UpdateRotation();
            //this.m_MoveDirection = transform.rotation * Vector3.forward;
            // 角度に応じてX、Zの方向を切り替える
            if (transform.rotation.eulerAngles.y >= 337.5f || transform.rotation.eulerAngles.y < 22.5f)
            {
                MoveDirection.x = 0.0f;
                MoveDirection.z = 1.0f;
			}
            else if (transform.rotation.eulerAngles.y >= 22.5f && transform.rotation.eulerAngles.y < 67.5f)
            {
                MoveDirection.x = 0.7f;
                MoveDirection.z = 0.0f;
			}
            else if (transform.rotation.eulerAngles.y >= 67.5f && transform.rotation.eulerAngles.y < 112.5f)
            {
                MoveDirection.x = 1.0f;
                MoveDirection.z = 0.0f;
			}
            else if (transform.rotation.eulerAngles.y >= 112.5f && transform.rotation.eulerAngles.y < 157.5f)
            {
                MoveDirection.x = 0.7f;
                MoveDirection.z = -0.5f;
			}
            else if (transform.rotation.eulerAngles.y >= 157.5f && transform.rotation.eulerAngles.y < 202.5f)
            {
                MoveDirection.x = 0.0f;
                MoveDirection.z = -1.0f;
			}
            else if (transform.rotation.eulerAngles.y >= 202.5f && transform.rotation.eulerAngles.y < 247.5f)
            {
                MoveDirection.x = -0.7f;
                MoveDirection.z = -0.5f;
			}
            else if (transform.rotation.eulerAngles.y >= 247.5f && transform.rotation.eulerAngles.y < 292.5f)
            {
                MoveDirection.x = -1.0f;
                MoveDirection.z = 0.0f;
			}
            else if (transform.rotation.eulerAngles.y >= 292.5f && transform.rotation.eulerAngles.y < 337.5f)
            {
                MoveDirection.x = -0.7f;
                MoveDirection.z = 0.0f;
			}

            // 上方向への慣性を切る
            MoveDirection.y = 0;
			// 発動中重力無効
			GetComponent<Rigidbody>().useGravity = false;
            // その方向へ移動
            GetComponent<Rigidbody>().AddForce(MoveDirection.x, 10, MoveDirection.z);
        }
    }


    /// <summary>
    /// 下降共通動作
    /// </summary>
    /// <param name="RiseSpeed">落下速度</param>
    /// <param name="animator">本体のanimator</param>
    protected void FallDone(Vector3 RiseSpeed,Animator animator)
    {
		// 固定があった場合解除
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		IsStep = false;
        IsWrestle = false;
        GetComponent<Rigidbody>().useGravity = true;
        animator.SetTrigger("Fall");
        RiseSpeed = new Vector3(0, -this.RiseSpeed, 0);
    }

    /// <summary>
    /// くっついている格闘オブジェクトをすべて消す
    /// </summary>
    protected void DestroyWrestle()
    {
        for (int i = 0; i < WrestleRoot.Length; i++)
        {
            // あらかじめ子があるかチェックしないとGetChildを使うときはエラーになる
            if (WrestleRoot[i] != null && this.WrestleRoot[i].GetComponentInChildren<Wrestle_Core>() != null)
            {
                var wrestle = WrestleRoot[i].GetComponentInChildren<Wrestle_Core>();

                if (wrestle != null)
                {
                    Destroy(wrestle.gameObject);
                }
            }
        }
		IsWrestle = false;
	}

    /// <summary>
    /// ステップの開始時間
    /// </summary>
    private float _StepStartTime;

    // 最大ブースト量算出関数
    public float GetMaxBoost(int nowlevel)
    {
        return (float)(BoostGrowth * BoostLevel + Boost_OR);
    }

    // 最大覚醒ゲージ量算出関数
    public float GetMaxArousal(int nowlevel)
    {
        return (float)(ArousalGrowth * (nowlevel - 1) + Arousal_OR);
    }

    // 最大HP算出関数
    public int GetMaxHitpoint(int nowlevel)
    {
        return (int)(NowHitpointGrowth * Level + NowHitpoint_OR);
    }

    /// <summary>
    /// ワールド座標でのカメラの基底ベクトルを計算し、それを基にキャラクターの回転を計算する
    /// </summary>
    protected void UpdateRotation()
    {
        var finalRot = transform.rotation;
        float horizontal = 0;        // 横入力を検出
		float vertical = 0;            // 縦入力を検出
									   // プレイヤー時
		if (IsPlayer == CHARACTERCODE.PLAYER)
		{
			if (ControllerManager.Instance.Top)
			{
				vertical = 1.0f;
			}
			else if (ControllerManager.Instance.Under)
			{
				vertical = -1.0f;
			}
			else if (ControllerManager.Instance.LeftUpper || ControllerManager.Instance.RightUpper)
			{
				vertical = 0.6f;
			}
			else if (ControllerManager.Instance.LeftUnder || ControllerManager.Instance.RightUnder)
			{
				vertical = -0.6f;
			}

			if (ControllerManager.Instance.Left)
			{
				horizontal = -1.0f;
			}
			else if (ControllerManager.Instance.Right)
			{
				horizontal = 1.0f;
			}
			else if (ControllerManager.Instance.LeftUpper || ControllerManager.Instance.LeftUnder)
			{
				horizontal = -0.6f;
			}
			else if (ControllerManager.Instance.RightUpper || ControllerManager.Instance.RightUnder)
			{
				horizontal = 0.6f;
			}
		}
		// CPU時
		else
		{
			if(Cpucontroller.Top)
			{
				vertical = 1.0f;
			}
			else if(Cpucontroller.Under)
			{
				vertical = -1.0f;
			}
			else if (Cpucontroller.LeftUpper || Cpucontroller.RightUpper)
			{
				vertical = 0.6f;
			}
			else if (Cpucontroller.LeftUnder || Cpucontroller.RightUnder)
			{
				vertical = -0.6f;
			}

			if (Cpucontroller.Left)
			{
				horizontal = -1.0f;
			}
			else if (Cpucontroller.Right)
			{
				horizontal = 1.0f;
			}
			else if (Cpucontroller.LeftUpper || Cpucontroller.LeftUnder)
			{
				horizontal = -0.6f;
			}
			else if (Cpucontroller.RightUpper || Cpucontroller.RightUnder)
			{
				horizontal = 0.6f;
			}
		}

        var toWorldVector = MainCamera.transform.rotation;
        // ベクトルは平行移動の影響を受けないので逆行列は使わない
        // スケールの影響は受けるがここでは無視する。
		        

        // ロックオン時特殊処理
        // 案1：敵と重なるような状態になったら一時的に移動方向を固定する(角度が特異姿勢になるので、暴走する）
        // 案2：敵と重なるような状態になったらカメラを反転する
        // ガンダムは1と2の併用？2はやってないか。一定距離離れたら固定が解除されているだけっぽい→1で確定
        if (this.IsRockon)
        {
            // 敵（ロックオン対象）の座標を取得
            Player_Camera_Controller targetspec = GetComponentInChildren<Player_Camera_Controller>();
            if (targetspec.Enemy == null)
            {
                IsRockon = false;
                return;
            }
            Vector3 targetpos = targetspec.Enemy.transform.position;
            // 自分の座標を取得
            Vector3 myPos = transform.position;


            // XとZの差が一定値以下で移動方向固定(空中移動時限定）
            if (!this.IsGrounded)
            {
                if (Mathf.Abs(targetpos.x - myPos.x) < 10.0f && Mathf.Abs(targetpos.z - myPos.z) < 10.0f)
                {
                    Rotatehold = true;
                    return;
                }
            }
           
        }
        // 横入力時
        if (0.0f != horizontal)
        {
            // ワールド座標でのカメラの横ベクトルを計算
            var wRight = toWorldVector * Vector3.right;
            wRight.y = 0.0f;
            wRight.Normalize();
            if (0.0f > horizontal)
            {
                // ネガティブ側が押されているので反転
                wRight = -wRight;
            }
            finalRot = Quaternion.LookRotation(wRight);
        }

        // 縦入力時
        if (0.0f != vertical)
        {
            if (0.0f != horizontal)
            {
                // 横移動をすでに行っている場合 45°回転する
                var q = Quaternion.AngleAxis(-45 * vertical * horizontal, Vector3.up);
                finalRot = q * finalRot;
            }
            else
            {
                // ワールド座標でのカメラの視線ベクトル
                var wForward = toWorldVector * Vector3.forward;
                wForward.y = 0;
                wForward.Normalize();

                if (0.0f > vertical)
                {
                    wForward = -wForward;
                }

                finalRot = Quaternion.LookRotation(wForward);
            }
        }
        //Debug.Log("Rotation:" + finalRot);
        
        transform.rotation = finalRot;
        
    }

    // 上記のステップ用（本体を回転させない）
    protected void UpdateRotation_step()
    {
        var finalRot = transform.rotation;
		float horizontal = 0;        // 横入力を検出
		float vertical = 0;            // 縦入力を検出
		if (ControllerManager.Instance.Top)
		{
			vertical = 1.0f;
		}
		else if (ControllerManager.Instance.Under)
		{
			vertical = -1.0f;
		}

		if (ControllerManager.Instance.Left)
		{
			horizontal = -1.0f;
		}
		else if (ControllerManager.Instance.Right)
		{
			horizontal = 1.0f;
		}
		var toWorldVector = MainCamera.transform.rotation;
		
        // 横入力時
        if (0.0f != horizontal)
        {
            // ワールド座標でのカメラの横ベクトルを計算
            var wRight = toWorldVector * Vector3.right;
            wRight.y = 0.0f;
            wRight.Normalize();
            if (0.0f > horizontal)
            {
                // ネガティブ側が押されているので反転
                wRight = -wRight;
            }
            finalRot = Quaternion.LookRotation(wRight);
        }
        // 縦入力時
        if (0.0f != vertical)
        {
            if (0.0f != horizontal)
            {
                // 横移動をすでに行っている場合 45°回転する
                var q = Quaternion.AngleAxis(-45 * vertical * horizontal, Vector3.up);
                finalRot = q * finalRot;
            }
            else
            {
                // ワールド座標でのカメラの視線ベクトル
                var wForward = toWorldVector * Vector3.forward;
                wForward.y = 0;
                wForward.Normalize();

                if (0.0f > vertical)
                {
                    wForward = -wForward;
                }

                finalRot = Quaternion.LookRotation(wForward);
            }
        }
		StepRotation = finalRot;
    }

    /// <summary>
    /// 着地後共通動作
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="landinghash"></param>
    protected void LandingDone2(Animator animator,int landinghash)
    {
        // 地響き防止
        MoveDirection = transform.rotation * new Vector3(0, 0, 0);
		// モーション終了時にアイドルへ移行
		// 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
		if (Time.time > LandingTime + LandingWaitTime)
        {
            animator.SetTrigger("Landing");
            // ブースト量を初期化する
            Boost = GetMaxBoost(this.BoostLevel);
        }
    }

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="idleID">アイドルモーションのID</param>
	protected virtual void FirstSetting(Animator animator,int idleID)
	{
		// CharacterControllerを取得
		//this.m_charactercontroller = GetComponent<CharacterController>();
		// Rigidbodyを取得
		RigidBody = GetComponent<Rigidbody>();

		// PCでなければカメラを無効化しておく
		if (IsPlayer != CHARACTERCODE.PLAYER)
		{
			// 自分にひっついているカメラオブジェクトを探し、カメラを切っておく
			transform.Find("Main Camera").GetComponent<Camera>().enabled = false;
		}
		// 変数の初期設定はキャラごとに行う(アニメーションファイルの名前はここで入力。オーバーライドした場合継承元の内容も同時実行できたはず？）        

		// 設置関係を初期化
		var collider = GetComponent<CapsuleCollider>();
		if (null == collider)
		{
			Debug.LogError("カプセルコライダが見つからない");
			Application.Quit();
		}
		LayOriginOffs = new Vector3(0.0f, ColliderHeight, 0.0f);
		Laylength = collider.radius + collider.height;// / 2 + 1.5f;//0.2f;
													  //this.m_layOriginOffs = new Vector3(0.0f, m_Collider_Height, 0.0f);
													  //this.m_laylength = m_charactercontroller.radius + m_charactercontroller.height / 2 + 1.5f;
		LayMask = 1 << 8;       // layMaskは無視するレイヤーを指定する。8のgroundレイヤー以外を無視する

		
		// ジャンプ硬直
		JumpTime = -JumpWaitTime;

		// 着地硬直
		LandingTime = -LandingWaitTime;

		// 歩行速度(m_WalkSpeed等はそれぞれで設定）
		MoveDirection = Vector3.zero;
		// 吹き飛び速度
		BlowDirection = Vector3.zero;

		// 初期アニメIdleを再生する
        animator.SetTrigger("Idle");

		// ブースト量を初期化する
		Boost = GetMaxBoost(this.BoostLevel);

		// 上体を初期化する
		BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		// 歩き撃ちフラグを初期化する
		RunShotDone = false;

		// 覚醒ゲージ量を初期化する
		Arousal = GetMaxArousal(ArousalLevel);
		// 覚醒ゲージを初期化する
		Arousal = 0;
		// 覚醒状態を初期化する
		IsArousal = false;
		// 覚醒演出状態を初期化する
		ArousalAttackProduction = false;

		// PC時・HP・覚醒ゲージ・ソウルジェム汚染率を初期化する
		if (IsPlayer != CHARACTERCODE.ENEMY)
		{
			// SavingParameterからのステートの変更を受け付ける
			int charactername = (int)CharacterName;
			// HP
			NowHitpoint = savingparameter.GetNowHP(charactername);
			// 覚醒ゲージ
			Arousal = savingparameter.GetNowArousal(charactername);			
		}
		// 敵の時HP、覚醒ゲージを初期化する
		else
		{
			NowHitpoint = GetMaxHitpoint(this.Level);
			Arousal = 0;
		}

		// ロックオンを初期化する
		IsRockon = false;

		// 進行方向固定フラグを初期化する
		Rotatehold = false;

		// アーマー状態を初期化する
		IsArmor = false;

		
		// 時間停止のルートを初期化する
		TimeStopMaster = false;

		// ダウン値の閾値を初期化する
		DownRatioBias = MadokaDefine.DOWNRATIO;

		// ダウン値を初期化する
		NowDownRatio  = 0;

		// ダウン時間を初期化する
		DownTime = 0;

		// ダウン時の打ち上げ量を初期化する
		LaunchOffset = 10.0f;

		// 死亡時の爆発フラグを初期化する
		Explode = false;

		// 格闘時の動作速度を初期化する
		WrestlSpeed = 0.0f;
		// 格闘時の追加入力の有無を初期化する
		AddInput = false;

		// テンキー入力があったか否か
		HasVHInput = false;
		// ショット入力があったか否か
		HasShotInput = false;
		// ジャンプ入力があったか否か
		HasJumpInput = false;
		// ダッシュキャンセル入力があったか否か
		HasDashCancelInput = false;
		// 空中ダッシュ入力があったか否か
		HasAirDashInput = false;
		// サーチ入力があったか否か
		HasSearchInput = false;
		// サーチキャンセル入力があったか否か
		HasSearchCancelInput = false;
		// 格闘入力があったか否か
		HasWrestleInput = false;
		// サブ射撃入力があったか否か
		HasSubShotInput = false;
		// 特殊射撃入力があったか否か
		HasExShotInput = false;
		// 特殊格闘入力があったか否か
		HasExWrestleInput = false;
		// アイテム入力があったか否か
		HasItemInput = false;
		// ポーズ入力があったか否か
		HasMenuInput = false;
		// 覚醒入力があったか否か
		HasArousalInput = false;
		// 覚醒技入力があったか否か
		HasArousalAttackInput = false;
		// 前入力があったか否か
		HasFrontInput = false;
		// 左入力があったか否か
		HasLeftInput = false;
		// 右入力があったか否か
		HasRightInput = false;
		// 後入力があったか否か
		HasBackInput = false;


		// m_DownRebirthTime/waitの初期化とカウントを行う
		// m_DamagedTime/waitの初期化とカウントを行う
		// m_DownTime/waitの初期化とカウントを行う

		// m_DownRebirthTime/waitの初期化を行う(ダウン値がリセットされるまでの時間）
		DownRebirthTime = 0;
		DownRebirthWaitTime = 3.0f;
		// m_DamagedTime/waitの初期化とカウントを行う(ダメージによる硬直時間）
		DamagedTime = 0;
		DamagedWaitTime = 1.0f;
		// m_DownTimeの初期化とカウントを行う(ダウン時の累積時間）
		DownTime = 0;
		DownWaitTime = 3.0f;

		// 復帰時のブースト消費量
		ReversalUseBoost = 20.0f;

		// 1Fあたりの射撃チャージゲージ増加量
		ShotIncrease = 1;
		// 1Fあたりの格闘チャージゲージ増加量
		WrestleIncrease = 1;
		// 1Fあたりの射撃チャージゲージ減衰量
		ShotDecrease = 1;
		// 1Fあたりの格闘チャージゲージ減衰量
		WrestleDecrease = 1;

		// チャージ最大値


		// 壁面接触フラグ
		_Hitjumpover = false;
		_Hitunjumpover = false;
		// リロードクラス作成
		ReloadSystem = new Reload();

		// ポーズコントローラー取得
		Pausecontroller = GameObject.Find("PauseManager");

		// 足音取得(StageSettingで決めているので、その値を拾う
		GameObject stagesetting = GameObject.Find("StageSetting");
		if (stagesetting != null)
		{
			var ss = stagesetting.GetComponent<StageSetting>();
			int ft = ss.getFootType();
			if (ft == 0)
			{
				_Foottype = StageSetting.FootType.FootType_Normal;
			}
			else if (ft == 1)
			{
				_Foottype = StageSetting.FootType.FootType_Wood;
			}
			else if (ft == 2)
			{
				_Foottype = StageSetting.FootType.FootType_Jari;
			}
			else if (ft == 3)
			{
				_Foottype = StageSetting.FootType.FootType_Snow;
			}
			else if (ft == 4)
			{
				_Foottype = StageSetting.FootType.FootType_Carpet;
			}
		}
		
	}

    /// <summary>
    ///  継承先のUpdate開幕で実行すること
    /// また、時間停止状態の場合、falseを返す
    /// </summary>
    /// <param name="isSpindown">錐揉みダウン状態</param>
    /// <param name="animator">このオブジェクトのアニメーター</param>
    /// <param name="downid">ダウン時のID</param>
    /// <param name="airdashid">空中ダッシュ時のID</param>
    /// <param name="airshotid">空中射撃時のID</param>
    /// <param name="fallid">落下時のID</param>
    /// <param name="jumpingid">上昇中のID</param>
    /// <param name="blowid">吹き飛び時のID</param>
    /// <param name="idleid">アイドル時のID</param>
    /// <param name="runid">走行時のID</param>
	/// <param name="damageid">ダメージのID</param>
    /// <returns></returns>
    protected bool Update_Core(bool isSpindown, Animator animator, int downid,int airdashid,int airshotid,int jumpingid, int fallid,int idleid,int blowid,int runid,int frontstepid,int leftstepid,int rightstepid,int backstepid,int damageid)
    {
        // 接地判定
        IsGrounded = onGround2(isSpindown);

		

		// PC死亡時、PauseControllerを消す(メニュー画面に移行させないため）
		// また、ショット入力でタイトル画面へ移行する
		if (NowHitpoint < 1 && IsPlayer == CHARACTERCODE.PLAYER)
        {
			// ゲームオーバーをアクティブにする
			GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>().GameOver.SetActive(true);
            GameObject PauseController = GameObject.Find("Pause Controller");
            if (PauseController)
            {
                Destroy(PauseController);
            }
            if (GetShotInput())
            {
                // 例外１：最初のスコノ戦で負けたらプロローグ３へ
                if (savingparameter.story == 1)
                {
                    FadeManager.Instance.LoadLevel("Prologue03", 1.0f);
                }
                else
                {
                    FadeManager.Instance.LoadLevel("title", 1.0f);
                }
            }
        }

		// 入力取得
		// 方向キー取得
		HasVHInput = GetVHInput();
		// ショット入力があったか否か
		HasShotInput = GetShotInput();
		// ジャンプ入力があったか否か
		HasJumpInput = GetJumpInput();
		// ジャンプ長押し入力があったか否か
		HasJumpingInput = GetJumpingInput();
		// ダッシュキャンセル入力があったか否か
		HasDashCancelInput = GetDashCancelInput();
		// 空中ダッシュ入力（ダッシュキャンセルののちボタン長押し維持）があったか否か（CPU専用コマンド）
		HasAirDashInput = GetAirDashInput();
		// サーチ入力があったか否か
		//HasSearchInput = GetSearchInput();
		// サーチキャンセル入力があったか否か
		HasSearchCancelInput = GetUnSerchInput();
		// 格闘入力があったか否か
		HasWrestleInput = GetWrestleInput();
		// サブ射撃入力があったか否か
		HasSubShotInput = GetSubShotInput();
		// 特殊射撃入力があったか否か
		HasExShotInput = GetExShotInput();
		// 特殊格闘入力があったか否か
		HasExWrestleInput = GetExWrestleInput();
		// アイテム入力があったか否か
		HasItemInput = GetItemInput();
		// メニュー入力があったか否か
		HasMenuInput = GetMenuInput();
		// 覚醒入力があったか否か
		HasArousalInput = GetArousalInput();
		// 覚醒技入力があったか否か
		HasArousalAttackInput = GetArousalInput();
		// 前入力があったか否か
		HasFrontInput = GetFrontInput();
		// 左入力があったか否か
		HasLeftInput = GetLeftInput();
		// 右入力があったか否か
		HasRightInput = GetRightInput();
		// 後入力があったか否か
		HasBackInput = GetBackInput();

		// プレイヤー操作の場合（CPU操作の場合はまた別に）
		if (IsPlayer == CHARACTERCODE.PLAYER)
        {   
            // 位置を保持
            savingparameter.nowposition = this.transform.position;
            // 角度を保持
            savingparameter.nowrotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        
       

        // 死亡時、エフェクトが消滅したら自壊させる
        if (Explode)
        {
            if (GetComponentInChildren<Deadfx>() == null)
            {
                Destroy(gameObject);
            }
        }

        // 時間停止中にやってほしくない処理はここ以降に記述すること

        // チャージショット関連の入力を取得(時間停止中に減衰されると困るので、ここで管理。また、通常の射撃や格闘より優先度は高くする
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 射撃
            HasShotChargeInput = GetShotChargeInput();
            // 格闘
            HasWrestleChargeInput = GetWrestleChargeInput();
        }

        // キャラを取得
        int character = (int)CharacterName;
        // 覚醒入力を取得し、その場合覚醒開始画面へ移行してポーズ  
        if (HasArousalInput && !IsArousal && savingparameter.GetGemContimination(character) < 100.0f)
        {
            // 最低でもLV1時の覚醒ゲージがないと覚醒不可(ポーズを強制解除して抜ける）
            if (Arousal < GetMaxArousal(1))
            {
                unuseArousal();
            }
            else
            {
                //float OR_GemCont = savingparameter.GetGemContimination(character);                
                arousalStart(damageid, blowid);
            }
        }

        // 覚醒時、覚醒ゲージ減少
        if (IsArousal)
        {
            Arousal -= Time.deltaTime * 10;
            savingparameter.SetNowArousal((int)CharacterName, Arousal);
            if (Arousal <= 0)
            {
                // 覚醒状態解除
                // 覚醒エフェクトを消す
                ArousalEffect.SetActive(false);
                // ゲージを0にする
                Arousal = 0;
                // 覚醒フラグを折る
                IsArousal = false;
            }
        }


        //m_nowDownRatioが0を超えていて、Damage_Initではなく（ダウン値加算前にリセットされる）m_DownRebirthTimeが規定時間を経過し、かつダウン値が閾値より小さければダウン値をリセットする
        if (NowDownRatio > 0 && animator.GetCurrentAnimatorStateInfo(0).fullPathHash != downid)
        {
            //if ((Time.time > DownRebirthTime + DownRebirthWaitTime) && (NowDownRatio < DownRatioBias))
			if ((Time.time > DownRebirthTime + DownRebirthWaitTime) && (NowDownRatio < DownRatioBias))
			{
                DownRebirthTime = 0;
                NowDownRatio = 0.0f;
				DownTime = 0;
				Invincible = false;
            }
        }

        // MoveDirection は アニメーションステート Walk で設定され、アニメーションステートが Idle の場合リセットされる。
        // 移動処理はステートの影響を受けないように UpdateAnimation のステートスイッチの前に処理する。
        // ステートごとに処理をする場合、それをメソッド化してそれぞれのステートに置く。
        // 走行速度を変更する、アニメーションステートが Run だった場合 RunSpeed を使う。
        var MoveSpeed = RunSpeed;
        // 空中ダッシュ時/空中ダッシュ射撃時
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == airdashid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == airshotid)
        {
            // rigidbodyにくっついている慣性が邪魔なので消す（勝手に落下開始する）
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            MoveSpeed = AirDashSpeed;
        }
        // 空中慣性移動時
        else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == jumpingid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == fallid)
        {
            MoveSpeed = AirMoveSpeed;
        }
        // 前後ステップ時
        else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == frontstepid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == backstepid)
        {
            GetComponent<Rigidbody>().useGravity = false;  // 重力無効
            MoveSpeed = StepInitialVelocity;
            MoveDirection.y = 0;         // Y移動無効 
		}
		// 左右ステップ時
		else if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash == leftstepid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == rightstepid)
		{
			// 一定距離動いていたら強制的にステップを終了させる
			if(Vector3.Distance(transform.position, StepStartPos) > 10)
			{
				if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash == leftstepid)
				{
					animator.SetTrigger("LeftStepBack");
				}
				else
				{
					animator.SetTrigger("RightStepBack");
				}
			}
			// MoveDirectionでは動かさない
			MoveDirection = Vector3.zero;
			var now = transform.position - StepFocus;
			var rotateAmount = StepDegree * 0.01f;//	StepInitialVelocity;
			StepDegree -= rotateAmount;

			if(Mathf.Abs(StepDegree) < 0.5f)
			{
				rotateAmount = StepDegree;
				StepDegree = 0;
			}
			var rot = Quaternion.AngleAxis(rotateAmount, Vector3.up) * now;
			// 座標と回転の設定
			transform.position = StepFocus + rot;
			transform.rotation = Quaternion.LookRotation(-now, Vector3.up);
		}
        // 格闘時(ガード含む）
        else if (IsWrestle)
        {
            GetComponent<Rigidbody>().useGravity = false;  // 重力無効            
            MoveSpeed = WrestlSpeed;
        }


        // キリカの時間遅延を受けているとき、1/4に
        if (Timestopmode == TimeStopMode.TIME_DELAY)
        {
            MoveSpeed *= 0.25f;
        }
        // ほむらの時間停止を受けているときなど、0に
        else if (Timestopmode == TimeStopMode.TIME_STOP || Timestopmode == TimeStopMode.PAUSE || Timestopmode == TimeStopMode.AROUSAL)
        {
            MoveSpeed = 0;
            return false;
        }

        if (RigidBody != null)
        {			
			Vector3 velocity = MoveDirection * MoveSpeed;			
            // 走行中/吹き飛び中/ダウン中
            if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == runid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == blowid || animator.GetCurrentAnimatorStateInfo(0).fullPathHash == downid)
            {
                velocity.y = MadokaDefine.FALLSPEED;      // ある程度下方向へのベクトルをかけておかないと、スロープ中に落ちる
            }
			// アイドル時は下方向ベクトル止める
			else if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash == idleid)
			{
				velocity.y = 0;
			}
            RigidBody.velocity = velocity;			
		}
        // HP表示を増減させる(回復は一瞬で、被ダメージは徐々に減る）
        if (NowHitpoint < DrawHitpoint)
        {
            DrawHitpoint -= 2;
        }
        else
        {
            DrawHitpoint = NowHitpoint;
        }

        // 時間停止を解除したら動き出す
        return true;
    }

	/// <summary>
	/// サイドステップ実行
	/// </summary>
	/// <param name="deg">角度</param>
	public void SideStep(float deg)
	{
		// 敵の位置をA,自分の位置をB,移動先の自分の位置をB'とすると
		// 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
		var enemy = MainCamera.GetComponentInChildren<Player_Camera_Controller>().Enemy;
		StepFocus = enemy.transform.position;
		// A→B
		//var now = transform.position - StepFocus;
		// A→B'
		//StepTarget = Quaternion.AngleAxis(deg, Vector3.up) * now;
		StepDegree = deg;

	}

    /// <summary>
    /// 継承先のLateUpdateで実行すること
    /// </summary>
    protected void LateUdate_Core()
    {
        if (this.IsPlayer != CHARACTERCODE.ENEMY)
        {
            // SavingParameterからのステートの変更を受け付ける
            int charactername = (int)CharacterName;
            // レベル
            Level = savingparameter.GetNowLevel(charactername);
            // 攻撃力
            StrLevel = savingparameter.GetStrLevel(charactername);
            // 防御力
            DefLevel = savingparameter.GetDefLevel(charactername);
            // 残弾数
            BulLevel = savingparameter.GetBulLevel(charactername);
            // ブースト量
            BoostLevel = savingparameter.GetBoostLevel(charactername);
            // 覚醒ゲージ
            ArousalLevel = savingparameter.GetArousalLevel(charactername);

            // HP
            NowHitpoint = savingparameter.GetNowHP(charactername);
            // 覚醒ゲージ量
            Arousal = savingparameter.GetNowArousal(charactername);

            
            // SavingParameterに現在のステートを渡す
            // 最大HP
            savingparameter.SetMaxHP(charactername, GetMaxHitpoint(Level));
        }
    }

    /// <summary>
    /// ポーズを解除する
    /// </summary>
    public void ReleasePause()
    {
        if (Timestopmode == TimeStopMode.PAUSE)
        {
            if (HasMenuInput)
            {
                Timestopmode = TimeStopMode.NORMAL;
            }
        }
    }

    /// <summary>
    /// アニメーションの速度を調整する
    /// </summary>
    /// <param name="animator">速度制御対象のanimator</param>
    /// <param name="speed">速度倍率(0～1）</param>
    /// <param name="timestopmode">どのような状態であるか</param>
    protected void ContlroleAnimationSpeed(Animator animator, float speed, TimeStopMode timestopmode)
    {
        if (Timestopmode == TimeStopMode.TIME_DELAY)
        {
            speed *= 0.25f;
        }
        // ほむらの時間停止を受けているときなど、0に
        else if (Timestopmode == TimeStopMode.TIME_STOP || Timestopmode == TimeStopMode.PAUSE || Timestopmode == TimeStopMode.AROUSAL)
        {
            speed = 0;
        }
        animator.speed = speed;
    }

    private const float ITEM_X = 30.0f;
    private const float ITEM_Y = 5.0f;

    private const float EQUIP_ITEM_X = 115.0f;
    private const float EQUIP_ITEM_Y = 1.0f;

    private const float ITEMNUM_X = 370.0f;
    private const float ITEMNUM_Y = 5.0f;

    // ステップ時共通動作
    // 第1引数：ステップの内容
    /// <summary>
    /// ステップ時共通動作
    /// 移動距離をカウントしつつ、既定の移動距離を移動すると戻りモーションを再生する
    /// </summary>
    /// <param name="animator">本体のAnimator</param>
    /// <param name="frontstephash">フロントステップのハッシュID</param>
    /// <param name="frontstepbackID">フロントステップ戻りのID</param>
    /// <param name="leftstephash">左ステップのハッシュID</param>
    /// <param name="leftstepbackID">左ステップ戻りのID</param>
    /// <param name="rightstephash">右ステップのハッシュID</param>
    /// <param name="rightstepbackID">右ステップ戻りのID</param>
    /// <param name="backstephash">バックステップのハッシュID</param>
    /// <param name="backstepbackID">バックステップ戻りのID</param>
    protected void StepMove(Animator animator, int frontstephash,int leftstephash, int rightstephash, int backstephash)
    {
		// ロック時は常時ロック相手の方向を見続ける
		if(IsRockon)
		{
			// 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
			var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
			// 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
			// このため、高低差がないとみなす
			Vector3 Target_VertualPos = target.Enemy.transform.position;
			Target_VertualPos.y = 0;
			Vector3 Mine_VerturalPos = transform.position;
			Mine_VerturalPos.y = 0;
			transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
		}
		


        // 移動距離をインクリメントする（初期位置との差では壁に当たったときに無限に動き続ける）
        SteppingLength += StepMove1F;

        // 地面オブジェクトに触れた場合は着地モーションへ移行(暴走防止のために、上昇中は判定禁止.時間で判定しないと、引っかかったときに無限ループする)
        if (SteppingLength > StepMoveLength)
        {
            if( animator.GetCurrentAnimatorStateInfo(0).fullPathHash == frontstephash)
            {
                animator.SetTrigger("FrontStepBack");
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash  == leftstephash)
            {
                animator.SetTrigger("LeftStepBack");
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash  == rightstephash)
            {
                animator.SetTrigger("RightStepBack");
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash  == backstephash)
            {
                animator.SetTrigger("BackStepBack");
            }
        }
    }

    // ステップ落下時のキャンセルダッシュ及び再ジャンプ受付
    protected bool CancelCheck(Animator animator)
    {
        // ブーストが残っていればキャンセルダッシュは受け付ける（方向入力は受け付けない）
        if (Boost > 0)
        {
            // 方向キーなしで再度ジャンプを押した場合、慣性ジャンプ
            if (!HasVHInput && ControllerManager.Instance.Jump)
            {
                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
                JumpDone(animator);
                return true;
            }
            // ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)            
            else if (ControllerManager.Instance.Jump)
            {
                CancelDashDone(animator);
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 本体を相手の方向へ向ける
    /// </summary>
    protected void RotateToTarget()
    {
        // ロックオン対象の座標を取得
        var target = GetComponentInChildren<Player_Camera_Controller>();
        // 対象の座標を取得
        Vector3 targetpos = target.Enemy.transform.position;
        // 本体の回転角度を拾う
        Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
        // 本体の回転を実行する
        this.transform.rotation = mainrot;
    }

    /// <summary>
    /// 歩き撃ちのアニメーションを戻す
    /// </summary>
    protected virtual void ReturnMotion(Animator animator)
    {
		animator.speed = 1.0f;
        // 歩き撃ちフラグを折る
        RunShotDone = false;
        // 上体を戻す
        BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);

		_StepStartTime = 0;
        // 方向キー入力があった場合かつ地上にいた場合
        if (IsGrounded && HasVHInput)
        {
            animator.SetTrigger("Run");
        }
        // 方向キー入力がなくかつ地上にいた場合
        else if (IsGrounded)
        {		
			animator.SetTrigger("Idle");
        }
        // 空中にいた場合
        else
        {
            animator.SetTrigger("Fall");
        }        
    }


    /// <summary>
    /// 装填開始（通常射撃）
    /// </summary>
    protected virtual void ShotDone(Animator animator,int runhash,int shotrunhash,int shotairdashhash,int airdashhash, int shothash)
    {
        // 一旦空中発射フラグを切る（この辺は他のキャラも考えるべきかもしれない。というかこれはcharacterControl_Baseにおいた方がいいかもしれない
        // 歩き撃ちができない場合もあるから基底にしてオーバーライドが妥当か？

        // 歩行時は上半身のみにして走行（下半身のみ）と合成(ブレンドツリーで合成し、上体を捻らせて銃口補正をかける)
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == runhash)
        {
            animator.SetTrigger("RunShot");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == airdashhash)
        {
            animator.SetTrigger("AirShot");
        }
        else
        {
            animator.SetTrigger("Shot");
        }

        Shotmode = ShotMode.RELORD;        // 装填状態へ移行
    }

    /// <summary>
    /// キャンセル等で本体に弾丸（矢など）があった場合消す
    /// </summary>
    protected virtual void DestroyArrow()
    {
		// 弾があるなら消す(MainShotRootの下に何かあるなら全部消す）
		// メイン射撃など
		int ChildCount = MainShotRoot.transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            Transform child = MainShotRoot.transform.GetChild(i);
            Destroy(child.gameObject);
        }
	}

    /// <summary>
    /// 上記の任意のフック以下のオブジェクトを消す
    /// </summary>
    /// <param name="root"></param>
    protected void DestroyObject(GameObject root)
    {
        int ChildCount = root.transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            Transform child = root.transform.GetChild(i);
            Destroy(child.gameObject);
        }
	}


    /// <summary>
    /// 空中ダッシュ・歩き射撃共通操作
    /// </summary>
    /// <param name="run">歩き射撃であるか否か</param>
    /// <param name="animator">制御対象のanimator</param>
    /// <param name="idlehash">idleのハッシュID</param>
    protected virtual void Moveshot(bool run, Animator animator, int idlehash)
    {
        // ロックオン中かつshotmode=RELOADの時は全体を相手の方へ向ける
        if (IsRockon && Shotmode != ShotMode.NORMAL && Shotmode != ShotMode.SHOTDONE)
        {
            RotateToTarget();
        }
        // 入力中はそちらへ進む
        if (HasVHInput)
        {
            // ロック時に矢を撃つときは本体が相手の方向を向いているため、専用の処理が要る
            if (IsRockon && Shotmode != ShotMode.NORMAL)
            {
                UpdateRotation_step();      // steprotは相手の方向を向いたまま動くので、こっちを使う
                MoveDirection = StepRotation * Vector3.forward;
			}
            else
            {
                UpdateRotation();
                MoveDirection = transform.rotation * Vector3.forward;
			}
        }
        // 入力が外れると、落下する
        else
        {
            if (run)
            {
                animator.SetTrigger("Idle");
            }
            MoveDirection = Vector3.zero;
		}
    }

    // 首もしくは上体ををロックオン対象へ向ける
    // obj              [in]:動かす対象
    // Correction       [in]:Y軸の補正値
    protected void SetNeckRotate(GameObject obj, float Correction)
    {
        // ロックオン対象の座標を取得
        //var target = obj.GetComponentInChildren<Player_Camera_Controller>();
        var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();

        // 対象の座標を取得
        Vector3 targetpos = target.Enemy.transform.position;
        // 動かす対象の座標を取得
        Vector3 Headpos = obj.transform.position;
        Vector3 HeadRot_E = CalcCorrection(Headpos, targetpos, Correction);
        // 回転を実行する
        obj.transform.localRotation = Quaternion.Euler(HeadRot_E);

    }

    /// <summary>
    /// ロックオン対象へ体を向けるときの補正値を計算する
    /// </summary>
    /// <param name="Headpos">本体の座標</param>
    /// <param name="targetpos">ロックオン対象の座標</param>
    /// <param name="Correction">Y軸の補正値</param>
    /// <returns>補正値</returns>
    Vector3 CalcCorrection(Vector3 Headpos, Vector3 targetpos, float Correction)
    {
        Vector3 HeadRot_E = Vector3.zero;

        // 頭部を相手に向けるための角度を出す（クォータニオンでやると範囲指定が難しいのでオイラー角）
        Quaternion HeadRot = Quaternion.LookRotation(targetpos - Headpos);
        // 取得した角度を本体の角度分減算する
        // 本体の回転角度を拾う
        Quaternion mainrot = transform.rotation;

        // 本体の角度分減算する
        HeadRot_E = HeadRot.eulerAngles - mainrot.eulerAngles;


        // リミットをここで実行(Eulerの場合-360－360で来る点に注意）
        // Y
        HeadRot_E.y = CalcCorrictionRetry(HeadRot_E);
        HeadRot_E.x = 0.0f;
        HeadRot_E.z = 0.0f;

        // 補正値を加算する
        HeadRot_E.y += Correction;

        return HeadRot_E;
    }

    // 補正計算を再実行する
    // Headrot              [in]:頭部の計算結果のY軸の角度
    float CalcCorrictionRetry(Vector3 HeadRot)
    {
        // Y
        if (HeadRot.y > 60.0f && HeadRot.y <= 120.0f)
        {
            HeadRot.y = 60.0f;
        }
        else if (HeadRot.y > 240.0f && HeadRot.y < 300.0f)
        {
            HeadRot.y = 300.0f;
        }
        else if (HeadRot.y < -60.0f && HeadRot.y >= -120.0f)
        {
            HeadRot.y = 300.0f;
        }
        else if (HeadRot.y < -240.0f && HeadRot.y > -300.0f)
        {
            HeadRot.y = 60.0f;
        }
        else
        {
            HeadRot.y = 0.0f;
        }

        return HeadRot.y;
    }



    /// <summary>
    /// 飛び越し可能な壁に接触した
    /// </summary>
    private bool _Hitjumpover;
	public bool Gethitjumpover()
	{
		return _Hitjumpover;
	}
    /// <summary>
    /// 飛び越し不能な壁に接触した
    /// </summary>
    private bool _Hitunjumpover;
	public bool Gethitunjumpover()
	{
		return _Hitunjumpover;
	}

    /// <summary>
    /// 接触を判定する
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay(Collider collision)
    {
        // 飛び越し可能な壁に接触した
        if (collision.gameObject.tag == "jumpover")
        {
            _Hitjumpover = true;
            return;
        }
        // 飛び越し不能な壁に接触した
        else if (collision.gameObject.tag == "unjumpover")
        {
            _Hitunjumpover = true;
            return;
        }
        _Hitjumpover = false;
        _Hitunjumpover = false;
    }

    /// <summary>
    /// 本体を強制停止させる
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="idlehash"></param>
    protected void EmagencyStop(Animator animator)
    {
        MoveDirection = Vector3.zero;
        animator.SetTrigger("Idle");
    }

    /// <summary>
    /// 全CharacterControllBase継承オブジェクトの配置位置を固定する
    /// </summary>
    public void FreezePositionAll()
    {
        CharacterControlBase[] Character = FindObjectsOfType(typeof(CharacterControlBase)) as CharacterControlBase[];
        foreach (CharacterControlBase i in Character)
        {
            i.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    /// <summary>
    /// 全CharacterControllBase継承オブジェクトの配置位置固定を解除する
    /// </summary>
    public void UnFreezePositionAll()
    {
        CharacterControlBase[] Character = FindObjectsOfType(typeof(CharacterControlBase)) as CharacterControlBase[];
        foreach (CharacterControlBase i in Character)
        {
            i.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    /// <summary>
    /// 覚醒開始無効処理
    /// </summary>
    private void unuseArousal()
    {
        // TODO:ポーズコントローラーのインスタンスを取得
        //var pausecontroller2 = Pausecontroller.GetComponent<PauseControllerInputDetector>();
        //// 時間を再度動かす
        //pausecontroller2.pauseController.DeactivatePauseProtocol();
    }

    /// <summary>
    /// 覚醒開始処理
    /// </summary>
    protected virtual void arousalStart(int damageid, int blowid)
    {
        if (Pausecontroller == null)
        {
            return;
        }
        // ポーズコントローラーのインスタンスを取得
        PauseControllerInputDetector pausecontroller2 = Pausecontroller.GetComponent<PauseManager>().ArousalPauseController;
        // 覚醒関係以外の時間を止める
        pausecontroller2.ProcessButtonPress();
		// 覚醒前処理
		ArousalInitialize(damageid, blowid);
		// カットインカメラ有効化
		// ArousalCameraを有効化
		ArousalCamera.enabled = true;
		// ポーズ実行
		FreezePositionAll();
		// カットインイベントを有効化
		Arousal_Camera_Controller CutinEvent = ArousalCamera.GetComponentInChildren<Arousal_Camera_Controller>();
		// カットインイベント発動
		CutinEvent.UseAsousalCutinCamera();
		// 時間停止
		TimeStopMaster = true;
		Timestopmode = TimeStopMode.AROUSAL;
	}

    // 覚醒開始時の共通処理
    protected virtual void ArousalInitialize(int damageid, int blowid)
    {            
        // ブースト全回復
        Boost = GetMaxBoost(BoostLevel);
        // 覚醒モード移行
        IsArousal = true;
        // エフェクト出現（エフェクトはCharacerControl_Baseのpublicに入れておく）
        ArousalEffect.SetActive(true);
      
		// ダメージ時はリバーサルにする
		if (AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == damageid || AnimatorUnit.GetCurrentAnimatorStateInfo(0).fullPathHash == blowid)
		{
			AnimatorUnit.SetTrigger("Reversal");
		}
		// インターフェースを一時的に消す
		GameObject.Find("BattleInterfaceCanvas").GetComponent<BattleInterfaceController>().UnDrawInterface();
	}

    /// <summary>
    /// 格闘開始（一応派生は認めておく。専用のはそっちで利用）
    /// </summary>
    /// <param name="animator">格闘攻撃の種類</param>
    /// <param name="skilltype">スキルのインデックス(キャラごとに異なる)</param>
    /// <param name="triggername">使用する格闘のトリガーの名前</param>
    protected virtual void WrestleDone(Animator animator, int skilltype,string triggername)
    {
        // 追加入力フラグをカット
        AddInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の方向を移動方向とする
        if (IsRockon && transform.rotation.eulerAngles.y != 0)
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
            MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
		}
        // アニメーション速度
        float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;

        // アニメーションを再生する
        animator.SetTrigger(triggername);
               
        // アニメーションの速度を調整する
        animator.speed = speed;
        // 移動速度を調整する
        WrestlSpeed = movespeed;
    }

	
    /// <summary>
    /// 回り込み近接・左(相手の斜め前へ移動して回り込むタイプ）
    /// </summary>
    /// <param name="animator">格闘攻撃の種類</param>
    /// <param name="skilltype">スキルのインデックス(キャラごとに異なる)</param>
    /// <param name="wrestlehash">使用する格闘のハッシュID</param>
    protected virtual void WrestleDone_GoAround_Left(Animator animator, int skilltype)
    {
        // 追加入力フラグをカット
        this.AddInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の左側を移動方向とする（通常時のロックオン時左移動をさせつつ前進させる）
        if (IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 自機をロックオン対象の左側に向ける(上記の角度から45度ずらす）
            Vector3 addrot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 10, transform.rotation.eulerAngles.z);
            // クォータニオンに変換
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            // 方向ベクトルを向けた方向に合わせる            
            MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
		}
        // 本体角度が0の場合カメラの方向に45度足した値をを移動方向とし、正規化して代入する
        else if (this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y = rotateOR.eulerAngles.y - 10;
            rotateOR = Quaternion.Euler(rotateOR_E);
            MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
		}
        // それ以外は本体の角度+45度を移動方向にする
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y - 10;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
		}
        // アニメーション速度
        float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;
        // アニメーションを再生する
        animator.SetTrigger("LeftWrestle");

        // アニメーションの速度を調整する
        animator.speed = speed;
        // 移動速度を調整する
        WrestlSpeed = movespeed;
    }


    /// <summary>
    /// 回り込み近接・右(相手の斜め前へ移動して回り込むタイプ）
    /// </summary>
    /// <param name="animator">制御対象のanimator</param>
    /// <param name="skilltype">スキルのインデックス/キャラごとに異なる</param>
    /// <param name="wrestlehash">使用する格闘のハッシュID</param>
    protected virtual void WrestleDone_GoAround_Right(Animator animator, int skilltype)
    {
        // 追加入力フラグをカット
        AddInput = false;
        // ステートを設定する
        // skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
        int skillIndex = skilltype;
        // 移動速度
        float movespeed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Movespeed;
        // 移動方向
        // ロックオン且つ本体角度が0でない時、相手の左側を移動方向とする（通常時のロックオン時左移動をさせつつ前進させる）
        if (IsRockon && transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 自機をロックオン対象の左側に向ける(上記の角度から45度ずらす）
            Vector3 addrot = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 10, transform.rotation.eulerAngles.z);
            // クォータニオンに変換
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            // 方向ベクトルを向けた方向に合わせる            
            MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
		}
        // 本体角度が0の場合カメラの方向に45度足した値をを移動方向とし、正規化して代入する
        else if (transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y = rotateOR.eulerAngles.y + 10;
            rotateOR = Quaternion.Euler(rotateOR_E);
            MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
		}
        // それ以外は本体の角度+45度を移動方向にする
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 10;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
		}
        // アニメーション速度
        float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;
        // アニメーションを再生する
        animator.SetTrigger("RightWrestle");

        // アニメーションの速度を調整する
        animator.speed = speed;
        // 移動速度を調整する
        this.WrestlSpeed = movespeed;
    }

    /// <summary>
    /// 後格闘（防御）
    /// </summary>
    /// <param name="animator">制御対象のanimator</param>
    /// <param name="skilltype">スキルのインデックス/キャラごとに異なる</param>
    /// <param name="wrestlehash">使用する格闘のハッシュID</param>
    protected virtual void GuardDone(Animator animator, int skilltype)
    {
        //1．追加入力フラグをカット
        this.AddInput = false;
        int skillIndex = skilltype;
        //2．ロックオンしている場合→自機をロックオン対象に向ける
        // ロックオン且つ本体角度が0でない時
        if (IsRockon && this.transform.rotation.eulerAngles.y != 0)
        {
            // ロックオン対象を取得
            var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // ロックオン対象の座標
            Vector3 targetpos = target.transform.position;
            // 自機の座標
            Vector3 mypos = transform.position;
            // 自機をロックオン対象に向ける
            transform.rotation = Quaternion.LookRotation(mypos - targetpos);
            // 方向ベクトルを向けた方向に合わせる            
            MoveDirection = Vector3.Normalize(transform.rotation * Vector3.forward);
		}
        // 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
        else if (IsRockon && this.transform.rotation.eulerAngles.y == 0)
        {
            // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
            Quaternion rotateOR = MainCamera.transform.rotation;
            Vector3 rotateOR_E = rotateOR.eulerAngles;
            rotateOR_E.x = 0;
            rotateOR_E.y += 180;
            rotateOR = Quaternion.Euler(rotateOR_E);
            MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
		}
        //   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
        else
        {
            Vector3 addrot = this.transform.eulerAngles;
            addrot.y = addrot.y + 180;
            Quaternion addrot_Q = Quaternion.Euler(addrot);
            MoveDirection = Vector3.Normalize(addrot_Q * Vector3.forward);
		}
        //3．アニメーション速度を設定する
        float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;
        // アニメーションを再生する
        animator.SetTrigger("BackWrestle");

        // アニメーションの速度を調整する
        animator.speed = speed;
        //7．移動速度を0にする
        WrestlSpeed = 0;
    }

	/// <summary>
	/// 前特殊格闘
	/// </summary>
	/// <param name="animator">制御対象のanimator</param>
	/// <param name="skilltype">スキルのインデックス/キャラごとに異なる</param>
	protected virtual void WrestleDone_UpperEx(Animator animator, int skilltype)
	{
		// 追加入力フラグをカット
		this.AddInput = false;
		// ステートを変更		
		int skillIndex = skilltype;
		// 移動速度取得
		float movespeed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Movespeed;
		// ロックオン中なら移動方向をロックオン対象のほうへ固定する
		if (IsRockon && this.transform.rotation.eulerAngles.y != 0)
		{
			// ロックオン対象を取得
			var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
			// ロックオン対象の座標
			Vector3 targetpos = target.transform.position;
			// 自機の座標
			Vector3 mypos = transform.position;
			// 自機をロックオン対象に向ける
			transform.rotation = Quaternion.LookRotation(mypos - targetpos);
			// 方向ベクトルを上向きにする            
			MoveDirection = new Vector3(0, 1, 0);
		}
		// 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
		else if (this.transform.rotation.eulerAngles.y == 0)
		{
			// 方向ベクトルを上向きにする            
			MoveDirection = new Vector3(0, 1, 0);
		}
		//   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
		else
		{
			Vector3 addrot = this.transform.eulerAngles;
			addrot.y = addrot.y + 180;
			// 方向ベクトルを上向きにする            
			MoveDirection = new Vector3(0, 1, 0);
		}
		// アニメーション速度を設定する
		float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;
		// アニメーションを変更する
        animator.SetTrigger("FrontEXWrestle");
		// アニメーションの速度を調整する
		animator.speed = speed;
		// 移動速度を上にする
		WrestlSpeed = movespeed;
	}

	/// <summary>
	/// 後特殊格闘
	/// </summary>
	/// <param name="animator">制御対象のanimator</param>
	/// <param name="skilltype">スキルのインデックス/キャラごとに異なる</param>
	/// <param name="wrestlehash">使用する格闘のハッシュID</param>
	protected virtual void WrestleDone_DownEx(Animator animator, int skilltype, int wrestlehash)
	{
		// 追加入力フラグをカット
		this.AddInput = false;
		// ステートを変更		
		int skillIndex = skilltype;
		// 移動速度取得
		float movespeed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Movespeed;
		// ロックオン中なら移動方向をロックオン対象のほうへ固定する
		if (IsRockon && this.transform.rotation.eulerAngles.y != 0)
		{
			// ロックオン対象を取得
			var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
			// ロックオン対象の座標
			Vector3 targetpos = target.transform.position;
			// 自機の座標
			Vector3 mypos = transform.position;
			// 自機をロックオン対象に向ける
			transform.rotation = Quaternion.LookRotation(mypos - targetpos);
			// 方向ベクトルを下向きにする            
			MoveDirection = new Vector3(0, -1, 0);
		}
		// 本体角度が0の場合カメラの方向を移動方向とし、正規化して代入する
		else if (this.transform.rotation.eulerAngles.y == 0)
		{
			// 方向ベクトルを下向きにする            
			MoveDirection = new Vector3(0, -1, 0);
		}
		//   ロックオンしていない場合→本体を前方方向へ向ける（現在の自分の角度がカメラ側を向いているので、180度加算してひっくり返す）
		else
		{
			Vector3 addrot = this.transform.eulerAngles;
			addrot.y = addrot.y + 180;
			this.transform.rotation = Quaternion.Euler(new Vector3(addrot.x, addrot.y, addrot.z));
			// 方向ベクトルを下向きにする            
			MoveDirection = new Vector3(0, -1, 0);
		}
		// アニメーション速度を設定する
		float speed = Character_Spec.cs[(int)CharacterName][skillIndex].m_Animspeed;
		// アニメーションを変更する
        animator.SetTrigger("BackEXWrestle");
		// アニメーションの速度を調整する
		animator.speed = speed;
		// 移動速度を設定する
		WrestlSpeed = movespeed;
	}

	/// <summary>
	/// 空中ダッシュ格闘を実行する
	/// </summary>
	/// <param name="Animator"></param>
	/// <param name="WresltleSpeed">突進速度（一応空中ダッシュの速度と一致させる）</param>
	/// <param name="skillindex"></param>
	public virtual void AirDashWrestleDone(Animator Animator,float WresltleSpeed,int skillindex)
	{
		// 追加入力フラグをカット
		AddInput = false;
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
		Animator.SetTrigger("BDWrestle");

		// アニメーションの速度を調整する
		Animator.speed = speed;
		// 移動速度を調整する
		WrestlSpeed = WresltleSpeed;
	}

	

    /// <summary>
    /// 格闘判定出現時に実行（一応派生は認めておく。専用のはそっちで利用。弓ほむら・魔獣以外使用禁止。理由は引数にWrestleTypeがとれなくなったため）
    /// </summary>
    /// <param name="wrestletype">格闘攻撃の種類</param>
    protected virtual void WrestleStart(WrestleType wrestletype)
	{
		// 判定を生成し・フックと一致させる  
		Vector3 pos = WrestleRoot[(int)wrestletype].transform.position;
		Quaternion rot = WrestleRoot[(int)wrestletype].transform.rotation;
		var obj = (GameObject)Instantiate(WrestleObject[(int)wrestletype], pos, rot);
		// 親子関係を再設定する(=判定をフックの子にする）
		if (obj.transform.parent == null)
		{
			obj.transform.parent = WrestleRoot[(int)wrestletype].transform;
			// 親子関係を付けておく
			obj.transform.GetComponent<Rigidbody>().isKinematic = true;
		}

		// ステートを設定する
		// skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
		int skillIndex = 0;/*(int)CharacterSkill.SkillType.WRESTLE_1 + (int)wrestletype;*/

		// キャラごとに構成が異なるので、ここで処理分岐(入力が格闘でありながら、動作が格闘でない技を持つキャラが多くいる）
		switch (CharacterName)
		{
			case Character_Spec.CHARACTER_NAME.MEMBER_MADOKA:
				if (wrestletype == WrestleType.WRESTLE_1)
					skillIndex = 4;
				else if (wrestletype == WrestleType.WRESTLE_2)
					skillIndex = 5;
				else if (wrestletype == WrestleType.WRESTLE_3)
					skillIndex = 6;
				else if (wrestletype == WrestleType.FRONT_WRESTLE_1)
					skillIndex = 8;
				else if (wrestletype == WrestleType.LEFT_WRESTLE_1)
					skillIndex = 9;
				else if (wrestletype == WrestleType.RIGHT_WRESTLE_1)
					skillIndex = 10;
				else if (wrestletype == WrestleType.BACK_WRESTLE)
					skillIndex = 11;
				else if (wrestletype == WrestleType.AIRDASH_WRESTLE)
					skillIndex = 12;
				else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1)
					skillIndex = 14;
				else if (wrestletype == WrestleType.BACK_EX_WRESTLE)
					skillIndex = 15;
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_MAMI:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B:
				if (wrestletype == WrestleType.WRESTLE_1)
					skillIndex = 4;
				else if (wrestletype == WrestleType.WRESTLE_2)
					skillIndex = 5;
				else if (wrestletype == WrestleType.WRESTLE_3)
					skillIndex = 6;
				else if (wrestletype == WrestleType.FRONT_WRESTLE_1)
					skillIndex = 7;
				else if (wrestletype == WrestleType.LEFT_WRESTLE_1)
					skillIndex = 8;
				else if (wrestletype == WrestleType.RIGHT_WRESTLE_1)
					skillIndex = 9;
				else if (wrestletype == WrestleType.BACK_WRESTLE)
					skillIndex = 10;
				else if (wrestletype == WrestleType.AIRDASH_WRESTLE)
					skillIndex = 11;
				else if (wrestletype == WrestleType.EX_WRESTLE_1)
					skillIndex = 12;
				else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1)
					skillIndex = 13;
				else if (wrestletype == WrestleType.BACK_EX_WRESTLE)
					skillIndex = 14;
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_KYOKO:

				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_YUMA:
				if (wrestletype == WrestleType.WRESTLE_1)
					skillIndex = 3;
				else if (wrestletype == WrestleType.WRESTLE_2)
					skillIndex = 4;
				else if (wrestletype == WrestleType.WRESTLE_3)
					skillIndex = 5;
				else if (wrestletype == WrestleType.FRONT_WRESTLE_1)
					skillIndex = 6;
				else if (wrestletype == WrestleType.LEFT_WRESTLE_1)
					skillIndex = 7;
				else if (wrestletype == WrestleType.RIGHT_WRESTLE_1)
					skillIndex = 8;
				else if (wrestletype == WrestleType.BACK_WRESTLE)
					skillIndex = 9;
				else if (wrestletype == WrestleType.AIRDASH_WRESTLE)
					skillIndex = 10;
				else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1)
					skillIndex = 12;
				else if (wrestletype == WrestleType.BACK_EX_WRESTLE)
					skillIndex = 13;
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_ORIKO:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_SCHONO:
				if (wrestletype == WrestleType.WRESTLE_1)
					skillIndex = 3;
				else if (wrestletype == WrestleType.WRESTLE_2)
					skillIndex = 4;
				else if (wrestletype == WrestleType.WRESTLE_3)
					skillIndex = 5;
				else if (wrestletype == WrestleType.FRONT_WRESTLE_1)
					skillIndex = 6;
				else if (wrestletype == WrestleType.LEFT_WRESTLE_1)
					skillIndex = 7;
				else if (wrestletype == WrestleType.RIGHT_WRESTLE_1)
					skillIndex = 8;
				else if (wrestletype == WrestleType.BACK_WRESTLE)
					skillIndex = 9;
				else if (wrestletype == WrestleType.AIRDASH_WRESTLE)
					skillIndex = 10;
				else if (wrestletype == WrestleType.EX_WRESTLE_1)
					skillIndex = 11;
				else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1)
					skillIndex = 12;
				else if (wrestletype == WrestleType.BACK_EX_WRESTLE)
					skillIndex = 13;
				break;
			case Character_Spec.CHARACTER_NAME.ENEMY_MAJYU:
				if (wrestletype == WrestleType.WRESTLE_1)
					skillIndex = 1;
				else if (wrestletype == WrestleType.WRESTLE_2)
					skillIndex = 2;
				else if (wrestletype == WrestleType.WRESTLE_3)
					skillIndex = 3;
				else if (wrestletype == WrestleType.FRONT_WRESTLE_1)
					skillIndex = 4;
				else if (wrestletype == WrestleType.BACK_WRESTLE)
					skillIndex = 5;
				else if (wrestletype == WrestleType.AIRDASH_WRESTLE)
					skillIndex = 6;
				else if (wrestletype == WrestleType.EX_FRONT_WRESTLE_1)
					skillIndex = 7;
				else if (wrestletype == WrestleType.BACK_EX_WRESTLE)
					skillIndex = 8;
				break;
		}



		// 格闘判定を拾う
		var wrestleCollision = GetComponentInChildren<Wrestle_Core>();

		// 各ステートを計算する
		// 攻撃力
		int offensive = Character_Spec.cs[(int)CharacterName][skillIndex].m_OriginalStr + Character_Spec.cs[(int)CharacterName][skillIndex].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ダウン値
		float downR = Character_Spec.cs[(int)CharacterName][skillIndex].m_DownPoint;
		// 覚醒ゲージ増加量
		float arousal = Character_Spec.cs[(int)CharacterName][skillIndex].m_arousal + Character_Spec.cs[(int)CharacterName][skillIndex].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ヒットタイプ
		CharacterSkill.HitType hittype = Character_Spec.cs[(int)CharacterName][skillIndex].m_Hittype;
		// 除外対象の名前
		string exclusionname = ObjectName.CharacterFileName[(int)CharacterName];

		// 打ち上げ量（とりあえず固定）

		// 格闘時に加算する力（固定）

		// 判定のセッティングを行う
		wrestleCollision.SetStatus(offensive, downR, arousal, hittype, exclusionname);	
	}


	/// <summary>
	/// 上記のint指定版(今後はこっちを使うこと）
	/// </summary>
	/// <param name="index">対象の格闘攻撃のインデックス</param>
	protected virtual void WrestleStart2(int index)
	{
		// 判定を生成し・フックと一致させる  
		Vector3 pos = WrestleRoot[index].transform.position;
		Quaternion rot = WrestleRoot[index].transform.rotation;
		var obj = Instantiate(WrestleObject[index], pos, rot);
		// 親子関係を再設定する(=判定をフックの子にする）
		if (obj.transform.parent == null)
		{
			obj.transform.parent = WrestleRoot[index].transform;
			// 親子関係を付けておく
			obj.transform.GetComponent<Rigidbody>().isKinematic = true;
		}

		// ステートを設定する
		// skilltypeのインデックス(格闘系はSkillType.Wrestle1+Xなので、Xにwrestletypeを代入）
		int skillIndex = 0;/*(int)CharacterSkill.SkillType.WRESTLE_1 + (int)wrestletype;*/

		// キャラごとに構成が異なるので、ここで処理分岐(入力が格闘でありながら、動作が格闘でない技を持つキャラが多くいる）
		switch (CharacterName)
		{
			case Character_Spec.CHARACTER_NAME.MEMBER_MADOKA:
				{
					if (index == 0)
						skillIndex = 4;
					else if (index == 1)
						skillIndex = 5;
					else if (index == 2)
						skillIndex = 6;
					else if (index == 3)
						skillIndex = 8;
					else if (index == 4)
						skillIndex = 9;
					else if (index == 5)
						skillIndex = 10;
					else if (index == 6)
						skillIndex = 11;
					else if (index == 7)
						skillIndex = 12;
					else if (index == 8)
						skillIndex = 14;
					else if (index == 9)
						skillIndex = 15;
				}
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_SAYAKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_MAMI:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B:
				{
					if (index == 0)
						skillIndex = 4;
					else if (index == 1)
						skillIndex = 5;
					else if (index == 2)
						skillIndex = 6;
					else if (index == 3)
						skillIndex = 8;
					else if (index == 4)
						skillIndex = 9;
					else if (index == 5)
						skillIndex = 10;
					else if (index == 6)
						skillIndex = 11;
					else if (index == 7)
						skillIndex = 12;
					else if (index == 8)
						skillIndex = 13;
				}
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_KYOKO:

				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_YUMA:
				{
					if (index == 0)
						skillIndex = 3;
					else if (index == 1)
						skillIndex = 4;
					else if (index == 2)
						skillIndex = 5;
					else if (index == 3)
						skillIndex = 6;
					else if (index == 4)
						skillIndex = 7;
					else if (index == 5)
						skillIndex = 8;
					else if (index == 6)
						skillIndex = 9;
					else if (index == 7)
						skillIndex = 10;
					else if (index == 8)
						skillIndex = 11;
				}
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_ORIKO:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_KIRIKA:
				break;
			case Character_Spec.CHARACTER_NAME.MEMBER_SCHONO:
				{
                    if (index == 0)
                        skillIndex = 3;
                    else if (index == 1)
                        skillIndex = 4;
                    else if (index == 2)
                        skillIndex = 5;
                    else if (index == 3)
                        skillIndex = 6;
                    else if (index == 4)
                        skillIndex = 7;
                    else if (index == 5)
                        skillIndex = 8;
                    else if (index == 6)
                        skillIndex = 9;
                    else if (index == 7)
                        skillIndex = 11;
                    else if (index == 8)
                        skillIndex = 12;
                    else if (index == 9)
                        skillIndex = 13;
                    else if (index == 10)
                        skillIndex = 10;
				}
				break;
			case Character_Spec.CHARACTER_NAME.ENEMY_MAJYU:
				{
					if (index == 0)
						skillIndex = 1;
					else if (index == 1)
						skillIndex = 2;
					else if (index == 2)
						skillIndex = 3;
					else if (index == 3)
						skillIndex = 4;
					else if (index == 4)
						skillIndex = 5;
					else if (index == 5)
						skillIndex = 6;
					else if (index == 6)
						skillIndex = 7;
					else if (index == 7)
						skillIndex = 8;
				}
				break;
		}



		// 格闘判定を拾う
		var wrestleCollision = GetComponentInChildren<Wrestle_Core>();

		// 各ステートを計算する
		// 攻撃力
		int offensive = Character_Spec.cs[(int)CharacterName][skillIndex].m_OriginalStr + Character_Spec.cs[(int)CharacterName][skillIndex].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ダウン値
		float downR = Character_Spec.cs[(int)CharacterName][skillIndex].m_DownPoint;
		// 覚醒ゲージ増加量
		float arousal = Character_Spec.cs[(int)CharacterName][skillIndex].m_arousal + Character_Spec.cs[(int)CharacterName][skillIndex].m_GrowthCoefficientStr * (this.StrLevel - 1);
		// ヒットタイプ
		CharacterSkill.HitType hittype = Character_Spec.cs[(int)CharacterName][skillIndex].m_Hittype;
		// 除外対象の名前
		string exclusionname = ObjectName.CharacterFileName[(int)CharacterName];

		// 打ち上げ量（とりあえず固定）

		// 格闘時に加算する力（固定）

		// 判定のセッティングを行う
		wrestleCollision.SetStatus(offensive, downR, arousal, hittype, exclusionname);
	}
       

	// 後格闘（防御）判定出現時に実行
	protected virtual void GuardStart()
	{
		// 判定を生成し・フックと一致させる  
		Vector3 pos = WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform.position;
		Quaternion rot = WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform.rotation;
		var obj = (GameObject)Instantiate(WrestleObject[(int)WrestleType.BACK_WRESTLE], pos, rot);
		// 親子関係を再設定する(=判定をフックの子にする）
		if (obj.transform.parent == null)
		{
			obj.transform.parent = WrestleRoot[(int)WrestleType.BACK_WRESTLE].transform;
			// 親子関係を付けておく
			obj.transform.GetComponent<Rigidbody>().isKinematic = true;
		}

	}

	/// <summary>
	/// 上記のインデックス指定版
	/// </summary>
	/// <param name="index">後格闘のスキルインデックス</param>
	protected virtual void GuardStart2(int index)
	{
		// 判定を生成し・フックと一致させる  
		Vector3 pos = WrestleRoot[index].transform.position;
		Quaternion rot = WrestleRoot[index].transform.rotation;
		var obj = Instantiate(WrestleObject[index], pos, rot);
		// 親子関係を再設定する(=判定をフックの子にする）
		if (obj.transform.parent == null)
		{
			obj.transform.parent = WrestleRoot[index].transform;
			// 親子関係を付けておく
			obj.transform.GetComponent<Rigidbody>().isKinematic = true;
		}

	}


	/// <summary>
	/// 被弾時HPを減少させる。SendMessageで弾丸などから呼ばれる
	/// </summary>
	/// <param name="arr">攻撃したキャラクター/与えるダメージ量</param>
	public void DamageHP(int[] arr)
	{
		int attackedcharacter = arr[0];
		int damage = arr[1];
		DamageHP(attackedcharacter, damage);
	}

	/// <summary>
	/// 上記の引数2個指定バージョン
	/// </summary>
	/// <param name="attackedcharacter"></param>
	/// <param name="damage"></param>
	public void DamageHP(int attackedcharacter, int damage)
	{
		// 防御力分ダメージを減衰する
		damage -= DefLevel * DamageLess;
		if (damage <= 0)
		{
			damage = 1;
		}
		NowHitpoint -= damage;
		// 死んだ場合、止めを刺したキャラに経験値が加算されるようにする（味方殺しでは経験値は増えない。また、まどかとアルティメットまどか、弓ほむらと銃ほむらは経験値とLVを共有する）
		if (IsPlayer == CHARACTERCODE.ENEMY && NowHitpoint < 1)
		{
			// 与える経験値の総量
			int addexp = Character_Spec.Exp[(int)CharacterName];
			if (savingparameter.GetNowHP(attackedcharacter) > 0)
			{
				savingparameter.AddExp(attackedcharacter, addexp);
				// 銃ほむらの場合、弓ほむらにも加算する
				if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA)
				{
					savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B, addexp);
				}
				// 弓ほむらの場合、銃ほむらにも加算する
				else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B)
				{
					savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA, addexp);
				}
				// まどかの場合、アルティメットまどかにも加算する
				else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA)
				{
					savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA, addexp);
				}
				// アルティメットまどかの場合、まどかにも加算する
				else if (attackedcharacter == (int)Character_Spec.CHARACTER_NAME.MEMBER_UL_MADOKA)
				{
					savingparameter.AddExp((int)Character_Spec.CHARACTER_NAME.MEMBER_MADOKA, addexp);
				}
			}
		}

		// PCにヒットさせた場合、savingparameterの値も変える
		if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
		{
			int charactername = (int)this.CharacterName;
			savingparameter.SetNowHP(charactername, NowHitpoint);
		}

		
	}
		

	/// <summary>
	/// 被弾側の覚醒ゲージを増加させる。SendMessageで弾丸などから呼ばれる
	/// </summary>
	/// <param name="arousal"></param>
	public void DamageArousal(float arousal)
	{
		Arousal += arousal;
		// PCにヒットさせた場合、savingparameterの値も変える
		if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
		{
			int charactername = (int)this.CharacterName;
			savingparameter.SetNowArousal(charactername, Arousal);
		}
	}

    /// <summary>
    ///  被弾時ダウン値を加算させる
    /// </summary>
    /// <param name="downratio"></param>
    public void DownRateInc(float downratio)
	{
		NowDownRatio += downratio;
	}

	// エフェクト破壊関数
	protected void BrokenEffect()
	{
		// ステップ
		if (gameObject.transform.Find("StepEffect(Clone)") != null)
		{
			Destroy(gameObject.transform.Find("StepEffect(Clone)").gameObject);
		}
		// 格闘ステップキャンセル
		if (gameObject.transform.Find("StepEffectCancel(Clone)") != null)
		{
			Destroy(gameObject.transform.Find("StepEffectCancel(Clone)").gameObject);
		}
	}

    /// <summary>
    /// 被弾時ステートを変える
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="damageID"></param>
    /// <param name="blowinithash"></param>
    /// <param name="blowID"></param>
    /// <param name="spindownID"></param>

    public virtual void DamageInit(Animator animator, int damageID, bool isBlow,int blowID,int spindownID)
	{
		// ステップ累積時間を戻す
		_StepStartTime = 0;
		// くっついているエフェクトを消す
		BrokenEffect();
		// くっついている格闘判定を消す
		DestroyWrestle();
		
		// 継承先で本体にくっついていたオブジェクトをカット
		// m_DownRebirthTimeのカウントを開始
		// 入力をポーズ以外すべて禁止
		// ダメージアニメーションを再生
		// 動作及び慣性をカット
		// 飛び越えフラグをカット
		// のけぞりならDamageInit→DamageDoneを呼ぶ
		// 吹き飛びならDamageInit→BlowDoneを呼ぶ

		// m_DownRebirthTimeのカウントを開始
		DownRebirthTime = Time.time;

		// （継承先で本体にくっついていたオブジェクトをカット）
		// （UpdateCoreで入力をポーズ以外すべて禁止）
		// ダメージアニメーションを再生
		
        animator.SetTrigger("Damage");

		// 動作及び慣性をカット
		MoveDirection = Vector3.zero;
		// 飛び越えフラグをカット	
		Rotatehold = false;

		// 固定状態をカット
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;



		// 死亡時は強制吹き飛び属性
		if (NowHitpoint < 1)
		{
			// 属性がEnemyなら爆発エフェクトを貼り付ける
			if (IsPlayer == CHARACTERCODE.ENEMY)
			{
				// エフェクトをロードする
				Object Explosion = Resources.Load("Explosion_death");
				// 現在の自分の位置にエフェクトを置く
				var obj = (GameObject)Instantiate(Explosion, transform.position, transform.rotation);
				// 親子関係を再設定する
				obj.transform.parent = this.transform;
				// 死亡爆発が起こったというフラグを立てる
				Explode = true;
			}
			NowDownRatio = 5;
		}


		// 吹き飛び
		if (isBlow)
		{
			BlowDone(animator,spindownID,blowID);
		}
		// のけぞりならDamageInit→DamageDoneを呼ぶ
		else
		{
			DamageDone(animator,damageID);
		}

	}

    /// <summary>
    /// のけぞりダメージ時、ダメージの処理を行う
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="damageID"></param>
    public virtual void DamageDone(Animator animator, int damageID)
	{
		// 重力をカット
		// ダメージ硬直の計算開始
		// ステートをDamageに切り替える
		animator.speed = 1.0f;
		// 重力をカット
		GetComponent<Rigidbody>().useGravity = false;
		// ダメージ硬直の計算開始
		DamagedTime = Time.time;
		// ガードフラグを折る
		IsGuard = false;
    }

    /// <summary>
    /// 吹き飛びダメージ時、ダメージの処理を行う
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="spindownID"></param>
    /// <param name="blowID"></param>
    public virtual void BlowDone(Animator animator,int spindownID,int blowID)
	{
		// Rotateの固定を解除        
		// 重力を復活
		// ステートをBlowへ切り替える
		// ダウン値がMAXならステートをDownに切り替える
		// ダウンアニメを再生する
		// m_launchOffsetだけ浮かし、攻撃と同じベクトルを与える。ここの値はm_BlowDirectionに保存したものを使う


		// 重力を復活
		GetComponent<Rigidbody>().useGravity = true;
		// 固定していた場合、固定解除
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

		// 錐揉みダウン（ダウン値MAX）なら錐揉みダウンアニメを再生し、ステートをSpinDownへ切り替える
		if (NowDownRatio >= DownRatioBias)
		{
			// 錐揉みダウンアニメを再生する
			animator.SetTrigger("Spindown");
			// 無敵をONにする
			Invincible = true;
		}
		// そうでないならステートをBlowに切り替える
		else
		{
			// Rotateの固定を解除        
			GetComponent<Rigidbody>().freezeRotation = false;
            // ダウンアニメを再生する
            animator.SetTrigger("Blow");
		}
		// 攻撃と同じベクトルを与える。ここの値はm_BlowDirectionに保存したものを使う
		// TODO:Velocityで飛ばしてるのでMoveDirectionに変えてそれで飛ばす
		RigidBody.AddForce(BlowDirection.x*10, 10, BlowDirection.z*10);
	}

    /// <summary>
    /// ダメージ(のけぞり）
    /// </summary>
    /// <param name="animator"></param>
    public virtual void Damage(Animator animator)
	{
		// 移行後復活 
		// ダメージ硬直終了
		if (Time.time > DamagedTime + DamagedWaitTime)
		{
			// 固定があった場合解除
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			// 空中にいた→ダウンアニメを再生する→Blowへ移行（飛ばされない）
			if (!IsGrounded)
			{
				// Rotateの固定を解除        
				RigidBody.freezeRotation = false;
                // ダウンアニメを再生する
                animator.SetTrigger("Blow");
				// 重力を復活
				GetComponent<Rigidbody>().useGravity = true;				
			}
			// 地上にいた→Idleへ移行し、Rotateをすべて0にして固定する
			else
			{
				RigidBody.useGravity = true;
				_StepStartTime = 0;
                animator.SetTrigger("Idle");
			}
		}
	}

    /// <summary>
    /// 吹き飛び
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void Blow(Animator animator)
	{
		// 接地までなにもせず、接地したらDownへ移行し、m_DownTimeを計算する
		// ブースト入力があった場合、ダウン値がMAX未満でブーストゲージが一定量あれば、Reversalへ変更	
		// ブースト量を減らす
		// rotationを0にして復帰アニメを再生する
		// 再固定する
		// ステートを復帰にする

		// 接地までなにもせず、接地したらDownへ移行し、m_DownTimeを計算する
		if (IsGrounded)
		{
            animator.SetTrigger("Down");
			// downへ移行
			DownTime = Time.time;
			// 速度を0まで落とす（吹き飛び時のベクトルを消す）
			MoveDirection = Vector3.zero;
			// 回転を戻す
			// rotationの固定を復活させ、0,0,0にする
			GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
		}
		// ブースト入力があった場合、ダウン値がMAX未満でブーストゲージが一定量あれば、Reversalへ変更	
		// rotationを0にして復帰アニメを再生する
		else if (NowDownRatio <= DownRatioBias &&  HasJumpInput && Boost >= ReversalUseBoost)
		{
			// ブースト量を減らす
			Boost -= ReversalUseBoost;
			// 復帰処理を行う
			ReversalInit(animator);
		}
	}

    /// <summary>
    /// 錐揉みダウン
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void SpinDown(Animator animator)
	{
		// 落下に入ったら落下速度を調整する
		MoveDirection.y = MadokaDefine.FALLSPEED;
		// 基本Blowと同じだが、着地と同時にアニメをダウンに切り替える
		if (IsGrounded)
		{
			// downへ移行
            animator.SetTrigger("Down");
			this.DownTime = Time.time;
			// 速度を0まで落とす（吹き飛び時のベクトルを消す）
			this.MoveDirection = Vector3.zero;
			// 回転を戻す
			// rotationの固定を復活させ、0,0,0にする
			this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
		}
	}



    /// <summary>
    /// ダウン
    /// </summary>
    /// <param name="animator"></param>
    protected virtual void Down(Animator animator)
	{
		// rotationの固定を復活させ、0,0,0にする
		this.GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
		this.GetComponent<Rigidbody>().freezeRotation = true;

		// m_DownTimeが規定値を超えると、復帰アニメを再生する
		if (Time.time > DownTime + DownWaitTime)
		{
			// ただし自機側ではHP0だと復活させない
			if (IsPlayer == CHARACTERCODE.PLAYER || IsPlayer == CHARACTERCODE.PLAYER_ALLY)
			{
				if (NowHitpoint < 1)
				{
					return;
				}
			}
			// 生きていれば起き上がる
			if (NowHitpoint > 1)
			{
				ReversalInit(animator);
			}
		}
	}

    /// <summary>
    /// ダウン復帰
    /// </summary>
    protected virtual void Reversal()
	{


	}

    /// <summary>
    /// ダウン復帰後処理（ダウン復帰アニメの最終フレームに実装）
    /// </summary>
    protected virtual void ReversalComplete()
	{
		// 復帰アニメが終わると、Idleにする
		// ダウン値を0に戻す
		NowDownRatio = 0.0f;
		// m_DownRebirthTimeを0にする
		DownRebirthTime = 0;
		// m_DownTimeを0にする
		DownTime = 0;
		// 無敵時間を解除する
		StartCoroutine(InvincibleCut());
	}

	
	public IEnumerator InvincibleCut()
	{
		yield return new WaitForSeconds(1.0f);
		Invincible = false;
	}

    /// <summary>
    /// 復帰処理
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="reversalhash"></param>
    protected void ReversalInit(Animator animator)
	{
		// rotationの固定を復活させ、0,0,0にする
		GetComponent<Rigidbody>().rotation = Quaternion.Euler(Vector3.zero);
		// 再固定する
		GetComponent<Rigidbody>().freezeRotation = true;
		// ステートを復帰にする
		animator.SetTrigger("Reversal");
	}
		

    /// <summary>
    /// Idle時共通操作
    /// </summary>
    protected virtual void Animation_Idle(Animator animator)
    {
		// ステップへの移行中だった場合何もしない(タイミングの問題でステップに切り替えてもステート変化が間に合わないことがある）
		if (_StepStartTime != 0)
		{
			if(Time.time - _StepStartTime > 1.0f)
			{
				_StepStartTime = 0;
			}
					
			return;			
		}
		// 固定があった場合解除
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		// 死んでいたらダウン
		if (NowHitpoint < 1)
        {
            animator.SetTrigger("Down");
        }
        // くっついているエフェクトの親元を消す
        BrokenEffect();
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        // ずれた本体角度を戻す(Yはそのまま）
        transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        MoveDirection = Vector3.zero;      // 速度を0に
        BlowDirection = Vector3.zero;
        Rotatehold = false;                // 固定フラグは折る
        // 慣性を殺す
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        AddInput = false;
        GetComponent<Rigidbody>().useGravity = true;
        // ブーストを回復させる
        Boost = GetMaxBoost(BoostLevel);
		// ガードフラグを折る
		IsGuard = false;
        // 地上にいるか？
        if (IsGrounded)
        {
            // 方向キーで走行
            if (HasVHInput)
            {
                animator.SetTrigger("Run");
            }
			// ジャンプ２回でキャンセルダッシュへ移行
			if((HasDashCancelInput|| HasAirDashInput) && Boost > 0)
			{
				CancelDashDone(animator);
			}
            // ジャンプでジャンプへ移行(GetButtonDownで押しっぱなしにはならない。GetButtonで押しっぱなしに対応）
            else if (HasJumpInput && Boost > 0)
            {
                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
                JumpDone(animator);
            }			
			// ステップの場合ステップ(非CPU時)
			if (IsPlayer == CHARACTERCODE.PLAYER)
            {
                if (ControllerManager.Instance.FrontStep) 
                {
                    StepDone(1, new Vector2(0,1),animator);
                }
                else if(ControllerManager.Instance.LeftFrontStep)
                {
                    StepDone(1, new Vector2(-1, 1), animator);
                }
                else if (ControllerManager.Instance.LeftStep)
                {
                    StepDone(1, new Vector2(-1, 0), animator);
                }
                else if (ControllerManager.Instance.LeftBackStep)
                {
                    StepDone(1, new Vector2(-1, -1), animator);
                }
                else if (ControllerManager.Instance.BackStep)
                {
                    StepDone(1, new Vector2(0, -1), animator);
                }
                else if (ControllerManager.Instance.RightBackStep)
                {
                    StepDone(1, new Vector2(1, -1), animator);
                }
                else if (ControllerManager.Instance.RightStep)
                {
                    StepDone(1, new Vector2(1, 0), animator);
                }
                else if (ControllerManager.Instance.RightFrontStep)
                {
                    StepDone(1, new Vector2(1, 1), animator);
                }
            }
            else
            {
                // CPU時左ステップ
                if (HasLeftStepInput)
                {
                    StepDone(1, new Vector2(-1, 0),animator);
                    HasLeftStepInput = false;
                }
                // CPU時右ステップ
                else if (HasRightStepInput)
                {
                    StepDone(1, new Vector2(1, 0),animator);
                    HasRightStepInput = false;
                }
            }

        }
        // いなければ落下
        else
        {
            animator.SetTrigger("Fall");
        }

    }

    /// <summary>
    /// Walk時共通動作
    /// </summary>
    protected virtual void Animation_Walk(Animator animator)
    {
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        // ずれた本体角度を戻す(Yはそのまま）
        transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
    }

    /// <summary>
    /// Jump時共通動作
    /// </summary>    
    protected virtual void Animation_Jump(Animator animator)
    {
        if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
		{
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            JumpTime = 0;
            CancelDashDone(animator );
        }
        else
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            // ずれた本体角度を戻す(Yはそのまま）
            transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
            // ジャンプしたので硬直を設定する
            JumpTime = Time.time;
        }
    }

    // Jumping時共通動作
    protected virtual void Animation_Jumping(Animator animator)
    {
        Vector3 RiseSpeed = new Vector3(MoveDirection.x, this.RiseSpeed, MoveDirection.z);
        if (Time.time > JumpTime + JumpWaitTime)
        {
            // ジャンプ後の硬直終了時の処理はここに入れる
            // ジャンプ中にブーストがある限り上昇
            if (Boost > 0)
            {
				// BD入力でBDへ移行
				if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
				{
					CancelDashDone(animator);
					return;
				}
				// ボタンを押し続ける限り上昇
				else if (HasJumpingInput)
                {
                    Boost = Boost - BoostLess;
                }
                // 離したら落下
                else if (!HasJumpingInput)
                {
                    FallDone(RiseSpeed,animator);
                }
                // ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)
                else if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)
				{
                    CancelDashDone(animator);
                }

                // ステップの場合ステップ
                // ステップの場合ステップ(非CPU時)
                if (IsPlayer == CHARACTERCODE.PLAYER)
                {
                    if (ControllerManager.Instance.FrontStep)
                    {
                        StepDone(1, new Vector2(0, 1), animator);
                    }
                    else if (ControllerManager.Instance.LeftFrontStep)
                    {
                        StepDone(1, new Vector2(-1, 1), animator);
                    }
                    else if (ControllerManager.Instance.LeftStep)
                    {
                        StepDone(1, new Vector2(-1, 0), animator);
                    }
                    else if (ControllerManager.Instance.LeftBackStep)
                    {
                        StepDone(1, new Vector2(-1, -1), animator);
                    }
                    else if (ControllerManager.Instance.BackStep)
                    {
                        StepDone(1, new Vector2(0, -1), animator);
                    }
                    else if (ControllerManager.Instance.RightBackStep)
                    {
                        StepDone(1, new Vector2(1, -1), animator);
                    }
                    else if (ControllerManager.Instance.RightStep)
                    {
                        StepDone(1, new Vector2(1, 0), animator);
                    }
                    else if (ControllerManager.Instance.RightFrontStep)
                    {
                        StepDone(1, new Vector2(1, 1), animator);
                    }
                    // レバー入力で慣性移動
                    else if (HasVHInput)
                    {
                        UpdateRotation();
                        RiseSpeed = new Vector3(0, RiseSpeed.y, 0) + transform.rotation * Vector3.forward;
                    }
                }
                else
                {
                    // CPU時左ステップ
                    if (HasLeftStepInput)
                    {
                        StepDone(1, new Vector2(-1, 0), animator);
                        HasLeftStepInput = false;
                    }
                    // CPU時右ステップ
                    else if (HasRightStepInput)
                    {
                        StepDone(1, new Vector2(1, 0), animator);
                        HasRightStepInput = false;
                    }
                    // レバー入力で慣性移動
                    else if (HasVHInput)
                    {
                        UpdateRotation();
                        RiseSpeed = new Vector3(0, RiseSpeed.y, 0) + transform.rotation * Vector3.forward;
                    }
                }
                

            }
            // ブースト切れを起こすとRiseSpeedを0のままにする（regidbodyの重力制御で落下を制御する）
            // そして下降へ移行する
            else
            {
                FallDone(RiseSpeed,animator);
            }

        }
        // 硬直時間中は上昇してもらう
        else
        {
            RiseSpeed = new Vector3(MoveDirection.x, this.RiseSpeed, MoveDirection.z);
            // 上昇算演
            UpdateRotation();
        }
        // ボタンを離したら下降へ移行
        if (!HasJumpingInput)
        {
            animator.SetTrigger("Fall");
            RiseSpeed = new Vector3(0, -this.RiseSpeed, 0);
        }

        // 上昇算演
        MoveDirection = RiseSpeed;
		// 上昇中にオブジェクトに触れた場合は着地モーションへ移行(暴走防止のために、硬直中は判定禁止)
		if (Time.time > this.JumpTime + this.JumpWaitTime)
        {
    //        if (IsGrounded) // 優先順位はこっちを下にしておかないと上昇前に引っかかる
    //        {
				//Debug.Log("03");
				//LandingDone(animator);
    //        }
        }
    }
		

	/// <summary>
	/// Run時共通動作
	/// </summary>
	/// <param name="animator"></param>
    protected virtual void Animation_Run(Animator animator)
    {
        RigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));

        // 接地中かどうか
        if (IsGrounded)
        {
            // 入力中はそちらへ進む
            if (HasVHInput && !IsWrestle)
            {
				// 足音はPC時のみ
				if (IsPlayer == CHARACTERCODE.PLAYER)
				{
					FootSteps();
				}
                UpdateRotation();
				MoveDirection = transform.rotation * Vector3.forward;
			}
            // ステップの場合ステップ
            // 前
            else if(ControllerManager.Instance.FrontStep)
            {
                StepDone(1, new Vector2(0, 1), animator);
            }
            // 左前ステップ
            else if(ControllerManager.Instance.LeftFrontStep)
            {
                StepDone(1, new Vector2(-1, 1), animator);
            }
            // 左
            else if(ControllerManager.Instance.LeftStep)
            {
                StepDone(1, new Vector2(-1, 0), animator);
            }
            // 左後ステップ
            else if (ControllerManager.Instance.LeftBackStep)
            {
                StepDone(1, new Vector2(-1, -1), animator);
            }
            // 後ステップ
            else if (ControllerManager.Instance.BackStep)
            {
                StepDone(1, new Vector2(0, -1), animator);
            }
            // 右後ステップ
            else if (ControllerManager.Instance.LeftBackStep)
            {
                StepDone(1, new Vector2(1, -1), animator);
            }
            // 右
            else if (ControllerManager.Instance.RightStep)
            {
                StepDone(1, new Vector2(-1, 0), animator);
            }
            // 右前ステップ
            else if (ControllerManager.Instance.RightFrontStep)
            {
                StepDone(1, new Vector2(-1, 0), animator);
            }
            // 入力が外れるとアイドル状態へ（ガード中は除く）
            else
            {
				if (!IsGuard)
				{
					animator.SetTrigger("Idle");
				}
            }

            // ジャンプでジャンプへ移行(GetButtonDownで押しっぱなしにはならない。GetButtonで押しっぱなしに対応）
            if (HasJumpInput && Boost > 0)
            {
                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
                JumpDone(animator);
            }
        }
        else
        {
            animator.SetTrigger("Fall");
        }
    }

	/// <summary>
	/// Fall時共通動作
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Fall(Animator animator)
	{
		// ステップ累積時間を戻す
		_StepStartTime = 0;
		// くっついているエフェクトを消す
		BrokenEffect();
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
		// ステップ時はm_MoveDirectionが消えたら困るので、一旦保持
		Vector3 MoveDirection_OR = MoveDirection;

		// 一応重力復活
		GetComponent<Rigidbody>().useGravity = true;
		// 飛び越えフラグをカット
		Rotatehold = false;
		// 追加入力の有無をカット
		AddInput = false;
		// ブーストがあれば慣性移動及び再上昇可。なければ不可
		if (Boost > 0)
		{
			if ((HasDashCancelInput || HasAirDashInput) && Boost > 0)// ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)           
			{
				CancelDashDone(animator);
				return;
			}
			// ジャンプボタンで再ジャンプ
			else if (HasJumpInput)
			{
				JumpDone(animator);
				return;
			}
			// ステップの場合ステップ
			// 前
			if (ControllerManager.Instance.FrontStep)
			{
				// MoveDirectionを再生する
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(0,1), animator);
				return;
			}
			// 左前
			else if(ControllerManager.Instance.LeftFrontStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(-1, 1), animator);
				return;
			}
			// 左
			else if(ControllerManager.Instance.LeftStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(-1, 0), animator);
				return;
			}
			// 左後
			else if(ControllerManager.Instance.LeftBackStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(-1, -1), animator);
				return;
			}
			// 後
			else if(ControllerManager.Instance.BackStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(0, -1), animator);
				return;
			}
			// 右後
			else if(ControllerManager.Instance.RightBackStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(1, -1), animator);
				return;
			}
			// 右
			else if(ControllerManager.Instance.Right)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(1, 0), animator);
				return;
			}
			// 右前
			else if(ControllerManager.Instance.RightFrontStep)
			{
				MoveDirection = MoveDirection_OR;
				StepDone(1, new Vector2(1, 1), animator);
				return;
			}
			// CPU時左ステップ
			else if (HasLeftStepInput)
			{
				StepDone(1, new Vector2(-1, 0), animator);
				HasLeftStepInput = false;
			}
			// CPU時右ステップ
			else if (HasRightStepInput)
			{
				StepDone(1, new Vector2(1, 0),animator);
				HasRightStepInput = false;
			}
		}
		// 方向キー入力で慣性移動
		if (HasVHInput)
		{
			UpdateRotation();
			MoveDirection = transform.rotation * Vector3.forward;
		}

		// 落下速度調整
		MoveDirection.y = MadokaDefine.FALLSPEED / 2;

		// 着地時に着陸へ移行
		if (IsGrounded)
		{
			MoveDirection = Vector3.zero;
			Debug.Log("04");
			LandingDone(animator);
		}
	}

	/// <summary>
	/// 着地時に実行
	/// </summary>
	/// <param name="animator"></param>
	protected virtual void Animation_Landing(Animator animator)
	{
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま）
		transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));		
		Rotatehold = false;                // 固定フラグは折る
		// 地響き防止
		MoveDirection = transform.rotation * new Vector3(0, 0, 0);
		// モーション終了時にアイドルへ移行
		// 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
		if (Time.time > LandingTime + LandingWaitTime)
		{
			_StepStartTime = 0;
            animator.SetTrigger("Idle");
			// ブースト量を初期化する
			Boost = GetMaxBoost(BoostLevel);
		}
	}

    /// <summary>
    /// ブーストダッシュ時に実行
    /// </summary>
    /// <param name="animator"></param>
	protected virtual void Animation_AirDash(Animator animator)
	{
		transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		// ずれた本体角度を戻す(Yはそのまま） 
		Vector3 RiseSpeed = new Vector3(0, 0, 0);
		// 重力補正をカット
		GetComponent<Rigidbody>().useGravity = false;
		// ブーストがある限り飛行
		if (Boost > 0)
		{
            // 入力中はそちらへ進む
            if (HasJumpingInput || HasAirDashInput)
            {
                // ホールド中旋回禁止
                if (!Rotatehold)
                {
                    UpdateRotation();
                }
                MoveDirection = transform.rotation * Vector3.forward;
			}
            // ボタンを離すと下降
            else
            {
                FallDone(RiseSpeed, animator);
            }
        }
		// ブースト0で下降へ移行&重力復活（慣性移動も不可）
		else
		{
			FallDone(RiseSpeed,animator);
			MoveDirection = RiseSpeed;
		}
		// 着地で着地モーションへ
		//if (IsGrounded)
		//{
		//	Debug.Log("05");
		//	LandingDone(animator);
		//}
		// 常時ブースト消費
		Boost = Boost - BoostLess;
	}

	/// <summary>
	/// ステップ実行時の処理
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="frontstephash"></param>
	/// <param name="leftstephash"></param>
	/// <param name="rightstephash"></param>
	/// <param name="backstephash"></param>
	protected void Animation_StepDone(Animator animator,int frontstephash,int leftstephash,int rightstephash,int backstephash)
	{
		StepMove(animator, frontstephash, leftstephash, rightstephash, backstephash);
		CancelCheck(animator);
	}

	/// <summary>
	/// ステップ終了アニメ実行時の処理
	/// </summary>
	/// <param name="animator"></param>
	protected void Animation_StepBack(Animator animator)
	{
		// ロック中は相手の方向を見る
		if(IsRockon)
		{
			// 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
			var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
			// 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
			// このため、高低差がないとみなす
			Vector3 Target_VertualPos = target.Enemy.transform.position;
			Target_VertualPos.y = 0;
			Vector3 Mine_VerturalPos = transform.position;
			Mine_VerturalPos.y = 0;
			transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
		}
		// ステップ終了アニメの終了を取る
		if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			// 着地したので硬直を設定する
			LandingTime = Time.time;
			// 無効になっていたら重力を復活させる
			GetComponent<Rigidbody>().useGravity = true;
			// 動作を停止する
			MoveDirection = new Vector3(0, 0, 0);
			// 累積時間を戻す
			_StepStartTime = 0;
            // アニメーションをIdleに戻す
            animator.SetTrigger("Idle");
		}	
	}

	// 地上走行中は足音を鳴らす
	private const float _WalkTime = 0.5f;
    private float _WalkTimer;

    private void FootSteps()
    {
        if (_WalkTimer > 0)
        {
            _WalkTimer -= Time.deltaTime;
        }
        if (_WalkTimer <= 0)
        {
            switch (_Foottype)
            {
                case StageSetting.FootType.FootType_Normal:
                    AudioManager.Instance.PlaySE("ashioto_normal");
                    break;
                case StageSetting.FootType.FootType_Jari:
                    AudioManager.Instance.PlaySE("ashioto_jari");
                    break;
                case StageSetting.FootType.FootType_Carpet:
                    AudioManager.Instance.PlaySE("ashiot_carpet");
                    break;
                case StageSetting.FootType.FootType_Snow:
                    AudioManager.Instance.PlaySE("ashioto_snow");
                    break;
                case StageSetting.FootType.FootType_Wood:
                    AudioManager.Instance.PlaySE("ashioto_wood");
                    break;
                default:
                    break;

            }
            _WalkTimer = _WalkTime;
        }
    }



    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}


