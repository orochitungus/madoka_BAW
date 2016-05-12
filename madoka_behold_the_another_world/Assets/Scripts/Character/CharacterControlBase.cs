using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    /// 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    /// </summary>
    public Vector3 LayOriginOffs;

    /// <summary>
    /// レイの長さ
    /// </summary>
    private float _Laylength;

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
    /// ポーズコントローラー
    /// </summary>
    private GameObject Pausecontroller;

    // CPU
    /// <summary>
    /// ルーチンを拾う
    /// </summary>
    public AIControl_Base.CPUMODE CPUMode;

    /// <summary>
    /// テンキー入力を拾う
    /// </summary>
    public AIControl_Base.TENKEY_OUTPUT CPUTenkey;

    /// <summary>
    /// キー入力を拾う
    /// </summary>
    public AIControl_Base.KEY_OUTPUT CPUKey;

    /// <summary>
    /// AnimationState を 2 つ用意し交互に切り替えて昔の状態を参照できるようにする。
    /// AnimatorのHash番号を保持する
    /// Animatorの構造はキャラごとに異なるので、保持は継承先で行う
    /// </summary>
    public int[] AnimState = new int[2];

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
    public float Boost_Growth;

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
    public float Arousal_Growth;

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
    public int NowHitpoint_Growth;

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
    /// ポーズ入力があったか否か
    /// </summary>
    protected bool HasPauseInput;

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
	private int ChargeMax;

	public int GetChargeMax()
	{
		return ChargeMax;
	}

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
	public float Collider_Height;

	/// <summary>
	///  射出する弾の方向ベクトル(スプレッドのときはこれを基準にしてずらす）
	/// </summary>
	public Vector3 BulletMoveDirection;

	/// <summary>
	/// 射出する弾の配置位置
	/// </summary>
	public Vector3 BulletPos;

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
        NowHitpoint_Growth = Character_Spec.HP_Grouth[(int)CharacterName];


        // ブースト量初期値(Lv1の時の値）
        Boost_OR = Character_Spec.Boost_OR[(int)CharacterName];
        // ブースト量成長係数
        Boost_Growth = Character_Spec.Boost_Growth[(int)CharacterName];

        // 覚醒ゲージ量初期値(LV1の時の値）
        Arousal_OR = Character_Spec.Arousal_OR[(int)CharacterName];
        // 覚醒ゲージ量成長係数
        Arousal_Growth = Character_Spec.Arousal_Growth[(int)CharacterName];

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

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
