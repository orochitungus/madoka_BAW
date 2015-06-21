using UnityEngine;
using System.Collections;


// キャラクター「スコノシュート」を制御するためのスクリプト
public class Scono_Battle_Control : CharacterControl_Base
{
    // 覚醒用カメラ1個目
    public Camera m_Insp_ArousalAttackCamera1;
    // 覚醒用カメラ2個目
    public Camera m_Insp_ArousalAttackCamera2;

    // 通常射撃用の弾丸
    public GameObject m_Insp_NormalShot;
    // サブ射撃用のフック
    public GameObject m_SubShotRoot;
    // サブ射撃用の弾丸
    public GameObject m_Insp_SubShot;
    // 特殊射撃用のフック
    public GameObject m_ExShotRoot;
    // 特殊射撃用の弾丸
    public GameObject m_Insp_ExShot;
    
    // 覚醒技のフック
    public GameObject m_Insp_ArousalRoot;
    // 覚醒技の弾丸
    public GameObject m_Insp_ArousalCore;

    // メイン射撃撃ち終わり時間
    private float m_mainshotendtime;
    // サブ射撃撃ち終わり時間
    private float m_subshotendtime;
    // 特殊射撃撃ち終わり時間
    private float m_exshotendtime;

    // 攻撃技の種類
    public enum Skilltype_Scono
    {
        // 攻撃系
        // 射撃属性
        SHOT,                   // 通常射撃
        SUB_SHOT,               // サブ射撃
        EX_SHOT,                // 特殊射撃
        // 格闘属性
        WRESTLE_1,              // N格1段目
        WRESTLE_2,              // N格2段目
        WRESTLE_3,              // N格3段目
        FRONT_WRESTLE1,         // 前格闘1段目
        LEFT_WRESTLE1,          // 左横格闘1段目
        RIGHT_WRESTLE1,         // 右横格闘1段目
        BACK_WRESTLE,           // 後格闘（防御）
        AIRDASH_WRESTLE,        // 空中ダッシュ格闘
        EX_WRESTLE_1,           // 特殊格闘1段目
        EX_FRONT_WRESTLE_1,     // 前特殊格闘1段目
        BACK_EX_WRESTLE,        // 後特殊格闘
        // アビリティ系
        DISABLE_BLUNT_FOOT,     // 鈍足無効
        DISABLE_POISON,         // 毒無効
        DISABLE_MENTALS,        // 精神系状態異常無効
    }



	// Use this for initialization
	void Start () 
    {
	    this.m_AnimationNames = new string[]
        {
            "Scono_neutral_copy",           //Idle,                   // 通常
            "Scono_walk_copy",              //Walk,                   // 歩行
            "Scono_Walk_under_only_copy",   //Walk_underonly,         // 歩行（下半身のみ）       
            "Scono_jump_copy",              //Jump,                   // ジャンプ開始
            "Scono_jump_under_only_copy",   //Jump_underonly,         // ジャンプ開始（下半身のみ）
            "Scono_jumping_copy",           //Jumping,                // ジャンプ中（上昇状態）
            "Scono_jumping_under_only_copy",//Jumping_underonly,      // ジャンプ中（下半身のみ）
            "Scono_fall_copy",              //Fall,                   // 落下
            "Scono_landing_copy",           //Landing,                // 着地
            "Scono_run_copy",               //Run,                    // 走行
            "scono_run_under_only_copy",    //Run_underonly,          // 走行（下半身のみ）
            "Scono_AirDash_copy",           //AirDash,                // 空中ダッシュ
            "Scono_frontstep_copy",         //FrontStep,              // 前ステップ中        
            "Scono_leftstep_copy",          //LeftStep,               // 左（斜め）ステップ中       
            "Scono_rightstep_copy",         //RightStep,              // 右（斜め）ステップ中        
            "Scono_backstep_copy",          //BackStep,               // 後ステップ中
            "Scono_frontstep_back_copy",    //FrontStepBack,          // 前ステップ終了
            "Scono_leftstep_back_copy",     //LeftStepBack,           // 左（斜め）ステップ終了
            "Scono_rightstep_back_copy",    //RightStepBack,          // 右（斜め）ステップ終了
            "Scono_backstep_back_copy",     //BackStepBack,           // 後ステップ終了
            "Scono_fall_copy",              //FallStep,               // ステップで下降中
            "Scono_normalshot_copy",        //Shot,                   // 通常射撃（装填）
            "Scono_normalshot_top_only_copy",//Shot_toponly,          // 通常射撃（上半身のみ）
            "",                             //Shot_run
            "Scono_normalshot_copy",        //Shot_Air,               // 空中で通常射撃
            "",                             //Charge_Shot,            // 射撃チャージ
            "Scono_subshot_copy",           //Sub_Shot,               // サブ射撃
            "Scono_Exshot_copy",            //EX_Shot,                // 特殊射撃            
            "Scono_Wrestle1_copy",          //Wrestle_1,              // N格1段目
            "Scono_Wrestle2_copy",          //Wrestle_2,              // N格2段目
            "Scono_wrestle3_copy",          //Wrestle_3,              // N格3段目
            "",                             //Charge_Wrestle,         // 格闘チャージ
            "Scono_frontwrestle_copy",      //Front_Wrestle_1,        // 前格闘1段目
            "",                             //Front_Wrestle_2,        // 前格闘2段目
            "",                             //Front_Wrestle_3,        // 前格闘3段目
            "Scono_leftwrestle_copy",       //Left_Wrestle_1,         // 左横格闘1段目
            "",                             //Left_Wrestle_2,         // 左横格闘2段目
            "",                             //Left_Wrestle_3,         // 左横格闘3段目
            "Scono_rightwrestle_copy",      //Right_Wrestle_1,        // 右横格闘1段目
            "",                             //Right_Wrestle_2,        // 右横格闘2段目
            "",                             //Right_Wrestle_3,        // 右横格闘3段目
            "Scono_backwrestle_copy",       //Back_Wrestle,           // 後格闘（防御）
            "Scono_bdwrestle_copy",         //AirDash_Wrestle,        // 空中ダッシュ格闘
            "Scono_exwrestle_copy",         //Ex_Wrestle_1,           // 特殊格闘1段目
            "",                             //Ex_Wrestle_2,           // 特殊格闘2段目
            "",                             //Ex_Wrestle_3,           // 特殊格闘3段目
            "Scono_frontexwrestle_copy",    //EX_Front_Wrestle_1,     // 前特殊格闘1段目
            "",                             //EX_Front_Wrestle_2,     // 前特殊格闘2段目
            "",                             //EX_Front_Wrestle_3,     // 前特殊格闘3段目
            "",                             //EX_Left_Wrestle_1,      // 左横特殊格闘1段目
            "",                             //EX_Left_Wrestle_2,      // 左横特殊格闘2段目
            "",                             //EX_Left_Wrestle_3,      // 左横特殊格闘3段目
            "",                             //EX_Right_Wrestle_1,     // 右横特殊格闘1段目
            "",                             //EX_Right_Wrestle_2,     // 右横特殊格闘2段目
            "",                             //EX_Right_Wrestle_3,     // 右横特殊格闘3段目        
            "Scono_backexwrestle_copy",     //BACK_EX_Wrestle,        // 後特殊格闘
            "Scono_rebirsal_copy",          //Reversal,               // ダウン復帰
            "Scono_Arousal_copy",           //Arousal_Attack,         // 覚醒技
            "Scono_Damage_copy",            //Damage,                 // ダメージ
            "Scono_Down_copy",              //Down,                   // ダウン
            "Scono_neutral_copy",           //Nomove                  // 動きなし
            "",                             //Blow                    // 吹き飛び
            "",                             //FrontStepBack_Standby,  // 前ステップ終了(アニメはなし）
            "",                             //LeftStepBack_Standby,   // 左（斜め）ステップ終了(アニメはなし）
            "",                             //RightStepBack_Standby,  // 右（斜め）ステップ終了(アニメはなし）
            "",                             //BackStepBack_Standby,   // 後ステップ終了(アニメはなし）
            "",                             //DamageInit,             // ダメージ前処理(アニメはなし）
            "",                             //BlowInit,               // 吹き飛び前処理(アニメはなし）
            "Scono_SpinDown_copy",          //SpinDown                // 錐揉みダウン
            "",                                                       // 穴埋め用の空要素
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

        this.m_WalkSpeed = 1.0f;                                // 移動速度（歩行の場合）
        this.m_RunSpeed = 20.0f;                                // 移動速度（走行の場合）
        this.m_AirDashSpeed = 20.0f;                            // 移動速度（空中ダッシュの場合）
        this.m_AirMoveSpeed = 10.0f;                            // 移動速度（空中慣性移動の場合）
        this.m_RateofRise = 8.0f;                               // 上昇速度

        // ブースト消費量
        this.m_JumpUseBoost = 20;       // ジャンプ時
        this.m_DashCancelUseBoost = 20;   // ブーストダッシュ時
        this.m_StepUseBoost = 20;         // ステップ時
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
        this.m_Collider_Height = 1.6f;

        // ロックオン距離
        this.m_Rockon_Range = 100.0f;

        // ロックオン限界距離
        this.m_Rockon_RangeLimit = 200.0f;

        // ショットのステート
        shotmode = ShotMode.NORMAL;

        // 弾のステート
        this.m_BulletMoveDirection = Vector3.zero;
        this.m_BulletPos = Vector3.zero;

        m_hasfroutexwrestle = false;
                
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
            m_reload.AllTogether(ref m_BulletNum[1], Time.time, Character_Spec.cs[(int)m_character_name][1].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][1].m_OriginalBulletNum,
               Character_Spec.cs[(int)m_character_name][1].m_reloadtime, ref m_subshotendtime);
            // 特殊射撃
            m_reload.OneByOne(ref m_BulletNum[2], Time.time, Character_Spec.cs[(int)m_character_name][2].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][2].m_OriginalBulletNum,
               Character_Spec.cs[(int)m_character_name][2].m_reloadtime, ref m_exshotendtime);
        }
        // 最初のステージで負けたらプロローグ３へ移行
        if (m_NowHitpoint < 1 && savingparameter.story == 1)
        {
            Application.LoadLevel("Prologue3");
        }
	}

    // 前特殊格闘を入れたかというフラグ
    private bool m_hasfroutexwrestle;

    // 攻撃行動全般(RunとAirDashは特殊なので使用しない）
    // run[in]      :走行中入力
    // AirDash[in]  :空中ダッシュ中入力
    private void AttackDone(bool run = false, bool AirDash = false)
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
            // ここに特殊射撃処理
            ExShotDone();
        }
        // 特殊格闘で特殊格闘へ移行
        else if (m_hasExWrestleInput)
        {
            // 前特殊格闘
            if (m_hasFrontInput)
            {
                WrestleDone_UpperEx((int)Skilltype_Scono.EX_FRONT_WRESTLE_1);
                // スコノの前特格と空中ダッシュ特格はループアニメなので、WrestleStartをanimファイルに貼るという方法が使えない（無限にくっつく）
                // そこでここで判定を生成し、fallに入ったら判定を消す
                // 判定を作る

                // 判定の場所
                Vector3 pos = m_WrestleRoot[18].transform.position;
                // 判定の角度（0固定）
                Quaternion rot = m_WrestleRoot[18].transform.rotation;
                // 判定を生成する
                var obj = (GameObject)Instantiate(m_WrestleObject[18], pos, rot);
                // 判定を子オブジェクトにする
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_WrestleRoot[18].transform;
                    // 親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
                // 判定に値をセットする
                // インデックス
                int characterName = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;

                obj.gameObject.GetComponent<Wrestle_Core>().SetStatus(
                    Character_Spec.cs[characterName][12].m_OriginalStr + Character_Spec.cs[characterName][12].m_GrowthCoefficientStr * (m_StrLevel - 1),    // offensive    [in]:攻撃力
                    Character_Spec.cs[characterName][12].m_DownPoint,                                                                                       // downR        [in]:ダウン値
                    Character_Spec.cs[characterName][12].m_arousal,                                                                                         // arousal      [in]:覚醒ゲージ増加量
                    Character_Spec.cs[characterName][12].m_Hittype                                                                                          // hittype      [in]:ヒットタイプ 
                    );
                m_hasfroutexwrestle = true;
            }
            // 空中で後特殊格闘
            else if (m_hasBackInput && !m_isGrounded)
            {
                WrestleDone_DownEx((int)Skilltype_Scono.BACK_EX_WRESTLE);
            }
            // それ以外
            else if (!m_hasfroutexwrestle)
            {
                WrestleDone(AnimationState.Ex_Wrestle_1, (int)Skilltype_Scono.EX_WRESTLE_1);
            }
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
                    // 角度60度以内なら上体回しで撃つ（歩き撃ち限定で上記の弾の方向ベクトルを加算する）
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
                WrestleDone(AnimationState.AirDash_Wrestle, (int)Skilltype_Scono.AIRDASH_WRESTLE);
                // こちらもループアニメなので、animに関数を貼る手段は使えないため判定をここで作る
                // 判定の場所
                Vector3 pos = m_WrestleRoot[14].transform.position;
                // 判定の角度（0固定）
                Quaternion rot = m_WrestleRoot[14].transform.rotation;
                // 判定を生成する
                var obj = (GameObject)Instantiate(m_WrestleObject[14], pos, rot);
                // 判定を子オブジェクトにする
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_WrestleRoot[14].transform;
                    // 親子関係を付けておく
                    obj.transform.rigidbody.isKinematic = true;
                }
                // 判定に値をセットする
                // インデックス
                int characterName = (int)Character_Spec.CHARACTER_NAME.MEMBER_SCHONO;
                obj.gameObject.GetComponent<Wrestle_Core>().SetStatus(
                    Character_Spec.cs[characterName][10].m_OriginalStr + Character_Spec.cs[characterName][12].m_GrowthCoefficientStr * (m_StrLevel - 1),    // offensive    [in]:攻撃力
                    Character_Spec.cs[characterName][10].m_DownPoint,                                                                                       // downR        [in]:ダウン値
                    Character_Spec.cs[characterName][10].m_arousal,                                                                                         // arousal      [in]:覚醒ゲージ増加量
                    Character_Spec.cs[characterName][10].m_Hittype                                                                                          // hittype      [in]:ヒットタイプ 
                    );
            }
            else
            {
                // 前格闘と横格闘への分岐はここで（2段目・3段目の分岐はやっているときに）

                // 前格闘で前格闘へ移行
                if (m_hasFrontInput)
                {
                    WrestleDone(AnimationState.Front_Wrestle_1, (int)Skilltype_Scono.FRONT_WRESTLE1);
                }
                // 左格闘で左格闘へ移行
                else if (m_hasLeftInput)
                {
                    WrestleDone_GoAround_Left(AnimationState.Left_Wrestle_1, (int)Skilltype_Scono.LEFT_WRESTLE1);
                }
                // 右格闘で右格闘へ移行
                else if (m_hasRightInput)
                {
                    WrestleDone_GoAround_Right(AnimationState.Right_Wrestle_1, (int)Skilltype_Scono.RIGHT_WRESTLE1);
                }
                // 後格闘で後格闘へ移行
                else if (m_hasBackInput)
                {
                    GuardDone((int)Skilltype_Scono.BACK_WRESTLE);
                }
                else
                {
                    // N格闘1段目  
                    WrestleDone(AnimationState.Wrestle_1, (int)Skilltype_Scono.WRESTLE_1);
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
        m_hasfroutexwrestle = false;
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
        // 本体にくっついていた弾丸をカット（あるなら）
        DestroyArrow();
        // 本体にくっついていた特射の弾丸をカット（あるなら）
        DestroyObject(m_ExShotRoot);
        // 同じく格闘判定をカット
        DestroyWrestle();
        m_hasfroutexwrestle = false;
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
    // タイミングによっては弾が残る
    protected override void CancelDashDone()
    {
        // メイン射撃の弾があるなら消す(m_ArrowRootの下に何かあるなら全部消す）
        DestroyArrow();
        // サブ射撃の弾があるなら消す
        DestroyObject(m_SubShotRoot);
        // 特殊射撃の弾があるなら消す
        DestroyObject(m_ExShotRoot);
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
    protected bool ShotEndCheck(string AnimationName, ShotType type)
    {
        while (this.animation.IsPlaying(AnimationName))
        {
            return false;
        }

        // 弾があるとき限定
        int BulletType = 0;
        // スコノには射撃CSがないので、typeの値をそのまま使うとずれる
        switch (type)
        {
            case ShotType.NORMAL_SHOT:
                BulletType = 0;
                break;
            case ShotType.SUB_SHOT:
                BulletType = 1;
                break;
            case ShotType.EX_SHOT:
                BulletType = 2;
                break;
            default:
                break;

        }

        // サブ射撃・特殊射撃の場合(強制的に停止か落下になる）
        if (type == ShotType.SUB_SHOT || type == ShotType.EX_SHOT)
        {
            if (Time.time > this.m_AttackTime + this.m_BulletWaitTime[BulletType])
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
            if (Time.time > this.m_AttackTime + this.m_BulletWaitTime[BulletType])
            {
                // 合成状態を解除
                ReturnMotion();
                // 地上にいて静止中
                if (m_isGrounded && !this.m_hasVHInput)//(this.m_charactercontroller.isGrounded && !this.m_hasVHInput) 
                {
                    // アイドルモードのアニメを起動する
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Idle]);
                    this.animation[m_AnimationNames[(int)AnimationState.Idle]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Idle;
                }
                // 地上にいて歩行中
                else if (m_isGrounded)//(this.m_charactercontroller.isGrounded)
                {
                    // 走行モードのアニメを起動する
                    this.animation.CrossFade(m_AnimationNames[(int)AnimationState.Run]);
                    this.animation[m_AnimationNames[(int)AnimationState.Run]].blendMode = AnimationBlendMode.Blend; // 合成モードを戻しておく
                    this.m_AnimState[0] = AnimationState.Run;
                }
                // 空中にいてダッシュ入力中でありかつブーストゲージがある
                else if (m_isGrounded && m_hasVHInput && m_hasJumpInput && this.m_Boost > 0)//(this.m_charactercontroller.isGrounded && m_hasVHInput && m_hasJumpInput && this.m_Boost > 0)
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
        int BulletType = 0;
        // スコノには射撃CSがないので、typeの値をそのまま使うとずれる
        switch (type)
        {
            case ShotType.NORMAL_SHOT:
                BulletType = 0;
                break;
            case ShotType.SUB_SHOT:
                BulletType = 1;
                break;
            case ShotType.EX_SHOT:
                BulletType = 2;
                break;
            default:
                break;

        }
        if (this.m_BulletNum[BulletType] > 0)
        {
            // ロックオン時本体の方向を相手に向ける       
            if (this.m_IsRockon)
            {
                RotateToTarget();
            }
            // 弾を消費する（サブ射撃なら1、特殊射撃なら2）
            // チャージ射撃除く            
            this.m_BulletNum[BulletType]--;
            // 撃ち終わった時間を設定する                
            // メイン（弾数がMax-1のとき）
            if (type == ShotType.NORMAL_SHOT && m_BulletNum[BulletType] == Character_Spec.cs[(int)m_character_name][BulletType].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][BulletType].m_OriginalBulletNum - 1)
            {
                m_mainshotendtime = Time.time;
            }
            // サブ（弾数が0のとき）
            else if (type == ShotType.SUB_SHOT && m_BulletNum[BulletType] == 0)
            {
                m_subshotendtime = Time.time;
            }
            // 特殊（弾数がMax-1のとき）
            else if (type == ShotType.EX_SHOT && m_BulletNum[BulletType] == Character_Spec.cs[(int)m_character_name][BulletType].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][BulletType].m_OriginalBulletNum - 1)
            {
                m_exshotendtime = Time.time;
            }
            

            // 弾丸を出現させる
            // サブ射撃
            if (type == ShotType.SUB_SHOT)
            {
                // 弾丸の出現ポジションをフックと一致させる
                Vector3 pos = m_SubShotRoot.transform.position;
                Quaternion rot = Quaternion.Euler(m_SubShotRoot.transform.rotation.eulerAngles.x, m_SubShotRoot.transform.rotation.eulerAngles.y, m_SubShotRoot.transform.rotation.eulerAngles.z);
                var obj = (GameObject)Instantiate(m_Insp_SubShot, pos, rot);
                // 親子関係を再設定する(=弾をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_SubShotRoot.transform;
                    obj.transform.rigidbody.isKinematic = true;
                }
            }
            // 特殊射撃（これはBulletではなくLaserなので通常射撃・サブ射撃とは処理が異なり、この時点で発射される）
            else if (type == ShotType.EX_SHOT)
            {
                // 弾丸の出現ポジションをフックと一致させる
                Vector3 pos = m_ExShotRoot.transform.position;
                Quaternion rot = Quaternion.Euler(m_ExShotRoot.transform.rotation.eulerAngles.x, m_ExShotRoot.transform.rotation.eulerAngles.y, m_ExShotRoot.transform.rotation.eulerAngles.z);
                var obj = (GameObject)Instantiate(m_Insp_ExShot, pos, rot);
                // 親子関係を再設定する(=弾をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ExShotRoot.transform;
                    obj.transform.rigidbody.isKinematic = true;
                }
                // 射出音を再生する
                AudioSource.PlayClipAtPoint(m_Insp_ExShootSE, transform.position);
            }
            // 通常射撃(varは暗黙的な初期化ができない)
            else if (type == ShotType.NORMAL_SHOT)
            {
                // 弾丸の出現ポジションをフックと一致させる
                Vector3 pos = m_ArrowRoot.transform.position;
                Quaternion rot = Quaternion.Euler(m_ArrowRoot.transform.rotation.eulerAngles.x, m_ArrowRoot.transform.rotation.eulerAngles.y, m_ArrowRoot.transform.rotation.eulerAngles.z);
                var obj = (GameObject)Instantiate(m_Insp_NormalShot, pos, rot);
                // 親子関係を再設定する(=弾をフックの子にする）
                if (obj.transform.parent == null)
                {
                    obj.transform.parent = m_ArrowRoot.transform;
                    // 弾の親子関係を付けておく
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
    // 射出音（特殊射撃）
    public AudioClip m_Insp_ExShootSE;

    // 射撃（射出）弾丸消費はここでやらない。アニメ中イベントはステートと同期するとは限らないので、弾丸消費はアニメ中イベントではやるべきではない
    protected void Shoot2(ShotType type)
    {
        // 通常射撃。この時点では親子なので、Homura_NormalShotAllowsを持ったスクリプトを拾う(デフォルトで持っていないので、スクリプトで検索するしかない）        
        var arrow = GetComponentInChildren<Scono_NormalShot>();
        // サブ射撃
        var subshot = GetComponentInChildren<Scono_SubShot>();
        // 特殊射撃
        var exshot = GetComponentInChildren<Scono_Exshot>();

        if (arrow != null || subshot != null)
        {
            // 弾のスクリプトの速度を設定する
            if (type == ShotType.NORMAL_SHOT)
            {
                arrow.m_Speed = Character_Spec.cs[(int)m_character_name][0].m_Movespeed;
            }
            else if (type == ShotType.SUB_SHOT)
            {
                subshot.m_Speed = Character_Spec.cs[(int)m_character_name][1].m_Movespeed;
            }
            
            // 弾の方向を決定する(本体と同じ方向に向けて打ち出す。ただしノーロックで本体の向きが0のときはベクトルが0になるので、このときだけはカメラの方向に飛ばす）

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
                Vector3 addrot = mainrot.eulerAngles + normalizeRot_OR - new Vector3(0, 0.0f, 0);
                Quaternion qua = Quaternion.Euler(addrot);
                // forwardをかけて本体と胸部の和を進行方向ベクトルへ変換                
                Vector3 normalizeRot = (qua) * Vector3.forward;
                // 移動ベクトルを確定する
                // 通常射撃
                if (arrow != null)
                {
                    arrow.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
                // サブ射撃
                if (subshot != null)
                {
                    subshot.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
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
                
                // 通常射撃
                if (arrow != null)
                {
                    arrow.m_MoveDirection = Vector3.Normalize(normalizeRot);
                }
                // サブ射撃
                if (subshot != null)
                {
                    subshot.m_MoveDirection = Vector3.Normalize(normalizeRot);
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
                    // 通常射撃
                    if (arrow != null)
                    {
                        arrow.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                    }
                    // サブ射撃
                    if (subshot != null)
                    {
                        subshot.m_MoveDirection = Vector3.Normalize(rotateOR * Vector3.forward);
                    }
                }
                // それ以外は本体の角度を射出角にする
                else
                {                   
                    if (arrow != null)
                    {
                        arrow.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                    }
                    if (subshot != null)
                    {
                        subshot.m_MoveDirection = Vector3.Normalize(this.transform.rotation * Vector3.forward);
                    }
                }
            }

            // 矢のフックの位置に弾の位置を代入する
            if (arrow != null)
            {
                this.m_BulletPos = this.m_ArrowRoot.transform.position;
                // 同じく回転角を代入する
                this.m_BulletMoveDirection = arrow.m_MoveDirection;
            }
            if (subshot != null)
            {
                this.m_BulletPos = this.m_SubShotRoot.transform.position;
                this.m_BulletMoveDirection = subshot.m_MoveDirection;
            }
            if (type == ShotType.NORMAL_SHOT)
            {
                setOffensivePower(Skilltype_Scono.SHOT);
            }
            else if (type == ShotType.SUB_SHOT)
            {
                setOffensivePower(Skilltype_Scono.SUB_SHOT);
            }
            
            shotmode = ShotMode.SHOT;

            // 固定状態を解除
            this.transform.rigidbody.constraints = RigidbodyConstraints.None;
            // ずれた本体角度を戻す(Yはそのまま）
            this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
            this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // 硬直時間を設定
            this.m_AttackTime = Time.time;
            // 射出音を再生する
            AudioSource.PlayClipAtPoint(m_Insp_ShootSE, transform.position);
        }
        // 特殊射撃時はビームを消去する
        else if (exshot != null)
        {
            DestroyObject(m_ExShotRoot);
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
    private void setOffensivePower(Skilltype_Scono kind)
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.WRESTLE_1].m_animationTime;
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.WRESTLE_2].m_animationTime;
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.WRESTLE_3].m_animationTime;
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.FRONT_WRESTLE1].m_animationTime;
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.LEFT_WRESTLE1].m_animationTime;
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
        float wrestletimeBias = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.RIGHT_WRESTLE1].m_animationTime;
        if (m_wrestletime > wrestletimeBias)
        {
            WrestleFinish(AnimationState.Idle);
        }
    }

    // ステップキャンセル時の処理
    protected override void StepCancel()
    {
        base.StepCancel();
        // 特射のビームを消す
        DestroyObject(m_ExShotRoot);
    }

    void LateUpdate()
    {
        // ロックオン時首を相手の方向へ向ける(本体角度との合成になっているので、本体角度分減算してやらないと正常に向かない）
        if (this.m_IsRockon && this.m_AnimState[0] != AnimationState.Shot && this.m_AnimState[0] != AnimationState.EX_Shot
           && this.m_AnimState[0] != AnimationState.Sub_Shot && this.m_AnimState[0] != AnimationState.Front_Wrestle_2
           && this.m_AnimState[0] != AnimationState.Shot_run && this.m_AnimState[0] != AnimationState.Charge_Shot && !m_RunShotDone)
        {
            SetNeckRotate(this.m_Head, 0.0f);
        }
        // 歩き射撃時,上体をロックオン対象へ向ける
        if (this.m_IsRockon && m_RunShotDone)
        {
            SetNeckRotate(this.m_Brest, 0.0f);
        }
        // ロックオンしていないときはそのまま本体角度分回す
        else if (m_RunShotDone)
        {
            // 本体角度
            Vector3 rotate = this.transform.rotation.eulerAngles;
            m_Brest.transform.rotation = Quaternion.Euler(0, rotate.y, 0);
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
        m_BulletNum[(int)Skilltype_Scono.SHOT] = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.SHOT].m_OriginalBulletNum;
        // サブ
        m_BulletNum[(int)Skilltype_Scono.SUB_SHOT] = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.SUB_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.SUB_SHOT].m_OriginalBulletNum;
        // 特殊射撃
        m_BulletNum[(int)Skilltype_Scono.EX_SHOT] = Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.EX_SHOT].m_GrowthCoefficientBul * (this.m_BulLevel - 1) + Character_Spec.cs[(int)m_character_name][(int)Skilltype_Scono.EX_SHOT].m_OriginalBulletNum;
        base.ArousalInitialize();
        m_nowState = ArousalState.INITIALIZE;
    }

    // 覚醒技のコアを作る（覚醒技のAnimファイルで呼び出す）
    public void CreateArousalCore()
    {
        // コアを作る
        // コア取り付け位置
        Vector3 pos = m_Insp_ArousalRoot.transform.position;
        // コア角度
        Quaternion rot = m_Insp_ArousalRoot.transform.rotation;
        // コア生成
        var core = (GameObject)Instantiate(m_Insp_ArousalCore, pos, rot);
        // コアをフックの子にする
        core.transform.parent = m_Insp_ArousalRoot.transform;
        m_nowState = ArousalState.FIRE;
    }

    // 覚醒技のコアを消す(animから呼ぶ）
    public void EraseArousalCore()
    {       
        if(m_Insp_ArousalRoot != null)
        {
            var core = m_Insp_ArousalRoot.GetComponentInChildren<Scono_ArousalCore>();
            if(core != null)
            {
                Destroy(core.gameObject);
            }
        }
        m_nowState = ArousalState.FINALIZE;
    }

    // 覚醒技を終了してIdleに戻す（animから呼ぶ）
    public void ReturnIdle()
    {
        this.m_Arousal = 0;
        if(m_isPlayer != CHARACTERCODE.ENEMY)
        {
            int characterindex = (int)m_character_name;
            savingparameter.SetNowArousal(characterindex, 0.0f);
        }
        // ずれた本体角度を戻す(Yはそのまま）
        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0));
        this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        this.animation.Play(m_AnimationNames[(int)AnimationState.Idle]);
        m_IsArmor = false;
        // 覚醒状態も解除する
        // 覚醒エフェクトを消す
        Destroy(m_arousalEffect);
        // ゲージを0にする
        m_Arousal = 0;
        // 覚醒フラグを折る
        m_isArousal = false;
        m_AnimState[0] = AnimationState.Idle;  
    }

    private enum ArousalState
    {
        INITIALIZE,     // アニメを再生
        FIRE_STANDBY,   // 手を上げる前の状態
        FIRE,           // ビームを放射
        FINALIZE,       // ビーム放射終了
        RETURN          // 覚醒技終了
    };

    ArousalState m_nowState;
    // ロックオン対象の座標
    private Vector3 m_targetPos;
    // FIREに入る前の角度
    private Quaternion m_originPos;
    // 現在の補正角度
    private float m_nowArousalRot;

    // 覚醒技発動処理
    protected override void ArousalAttack()
    {
        base.ArousalAttack();
        switch (m_nowState)
        {
            // アニメーションを再生開始
            case ArousalState.INITIALIZE:
                this.animation.Play(m_AnimationNames[(int)AnimationState.Arousal_Attack]);
                m_nowState = ArousalState.FIRE_STANDBY;
                break;
            // 手を上げる前の状態
            case ArousalState.FIRE_STANDBY:
                // ロックオン時
                if (m_IsRockon)
                {
                    // ロックオン対象の座標を取得
                    var target = GetComponentInChildren<Player_Camera_Controller>();
                    m_targetPos = target.Enemy.transform.position;
                    // 本体を相手の方向へ向ける
                    transform.rotation = Quaternion.LookRotation(m_targetPos - transform.position);                                        
                }
                m_nowArousalRot = 0;
                m_originPos = transform.rotation;
                break;
            // ビーム照射中は本体を相手の足元に向けつつ薙ぎ払う（非ロックオン時はその場で5度くらい下に向ける）
            case ArousalState.FIRE:
                // 本体角度を-55度左に向け、毎フレームごとに1度ずつオフセットしていく
                float yrot = m_originPos.eulerAngles.y - 55 + m_nowArousalRot;
                float xrot = m_originPos.eulerAngles.x + 5;
                float zrot = m_originPos.eulerAngles.z;
                Vector3 rot = new Vector3(xrot, yrot, zrot);
                transform.rotation = Quaternion.Euler(rot);
                m_nowArousalRot += 1.6f;
                break;
            // ビーム照射を終了して、手を下す
            case ArousalState.FINALIZE:                
                break;
            // アニメが終了したら覚醒ゲージを強制的に空にしてIdleに戻す
            case ArousalState.RETURN:                
                break;
            default:
                break;
        }
    }
}
