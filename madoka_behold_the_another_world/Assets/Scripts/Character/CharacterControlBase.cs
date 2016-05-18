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
    /// 落下開始時間
    /// </summary>
    private float _FallStartTime;

    // 格闘時の移動速度
    protected float WrestlSpeed;        // N格闘1段目

    // 追加入力の有無を保持。trueであり
    protected bool AddInput;


    // 格闘攻撃の種類
    public enum WrestleType
    {
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        CHARGE_WRESTLE,         // 格闘チャージ
        FRONT_WRESTLE_1,        // 前格闘1段目
        FRONT_WRESTLE_2,        // 前格闘2段目
        FRONT_WRESTLE_3,        // 前格闘3段目
        LEFT_WRESTLE_1,         // 左横格闘1段目
        LEFT_WRESTLE_2,         // 左横格闘2段目
        LEFT_WRESTLE_3,         // 左横格闘3段目
        RIGHT_WRESTLE_1,        // 右横格闘1段目
        RIGHT_WRESTLE_2,        // 右横格闘2段目
        RIGHT_WRESTLE_3,        // 右横格闘3段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_WRESTLE_1,           // 特殊格闘1段目
        EX_WRESTLE_2,           // 特殊格闘2段目
        EX_WRESTLE_3,           // 特殊格闘3段目
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        EX_FRONT_WRESTLE_2,     // 前特殊格闘2段目
        EX_FRONT_WRESTLE_3,     // 前特殊格闘3段目
        EX_LEFT_WRESTLE_1,      // 左横特殊格闘1段目
        EX_LEFT_WRESTLE_2,      // 左横特殊格闘2段目
        EX_LEFT_WRESTLE_3,      // 左横特殊格闘3段目
        EX_RIGHT_WRESTLE_1,     // 右横特殊格闘1段目
        EX_RIGHT_WRESTLE_2,     // 右横特殊格闘2段目
        EX_RIGHT_WRESTLE_3,     // 右横特殊格闘3段目
        BACK_EX_WRESTLE,        // 後特殊格闘
        // キャラごとの特殊な処理はこの後に追加、さやかのスクワルタトーレのような連続で切りつける技など

        WRESTLE_TOTAL
    };


    // N格1段目用判定の配置用フック(キャラごとに設定する。順番は上の列挙体と同じ）
    public GameObject[] WrestleRoot = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

    // 格闘判定のオブジェクト
    public GameObject[] WrestleObject = new GameObject[(int)WrestleType.WRESTLE_TOTAL];

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
    /// 移動キーが押されているかどうかをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetVHInput()
    {
        // PC時のみ。CPU時は何か別の関数を用意
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
           if(ControllerManager.Instance.Top || ControllerManager.Instance.Left || ControllerManager.Instance.Right || ControllerManager.Instance.Under)
           {
               return true;
           }
           else
           {
               return false;
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
        if (ControllerManager.Instance.Top)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 方向キー入力（下）が押されているかをチェックする（横が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetBackInput()
    {
        if (ControllerManager.Instance.Under)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 方向キー入力（左）が押されているかをチェックする（縦が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetLeftInput()
    {
        if (ControllerManager.Instance.Left)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 方向キー入力（右）が押されているかをチェックする（縦が入っていたらアウト）
    /// </summary>
    /// <returns></returns>
    protected bool GetRightInput()
    {
        if (ControllerManager.Instance.Right)
        {
            return true;
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
            if(ControllerManager.Instance.Shot)
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
            // 入力中はm_ShotChargeを増加
            if (ControllerManager.Instance.Shotting)
            {
                if (ShotCharge < 0)
                {
                    ShotCharge = 0;
                }
                ShotCharge += ShotIncrease;
            }
            // 解除するとm_ShotChargeを減衰
            else
            {
                if (ShotCharge > 0)
                {
                    ShotCharge -= ShotDecrease;
                }
            }
        }
        // MAX状態で離されるとチャージ量を0にしてtrue
        if (ShotCharge >= ChargeMax && (!ControllerManager.Instance.Shot))
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

        return false;
    }

    /// <summary>
    /// ジャンプ長押し入力があったか否かをチェックする
    /// </summary>
    /// <returns></returns>
    protected bool GetJumpintInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (ControllerManager.Instance.Jumping)
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
        return false;
    }

    // サーチ入力があったか否かをチェックする
    // カメラ側で取得が要るのでここはpublicに
    public bool GetSearchInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (ControllerManager.Instance.Search)
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
			// サブ射撃ボタン
			else if (ControllerManager.Instance.SubShot)
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
				// SG浄化
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
		return false;
	}


	/// <summary>
	/// ジャンプ時共通操作
	/// この関数を各キャラで継承させ、そこでアニメーションの再生を行う
	/// </summary>
	protected virtual void JumpDone()
	{
		Boost = Boost - JumpUseBoost;
	}

	/// <summary>
	/// ステップ中フラグ
	/// ステップモーション再生中に立てること
	/// </summary>
	public bool Stepdone;

    /// <summary>
    /// ステップ時共通操作
    /// </summary>
    /// <param name="Yforce">Y方向への上昇量</param>
    /// <param name="inputVector">入力の方向</param>
    /// <param name="animator">キャラクター本体のAnimator</param>
    /// <param name="stepanimations">ステップアニメのハッシュ番号。順に前、左前、左、左後、後、右後、右、右前とする</param>
    /// <param name="rainbow">虹エフェクトにするか否か（通常はfalse)</param>
    protected virtual void StepDone(float Yforce, Vector2 inputVector, Animator animator, int[] stepanimations, bool rainbow = false)
	{
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
        // 左右どっちかが入っていると左右ステップ
        // ベクトルを角度に変換する(degへ)
        float nowrot = Mathf.Atan2(inputVector.y, inputVector.x) * Mathf.Rad2Deg;

        // 角度演算用の配列
        // 角度1
        float[] rot1 = { -22.5f, 22.5f, 67.5f, 112.5f, -157.5f, -112.5f, -67.5f };
        // 角度2
        float[] rot2 = { 22.5f, 67.5f, 112.5f, 157.5f, -112.5f, -67.5f, -22.5f };

        // 左以外
        for (int i = 0; i < 7; i++)
        {
            if (nowrot >= rot1[i] && nowrot < rot2[i])
            {
                // ロックオン時は相手の方向を見てステップする
                if (this.IsRockon)
                {
                    // 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
                    var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
                    // 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
                    // このため、高低差がないとみなす
                    Vector3 Target_VertualPos = target.Enemy.transform.position;
                    Target_VertualPos.y = 0;
                    Vector3 Mine_VerturalPos = this.transform.position;
                    Mine_VerturalPos.y = 0;
                    transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
                    // アニメ再生
                    animator.Play(stepanimations[i]);
                    break;
                }
                // 非ロックオン時
                else
                {
                    // アニメ再生（強制的にフロントステップにする）
                    animator.Play(stepanimations[0]);
                    break;
                }
            }
        }

        // 左(Unityは±180までしか取れんのよ・・・）
        if (nowrot >= 157.5 || nowrot < -157.5f)
        {
            if (this.IsRockon)
            {
                // 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
                var target = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
                // 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
                // このため、高低差がないとみなす
                Vector3 Target_VertualPos = target.Enemy.transform.position;
                Target_VertualPos.y = 0;
                Vector3 Mine_VerturalPos = this.transform.position;
                Mine_VerturalPos.y = 0;
                this.transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
                // アニメ再生（左ステップ）
                animator.Play(stepanimations[3]);
            }
            // 非ロックオン時
            else
            {
                // アニメ再生（強制的にフロントステップにする）
                animator.Play(stepanimations[0]);
            }
        }
        this.Boost = this.Boost - this.StepUseBoost;
        // ステップ累積距離を0に初期化
        this.SteppingLength = 0.0f;

        // 空中ステップのために重力無効
        this.GetComponent<Rigidbody>().useGravity = false;

        // 移動方向取得
        UpdateRotation_step();

        // 何らかの理由でm_MoveDirectionが0になったとき、強制的に入れる（射撃直後にステップを入れると高確率でこの現象が起こる）
        if (MoveDirection == Vector3.zero)
        {
            // キーの入力方向を取る（ステップすべき方向を取る）
            int inputrot = 0;
            // 上
            if (inputVector.x == 0 && inputVector.y > 0)
            {
                inputrot = 1;
            }
            // 左上
            else if (inputVector.x < 0 && inputVector.y > 0)
            {
                inputrot = 2;
            }
            // 左
            else if (inputVector.x < 0 && inputVector.y == 0)
            {
                inputrot = 3;
            }
            // 左下
            else if (inputVector.x < 0 && inputVector.y < 0)
            {
                inputrot = 4;
            }
            // 下
            else if (inputVector.x == 0 && inputVector.y < 0)
            {
                inputrot = 5;
            }
            // 右下
            else if (inputVector.x > 0 && inputVector.y < 0)
            {
                inputrot = 6;
            }
            // 右
            else if (inputVector.x > 0 && inputVector.y == 0)
            {
                inputrot = 7;
            }
            // 右上
            else
            {
                inputrot = 8;
            }
            // 上に進む場合
            if (inputrot == 1)
            {
                MoveDirection = transform.rotation * Vector3.forward;
            }
            // 下に進む場合
            else if (inputrot == 5)
            {
                // 方向算出(現在の方向の後ろ側）
                float nowrotY = transform.rotation.eulerAngles.y + 180;
                Quaternion rot = Quaternion.Euler(new Vector3(0, nowrotY, 0));
                MoveDirection = rot * Vector3.forward;
            }
            // 左上・左・左下へ進む場合
            else if (2 <= inputrot && inputrot <= 4)
            {
                // 進行方向(rad)
                double nowrotY = transform.rotation.eulerAngles.y * Mathf.PI / 180;
                // 方向ベクトル算出：X
                float x = (float)Mathf.Sin((float)nowrotY - Mathf.PI / 2);
                // 方向ベクトル算出：Z
                float z = (float)Mathf.Sin((float)nowrotY);
                MoveDirection = new Vector3(x, 0, z);
            }
            // 右上・右・右下へ進む場合
            else
            {
                // 進行方向(rad)
                double nowrotY = transform.rotation.eulerAngles.y * Mathf.PI / 180;
                // 方向ベクトル算出：X
                float x = (float)Mathf.Cos((float)nowrotY);
                // 方向ベクトル算出：Z
                float z = (float)Mathf.Sin((float)nowrotY - Mathf.PI);
                MoveDirection = new Vector3(x, 0, z);
            }
        }

        // 格闘キャンセルステップは入力の関係でMoveDirectinが相手の方向を向いているため、MoveDirectionを再設定する
        if (rainbow)
        {
            this.MoveDirection = this.StepRotation * Vector3.forward;
        }
        _StepStartTime = Time.time;
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
            this.GetComponent<Rigidbody>().useGravity = true;
            // 地上にいたら着地
            if (IsGrounded)
            {
                // ブースト量を初期化する
                this.Boost = GetMaxBoost(this.BoostLevel);
            }
            // 空中にいたら角度を戻して落下
            else
            {
                FallDone(new Vector3(0, 0, 0),animator,fallhash);
            }
            // アニメーションを戻す
            animator.Play(idlehash);
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
            animator.Play(fallhash);
            DestroyWrestle();
            _FallStartTime = Time.time;
        }
        // 着地時にLandingを実行する
        if (IsGrounded)
        {
            LandingDone(animator,fallhash);
        }
    }

    /// <summary>
    /// チャージ射撃処理
    /// </summary>
    protected virtual void ChargeShot()
    {
    }

    /// <summary>
    /// サブ射撃処理
    /// </summary>
    protected virtual void SubShot()
    {
    }

    /// <summary>
    /// 特殊射撃処理
    /// </summary>
    protected virtual void ExShot()
    {
    }

    /// <summary>
    /// 格闘の累積時間
    /// </summary>    
    protected float Wrestletime;
    /// <summary>
    /// N格1段目
    /// </summary>    
    protected virtual void Wrestle1(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator,airdashhash,stepanimations);
    }

    /// <summary>
    /// N格2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void Wrestle2(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// N格3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void Wrestle3(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// ステップキャンセル実行時の処理
    /// </summary>
    protected virtual void StepCancel(Animator animator,int airdashhash, int[] stepanimations)
    {
        // キャンセルダッシュ入力を受け取ったら、キャンセルして空中ダッシュする
        if (HasDashCancelInput)
        {
            AddInput = false;
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            CancelDashDone(animator, airdashhash);
        }
        // ステップ入力を受け取ったら、キャンセルしてステップする
        else if (ControllerManager.Instance.FrontStep || ControllerManager.Instance.LeftFrontStep || ControllerManager.Instance.LeftStep || ControllerManager.Instance.LeftBackStep || ControllerManager.Instance.BackStep ||
            ControllerManager.Instance.RightBackStep || ControllerManager.Instance.RightStep || ControllerManager.Instance.RightFrontStep)
        {
            Vector2 stepinput = Vector2.zero;
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
            else if(ControllerManager.Instance.Right)
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
            StepDone(1, stepinput,animator, stepanimations, true);
        }
        // CPU時左ステップ
        else if (HasLeftStepInput)
        {
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            StepDone(1, new Vector2(-1, 0),animator, stepanimations, true);
            HasLeftStepInput = false;
        }
        // CPU時右ステップ
        else if (HasRightStepInput)
        {
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            StepDone(1, new Vector2(1, 0),animator,stepanimations, true);
            HasRightStepInput = false;
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
    /// <param name="stepanimations"></param>
    protected virtual void FrontWrestle1(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 前格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void FrontWrestle2(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 前格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void FrontWrestle3(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 左横格闘1段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void LeftWrestle1(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 左横格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void LeftWrestle2(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 左横格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void LeftWrestle3(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 右横格闘1段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void RightWrestle1(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 右横格闘2段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void RightWrestle2(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 右横格闘3段目
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    protected virtual void RightWrestle3(Animator animator, int airdashhash, int[] stepanimations)
    {
        Wrestletime += Time.deltaTime;
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 後格闘（防御）
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="idlehash">アイドル時のハッシュID</param>
    protected virtual void BackWrestle(Animator animator, int idlehash)
    {
        //1．ブーストゲージを減衰させる
        Boost -= Time.deltaTime * 5.0f;
        //2．ブーストゲージが0になると、強制的にIdleに戻す
        if (Boost <= 0)
        {
            animator.Play(idlehash);
            DestroyWrestle();
        }
        //3．格闘ボタンか下入力を離すと、強制的にIdleに戻す
        if (!ControllerManager.Instance.Wrestle || ControllerManager.Instance.UnderUp)
        {
            animator.Play(idlehash);
            DestroyWrestle();
        }
    }

    /// <summary>
    /// 空中ダッシュ格闘
    /// </summary>
    /// <param name="animator">本体のAnimator</param>
    /// <param name="airdashhash">空中ダッシュのハッシュID</param>
    /// <param name="stepanimations">ステップのアニメーション</param>
    /// <param name="fallhash">落下のハッシュID</param>
    protected virtual void AirDashWrestle(Animator animator, int airdashhash, int[] stepanimations,int fallhash)
    {
        StepCancel(animator, airdashhash, stepanimations);
        // 発動中常時ブースト消費
        Boost = Boost - BoostLess;
        // ブースト切れ時にFallDone、DestroyWrestleを実行する
        if (Boost <= 0)
        {
            animator.Play(fallhash);
            DestroyWrestle();
            _FallStartTime = Time.time;
        }
    }

    // 特殊格闘1段目（特殊格闘はキャラによっては射撃や回復だったりするのでStepCancelは継承先につけること
    protected virtual void ExWrestle1()
    {

    }

    // 特殊格闘2段目
    protected virtual void ExWrestle2()
    {
    }

    // 特殊格闘3段目
    protected virtual void ExWrestle3()
    {
    }

    /// <summary>
    /// 前特殊格闘1段目（全員共通で上昇技）
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    /// <param name="fallhash"></param>
    protected virtual void FrontExWrestle1(Animator animator, int airdashhash, int[] stepanimations, int fallhash)
    {
        // 毎フレームブーストを消費する
        Boost -= Time.deltaTime * 100.0f;
        // 重力無効
        this.GetComponent<Rigidbody>().useGravity = false;
        // ブーストが0になったらFallにする
        if (Boost <= 0)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            animator.Play(fallhash);
            DestroyWrestle();
            _FallStartTime = Time.time;
        }
        StepCancel(animator, airdashhash, stepanimations);
    }

    /// <summary>
    /// 前特殊格闘2段目
    /// </summary>
    protected virtual void FrontExWrestle2()
    {
    }

    /// <summary>
    /// 前特殊格闘3段目
    /// </summary>
    protected virtual void FrontExWrestle3()
    {
    }

    /// <summary>
    /// 左横特殊格闘1段目
    /// </summary>
    protected virtual void LeftExWrestle1()
    {
    }

    /// <summary>
    /// 左横特殊格闘2段目
    /// </summary>
    protected virtual void LeftExWrestle2()
    {
    }

    /// <summary>
    /// 左横特殊格闘3段目
    /// </summary>
    protected virtual void LeftExWrestle3()
    {
    }

    /// <summary>
    /// 右横特殊格闘1段目
    /// </summary>
    protected virtual void RightExWrestle1()
    {
    }

    /// <summary>
    /// 右横特殊格闘2段目
    /// </summary>
    protected virtual void RightExWrestle2()
    {
    }

    /// <summary>
    /// 右横特殊格闘3段目
    /// </summary>
    protected virtual void RightExWrestle3()
    {
    }

    /// <summary>
    /// 後特殊格闘（全員共通で下降技）
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="airdashhash"></param>
    /// <param name="stepanimations"></param>
    /// <param name="fallhash"></param>
    /// <param name="landinghash"></param>
    protected virtual void BackExWrestle(Animator animator, int airdashhash, int[] stepanimations, int fallhash,int landinghash)
    {
        // 毎フレームブーストを消費する
        Boost -= Time.deltaTime * 100;
        // ブーストが0になったらFallにする
        if (Boost <= 0)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            animator.Play(fallhash);
            DestroyWrestle();
            _FallStartTime = Time.time;
        }
        StepCancel(animator, airdashhash, stepanimations);
        // 接地したらLandingにする
        if (IsGrounded)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            LandingDone(animator,landinghash);
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
    protected void LandingDone(Animator animator, int landingHash)
    {
        // 格闘判定削除
        DestroyWrestle();
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        // 無効になっていたら重力を復活させる
        this.GetComponent<Rigidbody>().useGravity = true;
        animator.Play(landingHash);
        // 着地したので硬直を設定する
        LandingTime = Time.time;
    }


    /// <summary>
    /// 空中ダッシュ（キャンセルダッシュ）発動共通操作
    /// 弓ほむら・まどかのモーションキャンセルなどはこの前に行うこと
    /// </summary>
    protected virtual void CancelDashDone(Animator animator, int airdashhash)
    {
        if (this.Boost > 0)
        {
            // 格闘判定削除
            DestroyWrestle();
            // 一応歩き撃ちフラグはここでも折る
            RunShotDone = false;
            // 上体の角度を戻す
            BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            Rotatehold = false;
            this.Boost = Boost - DashCancelUseBoost;
            animator.Play(airdashhash);
            // 移動方向取得
            //UpdateRotation();
            //this.m_MoveDirection = transform.rotation * Vector3.forward;
            // 角度に応じてX、Zの方向を切り替える
            if (this.transform.rotation.eulerAngles.y >= 337.5f && this.transform.rotation.eulerAngles.y < 22.5f)
            {
                MoveDirection.x = 0.0f;
                MoveDirection.z = 0.0f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 22.5f && this.transform.rotation.eulerAngles.y < 67.5f)
            {
                MoveDirection.x = 0.7f;
                MoveDirection.z = 0.0f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 67.5f && this.transform.rotation.eulerAngles.y < 112.5f)
            {
                MoveDirection.x = 1.0f;
                MoveDirection.z = 0.0f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 112.5f && this.transform.rotation.eulerAngles.y < 157.5f)
            {
                MoveDirection.x = 0.7f;
                MoveDirection.z = -0.5f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 157.5f && this.transform.rotation.eulerAngles.y < 202.5f)
            {
                MoveDirection.x = 0.0f;
                MoveDirection.z = -1.0f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 202.5f && this.transform.rotation.eulerAngles.y < 247.5f)
            {
                MoveDirection.x = -0.7f;
                MoveDirection.z = -0.5f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 247.5f && this.transform.rotation.eulerAngles.y < 292.5f)
            {
                MoveDirection.x = -1.0f;
                MoveDirection.z = 0.0f;
            }
            else if (this.transform.rotation.eulerAngles.y >= 292.5f && this.transform.rotation.eulerAngles.y < 337.5f)
            {
                MoveDirection.x = -0.7f;
                MoveDirection.z = 0.0f;
            }

            // 上方向への慣性を切る
            this.MoveDirection.y = 0;
            // 発動中重力無効
            this.GetComponent<Rigidbody>().useGravity = false;
            // その方向へ移動
            GetComponent<Rigidbody>().AddForce(this.MoveDirection.x, 10, this.MoveDirection.z);
        }
    }


    /// <summary>
    /// 下降共通動作
    /// </summary>
    /// <param name="RiseSpeed">落下速度</param>
    /// <param name="animator">本体のanimator</param>
    /// <param name="fallhash">fall状態のハッシュコード</param>
    protected void FallDone(Vector3 RiseSpeed,Animator animator, int fallhash)
    {
        this.GetComponent<Rigidbody>().useGravity = true;
        animator.Play(fallhash);
        RiseSpeed = new Vector3(0, -this.RiseSpeed, 0);
        _FallStartTime = Time.time;
    }

    /// <summary>
    /// くっついている格闘オブジェクトをすべて消す
    /// </summary>
    protected void DestroyWrestle()
    {
        for (int i = 0; i < WrestleRoot.Length; i++)
        {
            // あらかじめ子があるかチェックしないとGetChildを使うときはエラーになる
            if (this.WrestleRoot[i] != null && this.WrestleRoot[i].GetComponentInChildren<Wrestle_Core>() != null)
            {
                var wrestle = this.WrestleRoot[i].GetComponentInChildren<Wrestle_Core>();

                if (wrestle != null)
                {
                    Destroy(wrestle.gameObject);
                }
            }
        }
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
		if (ControllerManager.Instance.Top)
		{
			vertical = 1.0f;
		}
		else if(ControllerManager.Instance.Under)
		{
			vertical = -1.0f;
		}
		
		if(ControllerManager.Instance.Left)
		{
			horizontal = -1.0f;
		}
		else if(ControllerManager.Instance.Right)
		{
			horizontal = 1.0f;
		}

        var toWorldVector = MainCamera.transform.rotation;
        // ベクトルは平行移動の影響を受けないので逆行列は使わない
        // スケールの影響は受けるがここでは無視する。

        // CPU時、ここで入力を取得
        if (this.IsPlayer != CHARACTERCODE.PLAYER)
        {
            // CPU情報
            var CPU = this.GetComponentInChildren<AIControl_Base>();
            // テンキーの種類からhorizontalとverticalを取得
            if ((int)CPU.m_tenkeyoutput <= 8)
            {
                horizontal = CPU.m_lever[(int)CPU.m_tenkeyoutput].x;
                vertical = CPU.m_lever[(int)CPU.m_tenkeyoutput].y;
            }
        }

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
        this.MoveDirection = transform.rotation * new Vector3(0, 0, 0);
        // モーション終了時にアイドルへ移行
        // 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
        if (Time.time > LandingTime + LandingWaitTime)
        {
            animator.Play(landinghash);
            // ブースト量を初期化する
            this.Boost = GetMaxBoost(this.BoostLevel);
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
