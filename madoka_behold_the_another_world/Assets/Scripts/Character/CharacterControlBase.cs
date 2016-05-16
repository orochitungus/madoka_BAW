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



	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
