using UnityEngine;
using System;
using System.Collections;

public class CharacterControl_Base_Quest : MonoBehaviour 
{
    // 通常カメラオブジェクトをフィールド変数に追加する
    public Camera m_MainCamera;

    // 地面設置確認用のレイの発射位置(コライダの中心とオブジェクトの中心)
    public Vector3 m_layOriginOffs;

    // レイの長さ
    private float m_laylength;
    // ground属性をもったレイヤー（この場合8）
    protected int m_layMask;

    // Animatorコンポーネント
    Animator m_animator;
    // CharacterController
    //CharacterController m_charactercontroller;
    // Capsule collider
    CapsuleCollider m_capsuleCollider;
    // RigidBody
    protected Rigidbody m_rigidbody;

    // コライダの地面からの高さ
    public float m_Collider_Height;

    // キャラクターコントローラーのコライダの中心位置
    //Vector3 colliderCenter;
    // キャラクターコントローラーのコライダの高さ
    //float colliderHight;

    // 地上にいるか否か
    protected bool m_isGrounded;

    // テンキー入力があったか否か
    protected bool m_hasVHInput;
    // ジャンプ入力があったか否か
    protected bool m_hasJumpInput;
    // ポーズ入力があったか否か
    protected bool m_hasPauseInput;
    // 決定入力があったか否か
    protected bool m_hasEnterInput;

    // 足音
    public AudioClip m_ashioto_normal;
    public AudioClip m_ashioto_wood;
    public AudioClip m_ashioto_jari;
    public AudioClip m_ashioto_snow;
    public AudioClip m_ashioto_carpet;

    // 使用する足音
    private StageSetting.FootType m_foottype;

   
    // ロックオン状態か否か
    public bool m_IsRockon;
    // 移動方向固定状態か否か
    protected bool m_Rotatehold;

    // アニメーションファイルの名前(下記のAnimationStateに対応)    
    public string[] m_AnimationNames = new string[(int)AnimationState.Number_Of_Animation];

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
        Number_Of_Animation
    }

    // ジャンプ時間
    public float m_JumpWaitTime;
    public float m_JumpTime;

    // 着地硬直
    public float m_LandingWaitTime;
    public float m_LandingTime;

    // 移動用ステート
    public Vector3 m_MoveDirection;   // 移動方向
    public Vector3 m_MoveDirection_OR;// 射撃などの射出前における移動方向
    public Vector3 m_BlowDirection;   // ダウンする時や吹き飛び属性の攻撃を食らった時の方向ベクトル
    public float m_WalkSpeed;         // 移動速度（歩行の場合）
    public float m_RunSpeed;          // 移動速度（走行の場合）
    public float m_AirDashSpeed;      // 移動速度（空中ダッシュの場合）
    public float m_AirMoveSpeed;      // 移動速度（空中慣性移動の場合）
    public float m_RateofRise;        // 上昇速度

    // ジャンプ時のブースト消費量
    protected float m_JumpUseBoost;
    // ダッシュキャンセル時のブースト消費量
    protected float m_DashCancelUseBoost;
    // ブースト消費量（1Fあたり）
    public float m_BoostLess;

    // プレイヤーであるか否か（これがfalseであると描画系の関数全カット）
    // それぞれの意味は以下の通り
    public enum CHARACTERCODE
    {
        PLAYER,         // プレイヤー操作
        PLAYER_ALLY,    // プレイヤー僚機
        ENEMY,          // 敵
        EXTRA           // NPC
    };
    public CHARACTERCODE m_isPlayer;

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
    public bool m_TimeStopMaster;

    // AnimationState を 2 つ用意し交互に切り替えて昔の状態を参照できるようにする。
    public AnimationState[] m_AnimState = new AnimationState[2];
    public int m_CurAnimPos;

	// Use this for initialization
	void Start () 
    {
        Initialize();
	}

    void Initialize()
    {      
        // 歩行速度設定
        m_RunSpeed = 15.0f;
        m_IsRockon = false;
        m_Rotatehold = false;
    }
	
	

    int hitcounter;
    bool hitcounterdone;
    const int hitcounterBias = 20;

    // 接地判定を行う。
    protected bool onGround()
    {
        // 接地しているとき
        if (Physics.Raycast(transform.position + this.m_layOriginOffs, -Vector3.up, this.m_laylength, this.m_layMask))
        {
            hitcounterdone = false;
            return true;
        }
        // 離れた時
        else
        {
            if (!hitcounterdone)
            {
                hitcounterdone = true;
                hitcounter = 0;
            }
            else
            {
                hitcounter++;
            }
        }

        if (hitcounter > hitcounterBias)
        {
            return false;
        }
        return true;
    }

    // テンキー入力判定を取る
    // 移動キーが押されているかどうかをチェックする HasVHInput を追加する。
    protected bool HasVHInput()
    {       
        if (0.0f != Input.GetAxisRaw("Horizontal"))
        {
            return true;
        }

        return (0.0f != Input.GetAxisRaw("Vertical"));       
    }

    // ジャンプ入力があったか否かをチェックする
    protected bool HasJumpInput()
    {
        // 長押し上昇があるのでこっちは長押しを認める
        if (Input.GetButton("Jump"))
        {
            return true;
        }
        return false;
    }

    // ポーズ入力があったか否かをチェックする
    protected bool HasPauseInput()
    {
        if (Input.GetButtonDown("Pause"))
        {
            return true;
        }        
        return false;
    }
    // 決定入力があったか否かをチェックする
    protected bool HasEnterInput()
    {
        if (Input.GetButtonDown("Enter"))
        {
            return true;
        }
        return false;
    }

    // 地上走行中は足音を鳴らす
    private const float m_walkTime = 0.5f;
    private float m_walkTimer;

    private void FootSteps()
    {
        if (m_walkTimer > 0)
        {
            m_walkTimer -= Time.deltaTime;
        }
        if (m_walkTimer <= 0)
        {
            switch (m_foottype)
            {
                case StageSetting.FootType.FootType_Normal:
                    AudioSource.PlayClipAtPoint(m_ashioto_normal, transform.position);
                    break;
                case StageSetting.FootType.FootType_Jari:
                    AudioSource.PlayClipAtPoint(m_ashioto_jari, transform.position);
                    break;
                case StageSetting.FootType.FootType_Carpet:
                    AudioSource.PlayClipAtPoint(m_ashioto_carpet, transform.position);
                    break;
                case StageSetting.FootType.FootType_Snow:
                    AudioSource.PlayClipAtPoint(m_ashioto_snow, transform.position);
                    break;
                case StageSetting.FootType.FootType_Wood:
                    AudioSource.PlayClipAtPoint(m_ashioto_wood, transform.position);
                    break;
                default:
                    break;

            }
            m_walkTimer = m_walkTime;
        }
    }

    // ワールド座標でのカメラの基底ベクトルを計算し、それを基にメッシュの回転を計算する
    protected void UpdateRotation()
    {
        var finalRot = transform.rotation;
        var horizontal = Input.GetAxisRaw("Horizontal");        // 横入力を検出
        var vertical = Input.GetAxisRaw("Vertical");            // 縦入力を検出
        var toWorldVector = this.m_MainCamera.transform.rotation;

        // ロックオン時特殊処理(多分クエストパートでロックオンはないと思うが）
        if (this.m_IsRockon)
        {
            // 敵（ロックオン対象）の座標を取得
            var targetspec = GetComponentInChildren<Player_Camera_Controller>();
            Vector3 targetpos = targetspec.Enemy.transform.localPosition;
            // 自分の座標を取得
            Vector3 myPos = this.transform.localPosition;


            // XとZの差が一定値以下で移動方向固定(空中移動時限定）
            if (!this.m_isGrounded)
            {
                if (Math.Abs(targetpos.x - myPos.x) < 10.0f && Math.Abs(targetpos.z - myPos.z) < 10.0f)
                {
                    this.m_Rotatehold = true;
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

    // 全CharacterControllBase継承オブジェクトの配置位置を固定する
    public void FreezePositionAll()
    {
        CharacterControl_Base_Quest[] Character = FindObjectsOfType(typeof(CharacterControl_Base_Quest)) as CharacterControl_Base_Quest[];
        foreach (CharacterControl_Base_Quest i in Character)
        {
            i.rigidbody.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    // 全CharacterControllBase継承オブジェクトの配置位置固定を解除する
    public void UnFreezePositionAll()
    {
        CharacterControl_Base_Quest[] Character = FindObjectsOfType(typeof(CharacterControl_Base_Quest)) as CharacterControl_Base_Quest[];
        foreach (CharacterControl_Base_Quest i in Character)
        {
            i.rigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    // 継承先のStart開幕で実行する共通初期化処理
    protected void FirstSetting()
    {
        // CharacterControllerを取得
        //this.m_charactercontroller = GetComponent<CharacterController>();
        // Rigidbodyを取得
        this.m_rigidbody = GetComponent<Rigidbody>();
        // カプセルコライダを取得
        this.m_capsuleCollider = GetComponent<CapsuleCollider>();

        // PCでなければカメラを無効化しておく
        if (m_isPlayer != CHARACTERCODE.PLAYER)
        {
            // 自分にひっついているカメラオブジェクトを探し、カメラを切っておく
            transform.Find("Main Camera").camera.enabled = false;
        }

        this.m_layOriginOffs = new Vector3(0.0f, m_Collider_Height, 0.0f);
        this.m_laylength = m_capsuleCollider.radius + m_capsuleCollider.height; //m_charactercontroller.radius + m_charactercontroller.height / 2 + 1.5f;
        this.m_layMask = 1 << 8;       // layMaskは無視するレイヤーを指定する。8のgroundレイヤー以外を無視する

        // アニメーションをIdleに初期化
        this.m_AnimState[0] = this.m_AnimState[1] = AnimationState.Idle;
        this.m_CurAnimPos = 0;

        // ロックオンを初期化する
        this.m_IsRockon = false;

        // 進行方向固定フラグを初期化する
        this.m_Rotatehold = false;

        // 時間停止のルートを初期化する
        this.m_TimeStopMaster = false;

        // テンキー入力があったか否か
        this.m_hasVHInput = false;
        // ジャンプ入力があったか否か
        this.m_hasJumpInput = false;
        // ポーズ入力があったか否か
        this.m_hasPauseInput = false;
               

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

    }

    // 継承先のUpdate開幕で実行すること
    // また、時間停止状態の場合、falseを返す
    protected bool Update_Core()
    {
        // 接地判定
        m_isGrounded = onGround();   

        // ESC強制終了（後で直す）
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // プレイヤー操作の場合（CPU操作の場合はまた別に）
        if (m_isPlayer == CHARACTERCODE.PLAYER)
        {
            // 方向キー取得
            this.m_hasVHInput = HasVHInput();
            // ジャンプ入力があったか否か
            this.m_hasJumpInput = HasJumpInput();
            // ポーズ入力があったか否か
            this.m_hasPauseInput = HasPauseInput();
            // 位置を保持
            savingparameter.nowposition = this.transform.position;
            // 角度を保持
            savingparameter.nowrotation = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        }
        // 時間停止中falseを強制返し
        // 通常時、ポーズ入力で停止.ただし死んだら無効
        if (m_timstopmode != TimeStopMode.PAUSE)
        {
            if (m_hasPauseInput)
            {
                this.m_TimeStopMaster = true;
                m_timstopmode = TimeStopMode.PAUSE;
                // 動作を止める
                FreezePositionAll();
            }
        }
        // MoveDirection は アニメーションステート Walk で設定され、アニメーションステートが Idle の場合リセットされる。
        // 移動処理はステートの影響を受けないように UpdateAnimation のステートスイッチの前に処理する。
        // ステートごとに処理をする場合、それをメソッド化してそれぞれのステートに置く。
        // 走行速度を変更する、アニメーションステートが Run だった場合 RunSpeed を使う。
        var MoveSpeed = this.m_RunSpeed;
        // キリカの時間遅延を受けているとき、1/4に
        if (m_timstopmode == TimeStopMode.TIME_DELAY)
        {
            MoveSpeed *= 0.25f;
        }
        // ほむらの時間停止を受けているときなど、0に
        else if (m_timstopmode == TimeStopMode.TIME_STOP || m_timstopmode == TimeStopMode.PAUSE || m_timstopmode == TimeStopMode.AROUSAL)
        {
            // アニメを止めておく
            //Time.timeScale = 0;
            MoveSpeed = 0;
            return false;
        }
        if (this.m_capsuleCollider != null) 
        {
            // 速度ベクトルを作る
            Vector3 velocity = this.m_MoveDirection * MoveSpeed;
            // 走行中/アイドル中
            if (this.m_AnimState[0] == AnimationState.Run || this.m_AnimState[0] == AnimationState.Idle )
            {
                velocity.y = MadokaDefine.FALLSPEEDQUEST;      // ある程度下方向へのベクトルをかけておかないと、スロープ中に落ちる          
            }
            this.m_rigidbody.velocity = velocity; 
        }
        return true;
    }

    protected void Update_Animation()
    {
        switch (m_AnimState[0])
        {
            case AnimationState.Idle:
                Animation_Idle();
                break;
            case AnimationState.Run:
                Animation_Run();
                break;
        }
    }

    // Idle時共通操作
    protected virtual void Animation_Idle()
    {
        this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        m_AnimState[1] = AnimationState.Idle;
        this.m_MoveDirection = Vector3.zero;      // 速度を0に
        this.m_BlowDirection = Vector3.zero;
        this.m_Rotatehold = false;                // 固定フラグは折る
        // 慣性を殺す
        this.rigidbody.velocity = Vector3.zero;
        this.rigidbody.angularVelocity = Vector3.zero;
        rigidbody.useGravity = true;
        // 地上にいるか？(階段でハマることがあるので一旦なしにしておく）
        //if (m_isGrounded)
        {
            // 方向キーで走行
            if (m_hasVHInput)
            {
                m_AnimState[0] = AnimationState.Run;
                this.animation.Play(m_AnimationNames[(int)AnimationState.Run]);
            }
        }
    }

    // Run時共通動作
    protected virtual void Animation_Run()
    {
        this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        m_AnimState[1] = AnimationState.Run;
        // 接地中かどうか(階段でハマることがあるので一旦なしにしておく）
        //if (m_isGrounded)
        {
            // 入力中はそちらへ進む
            if (m_hasVHInput)
            {
                FootSteps();
                UpdateRotation();
                this.m_MoveDirection = transform.rotation * Vector3.forward;
            }
            // 入力が外れるとアイドル状態へ
            else
            {
                m_AnimState[0] = AnimationState.Idle;
                this.animation.Play(m_AnimationNames[(int)AnimationState.Idle]);
            }
        }
    }


}
