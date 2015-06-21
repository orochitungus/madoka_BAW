using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// キャラクター「暁美　ほむら（弓）」を制御するためのスクリプト
public class Homura_Final_BattleControl : CharacterControl_Base 
{
    
    // 通常射撃用の用の矢
    public GameObject m_Insp_NormalArrow;
    // チャージ射撃用の矢
    public GameObject m_Insp_ChargeShotArrow;
    // サブ射撃用の矢
    public GameObject m_Insp_SubShotArrow;
    // 特殊射撃用の矢(中央）
    public GameObject m_Insp_ExShotArrow;
    // 特殊射撃用の矢（左）
    public GameObject m_Insp_ExShotArrow_L;
    // 特殊射撃用の矢（右）
    public GameObject m_Insp_ExShotArrow_R;
    // 特殊射撃用の矢のフック（左）
    public GameObject m_Insp_ArrowRoot_Left;
    // 特殊射撃用の矢のフック（右）
    public GameObject m_Insp_ArrowRoot_Right;

    // メイン射撃撃ち終わり時間
    private float m_mainshotendtime;
    // サブ射撃撃ち終わり時間
    private float m_subshotendtime;
    // 特殊射撃撃ち終わり時間
    private float m_exshotendtime;
    

    // 種類（キャラごとに技数は異なるので別々に作らないとアウト
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



    void Awake()
    {
        // 覚醒技専用カメラをOFFにする
        m_Insp_ArousalAttackCamera1.enabled = false;
        m_Insp_ArousalAttackCamera2.enabled = false;
    }

	// 開幕時に実行
	void Start () 
	{
        // アニメーションファイルの名前（必ずしも動作と一致するわけではない点に注意）
        this.m_AnimationNames = new string[]
        {
            "idle_homura_battle_ribon_copy",                 //Idle,                   // 通常
            "walk_homura_battle_ribon_copy",                 //Walk,                   // 歩行
            "walk_homura_battle_ribon_under_only_copy",      //Walk_underonly,         // 歩行（下半身のみ）       
            "jump_homura_battle_ribon_copy",                 //Jump,                   // ジャンプ開始
            "jump_homura_battle_ribon_under_only_copy",      //Jump_underonly,         // ジャンプ開始（下半身のみ）
            "jumping_homura_battle_ribon_copy",              //Jumping,                // ジャンプ中（上昇状態）
            "jumping_homura_battle_ribon_under_only_copy",   //Jumping_underonly,      // ジャンプ中（下半身のみ）
            "homura_bow_fall_copy",                          //Fall,                   // 落下
            "landing_homura_battle_ribon_copy",              //Landing,                // 着地
            "homura_bow_run_copy",                           //Run,                    // 走行
            "homura_bow_run_under_only_copy",                //Run_underonly,          // 走行（下半身のみ）
            "homura_bow_AirDash_copy",                       //AirDash,                // 空中ダッシュ
            "frontstep_homura_battle_ribon_copy",            //FrontStep,              // 前ステップ中        
            "homura_bow_leftstep_copy",                      //LeftStep,               // 左（斜め）ステップ中       
            "homura_bow_rightstep_copy",                     //RightStep,              // 右（斜め）ステップ中        
            "backstep_homura_battle_ribon_copy",             //BackStep,               // 後ステップ中
            "frontstep_back_homura_battle_ribon_copy",       //FrontStepBack,          // 前ステップ終了
            "homura_bow_leftstep_back_copy",                 //LeftStepBack,           // 左（斜め）ステップ終了
            "homura_bow_rightstep_back_copy",                //RightStepBack,          // 右（斜め）ステップ終了
            "backstep_back_homura_battle_ribon_copy",        //BackStepBack,           // 後ステップ終了
            "fall_homura_battle_ribon_copy",                 //FallStep,               // ステップで下降中
            "shot_homura_battle_ribon_copy",                 //Shot,                   // 通常射撃（装填）
            "shot_homura_battle_ribon_upper2_copy",          //Shot_toponly,          // 通常射撃（上半身のみ）
            "",                                              //Shot_run
            "shot_homura_battle_ribon_copy",                 //Shot_Air,               // 空中で通常射撃
            "chargeshot_homura_battle_ribon_copy",           //Charge_Shot,            // 射撃チャージ
            "subshot_homura_battle_ribon_copy",              //Sub_Shot,               // サブ射撃
            "exshot_homura_battle_ribon_copy",               //EX_Shot,                // 特殊射撃            
            "Homura_Final_Battle_Wrestle1_copy",             //Wrestle_1,              // N格1段目
            "Homura_Final_Battle_Wrestle2_copy",             //Wrestle_2,              // N格2段目
            "Homura_Final_Battle_Wrestle3_copy",             //Wrestle_3,              // N格3段目
            "wrestle1_homura_battle_ribon_copy",             //Charge_Wrestle,         // 格闘チャージ
            "front_wrestle_homura_battle_ribon_copy",        //Front_Wrestle_1,        // 前格闘1段目
            "shot_shoot_homura_battle_ribon_copy",           //Front_Wrestle_2,        // 通常射撃（射出）
            "",                                              //Front_Wrestle_3,        // 前格闘3段目
            "left_wrestle_homura_battle_ribon_copy",         //Left_Wrestle_1,         // 左横格闘1段目
            "",                                              //Left_Wrestle_2,         // 左横格闘2段目
            "",                                              //Left_Wrestle_3,         // 左横格闘3段目
            "right_wrestle_homura_battle_ribon_copy",        //Right_Wrestle_1,        // 右横格闘1段目
            "",                                              //Right_Wrestle_2,        // 右横格闘2段目
            "",                                              //Right_Wrestle_3,        // 右横格闘3段目
            "back_wrestle_homura_battle_ribon_copy",         //Back_Wrestle,           // 後格闘（防御）
            "bd_wrestle_homura_battle_ribon_copy",           //AirDash_Wrestle,        // 空中ダッシュ格闘
            "ex_wrestle_homura_battle_ribon_copy",           //Ex_Wrestle_1,           // 特殊格闘1段目
            "",                                              //Ex_Wrestle_2,           // 特殊格闘2段目
            "",                                              //Ex_Wrestle_3,           // 特殊格闘3段目
            "frontex_wrestle_homura_battle_ribon_copy",      //EX_Front_Wrestle_1,     // 前特殊格闘1段目
            "",                                              //EX_Front_Wrestle_2,     // 前特殊格闘2段目
            "",                                              //EX_Front_Wrestle_3,     // 前特殊格闘3段目
            "",                                              //EX_Left_Wrestle_1,      // 左横特殊格闘1段目
            "",                                              //EX_Left_Wrestle_2,      // 左横特殊格闘2段目
            "",                                              //EX_Left_Wrestle_3,      // 左横特殊格闘3段目
            "",                                              //EX_Right_Wrestle_1,     // 右横特殊格闘1段目
            "",                                              //EX_Right_Wrestle_2,     // 右横特殊格闘2段目
            "",                                              //EX_Right_Wrestle_3,     // 右横特殊格闘3段目        
            "backex_wrestle_homura_battle_ribon_copy",       //BACK_EX_Wrestle,        // 後特殊格闘
            "down_rebirth_homura_battle02_copy",             //Reversal,               // ダウン復帰
            "ex_burst_homura_battle_ribon_copy",             //Arousal_Attack,         // 覚醒技
            "damage_homura_battle_ribon_copy",               //Damage,                 // ダメージ
            "down_homura_battle_ribon_copy",                 //Down,                   // ダウン
            "nomove_homura_battle_ribon_copy",               //Nomove                  // 動きなし
            "",                                              //Blow                    // 吹き飛び
            "",                                              //FrontStepBack_Standby,  // 前ステップ終了(アニメはなし）
            "",                                              //LeftStepBack_Standby,   // 左（斜め）ステップ終了(アニメはなし）
            "",                                              //RightStepBack_Standby,  // 右（斜め）ステップ終了(アニメはなし）
            "",                                              //BackStepBack_Standby,   // 後ステップ終了(アニメはなし）
            "",                                              //DamageInit,             // ダメージ前処理(アニメはなし）
            "",                                              //BlowInit,               // 吹き飛び前処理(アニメはなし）
            "homura_bow_spin_down_copy",                     //SpinDown                // 錐揉みダウン
            "",                                              //穴埋め用の空要素
        };

        // 誰であるかを定義(インスペクターで拾う)
        // レベル・攻撃力レベル・防御力レベル・残弾数レベル・ブースト量レベル・覚醒ゲージレベルを初期化
        SettingPleyerLevel();
		
		// ここ以降はsavingparameterから拾う（タイトル→スタートの流れができてから）
			
		
        // 取得技（オートスキル含む）
        this.m_SkillUse = 10;      // 1つ習得するたびにインクリメント

        // レベル関係はここまで

       
        // ジャンプ硬直
        this.m_JumpWaitTime = 0.5f;

        //着地硬直
        this.m_LandingWaitTime = 1.0f;

        this.m_WalkSpeed    = 1.0f;                             // 移動速度（歩行の場合）
        this.m_RunSpeed     = 15.0f;                            // 移動速度（走行の場合）
        this.m_AirDashSpeed = 20.0f;                             // 移動速度（空中ダッシュの場合）
        this.m_AirMoveSpeed = 7.0f;                             // 移動速度（空中慣性移動の場合）
        this.m_RateofRise   = 5.0f;                             // 上昇速度

        // ブースト消費量
        this.m_JumpUseBoost = 10;       // ジャンプ時
        this.m_DashCancelUseBoost = 10;   // ブーストダッシュ時
        this.m_StepUseBoost = 10;         // ステップ時
        this.m_BoostLess = 0.5f;        // ジャンプの上昇・BD時の1F当たりの消費量
 
        // ステップ移動距離
        this.m_Step_Move_Length = 100.0f;
        
        // ステップ初速（X/Z軸）
        this.m_Step_Initial_velocity = 30.0f;
        // ステップ時の１F当たりの移動量
        this.m_Step_Move_1F = this.m_Step_Move_Length / this.m_Step_Initial_velocity;
        // ステップ終了時硬直時間
        this.m_StepBackTime = 0.4f;               

        // コライダの地面からの高さ
        this.m_Collider_Height = 1.5f;

        // ロックオン距離
        this.m_Rockon_Range = 100.0f;

        // ロックオン限界距離
        this.m_Rockon_RangeLimit = 200.0f;

        // ショットのステート
        shotmode = ShotMode.NORMAL;

		// 弾のステート
		this.m_BulletMoveDirection = Vector3.zero;
		this.m_BulletPos = Vector3.zero;
		
		// HPを初期化
		
         // メイン射撃撃ち終わり時間
        m_mainshotendtime = 0.0f;
        // サブ射撃撃ち終わり時間
        m_subshotendtime = 0.0f;
        // 特殊射撃撃ち終わり時間
        m_exshotendtime = 0.0f;
        

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
            m_reload.OneByOne(ref m_BulletNum[(int)ShotType.NORMAL_SHOT], Time.time, Character_Spec.cs[(int)m_character_name][(int)ShotType.NORMAL_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)ShotType.NORMAL_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)m_character_name][(int)ShotType.NORMAL_SHOT].m_reloadtime, ref m_mainshotendtime);
            // サブ射撃
            m_reload.AllTogether(ref m_BulletNum[(int)ShotType.SUB_SHOT], Time.time, Character_Spec.cs[(int)m_character_name][(int)ShotType.SUB_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)ShotType.SUB_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)m_character_name][(int)ShotType.SUB_SHOT].m_reloadtime, ref m_subshotendtime);
            // 特殊射撃
            m_reload.OneByOne(ref m_BulletNum[(int)ShotType.EX_SHOT], Time.time, Character_Spec.cs[(int)m_character_name][(int)ShotType.EX_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)ShotType.EX_SHOT].m_OriginalBulletNum,
                Character_Spec.cs[(int)m_character_name][(int)ShotType.EX_SHOT].m_reloadtime, ref m_exshotendtime);
        }
        // 特定条件下でZ軸回転するバグがあるので防止
        if (Math.Abs(this.transform.rotation.eulerAngles.z) > 0f)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, 0));
        }
	}

    // 攻撃行動全般(RunとAirDashは特殊なので使用しない）
    // run[in]      :走行中入力
    // AirDash[in]  :空中ダッシュ中入力
    private void AttackDone(bool run = false,bool AirDash = false)
    {
        // サブ射撃でサブ射撃へ移行
        if (m_hasSubShotInput)
        {
            // 歩き撃ちはしないので、強制停止
            if (run || AirDash)
            {
                EmagencyStop();
            }
            SubShotDone();
        }
        // 特殊射撃で特殊射撃へ移行
        else if (m_hasExShotInput)
        {
            // 歩き撃ちはしないので、強制停止
            if (run || AirDash)
            {
                EmagencyStop();
            }
            ExShotDone();
        }
        // 特殊格闘で特殊格闘へ移行
        else if (m_hasExWrestleInput)
        {
            // 前特殊格闘
            if (m_hasFrontInput)
            {
                WrestleDone_UpperEx((int)SkillType_Homura_B.EX_FRONT_WRESTLE_1);
            }
            // 空中で後特殊格闘
            else if (m_hasBackInput && !m_isGrounded)
            {
                WrestleDone_DownEx((int)SkillType_Homura_B.BACK_EX_WRESTLE);
            }
            // それ以外
            else
            {
                WrestleDone(AnimationState.Ex_Wrestle_1, (int)SkillType_Homura_B.EX_WRESTLE_1);
            }
        }
        // 射撃チャージでチャージ射撃へ移行
        else if (m_hasShotChargeInput)
        {
            // 歩き撃ちはしないので、強制停止
            if (run || AirDash)
            {
                this.m_MoveDirection = Vector3.zero;
                m_AnimState[0] = AnimationState.Idle;
            }
            ChargeShotDone();
        }
        // 射撃で射撃へ移行
        else if (m_hasShotInput)
        {
            if (run)
            {
                if (this.m_IsRockon)
                {
                    // ①　transform.TransformDirection(Vector3.forward)でオブジェクトの正面の情報を得る
                    var forward = this.transform.TransformDirection(Vector3.forward);
                    // ②　自分の場所から対象との距離を引く
                    // カメラからEnemyを求める
                    var target = m_MainCamera.transform.GetComponentInChildren<Player_Camera_Controller>();
                    var targetDirection = target.Enemy.transform.position - transform.position;
                    // ③　①と②の角度をVector3.Angleで取る　			
                    float angle = Vector3.Angle(forward, targetDirection);
                    // 角度60度以内なら上体回しで撃つ（歩き撃ち限定で上記の矢の方向ベクトルを加算する）
                    if (angle < 60)
                    {
                        m_RunShotDone = true;
                        ShotDone();
                    }
                    // それ以外なら強制的に停止して（立ち撃ちにして）撃つ
                    else
                    {
                        this.m_MoveDirection = Vector3.zero;
                        m_AnimState[0] = AnimationState.Idle;
                        ShotDone();
                    }
                }
                // 非ロック状態なら歩き撃ちフラグを立てる
                else
                {
                    m_RunShotDone = true;
                    ShotDone();
                }
            }
            else
            {
                ShotDone();
            }
        }
        // 格闘で格闘へ移行
        else if (m_hasWrestleInput)
        {
            if (AirDash)
            {
                WrestleDone(AnimationState.AirDash_Wrestle, (int)SkillType_Homura_B.AIRDASH_WRESTLE);
            }
            else
            {
                // 前格闘と横格闘への分岐はここで（2段目・3段目の分岐はやっているときに）

                // 前格闘で前格闘へ移行
                if (m_hasFrontInput)
                {
                    WrestleDone(AnimationState.Front_Wrestle_1, (int)SkillType_Homura_B.FRONT_WRESTLE_1);
                }
                // 左格闘で左格闘へ移行
                else if (m_hasLeftInput)
                {
                    WrestleDone_GoAround_Left(AnimationState.Left_Wrestle_1, (int)SkillType_Homura_B.LEFT_WRESTLE_1);
                }
                // 右格闘で右格闘へ移行
                else if (m_hasRightInput)
                {
                    WrestleDone_GoAround_Right(AnimationState.Right_Wrestle_1, (int)SkillType_Homura_B.RIGHT_WRESTLE_1);
                }
                // 後格闘で後格闘へ移行
                else if (m_hasBackInput)
                {
                    GuardDone((int)SkillType_Homura_B.BACK_WRESTLE);
                }
                else
                {
                    // N格闘1段目  
                    WrestleDone(AnimationState.Wrestle_1, (int)SkillType_Homura_B.WRESTLE_1);
                }
            }
        }
    }
   
    // 入力系のオーバーロード
    // idle(攻撃系モーションの入力受け付けを入れる）
    protected override void Animation_Idle()
    {
        // モーションを戻す
        ReturnMotion();
        base.Animation_Idle();
        // くっついている弾丸系のエフェクトを消す
        DestroyArrow();
        // 羽フックを壊す
        Destroy(m_wingHock);
        // 格闘の累積時間を初期化
        m_wrestletime = 0;
        // 地上にいるか？(落下開始時は一応禁止）
        if (m_isGrounded)
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
        AttackDone();
    }

    // run
    protected override void Animation_Run()
    {        
        ReturnMotion();
        base.Animation_Run();
        AttackDone(true, false);
    }

    // airDash
    protected override void Animation_AirDash()
    {
        base.Animation_AirDash();
        AttackDone(false, true);
    }

 

    // DamageInit
    public override void DamageInit(CharacterControl_Base.AnimationState animationstate)
    {
        base.DamageInit(animationstate);
        // ほむらで本体にくっついていた矢をカット（あるなら）
        DestroyArrow();
        // 同じく格闘判定をカット
        DestroyWrestle();
    }

    

    // 射撃中共通動作（空中にいるときはダッシュキャンセルを有効にする）
    protected override void Shot()
    {
        // キャンセルダッシュ受付
        if (this.m_hasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.m_isGrounded)
            {
                rigidbody.position = new Vector3(this.rigidbody.position.x, this.rigidbody.position.y + 3, this.rigidbody.position.z);
            }
            CancelDashDone();
        }
        base.Shot();    // base+関数名で継承元を実行可能
        
        		
    }

    // チャージ射撃中共通動作
    protected override void ChargeShot()
    {
        // キャンセルダッシュ受付
        if (this.m_hasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.m_isGrounded)
            {
                rigidbody.position = new Vector3(this.rigidbody.position.x, this.rigidbody.position.y + 3, this.rigidbody.position.z);
            }
            CancelDashDone();
        }
        base.ChargeShot();
       
    }

    // 歩き撃ち中共通動作
    protected override void ShotRun()
    {
        // キャンセルダッシュ受付
        if (this.m_hasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.m_isGrounded)
            {
                rigidbody.position = new Vector3(this.rigidbody.position.x, this.rigidbody.position.y + 3, this.rigidbody.position.z);
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
        if (this.m_hasDashCancelInput)
        {
            // 地上でキャンセルすると浮かないので浮かす
            if (this.m_isGrounded)
            {
                rigidbody.position = new Vector3(this.rigidbody.position.x, this.rigidbody.position.y + 3, this.rigidbody.position.z);
            }
            CancelDashDone();
        }
        base.ShotAirDash();
        moveshot(false);
    }


    // ダッシュキャンセル時の処理
    // タイミングによっては矢が残る
    protected override void CancelDashDone()
    {
        // 弾があるなら消す(m_ArrowRootの下に何かあるなら全部消す）
        DestroyArrow();
        // 格闘判定があるなら消す
        DestroyWrestle();
        // 歩き撃ちフラグを折る
        m_RunShotDone = false;
        // 上体を戻す
        m_Brest.transform.rotation = Quaternion.Euler(0, 0, 0);
        // モーションを戻す
        shotmode = ShotMode.NORMAL;
        DeleteBlend();
        // 固定状態を解除
        this.transform.rigidbody.constraints = RigidbodyConstraints.None;
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
        this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        // 通常のダッシュキャンセルの処理
        base.CancelDashDone();
    }


    // 攻撃アニメーション終了判定
    // 第1引数：アニメの名前
    // 第2引数：アニメの種類
    protected bool ShotEndCheck(string AnimationName,ShotType type)
    {
        while (this.animation.IsPlaying(AnimationName))
        {
            return false;
        }

        // サブ射撃・チャージ射撃・特殊射撃の場合(強制的に停止か落下になる）
        if (type == ShotType.SUB_SHOT || type == ShotType.CHARGE_SHOT || type == ShotType.EX_SHOT)
        {
            if (Time.time > this.m_AttackTime + this.m_BulletWaitTime[(int)type])
            {
                // 合成状態を解除
                ReturnMotion();
                // 地上
                if (m_isGrounded)
                {
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
                    this.animation[m_AnimationNames[(int)AnimationState.Idle]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Idle;
                    // ショットのステートを戻す
                    shotmode = ShotMode.NORMAL;
                }
                // 空中
                else
                {
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
                    this.animation[m_AnimationNames[(int)AnimationState.Fall]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Fall;
                    m_fallStartTime = Time.time;
                }
                // 固定状態を解除
                
                // ショットのステートを戻す
                shotmode = ShotMode.NORMAL;
                return true;
            }
        }
        // 通常射撃の場合
        else if (type == ShotType.NORMAL_SHOT)
        {
            // 終わった後でなく、開始時にも来るのでSHOTDONEを追加
            if (Time.time > this.m_AttackTime + this.m_BulletWaitTime[(int)type])
            {
                // 合成状態を解除
                ReturnMotion();
                // 地上にいて静止中
                if (m_isGrounded && !this.m_hasVHInput)
                {
                    // アイドルモードのアニメを起動する
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
                    this.animation[m_AnimationNames[(int)AnimationState.Idle]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Idle;
                }
                // 地上にいて歩行中
                else if (m_isGrounded)
                {
                    // 走行モードのアニメを起動する
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Run]);
                    this.animation[m_AnimationNames[(int)AnimationState.Run]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Run;
                }
                // 空中にいてダッシュ入力中でありかつブーストゲージがある
                else if (!m_isGrounded && m_hasVHInput && m_hasJumpInput && this.m_Boost > 0)                
                {
                    // 空中ダッシュのアニメを起動する
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.AirDash]);
                    this.animation[m_AnimationNames[(int)AnimationState.AirDash]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.AirDash;
                }
                // 空中にいて落下中(歩き撃ちをしていた場合を除く）
                else //if (!m_isGrounded && !this.m_ShotRun)
                {
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
                    this.animation[m_AnimationNames[(int)AnimationState.Fall]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Fall;
                    m_fallStartTime = Time.time;
                    // ショットのステートを戻す
                    shotmode = ShotMode.NORMAL;
                }
                return true;
            }
        }
        return false;
    }

    
    
    // 装填開始（チャージ射撃）
    protected void ChargeShotDone()
    {
        m_AnimState[0] = AnimationState.Charge_Shot;
        this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Charge_Shot]);
        this.shotmode = ShotMode.RELORD;
        // 弾数を戻しておく
        ///this.m_BulletNum[(int)ShotType.CHARGE_SHOT] = 1;
    }

    // 装填開始（サブ射撃）
    protected void SubShotDone()
    {
        m_AnimState[0] = AnimationState.Sub_Shot;
        this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Sub_Shot]);
        this.shotmode = ShotMode.RELORD;
    }

    // 装填開始（特殊射撃）
    protected void ExShotDone()
    {
        m_AnimState[0] = AnimationState.EX_Shot;
        this.animation.CrossFade(m_AnimationNames[(int)AnimationState.EX_Shot]);
        this.shotmode = ShotMode.RELORD;
    }

   

    // 射撃(通常射撃装填。この関数は通常射撃のアニメにリンクする。弾消費後の処理は共用できる）
    protected void Shoot(ShotType type)
    {       
        // 弾があるとき限定
        if (this.m_BulletNum[(int)type] > 0)
        {
            // ロックオン時本体の方向を相手に向ける       
            if (this.m_IsRockon)
            {
                RotateToTarget();
            }      
            // 弾を消費する（サブ射撃なら1、特殊射撃なら2）
            // チャージ射撃除く
            if (type != ShotType.CHARGE_SHOT)
            {
                this.m_BulletNum[(int)type]--;
                // 撃ち終わった時間を設定する                
                // メイン（弾数がMax-1のとき）
                if (type == ShotType.NORMAL_SHOT && m_BulletNum[(int)type] == Character_Spec.cs[(int)m_character_name][(int)type].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)type].m_OriginalBulletNum - 1)
                {
                    m_mainshotendtime = Time.time;
                }
                // サブ（弾数が0のとき）
                else if (type == ShotType.SUB_SHOT&& m_BulletNum[(int)type] == 0)
                {
                    m_subshotendtime = Time.time;
                }
                // 特殊（弾数がMax-1のとき）
                else if (type == ShotType.EX_SHOT && m_BulletNum[(int)type] == Character_Spec.cs[(int)m_character_name][(int)type].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)type].m_OriginalBulletNum - 1)
                {
                    m_exshotendtime = Time.time;
                }
            }
            // 矢の出現ポジションをフックと一致させる
            Vector3 pos = m_ArrowRoot.transform.position;
            Quaternion rot = Quaternion.Euler(m_ArrowRoot.transform.rotation.eulerAngles.x, m_ArrowRoot.transform.rotation.eulerAngles.y, m_ArrowRoot.transform.rotation.eulerAngles.z);
            //m_ArrowRoot.rigidbody.isKinematic = true;
            // 矢を出現させる
            // サブ射撃
            if (type == ShotType.SUB_SHOT)
            {
                var obj = (GameObject)Instantiate(m_Insp_SubShotArrow, pos, rot);
                // 親子関係を再設定する(=矢をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ArrowRoot.transform;
                    // 矢の親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
            }
            // 特殊射撃
            else if (type == ShotType.EX_SHOT)
            {
                // 中央の矢を作る
                var obj = (GameObject)Instantiate(m_Insp_ExShotArrow, pos, rot);
                // 親子関係を再設定する(=矢をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ArrowRoot.transform;
                    // 矢の親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
                // 左の矢を作る
                // 座標と角度を生成
                Vector3 pos_left = m_Insp_ArrowRoot_Left.transform.position;
                Quaternion rot_left = Quaternion.Euler(m_Insp_ArrowRoot_Left.transform.rotation.eulerAngles.x, m_Insp_ArrowRoot_Left.transform.rotation.eulerAngles.y, m_Insp_ArrowRoot_Left.transform.rotation.eulerAngles.z);
                // 矢を生成
                var obj_left = (GameObject)Instantiate(m_Insp_ExShotArrow_L, pos_left, rot_left);
                //// 親子関係を再設定する(=矢をフックの子にする）
                if (obj_left.transform.parent == null)
                {
                    obj_left.transform.parent = m_Insp_ArrowRoot_Left.transform;
                    obj_left.transform.rigidbody.isKinematic = true;
                }
                // 右の矢を作る
                Vector3 pos_right = m_Insp_ArrowRoot_Right.transform.position;
                Quaternion rot_right = Quaternion.Euler(m_Insp_ArrowRoot_Right.transform.rotation.eulerAngles.x, m_Insp_ArrowRoot_Right.transform.rotation.eulerAngles.y, m_Insp_ArrowRoot_Right.transform.rotation.eulerAngles.z);
                // 矢を生成
                var obj_right = (GameObject)Instantiate(m_Insp_ExShotArrow_R, pos_right, rot_right);
                // 親子関係を再設定する(=矢をフックの子にする）
                if (obj_right.transform.parent == null)
                {
                    obj_right.transform.parent = m_Insp_ArrowRoot_Right.transform;
                    obj_right.transform.rigidbody.isKinematic = true;
                }
            }
            // 通常射撃(varは暗黙的な初期化ができない)
            else if (type == ShotType.NORMAL_SHOT)
            {
                var obj = (GameObject)Instantiate(m_Insp_NormalArrow, pos, rot);
                // 親子関係を再設定する(=矢をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ArrowRoot.transform;
                    // 矢の親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
            }
            // チャージ射撃
            else if (type == ShotType.CHARGE_SHOT)
            {
                var obj = (GameObject)Instantiate(m_Insp_ChargeShotArrow, pos, rot);
                // 親子関係を再設定する(=矢をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ArrowRoot.transform;
                    // 矢の親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
            }
            // 空中ダッシュ時以外はいったん止める
            if (this.m_AnimState[0] != AnimationState.Shot_AirDash)
            {
                this.transform.rigidbody.constraints = RigidbodyConstraints.FreezePosition;
                this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
    // 射出音
    public AudioClip m_Insp_ShootSE;

    // 射撃（射出）弾丸消費はここでやらない。アニメ中イベントはステートと同期するとは限らないので、弾丸消費はアニメ中イベントではやるべきではない
    protected void Shoot2(ShotType type)
    {
        // この時点では親子なので、Homura_NormalShotAllowsを持ったスクリプトを拾う(デフォルトで持っていないので、スクリプトで検索するしかない）        
        var arrow = GetComponentInChildren<Homra_NormalShotAllow>();
        // 特殊射撃左の場合 
        var arrow_ex_L = GetComponentInChildren<Homura_EX_ShotArrow_L>(); 
        // 特殊射撃右の場合
        var arrow_ex_R = GetComponentInChildren<Homura_EX_ShotArrow_R>();

        if (arrow != null || arrow_ex_L != null || arrow_ex_R != null)
        {
            // 矢のスクリプトの速度を設定する
            // チャージ射撃は若干速く
            if (type == ShotType.CHARGE_SHOT)
            {
                arrow.m_Speed = Character_Spec.cs[(int)m_character_name][1].m_Movespeed;
            }
            else
            {
                if (arrow != null)
                {
                    arrow.m_Speed = Character_Spec.cs[(int)m_character_name][0].m_Movespeed;
                }
                // 特殊射撃の左右の2発
                if (arrow_ex_L != null)
                {
                    arrow_ex_L.m_Speed = Character_Spec.cs[(int)m_character_name][3].m_Movespeed;
                }
                if (arrow_ex_R != null)
                {
                    arrow_ex_R.m_Speed = Character_Spec.cs[(int)m_character_name][3].m_Movespeed;
                }
            }
            // 矢の方向を決定する(本体と同じ方向に向けて打ち出す。ただしノーロックで本体の向きが0のときはベクトルが0になるので、このときだけはカメラの方向に飛ばす）

            // ロックオン状態で歩き撃ちをしているとき
            if (this.m_IsRockon && this.m_RunShotDone)
            {
                // ロックオン対象の座標を取得
                var target = GetComponentInChildren<Player_Camera_Controller>();
                // 対象の座標を取得
                Vector3 targetpos = target.Enemy.transform.position;
                // 補正値込みの胸部と本体の回転ベクトルを取得
                // 本体
                Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
                // 胸部
                Vector3 normalizeRot_OR = m_Brest.transform.rotation.eulerAngles;
                // 本体と胸部と矢の補正値分回転角度を合成
                Vector3 addrot = mainrot.eulerAngles + normalizeRot_OR - new Vector3(0,74.0f,0); 
                Quaternion qua = Quaternion.Euler(addrot);
                // forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
                Vector3 normalizeRot = (qua) * Vector3.forward;
                // 移動ベクトルを確定する
                arrow.m_MoveDirection = Vector3.Normalize(normalizeRot);
            }
            // ロックオンしているとき
            else if (this.m_IsRockon)
            {
                // ロックオン対象の座標を取得
                var target = GetComponentInChildren<Player_Camera_Controller>();
                // 対象の座標を取得
                Vector3 targetpos = target.Enemy.transform.position;
                // 本体の回転角度を拾う
                Quaternion mainrot = Quaternion.LookRotation(targetpos - this.transform.position);
                // 正規化して代入する
                Vector3 normalizeRot = mainrot * Vector3.forward;
                // 特殊射撃左の時
                if (arrow_ex_L != null)
                {
                    arrow_ex_L.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
                // 特殊射撃右の時
                if (arrow_ex_R != null)
                {
                    arrow_ex_R.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
                // それ以外
                if(arrow != null)
                {                   
                    arrow.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
            }
            // ロックオンしていないとき
            else
            {
                // 本体角度が0の場合カメラの方向を射出角とし、正規化して代入する
                if (this.transform.rotation.eulerAngles == Vector3.zero)
                {
                    // ただしそのままだとカメラが下を向いているため、一旦その分は補正する
                    Quaternion rotateOR = m_MainCamera.transform.rotation;
                    Vector3 rotateOR_E = rotateOR.eulerAngles;
                    rotateOR_E.x = 0;
                    rotateOR = Quaternion.Euler(rotateOR_E);
                    // 特殊射撃左の時
                    if (arrow_ex_L != null)
                    {
                        arrow_ex_L.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                    }
                    // 特殊射撃右の時
                    if (arrow_ex_R != null)
                    {
                        arrow_ex_R.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                    }
                    // 特殊射撃以外
                    if (arrow != null)
                    {
                        arrow.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                    }
                }
                // それ以外は本体の角度を射出角にする
                else
                {
                    // 特殊射撃左の時
                    if (arrow_ex_L != null)
                    {
                        arrow_ex_L.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                    }
                    // 特殊射撃右の時
                    if (arrow_ex_R != null)
                    {
                        arrow_ex_R.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                    }
                    // 特殊射撃以外
                    if (arrow != null)
                    {
                        arrow.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                    }
                }
            }

            // 矢のフックの位置に弾の位置を代入する
            this.m_BulletPos = this.m_ArrowRoot.transform.position;
            // 同じく回転角を代入する
            if (arrow != null)
            {
                this.m_BulletMoveDirection = arrow.m_MoveDirection;
            }
            if (type == ShotType.NORMAL_SHOT)
            {
                setOffensivePower(SkillType_Homura_B.SHOT);
            }
            else if (type == ShotType.CHARGE_SHOT)
            {
                setOffensivePower(SkillType_Homura_B.CHARGE_SHOT);
            }
            else if (type == ShotType.SUB_SHOT)
            {
                setOffensivePower(SkillType_Homura_B.SUB_SHOT);
            }
            else if (type == ShotType.EX_SHOT)
            {
                setOffensivePower(SkillType_Homura_B.EX_SHOT);
            }
            shotmode = ShotMode.SHOT;

            // 固定状態を解除
            // ずれた本体角度を戻す(Yはそのまま）
            this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
            this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            
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

    // 射撃攻撃の攻撃力とダウン値を決定する
    // kind[in]     :射撃の種類
    private void setOffensivePower(SkillType_Homura_B kind)
    {
        // 攻撃力を決定する(ここの2がスキルのインデックス。下も同様）
        this.m_offensive_power = Character_Spec.cs[(int)m_character_name][(int)kind].m_OriginalStr + Character_Spec.cs[(int)m_character_name][(int)kind].m_GrowthCoefficientStr * (this.m_StrLevel - 1);
        // ダウン値を決定する
        this.m_downratio_power = Character_Spec.cs[(int)m_character_name][(int)kind].m_DownPoint;
        // 覚醒ゲージ増加量を決定する
        m_arousalRatio = Character_Spec.cs[(int)m_character_name][(int)kind].m_arousal;
    }

    // N格闘1段目
    protected override void Wrestle1()
    {
        base.Wrestle1();
        // 追加の格闘入力を受け取ったら、派生フラグを立てる
        if (this.m_hasWrestleInput || this.m_hasExWrestleInput)
        {
            this.m_addInput = true;
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.WRESTLE_1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }
    // N格闘2段目
    protected override void Wrestle2()
    {
        base.Wrestle2();
        // 追加の格闘入力を受け取ったら、派生フラグを立てる
        if (this.m_hasWrestleInput || this.m_hasExWrestleInput)
        {
            this.m_addInput = true;
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.WRESTLE_2].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }
    // N格闘3段目
    protected override void Wrestle3()
    {
        base.Wrestle3();
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.WRESTLE_2].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // 特殊格闘(StepCancelは格闘属性でないキャラもいるのでこっちにつける）
    protected override void ExWrestle1()
    {
        m_wrestletime += Time.deltaTime;
        base.ExWrestle1();
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.EX_WRESTLE_1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // 前格闘
    protected override void FrontWrestle1()
    {
        base.FrontWrestle1();
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.FRONT_WRESTLE_1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // 左格闘
    protected override void LeftWrestle1()
    {
        base.LeftWrestle1();
        // 強制的にロックオン対象の方向を向ける
        if (m_IsRockon)
        {
            // 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
            // このため、高低差がないとみなす
            Vector3 Target_VertualPos = target.Enemy.transform.position;
            Target_VertualPos.y = 0;
            Vector3 Mine_VerturalPos = this.transform.position;
            Mine_VerturalPos.y = 0;
            this.transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.LEFT_WRESTLE_1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // 右格闘
    protected override void RightWrestle1()
    {
        base.RightWrestle1();
        // 強制的にロックオン対象の方向を向ける
        if (m_IsRockon)
        {
            // 対象の座標を取得（カメラ(m_MainCamera)→Enemy)
            var target = m_MainCamera.GetComponentInChildren<Player_Camera_Controller>();
            // 角度を逆算(ステップ時常時相手の方向を向かせる.ただしWY回転のみ）
            // このため、高低差がないとみなす
            Vector3 Target_VertualPos = target.Enemy.transform.position;
            Target_VertualPos.y = 0;
            Vector3 Mine_VerturalPos = this.transform.position;
            Mine_VerturalPos.y = 0;
            this.transform.rotation = Quaternion.LookRotation(Target_VertualPos - Mine_VerturalPos);
        }
        // 一定時間経ったら、強制終了
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.RIGHT_WRESTLE_1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    

    void LateUpdate()
    {
        // ロックオン時首を相手の方向へ向ける(本体角度との合成になっているので、本体角度分減算してやらないと正常に向かない）
        // あと、射撃時はここをやらないこと（弓ほむら・まどかは上体を曲げるので、上体ごと相手の方を向ける）
        if (this.m_IsRockon && this.m_AnimState[0] != AnimationState.Shot && this.m_AnimState[0] != AnimationState.EX_Shot
            && this.m_AnimState[0] != AnimationState.Sub_Shot && this.m_AnimState[0] != AnimationState.Front_Wrestle_2
            && this.m_AnimState[0] != AnimationState.Shot_run && this.m_AnimState[0] != AnimationState.Charge_Shot && !m_RunShotDone)
        {
            SetNeckRotate(this.m_Head, 0.0f);
        }

        // 歩き射撃時,上体をロックオン対象へ向ける
        if (this.m_IsRockon && m_RunShotDone)
        {
            SetNeckRotate(this.m_Brest, 74.0f);
        }
        // ロックオンしていないときはそのまま本体角度＋規定角度回す
        else if (m_RunShotDone)
        {
            // 本体角度
            Vector3 rotate = this.transform.rotation.eulerAngles;
            m_Brest.transform.rotation = Quaternion.Euler(0, 74 + rotate.y, 0);
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
        // チャージ射撃
        else if (this.m_AnimState[0] == AnimationState.Charge_Shot)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.Charge_Shot], ShotType.CHARGE_SHOT))
            {
                this.m_AnimState[1] = AnimationState.Charge_Shot;
            }
        }
        // サブ射撃
        else if (this.m_AnimState[0] == AnimationState.Sub_Shot)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.Sub_Shot], ShotType.SUB_SHOT))
            {
                this.m_AnimState[1] = AnimationState.Sub_Shot;
            }
        }
        // 特殊射撃
        else if (this.m_AnimState[0] == AnimationState.EX_Shot)
        {
            if (!ShotEndCheck(m_AnimationNames[(int)AnimationState.EX_Shot], ShotType.EX_SHOT))
            {
                this.m_AnimState[1] = AnimationState.EX_Shot;
            }
        }
        // savingparameterの値を反映させる
        LateUdate_Core();
    }

    
    

    // 覚醒時の処理を行う
    protected override void ArousalInitialize()
    {
        // 全武装回復
        // メイン
        m_BulletNum[(int)SkillType_Homura_B.SHOT] = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.SHOT].m_OriginalBulletNum;
        // サブ
        m_BulletNum[(int)SkillType_Homura_B.SUB_SHOT] = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.SUB_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.SUB_SHOT].m_OriginalBulletNum;
        // 特殊射撃
        m_BulletNum[(int)SkillType_Homura_B.EX_SHOT] = Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.EX_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)SkillType_Homura_B.EX_SHOT].m_OriginalBulletNum;
        base.ArousalInitialize();
    }

    // 覚醒技の羽フック
    public GameObject m_Insp_WingHock;
    // 覚醒技の羽根エフェクト
    public GameObject m_Insp_WingEffect;
    // 覚醒技の攻撃判定
    public GameObject m_Insp_WingAttacker;

    // 覚醒技中のステート
    private enum ArousalState
    {
        INITIALIZE,     // モーションを歩行へ変更&重力無効化＆羽フックとりつけ
        FEATHERSET,     // 移動しつつエフェクトと判定を順番に取り付け
        ATTACK,         // （ロックオンしている相手に向けて）飛行
        END             // 覚醒ゲージが空になると、Armorを戻してfallへ移行
    };

    private ArousalState m_nowState;
    private GameObject m_wingHock;

    // 覚醒技発動処理
    protected override void ArousalAttack()
    {
        base.ArousalAttack();
        if (!m_InitializeArousal)
        {
            m_nowState = ArousalState.INITIALIZE;
            m_InitializeArousal = true;
        }
        switch (m_nowState)
        {
            case ArousalState.INITIALIZE:   // モーションを歩行に変更&重力を無効化する&本体に羽根フックを取り付ける
                // モーションを歩行に
                this.animation.Play(m_AnimationNames[(int)AnimationState.Walk]);
                // 重力無効
                this.rigidbody.useGravity = false;
                // 羽フック取り付け
                // フック取り付け位置
                Vector3 hockpos = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z - 0.3f);
                m_wingHock = (GameObject)Instantiate(m_Insp_WingHock, hockpos, Quaternion.identity);
                m_wingHock.transform.parent = this.transform;                
                m_nowState = ArousalState.FEATHERSET;
                // 変数初期化
                m_wingAppearCounter = 0;
                m_wingboneCounter = 1;
                m_leftwing_set = false;
                m_arousalAttackTime = 0;
                // 専用カメラをONにする
                m_Insp_ArousalAttackCamera1.enabled = true;
                break;
            case ArousalState.FEATHERSET:   // 羽根フックに一定周期でエフェクトと攻撃判定を取り付けかつ前進させる   
                // 前方向に向けて歩き出す
                // ロックオン時は強制的に相手の方向を向く
                if (m_IsRockon)
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
                this.m_MoveDirection = transform.rotation * Vector3.forward;
                rigidbody.position = rigidbody.position + this.m_MoveDirection * m_WalkSpeed * Time.deltaTime;
                // エフェクト/判定取り付け開始(どのフックに取り付けるか）
                // 一応左→右の順で取り付けていく
                // フックの名前(左）
                string hockname_L = "homura_wing(Clone)/wing_bornl" + Convert.ToString(m_wingboneCounter);
                string hockname_R = "homura_wing(Clone)/wing_bornr" + Convert.ToString(m_wingboneCounter);
                // m_wingappearTime経過して、左の羽根をつけていないなら左の羽根と判定をつける
                if (m_wingAppearCounter > m_wingappearTime && !m_leftwing_set)
                {
                    m_leftwing_set = true;
                    SetWing(hockname_L);
                }
                // さらにm_wingapperTime経過したら右の羽根と判定をつけてリセット
                else if (m_wingAppearCounter > m_wingappearTime * 2)
                {
                    SetWing(hockname_R);
                    m_wingAppearCounter = 0;
                    m_leftwing_set = false;
                    m_wingboneCounter++;
                    // 半分出したらカメラを横に
                    if (m_wingboneCounter > 9)
                    {
                        m_Insp_ArousalAttackCamera1.enabled = false;
                        m_Insp_ArousalAttackCamera2.enabled = true;
                    }
                    // 全部出したらカメラを戻して飛行ポーズにして次へ行く
                    if (m_wingboneCounter > m_maxboneNum)
                    {
                        m_Insp_ArousalAttackCamera2.enabled = false;
                        this.animation.Play(m_AnimationNames[(int)AnimationState.AirDash]);
                        m_nowState = ArousalState.ATTACK;
                    }
                }
                m_wingAppearCounter += Time.deltaTime;
                break;
            case ArousalState.ATTACK:       // （ロックオンしている相手に向けて）飛行開始
                // 前方向に向けて飛行開始
                rigidbody.position = rigidbody.position + this.m_MoveDirection * m_AirDashSpeed * Time.deltaTime;
                m_arousalAttackTime += Time.deltaTime;
                // 飛行時間を終えたらENDへ移行
                if (m_arousalAttackTime > m_arousalAttackTotal)
                {
                    // 羽フックを壊す
                    Destroy(m_wingHock);
                    m_nowState = ArousalState.END;
                }
                break;            
            case ArousalState.END:          // 覚醒ゲージが空になったので、Armorを戻してfallへ移行
                this.animation.Play(m_AnimationNames[(int)AnimationState.Fall]);
                m_IsArmor = false;
                m_AnimState[0] = AnimationState.Fall;
                m_fallStartTime = Time.time;
                break;
            default:
                break;
        }
           
        // 羽根の相対角度は固定する
        if (m_wingHock != null)
        {
            m_wingHock.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        // 全モード共通で、HPが0になると羽と覚醒エフェクトを切ってダウン
    }

    // 羽根エフェクトと攻撃判定を取り付ける
    // hockname [in]:設置位置のフックの名前
    private void SetWing(string hockname)
    {
        // 親取得
        GameObject parenthock = transform.FindChild(hockname).gameObject;
        // エフェクト設置
        GameObject wing = (GameObject)Instantiate(m_Insp_WingEffect, parenthock.transform.position, transform.rotation);
        wing.transform.parent = parenthock.transform;
        // 判定設置
        GameObject decision = (GameObject)Instantiate(m_Insp_WingAttacker, parenthock.transform.position, transform.rotation);
        decision.transform.parent = parenthock.transform;
        // 判定に攻撃力を設定する
        int offensive = m_growthcoffecient_str * (this.m_level - 1) + m_basis_offensive;
        var decision_instance = decision.GetComponentInChildren<Bazooka_ShockWave>();        
        decision_instance.SetDamage(offensive);
        decision_instance.SetCharacter((int)Character_Spec.CHARACTER_NAME.MEMBER_HOMURA_B);
        // 判定に吹き飛び判定を設定する
        decision_instance.SetDownratio(m_downratioArousal);
        // 判定に自分が敵か味方かを教える
        if (m_isPlayer == CHARACTERCODE.PLAYER || m_isPlayer == CHARACTERCODE.PLAYER_ALLY)
        {
            decision_instance.m_IsPlayer = true;
        }
        else
        {
            decision_instance.m_IsPlayer = false;
        }
    }

    // 覚醒技基礎攻撃力
    private const int m_basis_offensive = 270;
    // 覚醒技攻撃力成長係数
    private const int m_growthcoffecient_str = 5;
    // 覚醒技ダウン値
    private const int m_downratioArousal = 5;

    // 羽根と判定が出るときの間隔
    private const float m_wingappearTime = 0.1f;
    // 羽根と判定が出るときのタイマー
    private float m_wingAppearCounter;
    // 羽根と判定が何番目のボーンであるかを示すカウンター
    private int m_wingboneCounter;
    // 左側の羽根と判定をセットしたフラグ
    private bool m_leftwing_set;
    // ボーンの最大数
    private const int m_maxboneNum = 17;
    // 専用カメラ1個目
    public Camera m_Insp_ArousalAttackCamera1;
    // 専用カメラ2個目
    public Camera m_Insp_ArousalAttackCamera2;
    // 総発動時間(秒）
    private const float m_arousalAttackTotal = 10.0f;
    // 累積発動時間（秒）
    private float m_arousalAttackTime;
}