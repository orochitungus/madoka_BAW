using UnityEngine;
using System;
using System.Collections;

// アクションステージで使うキャラの基底クラス
// ただモードチェンジがあるマミ・さやか・各種ボスは使えないかもしれない
public partial class CharacterControl_Base : MonoBehaviour 
{
    // 各種G変数追加（時間停止に備え、物理量は常時保存）
    // 通常カメラオブジェクトをフィールド変数に追加する
    public Camera MainCamera;
    // 覚醒用カメラのオブジェクト
    public Camera m_Insp_ArousalCamera;
    // 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    public Vector3 LayOriginOffs;
    // レイの長さ
    private float Laylength;
    // ground属性をもったレイヤー（この場合8）
    protected int LayMask;

    protected CharacterController m_charactercontroller;
    protected Rigidbody Rigidbody;

    // 敵の哨戒モードにおける起点(僚機の時はPCを指定してもいいかもしれない）
    public GameObject StartingPoint;
    public GameObject EndingPoint;

    // 歩き撃ちフラグ
    protected bool RunShotDone;
	/// <summary>
	/// 覚醒技演出中フラグ
	/// </summary>
	public bool	ArousalAttackProduction;

    // プレイヤーであるか否か（これがfalseであると描画系の関数全カット）
    // それぞれの意味は以下の通り
    public enum CHARACTERCODE
    {
        PLAYER,         // プレイヤー操作
        PLAYER_ALLY,    // プレイヤー僚機
        ENEMY           // 敵
    };
    public CHARACTERCODE IsPlayer;

    // 弾の種類(CharacterSpecの配置順に合わせること）
    public enum ShotType
    {
        NORMAL_SHOT,    // 通常射撃
        CHARGE_SHOT,    // チャージ射撃
        SUB_SHOT,       // サブ射撃
        EX_SHOT,        // 特殊射撃        
    };

    // 回復音
    // HP
    public AudioClip RebirthHP1;
    // 蘇生
    public AudioClip Regenaration;
    // SG浄化
    public AudioClip PurificationSG;

    // 射撃時の射出モード
    public enum ShotMode
    {
        NORMAL,     // 通常状態
        RELORD,     // 構え
        SHOT,       // 射出
        SHOTDONE,   // 射出完了
        AIRDASHSHOT // 射出（空中ダッシュ射撃）
    };
    protected ShotMode shotmode;
    public ShotMode GetShotmode()
    {
        return shotmode;
    }
    public void SetShotmode(ShotMode sm)
    {
        shotmode = sm;
    }

    // 誰であるか
    public Character_Spec.CHARACTER_NAME m_character_name;

    // 頭部オブジェクト
    public GameObject m_Head;

    // 胸部オブジェクト
    public GameObject BrestObject;

    // 覚醒時エフェクト
    public GameObject m_Insp_ArousalEffect;

    // ポーズコントローラー
    private GameObject m_pausecontroller;

    // CPU
    // ルーチンを拾う
    AIControl_Base.CPUMODE m_cpumode;
    // テンキー入力を拾う
	AIControl_Base.TENKEY_OUTPUT m_tenkey;
    // キー入力を拾う
	AIControl_Base.KEY_OUTPUT m_key; 

    // AnimationState を 2 つ用意し交互に切り替えて昔の状態を参照できるようにする。
    public AnimationState[] m_AnimState = new AnimationState[2];
    public int CurAnimPos;

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
    public TimeStopMode m_timstopmode;
    // 時間停止・遅延のmasterであるか否か（これがtrueだと時間停止の影響を受けない.発動者にのみtureにする。）
    public bool TimeStopMaster;

    public float m_Rockon_Range;    // ロックオン距離（この距離を超えて撃つと誘導や補正が入らない）
    public float m_Rockon_RangeLimit;// ロックオン限界距離（この距離を超えるとロックオン判定に入らない）

    // ジャンプ時間
    public float m_JumpWaitTime;
    public float JumpTime;

    // 着地硬直
    public float _LandingWaitTime;
    public float LandingTime;

    // 攻撃行動硬直
    public float m_AttackTime;

    // 移動用ステート
    public Vector3 MoveDirection;   // 移動方向
    public Vector3 m_MoveDirection_OR;// 射撃などの射出前における移動方向
    public Vector3 BlowDirection;   // ダウンする時や吹き飛び属性の攻撃を食らった時の方向ベクトル
    public float m_WalkSpeed;         // 移動速度（歩行の場合）
    public float m_RunSpeed;          // 移動速度（走行の場合）
    public float m_AirDashSpeed;      // 移動速度（空中ダッシュの場合）
    public float m_AirMoveSpeed;      // 移動速度（空中慣性移動の場合）
    public float RiseSpeed;        // 上昇速度


    // ジャンプ時のブースト消費量
    protected float m_JumpUseBoost;

    // ダッシュキャンセル時のブースト消費量
    protected float DashCancelUseBoost;

    // ダッシュキャンセル硬直時間
    protected float m_DashCancelWaittime;
    // ダッシュキャンセル累積時間
    protected float m_DashCancelTime;

    // ステップ時のブースト消費量
    protected float StepUseBoost;

    // ダウン回避時のブースト消費量
    protected float m_ReversalUseBoost;

    // レベル
    public int Level = 1;

    // 取得技（オートスキル含む）
    public int m_SkillUse = 1;      // 1つ習得するたびにインクリメント

    // 攻撃力レベル
    public int m_StrLevel = 1;

    // 防御力レベル
    public int m_DefLevel = 1;

    // 残弾数レベル
    public int m_BulLevel = 1;

    // ブースト量レベル
    public int BoostLevel = 1;

    // 覚醒ゲージレベル
    public int m_ArousalLevel = 1;

    // ブースト量
    public float Boost;
    // ブースト量初期値(Lv1の時の値）
    public float Boost_OR;
    // ブースト量成長係数
    public float BoostGrowth;

    // 覚醒ゲージ量
    public float Arousal;
    public void AddArousal(float arousal)
    {
        Arousal = Arousal + arousal;
    }
    // 覚醒ゲージ量初期値(LV1の時の値）
    public float Arousal_OR;
    // 覚醒ゲージ量成長係数
    public float ArousalGrowth;

    // 覚醒状態であるか否か
    public bool IsArousal;

    // ブースト消費量（1Fあたり）
    public float m_BoostLess;

    // ステップ移動距離
    public float m_Step_Move_Length;

    // ステップ初速（X/Z軸）
    public float m_Step_Initial_velocity;
    // ステップ時の１F当たりの移動量
    public float m_Step_Move_1F;

    // ステップ累積移動距離
    public float SteppingLength;

    // ステップ時の移動角度
    public Quaternion StepRotation;

    // ステップ終了時の硬直時間
    public float m_StepBackTime;

    // 現在のダウン値
    public float NowDownRatio;

    // 現在のHP
    public int NowHitpoint;
    // 現在のHPの表示値
    public int m_DrawHitpoint;
	// HP初期値（Lv1のときの値）
	public int NowHitpoint_OR;
	// HP成長係数
	public int NowHitpointGrowth;
    // 現在のソウルジェム汚染率
    public float m_GemContamination;
	
    // ダウン値の閾値（これを超えるとダウン状態へ移行.基本全員5でOK、一部のボスをそれ以上に）
    public float DownRatio;

    // ダウン時の打ち上げ量
    public float LaunchOffset;

    // ブーストゲージ用テクスチャ
    public Texture2D m_BoostTex;

    // 覚醒ゲージ用テクスチャ
    public Texture2D m_ArousalTex;
    // 覚醒可能「OK!!」表示用テクスチャ
    public Texture2D m_ArousalOK;
    // ブーストゲージ/覚醒ゲージ外枠
    public Texture2D m_WindowParts;

    // 装備アイテム用テクスチャ
    public Texture2D m_ItemEquip;

    // ロックオン状態か否か
    public bool IsRockon;
    // ロックオンボタンを何F押したか
    public int m_RockonButtonPress;
    // ロックオン解除までの時間
    protected int m_RockonCancelTime;

    // 移動方向固定状態か否か
    protected bool Rotatehold;

    // アーマー状態か否か
    public bool IsArmor;

    // 地上にいるか否か
    protected bool IsGrounded;

    // AIが取るのでアクセサ用意
    public bool GetInGround()
    {
        return IsGrounded;
    }

    // 入力関係
    // 攻撃系は離したときか入力の瞬間に判定をとる（そうしないと連続入力などがとれない）

    // テンキー入力があったか否か
    protected bool HasVHInput;
    // 射撃入力があったか否か
    protected bool HasShotInput;
    // ジャンプ入力があったか否か
    protected bool m_hasJumpInput;
    // ダッシュキャンセル入力があったか否か
    protected bool HasDashCancelInput;
    // 空中ダッシュ入力があったか否か
    protected bool m_hasAirDashInput;
    // サーチ入力があったか否か
    protected bool m_hasSearchInput;
    // サーチキャンセル入力があったか否か
    public bool m_hasSearchCancelInput;
    // 格闘入力があったか否か
    protected bool m_hasWrestleInput;
    // サブ射撃入力があったか否か
    protected bool m_hasSubShotInput;
    // 特殊射撃入力があったか否か
    protected bool m_hasExShotInput;
    // 特殊格闘入力があったか否か
    protected bool m_hasExWrestleInput;
    // アイテム入力があったか否か
    protected bool m_hasItemInput;
    // ポーズ入力があったか否か
    protected bool m_hasPauseInput;
    // 覚醒入力があったか否か
    protected bool m_hasArousalInput;
    // 覚醒技入力があったか否か（コマンドは同じだが、処理順番の関係上別にしておかないとCPUは覚醒した時点で覚醒技を入力したという扱いになってしまう）
    protected bool m_hasArousalAttackInput;
    // 射撃チャージ入力があったか否か
    protected bool m_hasShotChargeInput;
    // 格闘チャージ入力があったか否か
    protected bool m_hasWrestleChargeInput;
    // 前入力があったか否か
    protected bool m_hasFrontInput;
    // 左入力があったか否か
    protected bool m_hasLeftInput;
    // 右入力があったか否か
    protected bool m_hasRightInput;
    // 後入力があったか否か
    protected bool m_hasBackInput;
    // 左ステップ入力があったか否か（CPU専用）
    protected bool HasLeftStepInput;
    // 右ステップ入力があったか否か（CPU専用）
    protected bool HasRightStepInput;
    // ソウルバースト入力があったか否か
    protected bool m_hasSoulBurstInput;

    // ステップ入力強制解除
    public void StepStop()
    {
        HasLeftStepInput = false;
        HasRightStepInput = false;
    }

    // 使用可能な武装（弾切れもしくはリロード中は使用不可にする.順番はCharacter_Specのインデックスの射撃武装）
    public bool []WeaponUseAble = new bool[20];
    // 弾数を消費するタイプの武装の場合、残弾数
    public int[] BulletNum = new int[20];
    // 射撃攻撃の硬直時間
    public float[] BulletWaitTime = new float[20];

    // 追撃可能時間(ダメージ硬直時間）
    public float m_DamagedWaitTime;
    // 追撃可能累積時間    
    public float m_DamagedTime;

    // ダウン値復帰時間
    public float m_DownRebirthWaitTime;
    // ダウン値累積時間
    public float m_DownRebirthTime;

    // ダウン時間（ダウンしている時間）
    public float m_DownWaitTime;
    // ダウン累積時間
    public float DownTime;

    // 累積押し時間（射撃）をカウント
    protected int ShotCharge;
    public int GetShotCharge()
    {
        return ShotCharge;
    }
	// 累積押し時間（格闘）をカウント
    protected int WrestleCharge;
    public int GetWrestleCharge()
    {
        return WrestleCharge;
    }

    // 1Fあたりの射撃チャージゲージ増加量
    protected int ShotIncrease;
    // 1Fあたりの格闘チャージゲージ増加量
    protected int WrestleIncrease;
    // 1Fあたりの射撃チャージゲージ減衰量
    protected int ShotDecrease;
    // 1Fあたりの格闘チャージゲージ減衰量
    protected int WrestleDecrease;
    // チャージ最大値（固定。ここを超えたら発射）
    private int ChargeMax;

    public int GetChargeMax()
    {
        return ChargeMax;
    }

    // モード状態（モードチェンジがないキャラは常時Normalのまま）
    public enum ModeState
    {
        NORMAL,
        ANOTHER_MODE
    }
    // 現在のモード
    public ModeState Nowmode;

    // コライダの地面からの高さ
    public float Collider_Height;


    // 現在の動作の内容
    // アニメーションステートを定義（キャラごとに技構成は異なるがその都度書くのが面倒なので一応全部網羅しておく）
    public enum AnimationState
    {
        Idle,                   // 通常
        Walk,                   // 歩行
        Walk_underonly,         // 歩行（下半身のみ）
        Jump,                   // ジャンプ開始
        Jump_underonly,         // ジャンプ開始（下半身のみ）
        Jumping,                // ジャンプ中（上昇状態）
        Jumping_underonly,      // ジャンプ中（下半身のみ）
        Fall,                   // 落下
        Landing,                // 着地
        Run,                    // 走行
        Run_underonly,          // 走行（下半身のみ）
        AirDash,                // 空中ダッシュ
        FrontStep,              // 前ステップ中        
        LeftStep,               // 左（斜め）ステップ中       
        RightStep,              // 右（斜め）ステップ中        
        BackStep,               // 後ステップ中
        FrontStepBack,          // 前ステップ終了
        LeftStepBack,           // 左（斜め）ステップ終了
        RightStepBack,          // 右（斜め）ステップ終了
        BackStepBack,           // 後ステップ終了
        FallStep,               // ステップで下降中
        Shot,                   // 通常射撃
        Shot_toponly,           // 通常射撃（上半身のみ）
        Shot_run,               // 通常射撃歩き撃ち
        Shot_AirDash,           // 通常射撃（空中ダッシュ）
        Charge_Shot,            // 射撃チャージ
        Sub_Shot,               // サブ射撃
        EX_Shot,                // 特殊射撃
        // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
        Wrestle_1,              // N格1段目
        Wrestle_2,              // N格2段目
        Wrestle_3,              // N格3段目
        Charge_Wrestle,         // 格闘チャージ
        Front_Wrestle_1,        // 前格闘1段目
        Front_Wrestle_2,        // 前格闘2段目
        Front_Wrestle_3,        // 前格闘3段目
        Left_Wrestle_1,         // 左横格闘1段目
        Left_Wrestle_2,         // 左横格闘2段目
        Left_Wrestle_3,         // 左横格闘3段目
        Right_Wrestle_1,        // 右横格闘1段目
        Right_Wrestle_2,        // 右横格闘2段目
        Right_Wrestle_3,        // 右横格闘3段目
        Back_Wrestle,           // 後格闘（防御）
        AirDash_Wrestle,        // 空中ダッシュ格闘
        Ex_Wrestle_1,           // 特殊格闘1段目
        Ex_Wrestle_2,           // 特殊格闘2段目
        Ex_Wrestle_3,           // 特殊格闘3段目
        EX_Front_Wrestle_1,     // 前特殊格闘1段目
        EX_Front_Wrestle_2,     // 前特殊格闘2段目
        EX_Front_Wrestle_3,     // 前特殊格闘3段目
        EX_Left_Wrestle_1,      // 左横特殊格闘1段目
        EX_Left_Wrestle_2,      // 左横特殊格闘2段目
        EX_Left_Wrestle_3,      // 左横特殊格闘3段目
        EX_Right_Wrestle_1,     // 右横特殊格闘1段目
        EX_Right_Wrestle_2,     // 右横特殊格闘2段目
        EX_Right_Wrestle_3,     // 右横特殊格闘3段目
        BACK_EX_Wrestle,        // 後特殊格闘
        Reversal,               // ダウン復帰
        Arousal_Attack,         // 覚醒技        
        Damage,                 // ダメージ(のけぞり）
        Down,                   // ダウン
        Nomove,                 // 動きなし        
        Blow,                   // 吹き飛び
        FrontStepBack_Standby,  // 前ステップ終了(アニメはなし）
        LeftStepBack_Standby,   // 左（斜め）ステップ終了(アニメはなし）
        RightStepBack_Standby,  // 右（斜め）ステップ終了(アニメはなし）
        BackStepBack_Standby,   // 後ステップ終了(アニメはなし）
        DamageInit,             // ダメージ前処理(アニメはなし）
        BlowInit,               // 吹き飛び前処理(アニメはなし）
        SpinDown,               // 錐揉み墜落（ダウン値MAX)
        Number_Of_Animation
    }

    // 射出する弾の方向ベクトル(スプレッドのときはこれを基準にしてずらす）
    public Vector3 BulletMoveDirection;
    // 射出する弾の配置位置
    public Vector3 BulletPos;
    // 射出する弾の攻撃力
    public int OffensivePowerOfBullet;
    // 射出する弾のダウン値
    public float DownratioPowerOfBullet;
    // 射出する弾の覚醒ゲージ増加量
    public float ArousalRatioOfBullet;

    // 通常射撃用弾丸の配置用フック(他の専用武器は派生先で用意）
    public GameObject MainShotRoot;
    // リロードクラス
    protected Reload ReloadSystem;

    // 足音
    public AudioClip m_ashioto_normal;
    public AudioClip m_ashioto_wood;
    public AudioClip m_ashioto_jari;
    public AudioClip m_ashioto_snow;
    public AudioClip m_ashioto_carpet;

    // 使用する足音
    private StageSetting.FootType m_foottype;
    

    // アニメーションファイルの名前(下記のAnimationStateに対応)    
    public string[] m_AnimationNames = new string[(int)AnimationState.Number_Of_Animation];

    // プレイヤーのレベル設定を行う
    protected void SettingPleyerLevel()
    {
        // 自機もしくは自機の僚機
        if (IsPlayer != CHARACTERCODE.ENEMY)
        {
            // 0を拾った場合は1に
            // 個別ステートを初期化（インスペクタでもできるけど一応こっちでやっておこう）
            // 後、ブースト量などはここで設定しておかないと下で初期化できない
            // レベル
            this.Level = savingparameter.GetNowLevel((int)m_character_name);           
            // 攻撃力レベル
            this.m_StrLevel = savingparameter.GetStrLevel((int)m_character_name);        
            // 防御力レベル
            this.m_DefLevel = savingparameter.GetDefLevel((int)m_character_name);            
            // 残弾数レベル
            this.m_BulLevel = savingparameter.GetBulLevel((int)m_character_name);            
            // ブースト量レベル
            this.BoostLevel = savingparameter.GetBoostLevel((int)m_character_name);            
            // 覚醒ゲージレベル
            this.m_ArousalLevel = savingparameter.GetArousalLevel((int)m_character_name);
            			
        }
        // 敵の場合はインスペクターで設定
       
        // 1を割っていた場合は1に
        if (this.Level < 1)
        {
            this.Level = 1;
        }
        if (this.m_StrLevel < 1)
        {
            this.m_StrLevel = 1;
        }
        if (this.m_DefLevel < 1)
        {
            this.m_DefLevel = 1;
        }
        if (this.m_BulLevel < 1)
        {
            this.m_BulLevel = 1;
        }
        if (this.BoostLevel < 1)
        {
            this.BoostLevel = 1;
        }
        if (this.m_ArousalLevel < 1)
        {
            this.m_ArousalLevel = 1;
        }

		// HP初期値
		this.NowHitpoint_OR = Character_Spec.HP_OR[(int)m_character_name];
		// HP成長係数
		this.NowHitpointGrowth = Character_Spec.HP_Grouth[(int)m_character_name];
				
		
        // ブースト量初期値(Lv1の時の値）
        this.Boost_OR = Character_Spec.Boost_OR[(int)m_character_name];
        // ブースト量成長係数
        this.BoostGrowth = Character_Spec.Boost_Growth[(int)m_character_name];

        // 覚醒ゲージ量初期値(LV1の時の値）
        this.Arousal_OR = Character_Spec.Arousal_OR[(int)m_character_name];
        // 覚醒ゲージ量成長係数
        this.ArousalGrowth = Character_Spec.Arousal_Growth[(int)m_character_name];

        // 使用可能武器をセット（初期段階で使用不能にしておきたいものは、各キャラのStartでこの関数を呼んだ後に再定義）
        for (int i = 0; i < Character_Spec.cs[(int)m_character_name].Length; i++)
        {
            // 使用の可否を初期化
            WeaponUseAble[i] = true;
            // 弾があるものは残弾数を初期化
            if (Character_Spec.cs[(int)m_character_name][i].m_OriginalBulletNum > 0)
            {
                this.BulletNum[i] = Character_Spec.cs[(int)m_character_name][i].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][i].m_OriginalBulletNum;
            }
            // 硬直時間があるものは硬直時間を初期化
            if (Character_Spec.cs[(int)m_character_name][i].m_WaitTime > 0)
            {
                this.BulletWaitTime[i] = Character_Spec.cs[(int)m_character_name][i].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][i].m_WaitTime;
            }
        }
        // ダッシュキャンセル硬直時間
        this.m_DashCancelTime = 0.2f;
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

    protected int Hitcounter;
    bool Hitcounterdone;
    const int HitcounterBias = 20;

    // 接地判定を行う。足元に5本(中心と前後左右)レイを落とし、そのいずれかが接触していれば接地。全部外れていれば落下
    protected bool onGround2()
    {
        // 座標
        Vector3[] layStartPos = new Vector3[5];
        // カプセルコライダ
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        // 中心
        layStartPos[0] = transform.position + this.LayOriginOffs;
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
            if (m_AnimState[0] == AnimationState.SpinDown)
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
        // 1つでもヒットしたら接地とする
        if (hitcount > 0)
        {
            return true;
        }

        return false;
    }

    // 接地判定を行う。要はKanonの時と同じで、レイの高さ分だけ判定をとる。CharacterControllerとどっちを取るかは要実験
    protected bool onGround()
    {
        // 錐揉みダウン時
        if (m_AnimState[0] == AnimationState.SpinDown)
        {
            CapsuleCollider collider = GetComponent<CapsuleCollider>();
            if (Physics.Raycast(transform.position + this.LayOriginOffs, -Vector3.up, this.Laylength - collider.radius, this.LayMask))           
            {
                Hitcounterdone = false;
                return true;
            }
            else
            {
                Hitcounterdone = true;
                Hitcounter = HitcounterBias + 1;
                return false;
            }
        }
        // 通常時
        else
        {
            // ジャンプを開始したとき
            if (m_AnimState[0] == AnimationState.Jump)
            {
                Hitcounterdone = true;
                Hitcounter = HitcounterBias + 1;
                return false;
            }
            // ジャンプ中
            else if (m_AnimState[0] == AnimationState.Jumping && !Physics.Raycast(transform.position + this.LayOriginOffs, -Vector3.up, this.Laylength, this.LayMask))
            {
                Hitcounterdone = true;
                Hitcounter = HitcounterBias + 1;
                return false;
            }
            // 接地しているとき
            else if (Physics.Raycast(transform.position + this.LayOriginOffs, -Vector3.up, this.Laylength, this.LayMask))
            {
                Hitcounterdone = false;
                return true;
            }
            // 離れた時
            else
            {
                if (!Hitcounterdone)
                {
                    Hitcounterdone = true;
                    Hitcounter = 0;
                }
                else
                {
                    Hitcounter++;
                }
            }
        }
        if (Hitcounter > HitcounterBias)
        {
            return false;
        }
        return true;
    }

   

    // 過去のレバー入力を保持。(N→特定の方向が過去のFにあった場合、その方向へステップする)
    // Idle,walk,Fallのみ（格闘ステップキャンセルを入れるなら格闘などでも）
    // 閾値として0.3以下の誤差は無視する
    protected Vector2[] m_PastInputs = new Vector2[80];


    // 下記の関数用の配列
    // 角度1
    protected float[] rot1 = { -22.5f, 22.5f, 67.5f, 112.5f, -157.5f, -112.5f, -67.5f };
    // 角度2
    protected float[] rot2 = { 22.5f, 67.5f, 112.5f, 157.5f, -112.5f, -67.5f, -22.5f };

    // 初期化
    protected void ResetPastInputs()
    {
        for (int i = 0; i < m_PastInputs.Length; i++)
        {
            m_PastInputs[i] = new Vector2(0, 0);
        }
    }

    // テスト用に射出終了で止めておく
    protected void stopAnimation()
    {
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Shot]].speed = 0.0f;
    }     

    // レバー入力を取得する(現在の値が一番下に来る)
    protected void GetPastInputs()
    {
        // 今までの値を１つずつ上へ移動させる
        for (int i = 0; i < m_PastInputs.Length - 1; i++)
        {
            m_PastInputs[i] = m_PastInputs[i + 1];
        }
        // 現在の値を最後に持ってくる
        m_PastInputs[m_PastInputs.Length - 1] = CheckVHInput();
    }

    // 過去30Fの入力を拾う
    // (N→特定の方向が2回通ったらその方向へステップを行う）
    protected Vector2 StepCheck()
    {        
        Vector2 CheckOUTValue = new Vector2(0, 0);
        // CPU時除く(CPUのステップはまた別にとる）
        if (IsPlayer != CHARACTERCODE.PLAYER)
        {
            return CheckOUTValue;
        }
        // 45度単位でカウントを取り、N(=0，0）の後にその方向が2回あればその方向を返す
        // 保存用配列
        // 0:N
        // 1:X:Cos(-22.5)～Cos22.5,Y:Sin(-22.5)～Sin22.5
        // 2:X:Cos22.5～Cos67.5,Y:Sin22.5-Sin67.5
        // ↓
        // 8まで

        InputPattern[] inputcount = new InputPattern[8];

        // 一応初期化しておく
        for (int i = 0; i < inputcount.Length; i++)
        {
            inputcount[i] = new InputPattern(i, 0, new Vector2(0, 0));
        }

        // PastInputsの値を拾い、N→どこかへ入力があるかどうか調べる。-2はラストがPastInputs.Length - 1になるための帳尻合わせ（PastInputs.Length - 1が最後なので更に1を引かないとインデックスオーバー）
        for (int i = 0; i < m_PastInputs.Length - 2; i++)
        {
            // Nがある？
            if (m_PastInputs[i].x == 0 && m_PastInputs[i].y == 0)
            {
                // そのあとNでない？(=最低片方0でない）
                if (m_PastInputs[i + 1].x != 0 || m_PastInputs[i + 1].y != 0)
                {
                    // 入力方向は8方向のうちのどれ？
                    // ベクトルを角度に変換する(degへ)
                    float nowrot = Mathf.Atan2(m_PastInputs[i + 1].y, m_PastInputs[i + 1].x) * Mathf.Rad2Deg;
                    for (int j = 0; j < 8; j++)
                    {
                        // 左より前
                        if (j < 4)
                        {
                            if (nowrot >= rot1[j] && nowrot < rot2[j])
                            {
                                inputcount[j].InputCount++;
                                inputcount[j].InputRot = j;
                                inputcount[j].InputVector.x = m_PastInputs[i + 1].x;
                                inputcount[j].InputVector.y = m_PastInputs[i + 1].y;
                            }
                        }
                        // 左
                        else if (j == 4)
                        {
                            if (nowrot >= 157.5f || nowrot < -157.5f)
                            {
                                inputcount[4].InputCount++;
                                inputcount[4].InputRot = 4;
                                inputcount[4].InputVector.x = m_PastInputs[i + 1].x;
                                inputcount[4].InputVector.y = m_PastInputs[i + 1].y;
                            }
                        }
                        // 左より後
                        else
                        {
                            if (nowrot >= rot1[j - 1] && nowrot < rot2[j - 1])
                            {
                                inputcount[j].InputCount++;
                                inputcount[j].InputRot = j;
                                inputcount[j].InputVector.x = m_PastInputs[i + 1].x;
                                inputcount[j].InputVector.y = m_PastInputs[i + 1].y;
                            }
                        }
                    }
                }

            }
        }
        // 全入力パターンのカウントを取る(降順ソートなのでソートした後ひっくり返す)
        Array.Sort(inputcount);
        // ひっくり返す
        Array.Reverse(inputcount);

        // 過去に同様の入力が2回以上あった場合、その方向を返す
        if (inputcount[0].InputCount > 2)
        {
            // ステップの方向ベクトルも確定しておく
            return inputcount[0].InputVector;
        }

        return CheckOUTValue;
    }

    // 過去のジャンプ入力を保持
    protected enum JumpInputState
    {
        NONE,   // なし
        PUSH,   // 押した（押しっぱなし含む）
        PULL,   // 離した
    };
    protected JumpInputState[] m_PastInputs_Jump = new JumpInputState[30];
    
    // 初期化
    protected void ResetPastInputsJump()
    {
        for (int i = 0; i < m_PastInputs_Jump.Length; i++)
        {
            m_PastInputs_Jump[i] = JumpInputState.NONE;
        }
    }

    

    // 操作系関数
    // CPUフラグが立っているときはやらない（連動して動いてしまう）

    // 何を入力したか取得する
    // 方向キーの量を算出する
    protected Vector2 CheckVHInput()
    {
        Vector2 Value = new Vector2(0, 0);

        Value.x = Input.GetAxisRaw("Horizontal");
        Value.y = Input.GetAxisRaw("Vertical");
        return Value;
    }


    // 移動キーが押されているかどうかをチェックする HasVHInput を追加する。
    protected bool GetVHInput()
    {
        // PC時のみ。CPU時は何か別の関数を用意
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (0.0f != Input.GetAxisRaw("Horizontal"))
            {
                return true;
            }

            return (0.0f != Input.GetAxisRaw("Vertical"));
        }
        else
        {            
            return false;
        }
    }

    // 方向キー入力（上）が押されているかをチェックする（横が入っていたらアウト）
    protected bool GetFrontInput()
    {
        if (Input.GetAxisRaw("Vertical") > 0.0f && Math.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
        {
            return true;
        }
        return false;
    }
    // 方向キー入力（下）が押されているかをチェックする（横が入っていたらアウト）
    protected bool GetBackInput()
    {
        if (Input.GetAxisRaw("Vertical") < 0.0f && Math.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
        {
            return true;
        }
        return false;
    }

    // 方向キー入力（左）が押されているかをチェックする（縦が入っていたらアウト）
    protected bool GetLeftInput()
    {
        if (Input.GetAxisRaw("Horizontal") < 0.0f && Math.Abs(Input.GetAxisRaw("Vertical")) < 0.1f)
        {
            return true;
        }
        return false;
    }
    // 方向キー入力（右）が押されているかをチェックする（縦が入っていたらアウト）
    protected bool GetRightInput()
    {
        if (Input.GetAxisRaw("Horizontal") > 0.0f && Math.Abs(Input.GetAxisRaw("Vertical")) < 0.1f)
        {
            return true;
        }
        return false;
    }

    // ショット入力があったか否かをチェックする
    protected bool GetShotInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // GetButtonDownは瞬間をとる
            if(Input.GetButtonDown("Shot"))
            {
                return true;
            }
        }        
        return false;        
    }

    // 射撃入力が押しっぱなしであるか離されたかをチェックする
    // ret      :チャージ完了状態で離されたか否か
    protected bool GetShotChargeInput()
    {
        bool compleate = false;
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 入力中はm_ShotChargeを増加
            if (Input.GetButton("Shot") || Input.GetButton("EX_Shot") || Input.GetButton("Arousal"))
            {
                if (ShotCharge < 0)
                {
                    ShotCharge = 0;
                }
                ShotCharge += this.ShotIncrease;
            }
            // 解除するとm_ShotChargeを減衰
            else
            {
                if (ShotCharge > 0)
                {
                    ShotCharge -= this.ShotDecrease;
                }
            }
        }
        // MAX状態で離されるとチャージ量を0にしてtrue
        if (ShotCharge >= ChargeMax && (Input.GetButtonUp("Shot") || Input.GetButtonUp("EX_Shot") || Input.GetButtonUp("Arousal")))
        {
            ShotCharge = 0;
            compleate = true;
        }
        return compleate;
    }

    // 格闘入力が押しっぱなしであるか離されたかをチェックする
    // ret      :チャージ完了状態で離されたか否か
    protected bool GetWrestleChargeInput()
    {
        bool compleate = false;
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 入力中はm_ShotChargeを増加
            if (Input.GetButton("Wrestle") || Input.GetButton("EX_Wrestle") || Input.GetButton("Arousal"))
            {
                if (WrestleCharge < 0)
                {
                    WrestleCharge = 0;
                }
                WrestleCharge += this.WrestleIncrease;
            }
            // 解除するとm_ShotChargeを減衰
            else
            {
                if (this.WrestleCharge > 0)
                {
                    WrestleCharge -= this.WrestleDecrease;
                }
            }
        }
        // MAX状態で離されるとチャージ量を0にしてtrue
        if (WrestleCharge >= ChargeMax && (Input.GetButtonUp("Wrestle") || Input.GetButtonUp("EX_Wrestle") || Input.GetButtonUp("Arousal")))
        {
            WrestleCharge = 0;
            compleate = true;
        }
        return compleate;
    }

    // ジャンプ入力があったか否かをチェックする
    protected bool GetJumpInput()
    {
        // 長押し上昇があるのでこっちは長押しを認める
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (Input.GetButton("Jump"))
            {
                return true;
            }
        }
       
        return false;
    }    

    // ダッシュキャンセル入力があったか否かをチェックする
    protected bool GetDashCancelInput()
    {
        // 今までの値を１つずつ上へ移動させる
        for (int i = 0; i < m_PastInputs_Jump.Length - 1; i++)
        {
            m_PastInputs_Jump[i] = m_PastInputs_Jump[i + 1];
        }
        // 現在の値を取得して最後に持ってくる
        //if (Input.GetButtonDown("Jump") || Input.GetButton("Jump"))
		if (Input.GetButton("Jump"))
		{
            m_PastInputs_Jump[m_PastInputs_Jump.Length - 1] = JumpInputState.PUSH;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            m_PastInputs_Jump[m_PastInputs_Jump.Length - 1] = JumpInputState.PULL;
        }
        else
        {
            m_PastInputs_Jump[m_PastInputs_Jump.Length - 1] = JumpInputState.NONE;
        }
        // 最後まで数えて、PUSH→PULL→PUSHという流れがあればtrueを返す(連続入力を避けるために、trueを返す時は過去入力を一旦リセットする）
        for (int i = 0; i < m_PastInputs_Jump.Length; i++)
        {
            bool push = false;
            if (m_PastInputs_Jump[i] == JumpInputState.PUSH)
            {
                push = true;
            }
            // pushを拾った
            if (push)
            {
                // そこから数えた先にpullかnoneがあるか？
                for (int j = i; j < m_PastInputs_Jump.Length; j++)
                {
                    if (m_PastInputs_Jump[j] == JumpInputState.PULL || m_PastInputs_Jump[j] == JumpInputState.NONE)
                    {
                        // pullがあった場合、その先にpushがあるか？
                        for (int k = j; k < m_PastInputs_Jump.Length; k++)
                        {
                            if (m_PastInputs_Jump[k] == JumpInputState.PUSH)
                            {
                                // 過去入力をリセット
                                for (int x = 0; x < m_PastInputs_Jump.Length; x++)
                                {
                                    m_PastInputs_Jump[x] = JumpInputState.NONE;
                                }
                                this.m_DashCancelTime = Time.time;
                                return true;
                            }
                        }
                    }
                }
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
            if (Input.GetButtonDown("Search"))
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
            if (Input.GetAxisRaw("Search") > 0)
            {
                this.m_RockonButtonPress++;               
            }
            else if (Input.GetButtonUp("Search"))
            {
                if (this.m_RockonCancelTime <= this.m_RockonButtonPress)
                {
                    this.m_RockonButtonPress = 0;
                    return true;
                }
                this.m_RockonButtonPress = 0;               
            }
        }
        return false;
    }

    // 格闘入力があったか否か
    protected bool GetWrestleInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (Input.GetButtonDown("Wrestle"))
            {
                return true;
            }
        }
        return false;
    }
    // サブ射撃入力があったか否か(判定の都合上優先度はノーマル射撃及び格闘より上にすること)
    protected bool GetSubShotInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 同時押し（射撃＋格闘）
            if (Input.GetButtonDown("Wrestle") && Input.GetButtonDown("Shot"))
            {
                return true;
            }
            // サブ射撃ボタン
            else if (Input.GetButtonDown("Sub_Shot"))
            {
                return true;
            }
        }
        return false;
    }
    // 特殊射撃入力があったか否か(判定の都合上優先度はノーマル射撃及びジャンプより上にすること)
    protected bool GetExShotInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 同時押し（射撃＋ジャンプ）
            if (Input.GetButtonDown("Shot") && Input.GetButtonDown("Jump"))
            {
                return true;
            }
            // サブ射撃ボタン
            else if (Input.GetButtonDown("EX_Shot"))
            {
                return true;
            }
        }
        return false;
    }
    // 特殊格闘入力があったか否か(判定の都合上優先度はノーマル射撃及びジャンプより上にすること)
    protected bool GetExWrestleInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 同時押し（格闘＋ジャンプ）
            if (Input.GetButtonDown("Wrestle") && Input.GetButtonDown("Jump"))
            {
                return true;
            }
            // サブ射撃ボタン
            else if (Input.GetButtonDown("EX_Wrestle"))
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
            if (Input.GetButtonDown("Item"))
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
                    if (!savingparameter.ItemUseCheck(nowitem,0))
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
                    AudioSource.PlayClipAtPoint(RebirthHP1, transform.position);                    
                }
                // 蘇生（パーティーを組めるようになってから）
                else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_DEATH)
                {
                    // エフェクト作成
                    ItemEffect = Resources.Load("Resurrection1");
                    // SE再生
                    AudioSource.PlayClipAtPoint(Regenaration, transform.position);
                }
                // SG浄化
                else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_SOUL)
                {
                    // エフェクト作成
                    ItemEffect = Resources.Load("PurificationGem");                    
                    // SE再生
                    AudioSource.PlayClipAtPoint(PurificationSG, transform.position);
                }
                // 蘇生しつつ完全回復（蘇生はパーティーを組めるようになってから）
                else if (Item.itemspec[nowitem].ItemFuciton() == ItemSpec.ItemFunction.REBIRTH_FULL)
                {
                    // エフェクト作成
                    ItemEffect = Resources.Load("Resurrection1");
                    // SE再生
                    AudioSource.PlayClipAtPoint(Regenaration, transform.position);
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

    // ポーズ入力があったか否か
    protected bool HasMenuInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            if (Input.GetButtonDown("Pause"))
            {
                return true;
            }
        }
        return false;
    }
    // 覚醒入力があったか否か(優先度の都合上一番高くする)
    protected bool GetArousalInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 射撃・格闘・ジャンプ
            if (Input.GetButtonDown("Shot") && Input.GetButtonDown("Wrestle") && Input.GetButtonDown("Jump"))
            {
                return true;
            }
            // 特殊射撃＋格闘
            else if (Input.GetButtonDown("EX_Shot") && Input.GetButtonDown("Wrestle"))
            {
                return true;
            }
            // 特殊格闘＋射撃
            else if (Input.GetButtonDown("EX_Wrestle") && Input.GetButtonDown("Shot"))
            {
                return true;
            }
            else if (Input.GetButtonDown("Arousal"))
            {
                return true;
            }
        }
        return false;
    }
    // ソウルバースト入力があったか否か
    protected bool HasSoulBurstInput()
    {
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 射撃・格闘・ジャンプ・サーチ
            if (Input.GetButtonDown("Shot") && Input.GetButtonDown("Wrestle") && Input.GetButtonDown("Jump") && Input.GetButtonDown("Search"))
            {
                return true;
            }
            // 特殊射撃＋格闘＋サーチ
            else if (Input.GetButtonDown("EX_Shot") && Input.GetButtonDown("Wrestle") && Input.GetButtonDown("Search"))
            {
                return true;
            }
            // 特殊格闘＋射撃＋サーチ
            else if (Input.GetButtonDown("EX_Wrestle") && Input.GetButtonDown("Shot") && Input.GetButtonDown("Search"))
            {
                return true;
            }
            // 覚醒＋サーチ
            else if (Input.GetButtonDown("Arousal") && Input.GetButtonDown("Search"))
            {
                return true;
            }
        }
        return false;
    }

    // 動作系関数
    // 入力内容に応じて動作させ、規定フレームで次の動作を実行（何をやるかはキャラ任せ。キャラごとにオーバーライドすること）
    // また、F単位で加減算系の数値は時間停止時の処理に注意


    // ジャンプ時共通操作
    protected void JumpDone()
    {
        m_AnimState[0] = AnimationState.Jump;
        this.Boost = this.Boost - this.m_JumpUseBoost;
        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Jump]);
    }
      

    // アニメ(ステップ判定用はちと特殊なので共通とは別に用意)
    AnimationState[] m_anims = 
    { 
        AnimationState.RightStep, 
        AnimationState.RightStep,
        AnimationState.FrontStep,
        AnimationState.LeftStep,
        AnimationState.LeftStep,
        AnimationState.BackStep,
        AnimationState.RightStep,
    };

    // ステップ中フラグ

    private bool Stepdone;

    // ステップ時共通操作
    // 第1引数：Y方向への上昇量
    // 第2引数：入力の方向
    // 第3引数：虹エフェクトにするか否か（通常はfalse)
    protected virtual void StepDone(float Yforce, Vector2 inputVector,bool rainbow = false)
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
        
        // 左以外
        for (int i = 0; i < 7; i++)
        {
            if (nowrot >= rot1[i] && nowrot < rot2[i])
            {
                m_AnimState[0] = m_anims[(int)i];
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
                    this.transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
               
                    this.GetComponent<Animation>().Play(m_AnimationNames[(int)m_anims[(int)i]]);
                    break;
                }
                // 非ロックオン時
                else
                {
                    this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.FrontStep]);
                    break;
                }
            }
        }

        // 左(Unityは±180までしか取れんのよ・・・）
        if (nowrot >= 157.5 || nowrot < -157.5f)
        {
            m_AnimState[0] = m_anims[3];
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
                this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.LeftStep]);
            }
            // 非ロックオン時
            else
            {
                this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.FrontStep]);
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
                double nowrotY = transform.rotation.eulerAngles.y * Math.PI/180;
                // 方向ベクトル算出：X
                float x = (float)Math.Sin(nowrotY - Math.PI / 2);
                // 方向ベクトル算出：Z
                float z = (float)Math.Sin(nowrotY);
                MoveDirection = new Vector3(x, 0, z);
            }
            // 右上・右・右下へ進む場合
            else
            {
                // 進行方向(rad)
                double nowrotY = transform.rotation.eulerAngles.y * Math.PI / 180;
                // 方向ベクトル算出：X
                float x = (float)Math.Cos(nowrotY);
                // 方向ベクトル算出：Z
                float z = (float)Math.Sin(nowrotY - Math.PI);
                MoveDirection = new Vector3(x, 0, z);
            }            
        }

        // 格闘キャンセルステップは入力の関係でMoveDirectinが相手の方向を向いているため、MoveDirectionを再設定する
        if (rainbow)
        {
            this.MoveDirection = this.StepRotation * Vector3.forward;
        }
       
        // どうも無理っぽい（普通のジャンプに化けてしまい、加速度演算が無理？）
        // 急加速をかけたい場合は力積モードにする（第4引数で指定可能）→ジャンプに化ける。というか、おそらくこれではステップは無理
        //rigidbody.AddForce(this.StepForce * PastInputs[0].x, Yforce, this.StepForce * PastInputs[0].y);
        _StepStartTime = Time.time;
    }

    private float _StepStartTime;  // ステップの開始時間

    // ステップ終了時処理
    protected virtual void EndStep()
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
                m_AnimState[0] = AnimationState.Idle;
                // ブースト量を初期化する
                this.Boost = GetMaxBoost(this.BoostLevel);
            }
            // 空中にいたら角度を戻して落下
            else
            {                
                FallDone(new Vector3(0, 0, 0));
            }
            // アニメーションを戻す
            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Idle]);
            // 入力をリセットする
            ResetPastInputs();
        }
    }

   

    // 攻撃系処理（オーバーライド前提。基本攻撃系処理は空にしておいて、使うところでオーバーライドすること）
    // 通常射撃処理
    protected virtual void Shot()
    {
        m_AnimState[1] = AnimationState.Shot;
        if (IsGrounded)
        {
            Boost = GetMaxBoost(BoostLevel);
        }
    }

    // 歩き撃ち処理
    protected virtual void ShotRun()
    {
        // 終了判定の不具合を起こすのでここには書かない
        //AnimState[1] = AnimationState.Shot_run;
    }

    // 空中ダッシュ通常射撃処理
    protected virtual void ShotAirDash()
    {
        // ブースト切れ時にFallDone、DestroyWrestleを実行する
        if (Boost <= 0)
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Fall;
            FallStartTime = Time.time;
        }
        // 着地時にLandingを実行する
        if (IsGrounded)
        {
            LandingDone();
        }
    }

    // チャージ射撃処理
    protected virtual void ChargeShot()
    {
    }

    // サブ射撃処理
    protected virtual void SubShot()
    {
    }

    // 特殊射撃処理
    protected virtual void ExShot()
    {
    }

    // 格闘の累積時間
    protected float Wrestletime;
    // N格1段目
    protected virtual void Wrestle1()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();        
    }

    // N格2段目
    protected virtual void Wrestle2()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // N格3段目
    protected virtual void Wrestle3()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    protected virtual void StepCancel()
    {
        // キャンセルダッシュ入力を受け取ったら、キャンセルして空中ダッシュする
        if (this.HasDashCancelInput)
        {
            this.AddInput = false;
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            CancelDashDone();
        }
        // ステップ入力を受け取ったら、キャンセルしてステップする
        else if (StepCheck().x != 0 || StepCheck().y != 0)
        {
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            // ステップキャンセル成功の証しとしてエフェクトが虹になる
            StepDone(1, StepCheck(),true);
        }
        // CPU時左ステップ
        else if (HasLeftStepInput)
        {
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            StepDone(1, new Vector2(-1, 0),true);
            HasLeftStepInput = false;
        }
        // CPU時右ステップ
        else if (HasRightStepInput)
        {
            // くっついている格闘判定を捨てる
            DestroyWrestle();
            StepDone(1, new Vector2(1, 0),true);
            HasRightStepInput = false;
        }
    }

    // 格闘チャージ
    protected virtual void ChargeWrestle()
    {
    }

    // 前格闘1段目
    protected virtual void FrontWrestle1()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 前格闘2段目
    protected virtual void FrontWrestle2()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 前格闘3段目
    protected virtual void FrontWrestle3()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 左横格闘1段目
    protected virtual void LeftWrestle1()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 左横格闘2段目
    protected virtual void LeftWrestle2()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 左横格闘3段目
    protected virtual void LeftWrestle3()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 右横格闘1段目
    protected virtual void RightWrestle1()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 右横格闘2段目
    protected virtual void RightWrestle2()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 右横格闘3段目
    protected virtual void RightWrestle3()
    {
        Wrestletime += Time.deltaTime;
        StepCancel();
    }

    // 後格闘（防御）
    protected virtual void BackWrestle()
    {
        //1．ブーストゲージを減衰させる
        Boost -= Time.deltaTime * 5.0f;
        //2．ブーストゲージが0になると、強制的にIdleに戻す
        if (Boost <= 0)
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Idle;
        }
        //3．格闘ボタンか下入力を離すと、強制的にIdleに戻す
        if (Input.GetButtonUp("Wrestle") || Input.GetAxisRaw("Vertical") > 0.0f)
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Idle;
        }
    }

    // 空中ダッシュ格闘
    protected virtual void AirDashWrestle()
    {
        StepCancel();
        // 発動中常時ブースト消費
        this.Boost = this.Boost - this.m_BoostLess;
        // ブースト切れ時にFallDone、DestroyWrestleを実行する
        if (Boost <= 0)
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Fall;
            FallStartTime = Time.time;
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

    // 前特殊格闘1段目（全員共通で上昇技）
    protected virtual void FrontExWrestle1()
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
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Fall;
            FallStartTime = Time.time;
        }
        StepCancel();
    }

    // 前特殊格闘2段目
    protected virtual void FrontExWrestle2()
    {
    }

    // 前特殊格闘3段目
    protected virtual void FrontExWrestle3()
    {
    }

    // 左横特殊格闘1段目
    protected virtual void LeftExWrestle1()
    {
    }

    // 左横特殊格闘2段目
    protected virtual void LeftExWrestle2()
    {
    }

    // 左横特殊格闘3段目
    protected virtual void LeftExWrestle3()
    {
    }

    // 右横特殊格闘1段目
    protected virtual void RightExWrestle1()
    {
    }

    // 右横特殊格闘2段目
    protected virtual void RightExWrestle2()
    {
    }

    // 右横特殊格闘3段目
    protected virtual void RightExWrestle3()
    {
    }

    // 後特殊格闘（全員共通で下降技）
    protected virtual void BackExWrestle()
    {
        // 毎フレームブーストを消費する
        Boost -= Time.deltaTime * 100;
        // ブーストが0になったらFallにする
        if (Boost <= 0)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
            DestroyWrestle();
            m_AnimState[0] = AnimationState.Fall;
            FallStartTime = Time.time;
        }
        StepCancel();
        // 接地したらLandingにする
        if (IsGrounded)
        {
            // 判定オブジェクトを破棄する.一応くっついているものはすべて削除
            DestroyWrestle();
            LandingDone();
        }
    }

    // 覚醒技の初期化を実行したか？
    protected bool InitializeArousal;

    // 覚醒技発動中
    protected virtual void ArousalAttack()
    {
        m_AnimState[0] = AnimationState.Arousal_Attack;
    }

    // ワールド座標でのカメラの基底ベクトルを計算し、それを基にメッシュの回転を計算する
    protected void UpdateRotation()
    {
        var finalRot = transform.rotation;
        var horizontal = Input.GetAxisRaw("Horizontal");        // 横入力を検出
        var vertical = Input.GetAxisRaw("Vertical");            // 縦入力を検出
        var toWorldVector = this.MainCamera.transform.rotation;
        // ベクトルは平行移動の影響を受けないので逆行列は使わない
        // スケールの影響は受けるがここでは無視する。

        // CPU時、ここで入力を取得
        if (this.IsPlayer != CHARACTERCODE.PLAYER)
        {
            // CPU情報
            var CPU = this.GetComponentInChildren<AIControl_Base>();
            // テンキー入力を拾う
            this.m_tenkey = CPU.m_tenkeyoutput;
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
            Vector3 myPos = this.transform.position;


            // XとZの差が一定値以下で移動方向固定(空中移動時限定）
            if (!this.IsGrounded)
            {
                if (Math.Abs(targetpos.x - myPos.x) < 10.0f && Math.Abs(targetpos.z - myPos.z) < 10.0f)
                {
                    this.Rotatehold = true;
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
        var horizontal = Input.GetAxisRaw("Horizontal");        // 横入力を検出
        var vertical = Input.GetAxisRaw("Vertical");            // 縦入力を検出
        var toWorldVector = this.MainCamera.transform.rotation;
		
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
        this.StepRotation = finalRot;

    }
    

    // 空中ダッシュ（キャンセルダッシュ）発動共通操作
    // 弓ほむら・まどかのモーションキャンセルなどはこの前に行うこと
    protected virtual void CancelDashDone()
    {
        if (this.Boost > 0)
        {
            // 格闘判定削除
            DestroyWrestle();
            // 一応歩き撃ちフラグはここでも折る
            RunShotDone = false;
            // 上体の角度を戻す
            this.BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            m_AnimState[0] = AnimationState.AirDash;
            this.Rotatehold = false;
            this.Boost = this.Boost - this.DashCancelUseBoost;
            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.AirDash]);
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

    // 下降共通動作
    protected void FallDone(Vector3 RiseSpeed)
    {
		// 過去入力をリセット
		for (int x = 0; x < m_PastInputs_Jump.Length; x++)
		{
			m_PastInputs_Jump[x] = JumpInputState.NONE;
		}
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
        //this.animation.Play(m_AnimationNames[(int)AnimationState.Fall]);
        RiseSpeed = new Vector3(0, -this.RiseSpeed, 0);
        //this.m_MoveDirection = new Vector3(0, 0, 0);
        m_AnimState[0] = AnimationState.Fall;
        FallStartTime = Time.time;
    }
    // 着地共通動作
    protected void LandingDone()
    {
        // 格闘判定削除
        DestroyWrestle();
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0,this.transform.rotation.eulerAngles.y,0)); 
        // 無効になっていたら重力を復活させる
        this.GetComponent<Rigidbody>().useGravity = true;
        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Landing]);
        m_AnimState[0] = AnimationState.Landing;
        // 着地したので硬直を設定する
        this.LandingTime = Time.time;
    }

    // 着地後共通動作
    protected void LandingDone2(string file)
    {
        m_AnimState[1] = AnimationState.Landing;
        // 地響き防止
        this.MoveDirection = transform.rotation * new Vector3(0, 0, 0);
        // モーション終了時にアイドルへ移行
        // 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
        if (Time.time > this.LandingTime + this._LandingWaitTime)
        {
            m_AnimState[0] = AnimationState.Idle;
            this.GetComponent<Animation>().Play(file);
            // ブースト量を初期化する
            this.Boost = GetMaxBoost(this.BoostLevel);
        }
    }


	// 継承先のStart開幕で実行すること
	protected virtual void FirstSetting(Animator animator) 
	{
        // CharacterControllerを取得
        //this.m_charactercontroller = GetComponent<CharacterController>();
        // Rigidbodyを取得
        Rigidbody = GetComponent<Rigidbody>();

        // PCでなければカメラを無効化しておく
        if (this.IsPlayer != CHARACTERCODE.PLAYER)
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
        LayOriginOffs = new Vector3(0.0f, Collider_Height, 0.0f);
        Laylength = collider.radius + collider.height;// / 2 + 1.5f;//0.2f;
        //this.m_layOriginOffs = new Vector3(0.0f, m_Collider_Height, 0.0f);
        //this.m_laylength = m_charactercontroller.radius + m_charactercontroller.height / 2 + 1.5f;
        LayMask = 1 << 8;       // layMaskは無視するレイヤーを指定する。8のgroundレイヤー以外を無視する

        // アニメーションをIdleに初期化
        this.m_AnimState[0] = this.m_AnimState[1] = AnimationState.Idle;
        this.CurAnimPos = 0;

        // ジャンプ硬直
        JumpTime = -this.m_JumpWaitTime;

        // 着地硬直
        LandingTime = -_LandingWaitTime;

        // 歩行速度(m_WalkSpeed等はそれぞれで設定）
        MoveDirection = Vector3.zero;
        // 吹き飛び速度
        BlowDirection = Vector3.zero;

        // 初期アニメIdleを再生する
        this.GetComponent<Animation>().Play(this.m_AnimationNames[(int)AnimationState.Idle]);

        // ブースト量を初期化する
        Boost = GetMaxBoost(this.BoostLevel);
        
		// 上体を初期化する
        BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
		// 歩き撃ちフラグを初期化する
        RunShotDone = false;

        // 覚醒ゲージ量を初期化する
        Arousal = GetMaxArousal(this.m_ArousalLevel);
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
            int charactername = (int)this.m_character_name;
            // HP
            NowHitpoint = savingparameter.GetNowHP(charactername);
            // 覚醒ゲージ
            Arousal = savingparameter.GetNowArousal(charactername);
            // ソウルジェム汚染率
            m_GemContamination = savingparameter.GetGemContimination(charactername);
        }
        // 敵の時HP、覚醒ゲージ、ソウルジェム汚染率を初期化する
        else
        {
            NowHitpoint = GetMaxHitpoint(this.Level);
            Arousal = 0;
            m_GemContamination = 0;
        }

        // ロックオンを初期化する
        IsRockon = false;
        this.m_RockonButtonPress = 0;
        this.m_RockonCancelTime = 30;

        // 進行方向固定フラグを初期化する
        Rotatehold = false;

        // アーマー状態を初期化する
        IsArmor = false;

        // 過去入力を初期化する
        ResetPastInputs();
        ResetPastInputsJump();

        // 時間停止のルートを初期化する
        TimeStopMaster = false;

        // ダウン値の閾値を初期化する
        DownRatio = MadokaDefine.DOWNRATIO;

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
        this.m_hasJumpInput = false;
        // ダッシュキャンセル入力があったか否か
        this.HasDashCancelInput = false;
        // 空中ダッシュ入力があったか否か
        this.m_hasAirDashInput = false;
        // サーチ入力があったか否か
        this.m_hasSearchInput = false;
        // サーチキャンセル入力があったか否か
        this.m_hasSearchCancelInput = false;
        // 格闘入力があったか否か
        this.m_hasWrestleInput = false;
        // サブ射撃入力があったか否か
        this.m_hasSubShotInput = false;
        // 特殊射撃入力があったか否か
        this.m_hasExShotInput = false;
        // 特殊格闘入力があったか否か
        this.m_hasExWrestleInput = false;
        // アイテム入力があったか否か
        this.m_hasItemInput = false;
        // ポーズ入力があったか否か
        this.m_hasPauseInput = false;
        // 覚醒入力があったか否か
        this.m_hasArousalInput = false;
        // 覚醒技入力があったか否か
        this.m_hasArousalAttackInput = false;
        // 前入力があったか否か
        this.m_hasFrontInput = false;
        // 左入力があったか否か
        this.m_hasLeftInput = false;
        // 右入力があったか否か
        this.m_hasRightInput = false;
        // 後入力があったか否か
        this.m_hasBackInput = false;
        // ソウルバースト入力があったか否か
        this.m_hasSoulBurstInput = false;

        // m_DownRebirthTime/waitの初期化とカウントを行う
        // m_DamagedTime/waitの初期化とカウントを行う
        // m_DownTime/waitの初期化とカウントを行う

        // m_DownRebirthTime/waitの初期化を行う(ダウン値がリセットされるまでの時間）
        m_DownRebirthTime = 0;
        m_DownRebirthWaitTime = 3.0f;
        // m_DamagedTime/waitの初期化とカウントを行う(ダメージによる硬直時間）
        m_DamagedTime = 0;
        m_DamagedWaitTime = 1.0f;
        // m_DownTimeの初期化とカウントを行う(ダウン時の累積時間）
        DownTime = 0;
        m_DownWaitTime = 3.0f;

        // 復帰時のブースト消費量
        m_ReversalUseBoost = 20.0f;

        // 1Fあたりの射撃チャージゲージ増加量
        ShotIncrease = 2;
        // 1Fあたりの格闘チャージゲージ増加量
        WrestleIncrease = 2;
        // 1Fあたりの射撃チャージゲージ減衰量
        ShotDecrease = 4;
        // 1Fあたりの格闘チャージゲージ減衰量
        WrestleDecrease = 4;

        // チャージ最大値
        ChargeMax = 100;

        // インターフェースの描画フラグ
        if (IsPlayer == CHARACTERCODE.PLAYER)
            m_DrawInterface = true;
        else
            m_DrawInterface = false;

        // 壁面接触フラグ
        m_hitjumpover = false;
        m_hitunjumpover = false;
        // リロードクラス作成
        ReloadSystem = new Reload();

        // ポーズコントローラー取得
        m_pausecontroller = GameObject.Find("Pause Controller");

        // 足音取得(StageSettingで決めているので、その値を拾う
        GameObject stagesetting = GameObject.Find("StageSetting");
        if (stagesetting != null)
        {
            var ss = stagesetting.GetComponent<StageSetting>();
            int ft = ss.getFootType();
            if (ft == 0)
            {
                m_foottype = StageSetting.FootType.FootType_Normal;
            }
            else if (ft == 1)
            {
                m_foottype = StageSetting.FootType.FootType_Wood;
            }
            else if (ft == 2)
            {
                m_foottype = StageSetting.FootType.FootType_Jari;
            }
            else if (ft == 3)
            {
                m_foottype = StageSetting.FootType.FootType_Snow;
            }
            else if (ft == 4)
            {
                m_foottype = StageSetting.FootType.FootType_Carpet;
            }
        }
        // ゲームオーバー表示の文字列を初期化
        GameOverString_Init();
        
	}
       

    // 継承先のUpdate開幕で実行すること
    // また、時間停止状態の場合、falseを返す
    protected bool Update_Core()
    {
        // 接地判定
        IsGrounded = onGround2();       
                
        // ESC強制終了（後で直す）
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // PC死亡時、PauseControllerを消す(メニュー画面に移行させないため）
        // また、ショット入力でタイトル画面へ移行する
        if (NowHitpoint < 1 && IsPlayer == CHARACTERCODE.PLAYER)
        {
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
					FadeManager.Instance.LoadLevel("Prologue3", 1.0f);
                }
                else
                {
					FadeManager.Instance.LoadLevel("title", 1.0f);
                }
            }
        }

        
        // プレイヤー操作の場合（CPU操作の場合はまた別に）
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 方向キー取得
            this.HasVHInput = GetVHInput();
            // ショット入力があったか否か
            this.HasShotInput = GetShotInput();
            // ジャンプ入力があったか否か
            this.m_hasJumpInput = GetJumpInput();
            // ダッシュキャンセル入力があったか否か
            this.HasDashCancelInput = GetDashCancelInput();
            // サーチ入力があったか否か
            this.m_hasSearchInput = GetSearchInput();
            // サーチキャンセル入力があったか否か
            this.m_hasSearchCancelInput = GetUnSerchInput();
            // 格闘入力があったか否か
            this.m_hasWrestleInput = GetWrestleInput();
            // サブ射撃入力があったか否か
            this.m_hasSubShotInput = GetSubShotInput();
            // 特殊射撃入力があったか否か
            this.m_hasExShotInput = GetExShotInput();
            // 特殊格闘入力があったか否か
            this.m_hasExWrestleInput = GetExWrestleInput();
            // アイテム入力があったか否か
            this.m_hasItemInput = GetItemInput();
            // ポーズ入力があったか否か
            this.m_hasPauseInput = HasMenuInput();
            // 覚醒入力があったか否か
            this.m_hasArousalInput = GetArousalInput();
            // 覚醒技入力があったか否か
            this.m_hasArousalAttackInput = GetArousalInput();
            // 前入力があったか否か
            this.m_hasFrontInput = GetFrontInput();
            // 左入力があったか否か
            this.m_hasLeftInput = GetLeftInput();
            // 右入力があったか否か
            this.m_hasRightInput = GetRightInput();
            // 後入力があったか否か
            this.m_hasBackInput = GetBackInput();
            // ソウルバースト入力があったか否か
            this.m_hasSoulBurstInput = HasSoulBurstInput();
            // 位置を保持
            savingparameter.nowposition = this.transform.position;
            // 角度を保持
            savingparameter.nowrotation = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        }
        // CPU操作の場合（ステップのフラグもここで拾う）
        else
        {
            // CPU情報
            var CPU = this.GetComponentInChildren<AIControl_Base>();
            // キー入力を拾う
            this.m_key = CPU.m_keyoutput;
            // テンキー入力を拾う
            this.m_tenkey = CPU.m_tenkeyoutput;
            // テンキー入力に応じてフラグを立てる
            // 前
			if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.TOP)
            {
                m_hasFrontInput = true;
            }
            // 後
			else if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.UNDER)
            {
                m_hasBackInput = true;
            }
            // 左
			else if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.LEFT)
            {
                m_hasLeftInput = true;
            }
            // 右
			else if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.RIGHT)
            {
                m_hasRightInput = true;
            }
            // 左ステップ
			else if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.LEFTSTEP)
            {
                HasLeftStepInput = true;
            }
            // 右ステップ
			else if (m_tenkey == AIControl_Base.TENKEY_OUTPUT.RIGHTSTEP)
            {
                HasRightStepInput = true;
            }
            // 入力を受けたキーごとにフラグを立てる
            // CPUは同時押し判定を出さないので、1個ずつ来ると考えてOK
            switch (this.m_key)
            {
                case AIControl_Base.KEY_OUTPUT.SHOT:
                    this.HasShotInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.JUMP:
                    this.m_hasJumpInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.DASHCANCEL:
                    this.HasDashCancelInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.AIRDASH:
                    this.m_hasAirDashInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.SEARCH:
                    this.m_hasSearchInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.SEACHCANCEL:
                    this.m_hasSearchCancelInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.WRESTLE:
                    this.m_hasWrestleInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.SUBSHOT:
                    this.m_hasSubShotInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.EXSHOT:
                    this.m_hasExShotInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.EXWRESTLE:
                    this.m_hasExWrestleInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.ITEM:
                    this.m_hasItemInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.PAUSE:
                    this.m_hasPauseInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.AROUSAL:
                    this.m_hasArousalInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.AROUSALATTACK:
                    this.m_hasArousalAttackInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.CHARGESHOT:
                    this.m_hasShotChargeInput = true;
                    break;
                case AIControl_Base.KEY_OUTPUT.CHARGEWRESTE:
                    this.m_hasWrestleChargeInput = true;
                    break;  
                default:                    
                    // ショット入力があったか否か
                    this.HasShotInput = false;
                    // ジャンプ入力があったか否か
                    this.m_hasJumpInput = false;
                    // ダッシュキャンセル入力があったか否か
                    this.HasDashCancelInput = false;
                    // サーチ入力があったか否か
                    this.m_hasSearchInput = false;
                    // サーチキャンセル入力があったか否か
                    this.m_hasSearchCancelInput = false;
                    // 格闘入力があったか否か
                    this.m_hasWrestleInput = false;
                    // サブ射撃入力があったか否か
                    this.m_hasSubShotInput = false;
                    // 特殊射撃入力があったか否か
                    this.m_hasExShotInput = false;
                    // 特殊格闘入力があったか否か
                    this.m_hasExWrestleInput = false;
                    // アイテム入力があったか否か
                    this.m_hasItemInput = false;
                    // ポーズ入力があったか否か
                    this.m_hasPauseInput = false;
                    // 覚醒入力があったか否か
                    this.m_hasArousalInput = false;
                    // 覚醒技入力があったか否か
                    this.m_hasArousalAttackInput = false;
                    // 射撃チャージ入力があったか否か
                    this.m_hasShotChargeInput = false;
                    // 格闘チャージ入力があったか否か
                    this.m_hasWrestleChargeInput = false;
                    // 前入力があったか否か
                    this.m_hasFrontInput = false;
                    // 左入力があったか否か
                    this.m_hasLeftInput = false;
                    // 右入力があったか否か
                    this.m_hasRightInput = false;
                    // 後入力があったか否か
                    this.m_hasBackInput = false;
                    // ソウルバースト入力があったか否か
                    this.m_hasSoulBurstInput = false;
                    break;
            }
            // 入力を受けたテンキーに応じてフラグを立てる（この時点ではありなしさえ拾えばいい。実際の値を使うのはUpdateRotationなので、入力の有無さえ拾えればいい）
            if (this.m_tenkey != AIControl_Base.TENKEY_OUTPUT.NEUTRAL)
            {
                this.HasVHInput = true;
            }
            else
            {
                this.HasVHInput = false;
            }
        }

        // 死亡時、エフェクトが消滅したら自壊させる
        if (this.Explode)
        {
            if (GetComponentInChildren<Deadfx>() == null)
            {
                Destroy(gameObject);
            }
        }

        

        // 時間停止中falseを強制返し
        // 通常時、ポーズ入力で停止.ただし死んだら無効
        if (m_timstopmode != TimeStopMode.PAUSE)
        {
            if(m_hasPauseInput && NowHitpoint > 0)
            {
                this.TimeStopMaster = true;
                m_timstopmode = TimeStopMode.PAUSE;
                // 動作を止める
                FreezePositionAll();
            }
        }

        

        // 時間停止中にやってほしくない処理はここ以降に記述すること

        // チャージショット関連の入力を取得(時間停止中に減衰されると困るので、ここで管理。また、通常の射撃や格闘より優先度は高くする
        if (IsPlayer == CHARACTERCODE.PLAYER)
        {
            // 射撃
            this.m_hasShotChargeInput = GetShotChargeInput();
            // 格闘
            this.m_hasWrestleChargeInput = GetWrestleChargeInput();
        }
        else
        {
            // CPU情報
            var CPU = this.GetComponentInChildren<AIControl_Base>();
            // キー入力を拾う
            this.m_key = CPU.m_keyoutput;
          
            // 入力を受けたキーごとにフラグを立てる
            // CPUは同時押し判定を出さないので、1個ずつ来ると考えてOK
            switch (this.m_key)
            {
                default:
                    break;
            }
        }
                
        // キャラを取得
        int character = (int)m_character_name;
        // 覚醒入力を取得し、その場合覚醒開始画面へ移行してポーズ  
        if (m_hasArousalInput && !IsArousal && savingparameter.GetGemContimination(character) < 100.0f)
        {
            
            // 最低でもLV1時の覚醒ゲージがないと覚醒不可(ポーズを強制解除して抜ける）
            if (Arousal < GetMaxArousal(1))
            {
                unuseArousal();
            }
            else
            {
                float OR_GemCont = savingparameter.GetGemContimination(character);
                // ソウルジェム汚染率を追加
                savingparameter.SetGemContimination(character, OR_GemCont + MadokaDefine.SOULBURST_CONF);
                arousalStart();           
            }
        }
        // 覚醒中は覚醒技を発動
        else if (m_hasArousalAttackInput && IsArousal)
        {
            // アーマーをONにする
            IsArmor = true;
            // アニメーションを固有のものに変更
            m_AnimState[0] = AnimationState.Arousal_Attack;
            // 覚醒前処理を未実行に変更
            InitializeArousal = false;
        }

        // 覚醒時、覚醒ゲージ減少
        if (IsArousal)
        {
            Arousal -= Time.deltaTime * 10;
            savingparameter.SetNowArousal((int)m_character_name, Arousal);
            if (Arousal <= 0)
            {
                // 覚醒状態解除
                // 覚醒エフェクトを消す
                Destroy(m_arousalEffect);
                // ゲージを0にする
                Arousal = 0;
                // 覚醒フラグを折る
                IsArousal = false;
            }
        }


        //m_nowDownRatioが0を超えていて、Damage_Initではなく（ダウン値加算前にリセットされる）m_DownRebirthTimeが規定時間を経過し、かつダウン値が閾値より小さければダウン値をリセットする
        if (NowDownRatio > 0 && this.m_AnimState[0] != AnimationState.DamageInit)
        {
            if ((Time.time > m_DownRebirthTime + m_DownRebirthWaitTime) && (this.NowDownRatio < this.DownRatio))
            {
                this.m_DownRebirthTime = 0;
                this.NowDownRatio = 0.0f;
            }
        }

        
        // 入力を取得(ステップ中は処理落ちの原因なのでとらない)
        if (m_AnimState[0] != AnimationState.BackStep && m_AnimState[0] != AnimationState.FrontStep && m_AnimState[0] != AnimationState.LeftStep && m_AnimState[0] != AnimationState.RightStep)
        {
            GetPastInputs();
        }
                

        // MoveDirection は アニメーションステート Walk で設定され、アニメーションステートが Idle の場合リセットされる。
        // 移動処理はステートの影響を受けないように UpdateAnimation のステートスイッチの前に処理する。
        // ステートごとに処理をする場合、それをメソッド化してそれぞれのステートに置く。
        // 走行速度を変更する、アニメーションステートが Run だった場合 RunSpeed を使う。
        var MoveSpeed = this.m_RunSpeed;
        // 空中ダッシュ時/空中ダッシュ射撃時
        if (m_AnimState[0] == AnimationState.AirDash || m_AnimState[0] == AnimationState.Shot_AirDash)
        {
            // rigidbodyにくっついている慣性が邪魔なので消す（勝手に落下開始する）
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            MoveSpeed = this.m_AirDashSpeed;
        }
        // 空中慣性移動時
        else if (m_AnimState[0] == AnimationState.Jumping || m_AnimState[0] == AnimationState.Fall)
        {           
            MoveSpeed = this.m_AirMoveSpeed;
        }
        // ステップ時(重力補正カット)
        else if (m_AnimState[0] == AnimationState.BackStep || m_AnimState[0] == AnimationState.FrontStep || m_AnimState[0] == AnimationState.LeftStep || m_AnimState[0] == AnimationState.RightStep)
        {
            this.GetComponent<Rigidbody>().useGravity = false;  // 重力無効
            MoveSpeed = this.m_Step_Initial_velocity;
            this.MoveDirection.y = 0;         // Y移動無効            
        }
        // 格闘時(ガード含む）
        else if (m_AnimState[0] == AnimationState.Wrestle_1 || m_AnimState[0] == AnimationState.Wrestle_2 || m_AnimState[0] == AnimationState.Wrestle_3 ||
                 m_AnimState[0] == AnimationState.Front_Wrestle_1 || m_AnimState[0] == AnimationState.Front_Wrestle_2 || m_AnimState[0] == AnimationState.Front_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.Left_Wrestle_1 || m_AnimState[0] == AnimationState.Left_Wrestle_2 || m_AnimState[0] == AnimationState.Left_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.Right_Wrestle_1 || m_AnimState[0] == AnimationState.Right_Wrestle_2 || m_AnimState[0] == AnimationState.Right_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.Ex_Wrestle_1 || m_AnimState[0] == AnimationState.Ex_Wrestle_2 || m_AnimState[0] == AnimationState.Ex_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.EX_Front_Wrestle_1 || m_AnimState[0] == AnimationState.EX_Front_Wrestle_2 || m_AnimState[0] == AnimationState.EX_Front_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.EX_Left_Wrestle_1 || m_AnimState[0] == AnimationState.EX_Left_Wrestle_2 || m_AnimState[0] == AnimationState.EX_Left_Wrestle_3 ||
                 m_AnimState[0] == AnimationState.EX_Right_Wrestle_1 || m_AnimState[0] == AnimationState.EX_Right_Wrestle_2 || m_AnimState[0] == AnimationState.EX_Right_Wrestle_3 || 
                 m_AnimState[0] == AnimationState.Back_Wrestle || m_AnimState[0] == AnimationState.BACK_EX_Wrestle
                )
        {
            this.GetComponent<Rigidbody>().useGravity = false;  // 重力無効            
            MoveSpeed = this.WrestlSpeed;
        }
             

        // キリカの時間遅延を受けているとき、1/4に
        if (m_timstopmode == TimeStopMode.TIME_DELAY)
        {
            MoveSpeed *= 0.25f;
        }
        // ほむらの時間停止を受けているときなど、0に
        else if (m_timstopmode == TimeStopMode.TIME_STOP || m_timstopmode == TimeStopMode.PAUSE || m_timstopmode == TimeStopMode.AROUSAL)
        {
            MoveSpeed = 0;
            return false;
        }
        if (this.Rigidbody != null)//(this.m_charactercontroller != null)
        {            
            // 速度ベクトルを作る
            Vector3 velocity = this.MoveDirection * MoveSpeed;
            // 走行中/アイドル中/吹き飛び中/ダウン中
            if (this.m_AnimState[0] == AnimationState.Run || this.m_AnimState[0] == AnimationState.Idle || this.m_AnimState[0] == AnimationState.Blow || this.m_AnimState[0] == AnimationState.Down)
            {
                velocity.y = MadokaDefine.FALLSPEED;      // ある程度下方向へのベクトルをかけておかないと、スロープ中に落ちる
            }
            this.Rigidbody.velocity = velocity; //this.m_charactercontroller.Move(velocity * Time.deltaTime);
            
        }
        // HP表示を増減させる(回復は一瞬で、被ダメージは徐々に減る）
        if (NowHitpoint < m_DrawHitpoint)
        {
            m_DrawHitpoint -= 2;            
        }
        else
        {
            m_DrawHitpoint = NowHitpoint;
        }
        
        // 時間停止を解除したら動き出す
        //Time.timeScale = 1;
        return true;

    }
    
    // 覚醒開始無効処理
    private void unuseArousal()
    {
        // ポーズコントローラーのインスタンスを取得
        var pausecontroller2 = m_pausecontroller.GetComponent<PauseControllerInputDetector>();
        // 時間を再度動かす
        pausecontroller2.pauseController.DeactivatePauseProtocol();
    }

    // 覚醒開始処理
    private void arousalStart()
    {
        if (m_pausecontroller == null)
        {
            return;
        }
        // ポーズコントローラーのインスタンスを取得
        var pausecontroller2 = m_pausecontroller.GetComponent<PauseControllerInputDetector>();
        // 時間を止める
        pausecontroller2.pauseController.ActivatePauseProtocol();
        // 覚醒前処理
        ArousalInitialize();
        // カットインカメラ有効化
        // ArousalCameraを有効化
        m_Insp_ArousalCamera.enabled = true;        
        // ポーズ実行
        FreezePositionAll();
        // カットインイベントを有効化
        Arousal_Camera_Controller CutinEvent = m_Insp_ArousalCamera.GetComponentInChildren<Arousal_Camera_Controller>();
        // カットインイベント発動
        CutinEvent.UseAsousalCutinCamera();
        // 時間停止
        this.TimeStopMaster = true;
        m_timstopmode = TimeStopMode.AROUSAL;   
    }

    // 継承先のLateUpdateで実行すること
    protected void LateUdate_Core()
    {
        if (this.IsPlayer != CHARACTERCODE.ENEMY)
        {
            // SavingParameterからのステートの変更を受け付ける
            int charactername = (int)this.m_character_name;
            // レベル
            Level = savingparameter.GetNowLevel(charactername);
            // 攻撃力
            m_StrLevel = savingparameter.GetStrLevel(charactername);
            // 防御力
            m_DefLevel = savingparameter.GetDefLevel(charactername);
            // 残弾数
            m_BulLevel = savingparameter.GetBulLevel(charactername);
            // ブースト量
            BoostLevel = savingparameter.GetBoostLevel(charactername);
            // 覚醒ゲージ
            m_ArousalLevel = savingparameter.GetArousalLevel(charactername);
            // ソウルジェム汚染率
            m_GemContamination = savingparameter.GetGemContimination(charactername);
            // HP
            NowHitpoint = savingparameter.GetNowHP(charactername);            
            // 覚醒ゲージ量
            Arousal = savingparameter.GetNowArousal(charactername);
        }   
    }


    // ポーズを解除する
    public void ReleasePause()
    {
        if (m_timstopmode == TimeStopMode.PAUSE)
        {
            if (m_hasPauseInput)
            {
                m_timstopmode = TimeStopMode.NORMAL;
                //Time.timeScale = 1;
            }
            //return false; // ここでreturnをさせてUpdateを抜けさせると、そのキャラ「だけ」が止まる
            // 時間停止時は全オブジェクトにアクセスし、TimeStopのMasterでないキャラはReturnさせる
        }
    }

    // アニメーション速度を調整する
    // 第1引数：アニメの種類
    // 第2引数：アニメの速度
    // 第3引数：時間停止の状態
    protected void ContlroleAnimationSpeed(AnimationState animationstate, float speed,TimeStopMode timestopmode)
    {
        if (m_timstopmode == TimeStopMode.TIME_DELAY)
        {
            speed *= 0.25f;
        }
        // ほむらの時間停止を受けているときなど、0に
        else if (m_timstopmode == TimeStopMode.TIME_STOP || m_timstopmode == TimeStopMode.PAUSE || m_timstopmode == TimeStopMode.AROUSAL)
        {
            speed = 0;         
        }
        this.GetComponent<Animation>()[m_AnimationNames[(int)animationstate]].speed = speed;        
    }

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
	
    

    private const float ITEM_X = 30.0f;
    private const float ITEM_Y = 5.0f;

    private const float EQUIP_ITEM_X = 115.0f;
    private const float EQUIP_ITEM_Y = 1.0f;

    private const float ITEMNUM_X = 370.0f;
    private const float ITEMNUM_Y = 5.0f;

    // ステップ時共通動作
    // 第1引数：ステップの内容
    protected void StepMove(AnimationState astate)
    {
        // 移動距離をインクリメントする（初期位置との差では壁に当たったときに無限に動き続ける）
        this.SteppingLength += this.m_Step_Move_1F;

        // 地面オブジェクトに触れた場合は着地モーションへ移行(暴走防止のために、上昇中は判定禁止.時間で判定しないと、引っかかったときに無限ループする)
        if (this.SteppingLength > this.m_Step_Move_Length)
        {
            //MoveDirection.y = -2;                
            this.MoveDirection = transform.rotation * Vector3.forward;
            switch (astate)
            {
                case AnimationState.FrontStep:
                    this.m_AnimState[0] = AnimationState.FrontStepBack_Standby;
                    break;
                case AnimationState.BackStep:
                    this.m_AnimState[0] = AnimationState.BackStepBack_Standby;
                    break;
                case AnimationState.LeftStep:
                    this.m_AnimState[0] = AnimationState.LeftStepBack_Standby;
                    break;
                case AnimationState.RightStep:
                    this.m_AnimState[0] = AnimationState.RightStepBack_Standby;
                    break;
            }                        
        }      
    }

    // ステップ落下時のキャンセルダッシュ及び再ジャンプ受付
    protected bool CancelCheck()
    {
        // ブーストが残っていればキャンセルダッシュは受け付ける（方向入力は受け付けない）
        if (this.Boost > 0)
        {
            // 方向キーなしで再度ジャンプを押した場合、慣性ジャンプ
            if (!HasVHInput && Input.GetButton("Jump"))
            {
                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
                JumpDone();
                return true;
            }
            // ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)            
            else if (Input.GetButtonDown("Jump"))
            {
                CancelDashDone();
                return true;
            }
        }
        return false;
    }

    
    // HP・SG汚染率・LVを書き換える
    void FixedUpdate()
    {       
        if (this.IsPlayer != CHARACTERCODE.ENEMY)
        {
            // SavingParameterからのステートの変更を受け付ける
            int charactername = (int)this.m_character_name;
            // SavingParameterに現在のステートを渡す
            // 最大HP
            savingparameter.SetMaxHP(charactername, GetMaxHitpoint(Level));
        }
    }

    // 本体を相手の方向へ向ける
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

    // 覚醒時のエフェクト
    protected GameObject m_arousalEffect;

    // 覚醒開始時の共通処理(残弾の回復はキャラごとに覚醒でも回復禁止のものがあるのでこの関数のオーバーライドで行うこと）
    protected virtual void ArousalInitialize()
    {
        // ダメージ中はreversalに変更する
        if (m_AnimState[0] == AnimationState.Damage)
        {
            ReversalInit();
        }
	    // ブースト全回復
        Boost = GetMaxBoost(BoostLevel);
        // 覚醒モード移行
        IsArousal = true;
        // エフェクト出現（エフェクトはCharacerControl_Baseのpublicに入れておく）
        m_arousalEffect = Instantiate(m_Insp_ArousalEffect) as GameObject;
        // エフェクトと本体の動きを同期させる
        m_arousalEffect.GetComponent<Rigidbody>().isKinematic = true;
        m_arousalEffect.transform.position = this.transform.position;
        m_arousalEffect.transform.parent = transform;
       
        // 全ゲージ消去
        // CharacterControl_Base依存
        m_DrawInterface = false;
        // Player_Camera_Controler依存
        var camerastate = MainCamera.GetComponentInChildren<Player_Camera_Controller>();
        camerastate.m_DrawInterface = false;
    }

    // 全CharacterControllBase継承オブジェクトの配置位置を固定する
    public void FreezePositionAll()
    {
        CharacterControl_Base[] Character = FindObjectsOfType(typeof(CharacterControl_Base)) as CharacterControl_Base[];
        foreach (CharacterControl_Base i in Character)
        {
            i.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        }
    }
    
    // 全CharacterControllBase継承オブジェクトの配置位置固定を解除する
    public void UnFreezePositionAll()
    {
        CharacterControl_Base[] Character = FindObjectsOfType(typeof(CharacterControl_Base)) as CharacterControl_Base[];
        foreach (CharacterControl_Base i in Character)
        {
            i.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    // 走行射撃時、合成していたアニメを止めて合成モードを戻す
    // ダメージを受けたときにも行うこと
    protected void DeleteBlend()
    {
        // 再生していたアニメを止める
        this.GetComponent<Animation>().Stop(m_AnimationNames[(int)AnimationState.Run_underonly]);
        this.GetComponent<Animation>().Stop(m_AnimationNames[(int)AnimationState.Shot_toponly]);
        // 合成モードを戻す
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run_underonly]].blendMode = AnimationBlendMode.Blend;
        this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Shot_toponly]].blendMode = AnimationBlendMode.Blend;
    }

    // 歩き撃ちのアニメーションを戻す
    protected void ReturnMotion()
    {
        if (shotmode == ShotMode.SHOTDONE && Time.time > this.m_AttackTime + this.BulletWaitTime[(int)ShotType.NORMAL_SHOT])
        {
            //Debug.Log("ResutrnMotion");
            // 歩き撃ちフラグを折る
            RunShotDone = false;
            // 上体を戻す
            BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            shotmode = ShotMode.NORMAL;
            DeleteBlend();
            // 方向キー入力があった場合かつ地上にいた場合
            if (IsGrounded && HasVHInput)
            {
                this.m_AnimState[0] = AnimationState.Run;
            }
            // 方向キー入力がなくかつ地上にいた場合
            else if (IsGrounded)
            {
                this.m_AnimState[0] = AnimationState.Idle;
            }
            // 空中にいた場合
            else
            {
                this.m_AnimState[0] = AnimationState.Fall;
                FallStartTime = Time.time;
            }
        }
    }
    // 装填開始（通常射撃）
    protected void ShotDone()
    {
        // 一旦空中発射フラグを切る（この辺は他のキャラも考えるべきかもしれない。というかこれはcharacterControl_Baseにおいた方がいいかもしれない
        // 歩き撃ちができない場合もあるから基底にしてオーバーライドが妥当か？

        // 歩行時は上半身のみにして走行（下半身のみ）と合成
        if (this.m_AnimState[0] == AnimationState.Run)
        {
            // 下半身
            // レイヤー1に設定
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run_underonly]].layer = 1;
            // 合成モード設定
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run_underonly]].blendMode = AnimationBlendMode.Additive;
            // 再生
            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Run_underonly]);
            // weightを設定
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run_underonly]].weight = 0.5f;

            // 上半身
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Shot_toponly]].wrapMode = WrapMode.Once;
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Shot_toponly]].layer = 2;
            this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Shot_toponly]].blendMode = AnimationBlendMode.Additive;
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Shot_toponly], 0.1f);
            m_AnimState[0] = AnimationState.Shot_run;   // 歩き撃ちへ移行
        }
        else if (this.m_AnimState[0] == AnimationState.AirDash)
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Shot]);
            m_AnimState[0] = AnimationState.Shot_AirDash;   // 空中ダッシュ射撃へ移行
        }
        else
        {
            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Shot]);
            m_AnimState[0] = AnimationState.Shot;       // 通常撃ちへ移行
        }

        this.shotmode = ShotMode.RELORD;        // 装填状態へ移行
    }

    // キャンセル等で本体に弾丸（矢など）があった場合消す
    protected void DestroyArrow()
    {
        // 弾があるなら消す(m_ArrowRootの下に何かあるなら全部消す）
        int ChildCount = this.MainShotRoot.transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            Transform child = this.MainShotRoot.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    // 上記の任意のフック以下のオブジェクトを消す
    protected void DestroyObject(GameObject root)
    {
        int ChildCount = root.transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            Transform child = root.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    // 空中ダッシュ・歩き射撃共通操作
    // run :歩き射撃であるか否か
    protected void moveshot(bool run)
    {
        // ロックオン中かつshotmode=RELOADの時は全体を相手の方へ向ける
        if (this.IsRockon && shotmode != ShotMode.NORMAL && shotmode != ShotMode.SHOTDONE)
        {
            RotateToTarget();
        }
        // 入力中はそちらへ進む
        if (HasVHInput)
        {
            // ロック時に矢を撃つときは本体が相手の方向を向いているため、専用の処理が要る
            if (this.IsRockon && shotmode != ShotMode.NORMAL)
            {
                UpdateRotation_step();      // steprotは相手の方向を向いたまま動くので、こっちを使う
                this.MoveDirection = this.StepRotation * Vector3.forward;
            }
            else
            {
                UpdateRotation();
                this.MoveDirection = this.transform.rotation * Vector3.forward;
            }
        }
        // 入力が外れると、落下する
        else
        {
            if (run)
            {
                this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
            }
            this.MoveDirection = Vector3.zero;
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

    // ロックオン対象へ体を向けるときの補正値を計算する
    // Headpos              [in]：本体の座標
    // targetpos            [in]：ロックオン対象の座標
    // Correction           [in]：Y軸の補正値
    // retval                   ：補正値
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

    // 飛び越し可能な壁に接触した
    private bool m_hitjumpover;
    public bool Gethitjumpover()
    {
        return m_hitjumpover;
    }
    // 飛び越し不能な壁に接触した
    private bool m_hitunjumpover;
    public bool Gethitunjumpover()
    {
        return m_hitunjumpover;
    }

     // 接触を判定する
    private void OnTriggerStay(Collider collision)
    {
        // 飛び越し可能な壁に接触した
        if (collision.gameObject.tag == "jumpover")
        {
            m_hitjumpover = true;
            return;
        }
        // 飛び越し不能な壁に接触した
        else if (collision.gameObject.tag == "unjumpover")
        {
            m_hitunjumpover = true;
            return;
        }
        m_hitjumpover = false;
        m_hitunjumpover = false;
    }

    // 本体を強制停止させる
    protected void EmagencyStop()
    {
        this.MoveDirection = Vector3.zero;
        m_AnimState[0] = AnimationState.Idle;
    }
        
}

public class InputPattern : IComparable
{
    public int InputRot;        // 入力方向。0から順に右、右上、上、左上、左、左下、下、右下
    public int InputCount;      // 入力回数
    public Vector2 InputVector; // 入力方向

    // コンストラクタ
    public InputPattern(int InputRot, int InputCount, Vector2 InputVector)
    {
        this.InputRot = InputRot;
        this.InputCount = InputCount;
        this.InputVector = InputVector;
    }

    // 入力回数でソートする
    public int CompareTo(object obj)
    {
        return InputCount - ((InputPattern)obj).InputCount;
    }
}