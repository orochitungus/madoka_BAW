﻿//using UnityEngine;
//using System;
//using System.Collections;


//// CharacterControl_BaseのうちAnimationに関する関数は全部こっちにおいておく
//// 個別アクションはこれをオーバーライドする
//public partial class CharacterControl_Base
//{
//    // 1F前のMoveDirection
//    public Vector3 m_PreMoveDirection = Vector3.zero;
//    // Fallに移行した時間
//    protected float FallStartTime = 0.0f;

//    // Animation共通操作
//    protected void Update_Animation()
//    {       
//        switch (m_AnimState[0])
//        {
//            case AnimationState.Idle:                   // 通常
//                ContlroleAnimationSpeed(AnimationState.Idle, 1.0f, Timestopmode);
//                Animation_Idle();
//                break;
//            case AnimationState.Walk:                   // 歩行
//                Animation_Walk();
//                break;
//            case AnimationState.Walk_underonly:         // 歩行（下半身のみ）
//                Animation_Walk_underonly();
//                break;
//            case AnimationState.Jump:                   // ジャンプ開始
//                Animation_Jump();
//                break;
//            case AnimationState.Jump_underonly:         // ジャンプ開始（下半身のみ）
//                Animation_Jump_underonly();
//                break;
//            case AnimationState.Jumping:                // ジャンプ中（上昇状態）
//                Animation_Jumping();
//                break;
//            case AnimationState.Jumping_underonly:      // ジャンプ中（下半身のみ）
//                Animation_Jumping_underonly();
//                break;
//            case AnimationState.Fall:                   // 落下
//                ContlroleAnimationSpeed(AnimationState.Fall, 1.0f, Timestopmode);
//                this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Fall]].speed = 1.0f;
//                Animation_Fall();
//                break;
//            case AnimationState.Landing:                // 着地
//                Animation_Landing();
//                break;
//            case AnimationState.Run:                    // 走行
//                ContlroleAnimationSpeed(AnimationState.Run, 1.0f, Timestopmode);
//                this.GetComponent<Animation>()[m_AnimationNames[(int)AnimationState.Run]].speed = 1.0f;
//                Animation_Run();
//                break;
//            case AnimationState.Run_underonly:          // 走行（下半身のみ）
//                Animation_Run_underonly();
//                break;
//            case AnimationState.AirDash:                // 空中ダッシュ
//                ContlroleAnimationSpeed(AnimationState.AirDash, 1.0f, Timestopmode);
//                Animation_AirDash();
//                break;
//            case AnimationState.FrontStep:              // 前ステップ中
//                Animation_StepDone(AnimationState.FrontStep);
//                break;
//            case AnimationState.LeftStep:               // 左（斜め）ステップ中
//                Animation_StepDone(AnimationState.LeftStep);
//                break;
//            case AnimationState.RightStep:              // 右（斜め）ステップ中
//                Animation_StepDone(AnimationState.RightStep);
//                break;
//            case AnimationState.BackStep:               // 後ステップ中
//                Animation_StepDone(AnimationState.BackStep);
//                break;
//            case AnimationState.FrontStepBack_Standby:          // 前ステップ終了
//                Animation_StepBackStandby(AnimationState.FrontStepBack_Standby);
//                break;
//            case AnimationState.FrontStepBack:
//                EndStep();
//                break;
//            case AnimationState.LeftStepBack_Standby:           // 左（斜め）ステップ終了
//                Animation_StepBackStandby(AnimationState.LeftStepBack_Standby);
//                break;
//            case AnimationState.LeftStepBack:
//                EndStep();
//                break;
//            case AnimationState.RightStepBack_Standby:          // 右（斜め）ステップ終了
//                Animation_StepBackStandby(AnimationState.RightStepBack_Standby);
//                break;
//            case AnimationState.RightStepBack:
//                EndStep();
//                break;
//            case AnimationState.BackStepBack_Standby:           // 後ステップ終了                
//                Animation_StepBackStandby(AnimationState.BackStepBack_Standby);
//                break;
//            case AnimationState.BackStepBack:
//                EndStep();
//                break;
//            case AnimationState.FallStep:               // ステップで下降中（方向分岐があるので使わん)               
//                break;
//            case AnimationState.Shot:                   // 通常射撃
//                Shot();
//                break;
//            case AnimationState.Shot_toponly:           // 通常射撃（上半身のみ）/未使用
//                break;
//            case AnimationState.Shot_run:               // 歩き撃ち
//                ShotRun();
//                break;
//            case AnimationState.Shot_AirDash:           // 空中で通常射撃/未使用（とりあえず通常射撃はshotで統一）
//                ShotAirDash();
//                break;
//            case AnimationState.Charge_Shot:            // 射撃チャージ
//                ChargeShot();
//                break;
//            case AnimationState.Sub_Shot:               // サブ射撃
//                SubShot();
//                break;
//            case AnimationState.EX_Shot:                // 特殊射撃
//                ExShot();
//                break;
//            // 格闘属性は一応全部3段目まで作っておく(派生技に割り振るとかでもあり)
//            case AnimationState.Wrestle_1:              // N格1段目
//                Wrestle1();
//                break;
//            case AnimationState.Wrestle_2:              // N格2段目
//                Wrestle2();
//                break;
//            case AnimationState.Wrestle_3:              // N格3段目
//                Wrestle3();
//                break;
//            case AnimationState.Charge_Wrestle:         // 格闘チャージ
//                ChargeWrestle();
//                break;
//            case AnimationState.Front_Wrestle_1:        // 前格闘1段目
//                FrontWrestle1();
//                break;
//            case AnimationState.Front_Wrestle_2:        // 前格闘2段目
//                FrontWrestle2();
//                break;
//            case AnimationState.Front_Wrestle_3:        // 前格闘3段目
//                FrontWrestle3();
//                break;
//            case AnimationState.Left_Wrestle_1:         // 左横格闘1段目
//                LeftWrestle1();
//                break;
//            case AnimationState.Left_Wrestle_2:         // 左横格闘2段目
//                LeftWrestle2();
//                break;
//            case AnimationState.Left_Wrestle_3:         // 左横格闘3段目
//                LeftWrestle3();
//                break;
//            case AnimationState.Right_Wrestle_1:        // 右横格闘1段目
//                RightWrestle1();
//                break;
//            case AnimationState.Right_Wrestle_2:        // 右横格闘2段目
//                RightWrestle2();
//                break;
//            case AnimationState.Right_Wrestle_3:        // 右横格闘3段目
//                RightWrestle3();
//                break;
//            case AnimationState.Back_Wrestle:           // 後格闘（防御）
//                BackWrestle();
//                break;
//            case AnimationState.AirDash_Wrestle:        // 空中ダッシュ格闘
//                AirDashWrestle();
//                break;
//            case AnimationState.Ex_Wrestle_1:           // 特殊格闘1段目
//                ExWrestle1();
//                break;
//            case AnimationState.Ex_Wrestle_2:           // 特殊格闘2段目
//                ExWrestle2();
//                break;
//            case AnimationState.Ex_Wrestle_3:           // 特殊格闘3段目
//                ExWrestle3();
//                break;
//            case AnimationState.EX_Front_Wrestle_1:     // 前特殊格闘1段目
//                FrontExWrestle1();
//                break;
//            case AnimationState.EX_Front_Wrestle_2:     // 前特殊格闘2段目
//                FrontExWrestle2();
//                break;
//            case AnimationState.EX_Front_Wrestle_3:     // 前特殊格闘3段目
//                FrontExWrestle3();
//                break;
//            case AnimationState.EX_Left_Wrestle_1:      // 左横特殊格闘1段目
//                LeftExWrestle1();
//                break;
//            case AnimationState.EX_Left_Wrestle_2:      // 左横特殊格闘2段目
//                LeftExWrestle2();
//                break;
//            case AnimationState.EX_Left_Wrestle_3:      // 左横特殊格闘3段目
//                LeftExWrestle3();
//                break;
//            case AnimationState.EX_Right_Wrestle_1:     // 右横特殊格闘1段目
//                RightExWrestle1();
//                break;
//            case AnimationState.EX_Right_Wrestle_2:     // 右横特殊格闘2段目
//                RightExWrestle2();
//                break;
//            case AnimationState.EX_Right_Wrestle_3:     // 右横特殊格闘3段目
//                RightExWrestle3();
//                break;
//            case AnimationState.BACK_EX_Wrestle:        // 後特殊格闘
//                BackExWrestle();
//                break;
//            case AnimationState.Reversal:               // ダウン復帰
//                ContlroleAnimationSpeed(AnimationState.Reversal, 1.0f, Timestopmode);
//                Reversal();
//                break;
//            case AnimationState.Arousal_Attack:         // 覚醒技
//                ArousalAttack();
//                break;
//            case AnimationState.DamageInit:             // ダメージ(のけぞり前処理)
//                DamageInit(AnimationState.DamageInit);
//                break;
//            case AnimationState.Damage:                 // ダメージ(のけぞり）
//                Damage();
//                break;
//            case AnimationState.Down:                   // ダウン
//                ContlroleAnimationSpeed(AnimationState.Down, 1.0f, Timestopmode);
//                Down();
//                break;
//            case AnimationState.BlowInit:               // 吹き飛び前処理
//                DamageInit(AnimationState.BlowInit);
//                break;
//            case AnimationState.Blow:                   // 吹き飛び（吹き飛び属性の攻撃を食らったorダウン値が規定値を超えて強制ダウン）
//                Blow();
//                break;
//            case AnimationState.SpinDown:               // 錐揉みダウン
//                SpinDown();
//                break;
//            default:
//                break;
//        }
//        // 1F前のMoveDirectionを保持
//        this.m_PreMoveDirection = this.MoveDirection;
//    }
//    // 各アニメーション動作。攻撃モーションなどはキャラクターごとにこれらの関数をオーバーライドすること

//    // エフェクト破壊関数
//    protected void BrokenEffect()
//    {
//        // ステップ
//        if (gameObject.transform.FindChild("StepEffect(Clone)") != null)
//        {
//            Destroy(gameObject.transform.FindChild("StepEffect(Clone)").gameObject);
//        }
//        // 格闘ステップキャンセル
//        if(gameObject.transform.FindChild("StepEffectCancel(Clone)") != null)
//        {
//            Destroy(gameObject.transform.FindChild("StepEffectCancel(Clone)").gameObject);
//        }
//    }

//    // Idle時共通操作
//    protected virtual void Animation_Idle()
//    {
//        // 移行後復活
//        //IsStep = false;
//        //IsWrestle = false;
 
//        // 死んでいたらダウン
//        if (NowHitpoint <= 1)
//        {
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Down]);
//            m_AnimState[0] = AnimationState.Down;
//        }
//        // くっついているエフェクトの親元を消す
//        BrokenEffect();        
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0,this.transform.rotation.eulerAngles.y,0)); 
//        m_AnimState[1] = AnimationState.Idle;
//        this.MoveDirection = Vector3.zero;      // 速度を0に
//        this.BlowDirection = Vector3.zero;       
//        this.Rotatehold = false;                // 固定フラグは折る
//        // 慣性を殺す
//        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
//        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//        this.AddInput = false;
//        GetComponent<Rigidbody>().useGravity = true;
//        // ブーストを回復させる
//        this.Boost = GetMaxBoost(this.BoostLevel);
//        // 地上にいるか？
//        if (IsGrounded)
//        {
//            // 方向キーで走行
//            if (HasVHInput)
//            {
//                m_AnimState[0] = AnimationState.Run;
//                this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Run]);
//            }
//            // ジャンプでジャンプへ移行(GetButtonDownで押しっぱなしにはならない。GetButtonで押しっぱなしに対応）
//            if (this.HasJumpInput && Boost > 0)
//            {
//                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
//                JumpDone();
//            }          
//            // ステップの場合ステップ(非CPU時)
//            if (StepCheck().x != 0 || StepCheck().y != 0) 
//            {
//               StepDone(1, StepCheck());               
//            }
//            // CPU時左ステップ
//            else if (HasLeftStepInput)
//            {
//                StepDone(1, new Vector2(-1, 0));
//                HasLeftStepInput = false;
//            }
//            // CPU時右ステップ
//            else if (HasRightStepInput)
//            {
//                StepDone(1, new Vector2(1, 0));
//                HasRightStepInput = false;
//            }
            
//        }
//        // いなければ落下
//        else
//        {
//            m_AnimState[0] = AnimationState.Fall;
//            FallStartTime = Time.time;
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
//        }
        
//    }
//    // Walk時共通動作
//    protected virtual void Animation_Walk()
//    {
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        m_AnimState[1] = AnimationState.Walk;
//    }

//    // Walk_underonly時共通動作
//    protected virtual void Animation_Walk_underonly()
//    {
//        m_AnimState[1] = AnimationState.Walk_underonly;
//    }
//    // Jump時共通動作
//    protected virtual void Animation_Jump()
//    {
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        m_AnimState[1] = AnimationState.Jump;
//        // ジャンプしたので硬直を設定する
//        this.JumpTime = Time.time;
//        m_AnimState[0] = AnimationState.Jumping;        
//    }
//    // Jump_underonly時共通動作
//    protected virtual void Animation_Jump_underonly()
//    {
//        m_AnimState[1] = AnimationState.Jump_underonly;
//    }
//    // Jumping時共通動作
//    protected virtual void Animation_Jumping()
//    {
//        m_AnimState[1] = AnimationState.Jumping;
//        Vector3 RiseSpeed = new Vector3(MoveDirection.x, this.RiseSpeed, MoveDirection.z);// = new Vector3(0, 0, 0);
//        if (Time.time > this.JumpTime + this.JumpWaitTime)
//        {
//            // ジャンプ後の硬直終了時の処理はここに入れる
//            // ジャンプ中にブーストがある限り上昇
//            if (this.Boost > 0)
//            {               
//                // ボタンを押し続ける限り上昇
//                if (this.HasJumpInput)
//                {                   
//                    this.Boost = this.Boost - BoostLess;
//                }
//                // 離したら落下
//                else if (!this.HasJumpInput)
//                {
//                    FallDone(RiseSpeed);
//                }
//                // ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)
//                else if (this.HasDashCancelInput)
//                {
//                    CancelDashDone();
//                }

//                // ステップの場合ステップ
//                if (StepCheck().x != 0 || StepCheck().y != 0)
//                {
//                    StepDone(1, StepCheck());                    
//                }
//                // CPU時左ステップ
//                else if (HasLeftStepInput)
//                {
//                    StepDone(1, new Vector2(-1, 0));
//                    HasLeftStepInput = false;
//                }
//                // CPU時右ステップ
//                else if (HasRightStepInput)
//                {
//                    StepDone(1, new Vector2(1, 0));
//                    HasRightStepInput = false;
//                }
//                // レバー入力で慣性移動
//                else if (HasVHInput)
//                {
//                    UpdateRotation();
//                    // RiseSpeed = RiseSpeed + transform.rotation * Vector3.forward;    // これだと無限に加速する
//                    RiseSpeed = new Vector3(0,RiseSpeed.y,0) + transform.rotation * Vector3.forward;
//                }

//            }
//            // ブースト切れを起こすとRiseSpeedを0のままにする（regidbodyの重力制御で落下を制御する）
//            // そして下降へ移行する
//            else
//            {
//                FallDone(RiseSpeed);
//            }

//        }
//        // 硬直時間中は上昇してもらう
//        else
//        {
//            RiseSpeed = new Vector3(this.MoveDirection.x, this.RiseSpeed, this.MoveDirection.z);
//            // 上昇算演
//            UpdateRotation();
//        }
//        // ボタンを離したら下降へ移行
//        if (!this.HasJumpInput)
//        {
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
//            m_AnimState[0] = AnimationState.Fall;
//            FallStartTime = Time.time;
//            RiseSpeed = new Vector3(0, -this.RiseSpeed, 0);
//        }

//        // 上昇算演
//        this.MoveDirection = RiseSpeed;

//        // 上昇中にオブジェクトに触れた場合は着地モーションへ移行(暴走防止のために、硬直中は判定禁止)
//        if (Time.time > this.JumpTime + this.JumpWaitTime)
//        {
//            if (IsGrounded) // 優先順位はこっちを下にしておかないと上昇前に引っかかる
//            {
//                LandingDone();
//            }
//        }
//    }

//    // Jumping_underonly時共通動作
//    protected virtual void Animation_Jumping_underonly()
//    {
//        m_AnimState[1] = AnimationState.Jumping_underonly;
//    }

//    // Fall時共通動作
//    protected virtual void Animation_Fall()
//    {
//        // くっついているエフェクトを消す
//        BrokenEffect();     
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        // ステップ時はm_MoveDirectionが消えたら困るので、一旦保持
//        Vector3 MoveDirection_OR = this.MoveDirection;
        
//        m_AnimState[1] = AnimationState.Fall;
//        // 一応重力復活
//        this.GetComponent<Rigidbody>().useGravity = true;
//        // 飛び越えフラグをカット
//        Rotatehold = false;
//        // 追加入力の有無をカット
//        this.AddInput = false;
//        // 落下モーションでなければ落下モーションへ切り替え
//        if (!this.GetComponent<Animation>().IsPlaying(m_AnimationNames[(int)AnimationState.Fall]))
//        {
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);            
//        }
//        // ブーストがあれば慣性移動及び再上昇可。なければ不可
//        if (this.Boost > 0)
//        {            
//            if (this.HasDashCancelInput)// ジャンプ再入力で向いている方向へ空中ダッシュ(上昇は押しっぱなし)           
//            {                
//                CancelDashDone();
//            }
//            // ジャンプボタンで再ジャンプ
//            else if (this.HasJumpInput)
//            {
//                JumpDone();
//            }
//            // ステップの場合ステップ
//            if (StepCheck().x != 0 || StepCheck().y != 0)
//            {
//                // MoveDirectionを再生する
//                this.MoveDirection = MoveDirection_OR;
//                StepDone(1, StepCheck());                
//                return;
//            }
//            // CPU時左ステップ
//            else if (HasLeftStepInput)
//            {
//                StepDone(1, new Vector2(-1, 0));
//                HasLeftStepInput = false;
//            }
//            // CPU時右ステップ
//            else if (HasRightStepInput)
//            {
//                StepDone(1, new Vector2(1, 0));
//                HasRightStepInput = false;
//            }           
//        }

//        // 方向キー入力で慣性移動
//        if (HasVHInput)
//        {
//            UpdateRotation();
//            this.MoveDirection = transform.rotation * Vector3.forward;
//        }

//        // 落下速度調整
//        this.MoveDirection.y = MadokaDefine.FALLSPEED / 2;

//        // 着地時に着陸へ移行
//        if (IsGrounded)
//        {
//            MoveDirection = Vector3.zero;
//            LandingDone();
//        }
//    }

//    // Landing時共通動作
//    protected virtual void Animation_Landing()
//    {
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        m_AnimState[1] = AnimationState.Landing;
//        this.Rotatehold = false;                // 固定フラグは折る
//        // 地響き防止
//        this.MoveDirection = transform.rotation * new Vector3(0, 0, 0);
//        // モーション終了時にアイドルへ移行
//        // 硬直時間が終わるとIdleへ戻る。オバヒ着地とかやりたいならBoost0でLandingTimeの値を変えるとか
//        if (Time.time > this.LandingTime + this._LandingWaitTime)
//        {
//            m_AnimState[0] = AnimationState.Idle;
//            this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Idle]);
//            // ブースト量を初期化する
//            this.Boost = GetMaxBoost(this.BoostLevel);
//        }
//    }

//    // 地上走行中は足音を鳴らす
//    private const float m_walkTime = 0.5f;
//    private float m_walkTimer;

//    private void FootSteps()
//    {
//        if (m_walkTimer > 0)
//        {
//            m_walkTimer -= Time.deltaTime;
//        }
//        if (m_walkTimer <= 0)
//        {
//            switch (m_foottype)
//            {
//                case StageSetting.FootType.FootType_Normal:
//                    AudioSource.PlayClipAtPoint(m_ashioto_normal, transform.position);
//                    break;
//                case StageSetting.FootType.FootType_Jari:
//                    AudioSource.PlayClipAtPoint(m_ashioto_jari, transform.position);
//                    break;
//                case StageSetting.FootType.FootType_Carpet:
//                    AudioSource.PlayClipAtPoint(m_ashioto_carpet, transform.position);
//                    break;
//                case StageSetting.FootType.FootType_Snow:
//                    AudioSource.PlayClipAtPoint(m_ashioto_snow, transform.position);
//                    break;
//                case StageSetting.FootType.FootType_Wood:
//                    AudioSource.PlayClipAtPoint(m_ashioto_wood, transform.position);
//                    break;
//                default:
//                    break;

//            }
//            m_walkTimer = m_walkTime;
//        }
//    }

//    // Run時共通動作
//    protected virtual void Animation_Run()
//    {
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        m_AnimState[1] = AnimationState.Run;
//        // 接地中かどうか
//        if (IsGrounded)
//        {
//            // 入力中はそちらへ進む
//            if (HasVHInput)
//            {
//                FootSteps();
//                UpdateRotation();
//                this.MoveDirection = transform.rotation * Vector3.forward;
//            }
//            // ステップの場合ステップ
//            else if (StepCheck().x != 0 || StepCheck().y != 0) 
//            {
//                StepDone(1, StepCheck());                
//            }
//            // 入力が外れるとアイドル状態へ
//            else
//            {
//                m_AnimState[0] = AnimationState.Idle;
//                this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.Idle]);
//            }

//            // ジャンプでジャンプへ移行(GetButtonDownで押しっぱなしにはならない。GetButtonで押しっぱなしに対応）
//            if (this.HasJumpInput && Boost > 0)
//            {
//                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
//                JumpDone();
//            }
//        }
//        else
//        {
//            m_AnimState[0] = AnimationState.Fall;
//            FallStartTime = Time.time;
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.Fall]);
//        }
        
//    }

//    // Run_underonly時共通動作
//    protected virtual void Animation_Run_underonly()
//    {
//        m_AnimState[1] = AnimationState.Run_underonly;
//    }

//    // AirDash時共通動作
//    protected virtual void Animation_AirDash()
//    {
//        this.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
//        // ずれた本体角度を戻す(Yはそのまま）
//        //this.transform.rotation = Quaternion.Euler(new Vector3(0, this.transform.rotation.eulerAngles.y, 0)); 
//        Vector3 RiseSpeed = new Vector3(0, 0, 0);
//        // 重力補正をカット
//        this.GetComponent<Rigidbody>().useGravity = false;
//        m_AnimState[1] = AnimationState.AirDash;
//        // 
//        if (this.Boost > 0)
//        {
//            // 入力中はそちらへ進む
//            //if (m_hasVHInput)
//            if(this.HasJumpInput)
//            {
//                // 相手を飛び越えたとき、一定距離を離すと再入力を認める
//                //if (this.m_IsRockon)
//                //{
//                //    // 敵（ロックオン対象）の座標を取得
//                //    var targetspec = GetComponentInChildren<Player_Camera_Controller>();
//                //    Vector3 targetpos = targetspec.Enemy.transform.localPosition;
//                //    // 自分の座標を取得
//                //    Vector3 myPos = this.transform.localPosition;
//                //    // 落ち切ったら切るので、とりあえずカット（後で復活するかもしれない）
//                //    //if (this.m_Rotatehold)
//                //    //{
//                //    //    if (Math.Abs(targetpos.x - myPos.x) > 60.0f || Math.Abs(targetpos.z - myPos.z) > 60.0f)
//                //    //    {
//                //    //        this.m_Rotatehold = false;
//                //    //    }
//                //    //}
//                //}
//                // ホールド中旋回禁止
//                if (!this.Rotatehold)
//                {
//                    UpdateRotation();
//                }
//                this.MoveDirection = transform.rotation * Vector3.forward;
//            }
//            // 方向キーなしで再度ジャンプを押した場合、慣性ジャンプ(硬直時間を超えていること)
//            else if(!HasVHInput && (Time.time > this.m_DashCancelTime + m_DashCancelWaittime) && Input.GetButtonDown("Jump"))
//            {
//                // 慣性ジャンプを認めるために、ジャンプボタンの累積入力は初期化
//                ResetPastInputsJump();
//                // 上昇制御をAddForceにするとやりにくい（特に慣性ジャンプ）
//                JumpDone();
//            }            
//            // ボタンを離すと下降
//            else 
//            {
//                //ResetPastInputsJump();
//                FallDone(RiseSpeed);
//            }

//        }
//        // ブースト0で下降へ移行&重力復活（慣性移動も不可）
//        else
//        {
//            FallDone(RiseSpeed);
//            this.MoveDirection = RiseSpeed;
//        }
//        // 着地で着地モーションへ
//        if (IsGrounded)
//        {
//            LandingDone();
//        }
//        // 常時ブースト消費
//        this.Boost = this.Boost - this.BoostLess;
//    }

//    // Step時共通動作
//    protected virtual void Animation_StepDone(AnimationState x)
//    {
//        m_AnimState[1] = x;
//        StepMove(x);
//        CancelCheck();
//    }

//    // StepBackStandby時共通動作
//    protected virtual void Animation_StepBackStandby(AnimationState x)
//    {
//        // 実行していなかった場合、実行
//        if (x != AnimationState.FrontStepBack)
//        {
//            // くっついているエフェクトを消す
//            BrokenEffect();     
//            if (this.IsRockon)
//            {
//                switch (x)
//                {
//                    case AnimationState.LeftStepBack_Standby:
//                        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.LeftStepBack]);
//                        break;
//                    case AnimationState.RightStepBack_Standby:
//                        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.RightStepBack]);
//                        break;
//                    case AnimationState.BackStepBack_Standby:
//                        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.BackStepBack]);
//                        break;
//                    default:
//                        this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.FrontStepBack]);
//                        break;
//                }                
//            }
//            else
//            {
//                this.GetComponent<Animation>().Play(m_AnimationNames[(int)AnimationState.FrontStepBack]);
//            }
//        }
//        else
//        {
//            this.GetComponent<Animation>().CrossFade(m_AnimationNames[(int)AnimationState.FrontStepBack]);
//        }
//        m_AnimState[0] = AnimationState.FrontStepBack;
//        // 着地したので硬直を設定する
//        this.LandingTime = Time.time;
//        // 無効になっていたら重力を復活させる
//        this.GetComponent<Rigidbody>().useGravity = true;
//        // 動作を停止する
//        this.MoveDirection = new Vector3(0, 0, 0);        
//    }

    
//}