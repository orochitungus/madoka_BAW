using UnityEngine;
using System.Collections;

// 戦闘用キャラクター「魔獣」を制御するためのスクリプト
public class majyu_BattleControl : CharacterControl_Base 
{
    // メイン射撃撃ち終わり時間
    private float m_mainshotendtime;
    // 通常射撃用のビーム
    public GameObject m_Insp_NormalBeam;
    // ビーム射出音
    public AudioClip m_Insp_ShootSE;



    // 種類（キャラごとに技数は異なるので別々に作らないとアウト
    public enum SkillType_Majyu
    {
        // 攻撃系
        // 射撃属性
        SHOT,                   // 通常射撃
        // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        FRONT_WRESTLE_1,        // 前格闘1段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        BACK_EX_WRESTLE,        // 後特殊格闘
        // なし(派生がないとき用）
        NONE
    }



	// Use this for initialization
	void Start ()
    {
	     // アニメーションファイルの名前
        this.m_AnimationNames = new string[]
        {
            "majyu_b_idle_copy",                             //Idle,                   // 通常
            "majyu_b_walk_copy",                             //Walk,                   // 歩行
            "majyu_b_walk_u_only_copy",                      //Walk_underonly,         // 歩行（下半身のみ）       
            "majyu_b_jump_copy",                             //Jump,                   // ジャンプ開始
            "majyu_b_jump_u_only_copy",                      //Jump_underonly,         // ジャンプ開始（下半身のみ）
            "majyu_b_jamping_copy",                          //Jumping,                // ジャンプ中（上昇状態）
            "majyu_b_jamping_u_only_copy",                   //Jumping_underonly,      // ジャンプ中（下半身のみ）
            "majyu_b_fall_copy",                             //Fall,                   // 落下
            "majyu_b_landing_copy",                          //Landing,                // 着地
            "majyu_b_run_copy",                              //Run,                    // 走行
            "majyu_b_run_u_only_copy",                       //Run_underonly,          // 走行（下半身のみ）
            "majyu_b_air_dash_copy",                         //AirDash,                // 空中ダッシュ
            "majyu_b_frontstep_copy",                        //FrontStep,              // 前ステップ中        
            "majyu_b_leftstep_copy",                         //LeftStep,               // 左（斜め）ステップ中       
            "majyu_b_rightstep_copy",                        //RightStep,              // 右（斜め）ステップ中        
            "majyu_b_backstep_copy",                         //BackStep,               // 後ステップ中
            "majyu_b_frontstep_back_copy",                   //FrontStepBack,          // 前ステップ終了
            "majyu_b_leftstep_back_copy",                    //LeftStepBack,           // 左（斜め）ステップ終了
            "majyu_b_rightstep_back_copy",                   //RightStepBack,          // 右（斜め）ステップ終了
            "majyu_b_backstep_back_copy",                    //BackStepBack,           // 後ステップ終了
            "majyu_b_fall_copy",                             //FallStep,               // ステップで下降中
            "majyu_b_normal_shot_copy",                      //Shot,                   // 通常射撃
            "majyu_b_nomal_shot_u_only_copy",                //Shot_toponly,           // 通常射撃（上半身のみ）
            "",                                              //Shot_run,               // 通常射撃歩き撃ち
            "majyu_b_normal_shot_copy",                      //Shot_Airdash,           // 通常射撃（空中ダッシュ）
            "",                                              //Charge_Shot,            // 射撃チャージ
            "",                                              //Sub_Shot,               // サブ射撃
            "",                                              //EX_Shot,                // 特殊射撃            
            "majyu_b_normal_wrestle_1_copy",                 //Wrestle_1,              // N格1段目
            "majyu_b_normal_wrestle_2_copy",                 //Wrestle_2,              // N格2段目
            "majyu_b_normal_wrestle_3_copy",                 //Wrestle_3,              // N格3段目
            "",                                              //Charge_Wrestle,         // 格闘チャージ
            "majyu_b_back_wrestle_copy",                    //Front_Wrestle_1,        // 前格闘1段目
            "",                                              //Front_Wrestle_2,        // 前格闘2段目
            "",                                              //Front_Wrestle_3,        // 前格闘3段目
            "",                                              //Left_Wrestle_1,         // 左横格闘1段目
            "",                                              //Left_Wrestle_2,         // 左横格闘2段目
            "",                                              //Left_Wrestle_3,         // 左横格闘3段目
            "",                                              //Right_Wrestle_1,        // 右横格闘1段目
            "",                                              //Right_Wrestle_2,        // 右横格闘2段目
            "",                                              //Right_Wrestle_3,        // 右横格闘3段目
            "majyu_b_front_wrestle_copy",                     //Back_Wrestle,           // 後格闘（防御）
            "majyu_b_BD_wrestle_copy",                       //AirDash_Wrestle,        // 空中ダッシュ格闘
            "",                                              //Ex_Wrestle_1,           // 特殊格闘1段目
            "",                                              //Ex_Wrestle_2,           // 特殊格闘2段目
            "",                                              //Ex_Wrestle_3,           // 特殊格闘3段目
            "majyu_b_front_ex_wrestle_copy",                 //EX_Front_Wrestle_1,     // 前特殊格闘1段目
            "",                                              //EX_Front_Wrestle_2,     // 前特殊格闘2段目
            "",                                              //EX_Front_Wrestle_3,     // 前特殊格闘3段目
            "",                                              //EX_Left_Wrestle_1,      // 左横特殊格闘1段目
            "",                                              //EX_Left_Wrestle_2,      // 左横特殊格闘2段目
            "",                                              //EX_Left_Wrestle_3,      // 左横特殊格闘3段目
            "",                                              //EX_Right_Wrestle_1,     // 右横特殊格闘1段目
            "",                                              //EX_Right_Wrestle_2,     // 右横特殊格闘2段目
            "",                                              //EX_Right_Wrestle_3,     // 右横特殊格闘3段目        
            "majyu_b_back_ex_wrestle_copy",                  //BACK_EX_Wrestle,        // 後特殊格闘
            "majyu_b_down_rebirth02_copy",                   //Reversal,               // ダウン復帰
            "",                                              //Arousal_Attack,         // 覚醒技
            "majyu_b_damage02_copy",                         //Damage,                 // ダメージ
            "majyu_b_down_copy",                             //Down,                   // ダウン
            "",                                              //Nomove                  // 動きなし
            "",                                              //Blow                    // 吹き飛び
            "",                                              //FrontStepBack_Standby,  // 前ステップ終了(アニメはなし）
            "",                                              //LeftStepBack_Standby,   // 左（斜め）ステップ終了(アニメはなし）
            "",                                              //RightStepBack_Standby,  // 右（斜め）ステップ終了(アニメはなし）
            "",                                              //BackStepBack_Standby,   // 後ステップ終了(アニメはなし）
            "",                                              //DamageInit,             // ダメージ前処理(アニメはなし）
            "",                                              //BlowInit,               // 吹き飛び前処理(アニメはなし）
            "majyu_spinDown_copy",                           //SpinDown                // 錐揉みダウン
        };

        // 個別ステートを初期化（インスペクタでもできるけど一応こっちでやっておこう）
        // 後、ブースト量などはここで設定しておかないと下で初期化できない
        // レベル関連（この辺はインスペクタから上書きするので、実際に使用するときは消すこと）
        SettingPleyerLevel();
      
		
		// HP初期化
		this.NowHitpoint = GetMaxHitpoint(this.Level);

        // ジャンプ硬直
        this.JumpWaitTime = 0.5f;

        //着地硬直
        this._LandingWaitTime = 1.5f;

        this.WalkSpeed = 3.0f;                             // 移動速度（歩行の場合）
        this.RunSpeed = 2.0f * this.WalkSpeed;          // 移動速度（走行の場合）
        this.AirDashSpeed = 8.0f * this.WalkSpeed;          // 移動速度（空中ダッシュの場合）
        this.AirMoveSpeed = 3.5f * this.WalkSpeed;          // 移動速度（空中慣性移動の場合）
        this.RiseSpeed = 2.0f * this.WalkSpeed;        // 上昇速度

        // ブースト消費量
        this.JumpUseBoost = 20;       // ジャンプ時
        this.DashCancelUseBoost = 20;   // ブーストダッシュ時
        this.StepUseBoost = 20;         // ステップ時
        this.BoostLess = 0.5f;        // ジャンプの上昇・BD時の1F当たりの消費量

        // ステップ移動距離
        this.StepMoveLength = 10.0f;

        // ステップ初速（X/Z軸）
        this.StepInitialVelocity = 30.0f;
        // ステップ時の１F当たりの移動量
        this.StepMove1F = 1.0f;
        // ステップ終了時硬直時間
        this.StepBackTime = 0.7f;

        // コライダの地面からの高さ
        this.ColliderHeight = 1.5f;

        // ロックオン距離
        this.RockonRange = 90.0f;

        // ロックオン限界距離
        this.RockonRangeLimit = 200.0f;

        // ショットのステート
        shotmode = ShotMode.NORMAL;

        // 弾のステート
        this.BulletMoveDirection = Vector3.zero;
        this.BulletPos = Vector3.zero;

        // メイン射撃撃ち終わり時間
        m_mainshotendtime = 0.0f;

        // 共通ステートを初期化
        FirstSetting();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // 共通アップデート処理(時間停止系の状態になると入力は禁止)
        if (Update_Core())
        {
            // アニメーション制御＆入力取得
            Update_Animation();
            // リロード実行           
            // メイン射撃
            ReloadSystem.OneByOne(ref BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)CharacterName][(int)ShotType.NORMAL_SHOT].m_reloadtime, ref m_mainshotendtime);
        }
	}

    // 攻撃行動全般（RunとAirDashは特殊なので使用しない）
    // run[in]      :走行中入力
    // AirDash[in]  :空中ダッシュ中入力
    private void AttackDone(bool run = false,bool AirDash = false)
    {
        // 射撃で射撃へ移行
        if (HasShotInput)
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
                        ShotDone();
                    }
                    // それ以外なら強制的に停止して（立ち撃ちにして）撃つ
                    else
                    {
                        this.MoveDirection = Vector3.zero;
                        m_AnimState[0] = AnimationState.Idle;
                        ShotDone();
                    }
                }
                // 非ロック状態なら歩き撃ちフラグを立てる
                else
                {
                    RunShotDone = true;
                    ShotDone();
                }
            }
            else
            {
                ShotDone();
            }
        }
        // 特殊格闘で特殊格闘へ移行
        else if (HasExWrestleInput)
        {
            // 前特殊格闘
            if (HasFrontInput)
            {
                WrestleDone_UpperEx((int)SkillType_Majyu.EX_FRONT_WRESTLE_1);
            }
            // 空中で後特殊格闘
            else if (HasBackInput && !IsGrounded)
            {
                WrestleDone_DownEx((int)SkillType_Majyu.BACK_EX_WRESTLE);
            }
            // それ以外
            else
            {
                WrestleDone(AnimationState.Wrestle_1, (int)SkillType_Majyu.WRESTLE_1);
            }
        }
        // 格闘で格闘へ移行
        else if (HasWrestleInput)
        {
            if (AirDash)
            {
                WrestleDone(AnimationState.AirDash_Wrestle, (int)SkillType_Majyu.AIRDASH_WRESTLE);
            }
            else
            {
                // 前格闘への分岐はここで（2段目・3段目の分岐はやっているときに）
                // 前格闘
                if (HasFrontInput)
                {
                    WrestleDone(AnimationState.Front_Wrestle_1, (int)SkillType_Majyu.FRONT_WRESTLE_1);
                }
                // 後格闘（防御）
                else if (HasBackInput)
                {
                    GuardDone((int)SkillType_Majyu.BACK_WRESTLE);
                }
                else
                {
                    // N格闘1段目  
                    WrestleDone(AnimationState.Wrestle_1, (int)SkillType_Majyu.WRESTLE_1);
                }
            }
        }
    }

    // 入力系のオーバーロード
    // idle(攻撃系モーションの入力受け付けを入れる）
    protected override void Animation_Idle()
    {
        ReturnMotion();
        base.Animation_Idle();
        // 格闘の累積時間を初期化
        Wrestletime = 0;
       
        // キャンセルダッシュ入力でキャンセルダッシュ
        if (HasDashCancelInput)
        {
            CancelDashDone();
        }
        
        // 地上にいるか？(落下開始時は一応禁止）
        if (IsGrounded)
        {
            AttackDone();
        }
    }

     // jumping(ジャンプ硬直中は攻撃行動は禁止）
    protected override void Animation_Jumping()
    {
        base.Animation_Jumping();
        AttackDone();
    }
    
    // fall
    protected override void Animation_Fall()
    {
        ReturnMotion();
        base.Animation_Fall();
        // キャンセルダッシュ入力でキャンセルダッシュ
        if (HasDashCancelInput)
        {
            CancelDashDone();
        }
        
        AttackDone();
    }

    // run
    protected override void Animation_Run()
    {
        base.Animation_Run();
        // キャンセルダッシュ入力でキャンセルダッシュ
        if (HasDashCancelInput)
        {
            CancelDashDone();
        }
        AttackDone(true, false);
        
    }
    // AirDash
    protected override void Animation_AirDash()
    {
        base.Animation_AirDash();
        AttackDone(false, true);
    }

    // DamageInit
    public override void DamageInit(CharacterControl_Base.AnimationState animationstate)
    {
        base.DamageInit(animationstate);
        // 本体にくっついていたビームをカット(あるなら）
        DestroyArrow();
        // 同じく格闘判定をカット
        DestroyWrestle();
    }        

    // 射撃中共通動作（空中にいるときはダッシュキャンセルを有効にする）
    protected override void Shot()
    {
        // キャンセルダッシュ受付
        if (this.HasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.IsGrounded)
            {
                GetComponent<Rigidbody>().position = new Vector3(this.GetComponent<Rigidbody>().position.x, this.GetComponent<Rigidbody>().position.y + 3, this.GetComponent<Rigidbody>().position.z);
            }
            //CancelDashDone();
        }
        base.Shot();
    }

    // 歩き撃ち中共通動作
    protected override void ShotRun()
    {
        // キャンセルダッシュ受付
        if (this.HasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.IsGrounded)
            {
                GetComponent<Rigidbody>().position = new Vector3(this.GetComponent<Rigidbody>().position.x, this.GetComponent<Rigidbody>().position.y + 3, this.GetComponent<Rigidbody>().position.z);
            }
            CancelDashDone();
        }
        base.ShotRun();
        moveshot(true);
    }

    // 空中ダッシュ射撃中共通動作
    protected override void ShotAirDash()
    {
        // キャンセルダッシュ受付
        if (this.HasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.IsGrounded)
            {
                GetComponent<Rigidbody>().position = new Vector3(this.GetComponent<Rigidbody>().position.x, this.GetComponent<Rigidbody>().position.y + 3, this.GetComponent<Rigidbody>().position.z);
            }
            CancelDashDone();
        }
        base.ShotAirDash();
        moveshot(false);
    }
    // ダッシュキャンセル時の処理
    // タイミングによってはビームが残る
    protected override void CancelDashDone()
    {
        // 弾があるなら消す(m_ArrowRootの下に何かあるなら全部消す）
        DestroyArrow();
        // 格闘判定があるなら消す
        DestroyWrestle();
        // 歩き撃ちフラグを折る
        RunShotDone = false;
        // 上体を戻す
        BrestObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        // モーションを戻す
        shotmode = ShotMode.NORMAL;
        DeleteBlend();
        // 固定状態を解除
        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        // 通常のダッシュキャンセルの処理
        base.CancelDashDone();
    }

     // 攻撃アニメーション終了判定
    // 第1引数：アニメの名前
    // 第2引数：アニメの種類
    protected bool ShotEndCheck(string AnimationName, ShotType type)
    {
        while (this.GetComponent<Animation>().IsPlaying(AnimationName))
        {
            return false;
        }
        // 通常射撃の場合
        if (type == ShotType.NORMAL_SHOT)
        {
            // 終わった後でなく、開始時にも来るのでSHOTDONEを追加
            if (Time.time > this.m_AttackTime + this.BulletWaitTime[(int)type])
            {
                // 合成状態を解除
                ReturnMotion();
                // 地上にいて静止中
                if (IsGrounded && !this.HasVHInput)
                {
                    // アイドルモードのアニメを起動する
                    this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
                    this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Idle]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Idle;
                }
                // 地上にいて歩行中
                else if (IsGrounded)
                {
                    // 走行モードのアニメを起動する
                    this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Run]);
                    this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Run;
                }
                // 空中にいてダッシュ入力中でありかつブーストゲージがある
                else if (!IsGrounded && HasVHInput && HasJumpInput && this.Boost > 0)
                {
                    // 空中ダッシュのアニメを起動する
                    this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.AirDash]);
                    this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.AirDash]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.AirDash;
                }
                // 空中にいて落下中(歩き撃ちをしていた場合を除く）
                else //if (!m_isGrounded && !this.m_ShotRun)
                {
                    this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
                    this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Fall]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Fall;
                    FallStartTime = Time.time;
                    // ショットのステートを戻す
                    shotmode = ShotMode.NORMAL;
                }
                return true;
            }
        }
        return false;
    }
    // 射撃(通常射撃装填。この関数は通常射撃のアニメにリンクする。弾消費後の処理は共用できる）
    protected void Shoot(ShotType type)
    {
         // 弾があるとき限定
        if (this.BulletNum[(int)type] > 0)
        {
            // ロックオン時本体の方向を相手に向ける       
            if (this.IsRockon)
            {
                RotateToTarget();
            }
            // 弾を消費する（サブ射撃なら1、特殊射撃なら2）
            if (type != ShotType.CHARGE_SHOT)
            {
                this.BulletNum[(int)type]--;
                // 撃ち終わった時間を設定する                
                // メイン（弾数がMax-1のとき）
                if (type == ShotType.NORMAL_SHOT && BulletNum[(int)type] == Character_Spec.cs[(int)CharacterName][(int)type].m_GrowthCoefficientBul * (this.BulLevel - 1) + Character_Spec.cs[(int)CharacterName][(int)type].m_OriginalBulletNum - 1)
                {
                    m_mainshotendtime = Time.time;
                }
            }
            // ビームの出現ポジションをフックと一致させる
            Vector3 pos = MainShotRoot.transform.position;
            Quaternion rot = Quaternion.Euler(MainShotRoot.transform.rotation.eulerAngles.x, MainShotRoot.transform.rotation.eulerAngles.y, MainShotRoot.transform.rotation.eulerAngles.z);
            // ビームを出現させる
            if (type == ShotType.NORMAL_SHOT)
            {
                var obj = (GameObject)Instantiate(m_Insp_NormalBeam, pos, rot);
                // 親子関係を再設定する(=ビームをフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = MainShotRoot.transform;
                    // 矢の親子関係を付けておく
                    obj.transform.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }        
    }

    // これもアニメの方につける
    // 射撃（射出）弾丸消費はここでやらない。アニメ中イベントはステートと同期するとは限らないので、弾丸消費はアニメ中イベントではやるべきではない
    protected void Shoot2(ShotType type)
    {
        // この時点では親子なので、Majyu_NormalShotを持ったスクリプトを拾う(デフォルトで持っていないので、スクリプトで検索するしかない）
        var beam = GetComponentInChildren<Majyu_NormalShot>();
        // ビームの速度を設定する
        if (beam != null)
        {
            beam.m_Speed = Character_Spec.cs[(int)CharacterName][0].m_Movespeed;
            // ロックオン状態で歩き撃ちしているとき
            if (this.IsRockon && this.RunShotDone)
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
                Vector3 addrot = mainrot.eulerAngles + normalizeRot_OR - new Vector3(0, 70.0f, 0);
                Quaternion qua = Quaternion.Euler(addrot);
                // forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
                Vector3 normalizeRot = (qua) * Vector3.forward;
                // 移動ベクトルを確定する
                beam.m_MoveDirection = Vector3.Normalize(normalizeRot);
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
                // 移動ベクトルを確定する
                beam.m_MoveDirection = Vector3.Normalize(normalizeRot);
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
                    beam.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                }
                // それ以外は本体の角度を射出角にする
                else
                {
                    beam.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                }
            }
            // 通常弾のフックの位置に弾の位置を代入する
            this.BulletPos = this.MainShotRoot.transform.position;
            // 同じく回転角を代入する
            this.BulletMoveDirection = beam.m_MoveDirection;
            // 攻撃力決定
            if (type == ShotType.NORMAL_SHOT)
            {
                // 攻撃力を決定する(ここの0がスキルのインデックス。下も同様）
                this.OffensivePowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.SHOT].m_OriginalStr + Character_Spec.cs[(int)CharacterName][0].m_GrowthCoefficientStr * (this.StrLevel - 1);
                // ダウン値を決定する
                this.DownratioPowerOfBullet = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.SHOT].m_DownPoint;
                // 覚醒ゲージ増加量を決定する
                ArousalRatioOfBullet = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.SHOT].m_arousal;
            }
            shotmode = ShotMode.SHOT;
            // 固定状態を解除
            this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            // ずれた本体角度を戻す(Yはそのまま）
            this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
            this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

            // 硬直時間を設定
            this.m_AttackTime = Time.time;
            // 射出音を再生する
            AudioSource.PlayClipAtPoint(m_Insp_ShootSE, transform.position);
        }
        // 弾がないときはとりあえずフラグだけは立てておく
        else
        {
            // 硬直時間を設定
            this.m_AttackTime = Time.time;
            shotmode = ShotMode.SHOTDONE;
        }
    }

    // N格闘1段目
    protected override void Wrestle1()
    {
        base.Wrestle1();
        // 追加の格闘入力を受け取ったら、派生フラグを立てる
        if (this.HasWrestleInput || this.HasExWrestleInput)
        {
            this.AddInput = true;
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.WRESTLE_1].m_animationTime;
        if (Wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // N格闘2段目
    protected override void Wrestle2()
    {
        base.Wrestle2();
        // 追加の格闘入力を受け取ったら、派生フラグを立てる
        if (this.HasWrestleInput || this.HasExWrestleInput)
        {
            this.AddInput = true;
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.WRESTLE_2].m_animationTime;
        if (Wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }
    // N格闘3段目
    protected override void Wrestle3()
    {
        base.Wrestle3();
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.WRESTLE_2].m_animationTime;
        if (Wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }
    // 前格闘
    protected override void FrontWrestle1()
    {
        base.FrontWrestle1();
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)CharacterName][(int)SkillType_Majyu.FRONT_WRESTLE_1].m_animationTime;
        if (Wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    void LateUpdate()
    {
        // ロックオン時首を相手の方向へ向ける(本体角度との合成になっているので、本体角度分減算してやらないと正常に向かない）
        if (this.IsRockon && this.m_AnimState[0] != AnimationState.Shot && this.m_AnimState[0] != AnimationState.EX_Shot
           && this.m_AnimState[0] != AnimationState.Sub_Shot && this.m_AnimState[0] != AnimationState.Front_Wrestle_2
           && this.m_AnimState[0] != AnimationState.Shot_run && this.m_AnimState[0] != AnimationState.Charge_Shot && !RunShotDone)
        {
            SetNeckRotate(this.m_Head, 0.0f);
        }
        // 歩き射撃時,上体をロックオン対象へ向ける
        if (this.IsRockon && RunShotDone)
        {
            SetNeckRotate(this.BrestObject, 0.0f);
        }
        // ロックオンしていないときはそのまま本体角度分回す
        else if (RunShotDone)
        {
            // 本体角度
            Vector3 rotate = this.transform.rotation.eulerAngles;
            BrestObject.transform.rotation = Quaternion.Euler(0, 74 + rotate.y, 0);
        }
        // アニメーション終了処理判定
        // 射撃
        if (this.m_AnimState[0] == AnimationState.Shot)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.Shot], ShotType.NORMAL_SHOT))
            {
                this.m_AnimState[1] = AnimationState.Shot;
            }
        }
        // 歩き射撃
        else if (this.m_AnimState[0] == AnimationState.Shot_run)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.Shot_run], ShotType.NORMAL_SHOT))
            {
                this.m_AnimState[1] = AnimationState.Shot_run;
            }
        }
        // 空中ダッシュ射撃
        else if (this.m_AnimState[0] == AnimationState.Shot_AirDash)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.Shot_AirDash], ShotType.NORMAL_SHOT))
            {
                this.m_AnimState[1] = AnimationState.Shot_AirDash;
            }
        }
        // savingparameterの値を反映させる
        LateUdate_Core();
    }
}
